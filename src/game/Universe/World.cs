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
using VolumetricStudios.VoxeliqGame.Environment;
using VolumetricStudios.VoxeliqGame.Profiling;
using VolumetricStudios.VoxeliqGame.Utils.Vector;

namespace VolumetricStudios.VoxeliqGame.Universe
{
    /// <summary>
    /// World service interface.
    /// </summary>
    public interface IWorldService
    {
        ChunkStorage Chunks { get; }
        void ToggleInfinitiveWorld();
        void SetBlock(int x, int y, int z, Block block);
        void SetBlock(Vector3Int position, Block block);
        Chunk GetChunk(int x, int z);
    }

    /// <summary>
    /// World.
    /// </summary>
    public class World : DrawableGameComponent, IWorldService, IWorldStatisticsService
    {
        public ChunkStorage Chunks { get; private set; }
        public ChunkCache ChunkCache { get; private set; }

        /// <summary>
        /// Bounding box for the world.
        /// </summary>
        public BoundingBox BoundingBox { get; set; }

        /// <summary>
        /// View range for the world.
        /// </summary>
        public const byte ViewRange = 6;

        /// <summary>
        /// Chunks drawn statistics.
        /// </summary>
        public int ChunksDrawn { get; protected set; } // chunks drawn statistics.

        /// <summary>
        /// Total chunks
        /// </summary>
        public int TotalChunks { get { return this.Chunks.Count; } }

        /// <summary>
        /// Is the world infinitive?
        /// </summary>
        public bool IsInfinitive { get; private set; }

        /// <summary>
        /// camera controller
        /// </summary>
        private ICameraControlService _cameraController;

        /// <summary>
        /// The camera service.
        /// </summary>
        public ICameraService Camera;

        /// <summary>
        /// Chunk builder.
        /// </summary>
        public ChunkBuilder ChunkBuilder { get; protected set; }

        /// <summary>
        /// Generation queue count.
        /// </summary>
        public int GenerationQueueCount { get { return this.ChunkBuilder.GenerationQueueCount; } }

        /// <summary>
        /// Building queue count.
        /// </summary>
        public int BuildingQueueCount { get { return this.ChunkBuilder.BuildingQueueCount; } }

        /// <summary>
        /// player
        /// </summary>
        private IPlayer _player;

        /// <summary>
        /// block effect.
        /// </summary>
        private Effect _blockEffect;

        /// <summary>
        /// block texture atlas
        /// </summary>
        private Texture2D _blockTextureAtlas;

        /// <summary>
        /// crack texture atlas
        /// </summary>
        private Texture2D _crackTextureAtlas;

        /// <summary>
        /// IForService to interract with fog-effect.
        /// </summary>
        private IFogService _fogService;

        /// <summary>
        /// Logging facility.
        /// </summary>
        private static readonly Logger Logger = LogManager.CreateLogger();

        /// <summary>
        /// Creates a new world.
        /// </summary>
        /// <param name="game"> </param>
        /// <param name="isInfinitive"></param>
        /// <param name="chunkStorage"> </param>
        /// <param name="chunkCache"> </param>
        public World(Game game, bool isInfinitive, ChunkStorage chunkStorage, ChunkCache chunkCache)
            :base(game)
        {
            this.Chunks = chunkStorage;
            this.ChunkCache = chunkCache;
            this.IsInfinitive = isInfinitive;
             
            // export services.
            this.Game.Services.AddService(typeof(IWorldStatisticsService), this);
            this.Game.Services.AddService(typeof(IWorldService), this);
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            this.Camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService)); //
            this._cameraController = (ICameraControlService)this.Game.Services.GetService(typeof(ICameraControlService));
            this._player = (IPlayer)this.Game.Services.GetService(typeof(IPlayer));
            this._fogService = (IFogService)this.Game.Services.GetService(typeof(IFogService));

            this.ChunkBuilder = new QueuedBuilder(this._player, this); // the chunk builder.        

            this._blockEffect = Game.Content.Load<Effect>("Effects\\BlockEffect");
            this._blockTextureAtlas = Game.Content.Load<Texture2D>("Textures\\terrain");
            this._crackTextureAtlas = Game.Content.Load<Texture2D>("Textures\\cracks");

