using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxeliqGame.Blocks;
using VolumetricStudios.VoxeliqGame.Common.Logging;
using VolumetricStudios.VoxeliqGame.Generators.Biomes;
using VolumetricStudios.VoxeliqGame.Generators.Terrain;
using VolumetricStudios.VoxeliqGame.Graphics;
using VolumetricStudios.VoxeliqGame.Processors;
using VolumetricStudios.VoxeliqGame.Universe;
using VolumetricStudios.VoxeliqGame.Utils.Vector;

namespace VolumetricStudios.VoxeliqGame.Chunks
{
    public interface IChunkCache
    {
        /// <summary>
        /// Returns true if world is in infinitive mode.
        /// </summary>
        bool IsInfinitive { get; }

        /// <summary>
        /// Toggles infitinitive world.
        /// </summary>
        void ToggleInfinitiveWorld();     

        /// <summary>
        /// Bounding box for the cache.
        /// </summary>
        BoundingBox BoundingBox { get; set; }

        /// <summary>
        /// Returns the chunk in given x-z position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        Chunk GetChunk(int x, int z);

        Dictionary<ChunkState, int> StateStatistics { get; }                          

        /// <summary>
        /// Sets a block in given x-y-z coordinate.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="block"></param>
        void SetBlock(int x, int y, int z, Block block);

        /// <summary>
        /// Sets a block in given x-y-z coordinate.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="block"></param>
        void SetBlock(Vector3Int position, Block block);

        /// <summary>
        /// returns the block at given coordinate.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        Block BlockAt(Vector3 position);

        /// <summary>
        /// returns the block at given coordinate.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        Block BlockAt(int x, int y, int z);

        /// <summary>
        /// Returns chunks drawn in last draw() call.
        /// </summary>
        int ChunksDrawn { get; }

        bool IsChunkInViewRange(Chunk chunk);

        bool IsChunkInCacheRange(Chunk chunk);
    }

    /// <summary>
    /// The chunk cache & manager.
    /// </summary>
    public class ChunkCache: DrawableGameComponent, IChunkCache
    {

        /// <summary>
        /// Range of viewable chunks.
        /// </summary>
        public const byte ViewRange = 1;

        public BoundingBox ViewRangeBoundingBox { get; set; }

        /// <summary>
        /// Chunk range cache.
        /// </summary>
        public const byte CacheRange = 1;

        public BoundingBox CacheRangeBoundingBox { get; set; }

        /// <summary>
        /// Is the world infinitive?
        /// </summary>
        public bool IsInfinitive { get; private set; }

        /// <summary>
        /// Chunks drawn statistics.
        /// </summary>
        public int ChunksDrawn { get; protected set; }

        // assets & resources
        private Effect _blockEffect; // block effect.
        private Texture2D _blockTextureAtlas; // block texture atlas
        private Texture2D _crackTextureAtlas; // crack texture atlas

        public BoundingBox BoundingBox { get; set; } // Bounding box for the cache.

        // required services.
        private IChunkStorage _chunkStorage;
        private ICamera _camera;
        private IPlayer _player;
        private IFogger _fogger;

        /// <summary>
        /// The terrain generator.
        /// </summary>
        protected TerrainGenerator Generator { get; set; }

        /// <summary>
        /// The chunk vertex builder.
        /// </summary>
        protected IVertexBuilder VertexBuilder { get; set; }


        public bool CacheThreadStarted { get; private set; }

        public Dictionary<ChunkState, int> StateStatistics { get; private set; }

        // misc.
        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility.

        public ChunkCache(Game game) 
            : base(game)
        {
            this.IsInfinitive = true;
            this.Game.Services.AddService(typeof(IChunkCache), this); // export service.

            this.CacheThreadStarted = false;

            this.StateStatistics = new Dictionary<ChunkState, int>
                                       {
                                           {ChunkState.AwaitingGenerate, 0},
                                           {ChunkState.Generating, 0},
                                           {ChunkState.AwaitingLighting, 0},
                                           {ChunkState.Lighting, 0},
                                           {ChunkState.AwaitingBuild, 0},
                                           {ChunkState.Building, 0},
                                           {ChunkState.Ready, 0},
                                           {ChunkState.AwaitingRelighting, 0},
                                           {ChunkState.AwaitingRebuild, 0},
                                           {ChunkState.AwaitingRemoval, 0},
                                       };
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            // import required services.
            this._chunkStorage = (IChunkStorage) this.Game.Services.GetService(typeof (IChunkStorage));
            this._camera = (ICamera) this.Game.Services.GetService(typeof (ICamera));
            this._player = (IPlayer) this.Game.Services.GetService(typeof (IPlayer));
            this._fogger = (IFogger) this.Game.Services.GetService(typeof (IFogger));
            this.VertexBuilder = (IVertexBuilder)this.Game.Services.GetService(typeof(IVertexBuilder));
            this.Generator = new FlatDebugTerrain();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            this._blockEffect = Game.Content.Load<Effect>("Effects\\BlockEffect");
            this._blockTextureAtlas = Game.Content.Load<Texture2D>("Textures\\terrain");
            this._crackTextureAtlas = Game.Content.Load<Texture2D>("Textures\\cracks");
        }

