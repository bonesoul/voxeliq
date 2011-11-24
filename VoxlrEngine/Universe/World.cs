/*    
 * Copyright (C) 2011, Hüseyin Uslu
 *  
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxlrEngine.Profiling;
using VolumetricStudios.VoxlrEngine.Screen;
using VolumetricStudios.VoxlrEngine.Utils.Vector;
using VolumetricStudios.VoxlrEngine.Universe.Builders;

namespace VolumetricStudios.VoxlrEngine.Universe
{
    public interface IWorldService
    {
        ChunkManager Chunks { get; }
        void ToggleFog();
        void ToggleInfinitiveWorld();
        void SetBlock(int x, int y, int z, Block block);
        void SetBlock(Vector3Int position, Block block);
        Chunk GetChunk(int x, int z);
    }

    public interface IWorldStatisticsService
    {
        int TotalChunks { get; }
        int ChunksDrawn { get; }
        int GenerationQueueCount { get; }
        int BuildingQueueCount { get; }
        bool IsInfinitive { get; }
        FogState FogState { get; }
    }

    public sealed class World : DrawableGameComponent, IWorldStatisticsService, IWorldService
    {
        private Effect _blockEffect;
        private Texture2D _blockTextureAtlas;
        private Texture2D _crackTextureAtlas;
        public ICameraService _camera;
        private ICameraControlService _cameraController;
        private IPlayer _player;
        private readonly Vector2[] _fogVectors = new[] {new Vector2(0, 0), new Vector2(175, 250), new Vector2(250, 400)};

        public ChunkManager Chunks { get; private set; }
        public int ChunksDrawn { get; private set; } // chunks drawn statistics.
        public int TotalChunks { get { return this.Chunks.Count; } }
        public bool IsInfinitive { get; private set; }
        public FogState FogState { get; private set; }
        public ChunkBuilder ChunkBuilder {get; private set;}        
        public int GenerationQueueCount { get { return this.ChunkBuilder.GenerationQueueCount; } }
        public int BuildingQueueCount { get { return this.ChunkBuilder.BuildingQueueCount; } }

        public BoundingBox BoundingBox { get; set; }       
        public const byte ViewRange = 1;
        
        public World(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IWorldStatisticsService), this);
            game.Services.AddService(typeof(IWorldService), this);
        }

        public override void Initialize()
        {
            this.IsInfinitive = true;
            this.FogState = FogState.None;
            this.Chunks = new ChunkManager();

            this._camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));
            this._cameraController = (ICameraControlService) this.Game.Services.GetService(typeof (ICameraControlService));
            this._player = (IPlayer)this.Game.Services.GetService(typeof(IPlayer));
            this.ChunkBuilder = new QueuedBuilder(this._player, this);

            _blockEffect = Game.Content.Load<Effect>("Effects\\BlockEffect");
            _blockTextureAtlas = Game.Content.Load<Texture2D>("Textures\\blocks");
            _crackTextureAtlas = Game.Content.Load<Texture2D>("Textures\\cracks");

            this._cameraController.LookAt(Vector3.Down);
            this._player.SpawnPlayer(new Vector2Int(1000, 1000));

            base.Initialize();
        }

        public void ToggleFog()
        {
            switch (FogState)
            {
                case FogState.None:
                    FogState = FogState.Near;
                    break;
                case FogState.Near:
                    FogState = FogState.Far;
                    break;
                case FogState.Far:
                    FogState = FogState.None;
                    break;
            }
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

            if (!this.Chunks.ContainsKey(x / Chunk.WidthInBlocks, z / Chunk.LenghtInBlocks)) return Block.Empty;
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
            var viewFrustrum = new BoundingFrustum(this._camera.View * this._camera.Projection);

            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game.GraphicsDevice.BlendState = BlendState.Opaque;

            _blockEffect.Parameters["World"].SetValue(Matrix.Identity);
            _blockEffect.Parameters["View"].SetValue(this._camera.View);
            _blockEffect.Parameters["Projection"].SetValue(this._camera.Projection);
            _blockEffect.Parameters["CameraPosition"].SetValue(this._camera.Position);
            _blockEffect.Parameters["FogColor"].SetValue(Color.White.ToVector4());
            _blockEffect.Parameters["FogNear"].SetValue(this._fogVectors[(byte)this.FogState].X);
            _blockEffect.Parameters["FogFar"].SetValue(this._fogVectors[(byte)this.FogState].Y);
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
                        Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, chunk.VertexBuffer.VertexCount, 0, chunk.IndexBuffer.IndexCount/3);
                    }

                    this.ChunksDrawn++;
                }              
            }
        }


        #endregion
    }

    #region enums

    public enum FogState:byte
    {
        None,
        Near,
        Far
    }

    #endregion
}
