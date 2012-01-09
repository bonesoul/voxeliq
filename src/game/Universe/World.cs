/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxeliqGame.Blocks;
using VolumetricStudios.VoxeliqGame.Chunks;
using VolumetricStudios.VoxeliqGame.Chunks.Builders;
using VolumetricStudios.VoxeliqGame.Common.Logging;
using VolumetricStudios.VoxeliqGame.Debugging.Profiling;
using VolumetricStudios.VoxeliqGame.Environment;
using VolumetricStudios.VoxeliqGame.Graphics;
using VolumetricStudios.VoxeliqGame.Processors;
using VolumetricStudios.VoxeliqGame.Utils.Vector;

namespace VolumetricStudios.VoxeliqGame.Universe
{
    /// <summary>
    /// World service interface.
    /// </summary>
    public interface IWorld
    {
        ChunkStorage Chunks { get; }

        /// <summary>
        /// Returns true if world is in infinitive mode.
        /// </summary>
        bool IsInfinitive { get; }

        /// <summary>
        /// Toggles infitinitive world.
        /// </summary>
        void ToggleInfinitiveWorld();        
    }

    /// <summary>
    /// Statistics interface.
    /// </summary>
    public interface IWorldStatisticsService
    {
        int ChunksDrawn { get; }
        int GenerationQueueCount { get; }
        int BuildingQueueCount { get; }
    }

    /// <summary>
    /// World.
    /// </summary>
    public class World : DrawableGameComponent, IWorld, IWorldStatisticsService
    {
        public ChunkStorage Chunks { get; set; } // chunk storage.
        private readonly ChunkCache _chunkCache;// chunk cache.

        public int ChunksDrawn { get; protected set; } // chunks drawn statistics.
        public bool IsInfinitive { get; private set; } // Is the world infinitive?

        public ChunkBuilder ChunkBuilder { get; protected set; } // Chunk builder.

        public int GenerationQueueCount { get { return this.ChunkBuilder.GenerationQueueCount; } } // Generation queue count.
        public int BuildingQueueCount { get { return this.ChunkBuilder.BuildingQueueCount; } } // Building queue count.

        // assets & resources
        private Effect _blockEffect; // block effect.
        private Texture2D _blockTextureAtlas; // block texture atlas
        private Texture2D _crackTextureAtlas; // crack texture atlas

        // required services.
        private ICameraControlService _cameraController;
        private ICameraService _camera;
        private IPlayer _player;
        private IFogService _fogService;

        // misc.
        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility.

        /// <summary>
        /// Creates a new world.
        /// </summary>
        /// <param name="game"> </param>
        /// <param name="chunkStorage"> </param>
        /// <param name="chunkCache"> </param>
        public World(Game game,ChunkStorage chunkStorage, ChunkCache chunkCache)
            :base(game)
        {
            this.Chunks = chunkStorage;
            this._chunkCache = chunkCache;
            this.IsInfinitive = true;
             
            // export services.
            this.Game.Services.AddService(typeof(IWorldStatisticsService), this);
            this.Game.Services.AddService(typeof(IWorld), this);
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            // import required services.
            this._camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService)); //
            this._cameraController = (ICameraControlService)this.Game.Services.GetService(typeof(ICameraControlService));
            this._player = (IPlayer)this.Game.Services.GetService(typeof(IPlayer));
            this._fogService = (IFogService)this.Game.Services.GetService(typeof(IFogService));

            this.ChunkBuilder = new QueuedBuilder(this.Game, this._player, this); // the chunk builder.        
            this.Game.Components.Add(this.ChunkBuilder);

            var vertexBuilder = new VertexBuilder(this.Game);
            this.Game.Components.Add(vertexBuilder);

            this._cameraController.LookAt(Vector3.Down);
            this._player.SpawnPlayer(new Vector2Int(1000, 1000));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            this._blockEffect = Game.Content.Load<Effect>("Effects\\BlockEffect");
            this._blockTextureAtlas = Game.Content.Load<Texture2D>("Textures\\terrain");
            this._crackTextureAtlas = Game.Content.Load<Texture2D>("Textures\\cracks");
        }

        public void ToggleInfinitiveWorld()
        {
            this.IsInfinitive = !this.IsInfinitive;
        }

        public void SpawnPlayer(Vector2Int relativePosition)
        {
            Profiler.Start("terrain-generation");
            for (int z = -ChunkCache.ViewRange; z <= ChunkCache.ViewRange; z++)
            {
                for (int x = -ChunkCache.ViewRange; x <= ChunkCache.ViewRange; x++)
                {
                    var chunk = new Chunk(this, new Vector2Int(relativePosition.X + x, relativePosition.Z + z));
                    this.Chunks[chunk.RelativePosition.X, chunk.RelativePosition.Z] = chunk;

                    if (chunk.RelativePosition == relativePosition) this._player.CurrentChunk = chunk;
                }
            }

            this.Chunks.SouthWestEdge = new Vector2Int(relativePosition.X - ChunkCache.ViewRange, relativePosition.Z - ChunkCache.ViewRange);
            this.Chunks.NorthEastEdge = new Vector2Int(relativePosition.X + ChunkCache.ViewRange, relativePosition.Z + ChunkCache.ViewRange);

            this._chunkCache.BoundingBox = new BoundingBox(new Vector3(this.Chunks.SouthWestEdge.X * Chunk.WidthInBlocks, 0, this.Chunks.SouthWestEdge.Z * Chunk.LenghtInBlocks), new Vector3((this.Chunks.NorthEastEdge.X + 1) * Chunk.WidthInBlocks, Chunk.HeightInBlocks, (this.Chunks.NorthEastEdge.Z + 1) * Chunk.LenghtInBlocks));

            this.ChunkBuilder.Start();
        }

        #region world-drawer

        public override void Draw(GameTime gameTime)
        {
            var viewFrustrum = new BoundingFrustum(this._camera.View * this._camera.Projection);

            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game.GraphicsDevice.BlendState = BlendState.Opaque;

            _blockEffect.Parameters["World"].SetValue(Matrix.Identity);
            _blockEffect.Parameters["View"].SetValue(this._camera.View);
            _blockEffect.Parameters["Projection"].SetValue(this._camera.Projection);
            _blockEffect.Parameters["CameraPosition"].SetValue(this._camera.Position);
            _blockEffect.Parameters["FogColor"].SetValue(Color.White.ToVector4());
            _blockEffect.Parameters["FogNear"].SetValue(this._fogService.FogVector.X);
            _blockEffect.Parameters["FogFar"].SetValue(this._fogService.FogVector.Y);
            _blockEffect.Parameters["SunColor"].SetValue(Color.White.ToVector3());
            _blockEffect.Parameters["BlockTextureAtlas"].SetValue(_blockTextureAtlas);

            this.ChunksDrawn = 0;
            foreach (EffectPass pass in this._blockEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                foreach (Chunk chunk in this.Chunks.Values)
                {
                    if (!chunk.Generated || !chunk.BoundingBox.Intersects(viewFrustrum) || chunk.IndexBuffer == null) continue;

                    lock (chunk)
                    {
                        if (chunk.IndexBuffer.IndexCount == 0) continue;
                        Game.GraphicsDevice.SetVertexBuffer(chunk.VertexBuffer);
                        Game.GraphicsDevice.Indices = chunk.IndexBuffer;
                        Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, chunk.VertexBuffer.VertexCount, 0, chunk.IndexBuffer.IndexCount / 3);
                    }

                    this.ChunksDrawn++;
                }
            }
        }

        #endregion
    }
}