            this._cameraController.LookAt(Vector3.Down);
            this._player.SpawnPlayer(new Vector2Int(1000, 1000));
        }

        public void ToggleInfinitiveWorld()
        {
            this.IsInfinitive = !this.IsInfinitive;
        }

        public bool IsInBounds(int x, int y, int z)
        {
            if (x < this.BoundingBox.Min.X || z < this.BoundingBox.Min.Z || x >= this.BoundingBox.Max.X || z >= this.BoundingBox.Max.Z || y < this.BoundingBox.Min.Y || y >= this.BoundingBox.Max.Y) return false;
            return true;
        }

        public Block BlockAt(Vector3 position)
        {
            return BlockAt((int)position.X, (int)position.Y, (int)position.Z);
        }

        public Block BlockAt(int x, int y, int z)
        {
            if (!IsInBounds(x, y, z)) return Block.Empty;

            if (!this.Chunks.ContainsKey(x / Chunk.WidthInBlocks, z / Chunk.LenghtInBlocks)) 
                return Block.Empty;

            return this.Chunks[x/Chunk.WidthInBlocks, z/Chunk.LenghtInBlocks].BlockAt(x%Chunk.WidthInBlocks, y, z%Chunk.LenghtInBlocks);
        }

        public void SetBlock(Vector3Int position, Block block)
        {
            this.SetBlock(position.X,position.Y,position.Z,block);
        }

        public void SetBlock(int x, int y, int z, Block block)
        {
            var chunk = GetChunk(x, z);
            chunk.SetBlock((byte) (x%Chunk.WidthInBlocks), (byte) y, (byte) (z%Chunk.LenghtInBlocks), block);
        }

        public Chunk GetChunk(int x, int z)
        {
            return !this.Chunks.ContainsKey(x / Chunk.WidthInBlocks, z / Chunk.LenghtInBlocks) ? null : this.Chunks[x / Chunk.WidthInBlocks, z / Chunk.LenghtInBlocks];
        }

        public void SpawnPlayer(Vector2Int relativePosition)
        {
            Profiler.Start("terrain-generation");
            for (int z = -ViewRange; z <= ViewRange; z++)
            {
                for (int x = -ViewRange; x <= ViewRange; x++)
                {
                    var chunk = new Chunk(this, new Vector2Int(relativePosition.X + x, relativePosition.Z + z));
                    this.Chunks[chunk.RelativePosition.X, chunk.RelativePosition.Z] = chunk;

                    if (chunk.RelativePosition == relativePosition) this._player.CurrentChunk = chunk;
                }
            }

            this.Chunks.SouthWestEdge = new Vector2Int(relativePosition.X - ViewRange, relativePosition.Z - ViewRange);
            this.Chunks.NorthEastEdge = new Vector2Int(relativePosition.X + ViewRange, relativePosition.Z + ViewRange);

            BoundingBox = new BoundingBox(new Vector3(this.Chunks.SouthWestEdge.X * Chunk.WidthInBlocks, 0, this.Chunks.SouthWestEdge.Z * Chunk.LenghtInBlocks), new Vector3((this.Chunks.NorthEastEdge.X + 1) * Chunk.WidthInBlocks, Chunk.HeightInBlocks, (this.Chunks.NorthEastEdge.Z + 1) * Chunk.LenghtInBlocks));

            this.ChunkBuilder.Start();
        }

        #region world-drawer

        public override void Draw(GameTime gameTime)
        {
            var viewFrustrum = new BoundingFrustum(this.Camera.View * this.Camera.Projection);

            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game.GraphicsDevice.BlendState = BlendState.Opaque;

            _blockEffect.Parameters["World"].SetValue(Matrix.Identity);
            _blockEffect.Parameters["View"].SetValue(this.Camera.View);
            _blockEffect.Parameters["Projection"].SetValue(this.Camera.Projection);
            _blockEffect.Parameters["CameraPosition"].SetValue(this.Camera.Position);
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

    /// <summary>
    /// Statistics interface.
    /// </summary>
    public interface IWorldStatisticsService
    {
        int TotalChunks { get; }
        int ChunksDrawn { get; }
        int GenerationQueueCount { get; }
        int BuildingQueueCount { get; }
        bool IsInfinitive { get; }
    }
}