        public bool IsChunkInViewRange(Chunk chunk)
        {
            return ViewRangeBoundingBox.Contains(chunk.BoundingBox) == ContainmentType.Contains;
        }

        public bool IsChunkInCacheRange(Chunk chunk)
        {
            return CacheRangeBoundingBox.Contains(chunk.BoundingBox) == ContainmentType.Contains;
        }

        public override void Update(GameTime gameTime)
        {
            if(!this.CacheThreadStarted)
            {
                var cacheThread = new Thread(CacheThread) { IsBackground = true };
                cacheThread.Start();

                this.CacheThreadStarted=true;
            }

            this.ViewRangeBoundingBox = new BoundingBox(
                new Vector3(this._player.CurrentChunk.WorldPosition.X - (ViewRange * Chunk.WidthInBlocks), 0, this._player.CurrentChunk.WorldPosition.Z - (ViewRange * Chunk.LenghtInBlocks)),
                new Vector3(this._player.CurrentChunk.WorldPosition.X + ((ViewRange + 1) * Chunk.WidthInBlocks), Chunk.HeightInBlocks, this._player.CurrentChunk.WorldPosition.Z + ((ViewRange + 1) * Chunk.LenghtInBlocks))
            );

            this.CacheRangeBoundingBox = new BoundingBox(
                new Vector3(this._player.CurrentChunk.WorldPosition.X - (CacheRange * Chunk.WidthInBlocks), 0, this._player.CurrentChunk.WorldPosition.Z - (CacheRange * Chunk.LenghtInBlocks)),
                new Vector3(this._player.CurrentChunk.WorldPosition.X + ((CacheRange + 1) * Chunk.WidthInBlocks), Chunk.HeightInBlocks, this._player.CurrentChunk.WorldPosition.Z + ((CacheRange + 1) * Chunk.LenghtInBlocks))
            );
        }

        private void CacheThread()
        {
            while(true)
            {
                if (this._player.CurrentChunk == null)
                    continue;

                this.Process();
            }
        }

        protected void Process()
        {
            this.StateStatistics[ChunkState.AwaitingGenerate] = 0;
            this.StateStatistics[ChunkState.Generating] = 0;
            this.StateStatistics[ChunkState.AwaitingLighting] = 0;
            this.StateStatistics[ChunkState.Lighting] = 0;
            this.StateStatistics[ChunkState.AwaitingBuild] = 0;
            this.StateStatistics[ChunkState.Building] = 0;
            this.StateStatistics[ChunkState.Ready] = 0;
            this.StateStatistics[ChunkState.AwaitingRelighting] = 0;
            this.StateStatistics[ChunkState.AwaitingRebuild] = 0;
            this.StateStatistics[ChunkState.AwaitingRemoval] = 0;


            foreach (var chunk in this._chunkStorage.Values)
            {
                if (this.IsChunkInViewRange(chunk))
                {
                    this.ProcessChunkInViewRange(chunk);
                }
                else
                {
                    if (this.IsChunkInCacheRange(chunk))
                        this.ProcessChunkInCacheRange(chunk);
                    else
                    {
                        chunk.ChunkState = ChunkState.AwaitingRemoval;
                        this._chunkStorage.Remove(chunk.RelativePosition.X, chunk.RelativePosition.Z);
                        chunk.Dispose();
                    }
                }

                this.StateStatistics[chunk.ChunkState]++;
            }

            if (this.IsInfinitive)
                this.RecacheChunks();
        }


        private void RecacheChunks()
        {
            this._player.CurrentChunk = this.GetChunk((int)_player.Position.X, (int)_player.Position.Z);

            for (int z = -CacheRange; z <= CacheRange; z++)
            {
                for (int x = -CacheRange; x <= CacheRange; x++)
                {
                    if (this._chunkStorage.ContainsKey(this._player.CurrentChunk.RelativePosition.X + x, this._player.CurrentChunk.RelativePosition.Z + z))
                        continue;

                    var chunk = new Chunk(new Vector2Int(this._player.CurrentChunk.RelativePosition.X + x, this._player.CurrentChunk.RelativePosition.Z + z));
                    this._chunkStorage[chunk.RelativePosition.X, chunk.RelativePosition.Z] = chunk;
                }
            }

            var southWestEdge = new Vector2Int(this._player.CurrentChunk.RelativePosition.X - Chunks.ChunkCache.ViewRange, this._player.CurrentChunk.RelativePosition.Z - Chunks.ChunkCache.ViewRange);
            var northEastEdge = new Vector2Int(this._player.CurrentChunk.RelativePosition.X + Chunks.ChunkCache.ViewRange, this._player.CurrentChunk.RelativePosition.Z + Chunks.ChunkCache.ViewRange);

            this.BoundingBox = new BoundingBox(new Vector3(southWestEdge.X * Chunk.WidthInBlocks, 0, southWestEdge.Z * Chunk.LenghtInBlocks), new Vector3((northEastEdge.X + 1) * Chunk.WidthInBlocks, Chunk.HeightInBlocks, (northEastEdge.Z + 1) * Chunk.LenghtInBlocks));
        }

        private void ProcessChunkInCacheRange(Chunk chunk)
        {
            if (chunk.ChunkState == ChunkState.Ready || chunk.ChunkState != ChunkState.AwaitingGenerate)
                return;

            switch (chunk.ChunkState) // switch on the chunk state.
            {
                case ChunkState.AwaitingGenerate:
                    this.Generate(chunk);
                    break;
                case ChunkState.AwaitingLighting:
                    this.Lighten(chunk);
                    break;
                default:
                    break;
            }
        }

        private void ProcessChunkInViewRange(Chunk chunk)
        {
            if (chunk.ChunkState == ChunkState.Ready || chunk.ChunkState == ChunkState.AwaitingRemoval)
                return;

            switch (chunk.ChunkState) // switch on the chunk state.
            {
                case ChunkState.AwaitingGenerate:
                    this.Generate(chunk);
                    break;
                case ChunkState.AwaitingLighting:
                    this.Lighten(chunk);
                    break;
                case ChunkState.AwaitingBuild:
                    this.Build(chunk);
                    break;
                case ChunkState.AwaitingRelighting:
                    this.Lighten(chunk);
                    break;
                case ChunkState.AwaitingRebuild:
                    this.Build(chunk);
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Generates the chunk.
        /// </summary>
        /// <param name="chunk">Chunk to be generated.</param>
        protected void Generate(Chunk chunk)
        {
            chunk.ChunkState = ChunkState.Generating; // set chunk state to generating.
            Generator.Generate(chunk);
            chunk.ChunkState = ChunkState.AwaitingLighting; // chunk should be lighten now.
        }

        /// <summary>
        /// Ligtens the chunk (calculates the lighting amount on chunks blocks).
        /// </summary>
        /// <param name="chunk">Chunk to lighten.</param>
        protected void Lighten(Chunk chunk)
        {
            chunk.ChunkState = ChunkState.Lighting; // set chunk state to generating.
            Lightning.Process(chunk);
            chunk.ChunkState = ChunkState.AwaitingBuild; // chunk should be built now.
        }

        /// <summary>
        /// Builds the chunk (calculates vertexes and indices).
        /// </summary>
        /// <param name="chunk">Chunk to build</param>
        protected void Build(Chunk chunk)
        {
            chunk.ChunkState = ChunkState.Building; // set chunk state to building.
            this.VertexBuilder.Build(chunk);
            chunk.ChunkState = ChunkState.Ready; // chunk is al ready now.
        }


        public override void Draw(GameTime gameTime)
        {
            var viewFrustrum = new BoundingFrustum(this._camera.View * this._camera.Projection);

            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game.GraphicsDevice.BlendState = BlendState.Opaque;

            // general parameters
            _blockEffect.Parameters["World"].SetValue(Matrix.Identity);
            _blockEffect.Parameters["View"].SetValue(this._camera.View);
            _blockEffect.Parameters["Projection"].SetValue(this._camera.Projection);
            _blockEffect.Parameters["CameraPosition"].SetValue(this._camera.Position);
            // texture parameters
            _blockEffect.Parameters["BlockTextureAtlas"].SetValue(_blockTextureAtlas);
            // atmospheric settings
            _blockEffect.Parameters["SunColor"].SetValue(World.SunColor);
            _blockEffect.Parameters["NightColor"].SetValue(World.NightColor);
            _blockEffect.Parameters["HorizonColor"].SetValue(World.HorizonColor);
            _blockEffect.Parameters["MorningTint"].SetValue(World.HorizonColor);
            _blockEffect.Parameters["EveningTint"].SetValue(World.HorizonColor);
            // time of day parameters
            _blockEffect.Parameters["TimeOfDay"].SetValue(Time.GetGameTimeOfDay());
            // fog parameters
            _blockEffect.Parameters["FogNear"].SetValue(this._fogger.FogVector.X);
            _blockEffect.Parameters["FogFar"].SetValue(this._fogger.FogVector.Y);
            

            this.ChunksDrawn = 0;
            foreach (EffectPass pass in this._blockEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                foreach (Chunk chunk in this._chunkStorage.Values)
                {
                    if(chunk.ChunkState != ChunkState.Ready)  // if chunk is not clean & ready yet,
                        continue;  // just pass it.

                    if(!IsChunkInViewRange(chunk))
                        continue;

                    if(!chunk.BoundingBox.Intersects(viewFrustrum)) // if chunk is not in view frustrum,
                        continue; // pas it.

                    if(chunk.IndexBuffer == null || chunk.VertexBuffer == null)
                        continue;

                    //if (!chunk.Generated || !chunk.BoundingBox.Intersects(viewFrustrum) || chunk.IndexBuffer == null) continue;

                    //lock (chunk)
                    //{
                        if (chunk.IndexBuffer.IndexCount == 0) continue;
                        Game.GraphicsDevice.SetVertexBuffer(chunk.VertexBuffer);
                        Game.GraphicsDevice.Indices = chunk.IndexBuffer;
                        Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, chunk.VertexBuffer.VertexCount, 0, chunk.IndexBuffer.IndexCount / 3);
                    //}

                    this.ChunksDrawn++;
                }
            }
        }

        // Returns the chunk in given x-z position.
        public Chunk GetChunk(int x, int z)
        {
            return !this._chunkStorage.ContainsKey(x / Chunk.WidthInBlocks, z / Chunk.LenghtInBlocks) ? null : this._chunkStorage[x / Chunk.WidthInBlocks, z / Chunk.LenghtInBlocks];
        }

        // Sets a block in given x-y-z coordinate.
        public void SetBlock(Vector3Int position, Block block)
        {
            this.SetBlock(position.X, position.Y, position.Z, block);
        }

        // Sets a block in given x-y-z coordinate.
        public void SetBlock(int x, int y, int z, Block block)
        {
            var chunk = this.GetChunk(x, z);
            chunk.SetBlock((byte)(x % Chunk.WidthInBlocks), (byte)y, (byte)(z % Chunk.LenghtInBlocks), block);
        }

        // returns the block at given coordinate.
        public Block BlockAt(Vector3 position)
        {
            return BlockAt((int)position.X, (int)position.Y, (int)position.Z);
        }

        // returns the block at given coordinate.
        public Block BlockAt(int x, int y, int z)
        {
            if (!IsInBounds(x, y, z)) return Block.Empty;

            if (!this._chunkStorage.ContainsKey(x / Chunk.WidthInBlocks, z / Chunk.LenghtInBlocks))
                return Block.Empty;

            return this._chunkStorage[x / Chunk.WidthInBlocks, z / Chunk.LenghtInBlocks].BlockAt(x % Chunk.WidthInBlocks, y, z % Chunk.LenghtInBlocks);
        }

        // returns true if given coordinate is in bounds.
        public bool IsInBounds(int x, int y, int z)
        {
            if (x < this.BoundingBox.Min.X || z < this.BoundingBox.Min.Z || x >= this.BoundingBox.Max.X || z >= this.BoundingBox.Max.Z || y < this.BoundingBox.Min.Y || y >= this.BoundingBox.Max.Y) return false;
            return true;
        }

        public void ToggleInfinitiveWorld()
        {
            this.IsInfinitive = !this.IsInfinitive;
        }
    }
}
