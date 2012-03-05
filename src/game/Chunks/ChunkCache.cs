using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxeliqGame.Blocks;
using VolumetricStudios.VoxeliqGame.Common.Logging;
using VolumetricStudios.VoxeliqGame.Graphics;
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

        public static int ViewRangeWidthInBlocks = Chunk.WidthInBlocks*ViewRange;
        public static int ViewRangeLenghtInBlocks = Chunk.LenghtInBlocks * ViewRange;

        /// <summary>
        /// Chunk range cache.
        /// </summary>
        public const byte CacheRange = 1;

        public static int CacheRangeWidthInBlocks = Chunk.WidthInBlocks * CacheRange;
        public static int CacheRangeLenghtInBlocks = Chunk.LenghtInBlocks * CacheRange;

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

        // misc.
        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility.

        public ChunkCache(Game game) 
            : base(game)
        {
            this.IsInfinitive = true;
            this.Game.Services.AddService(typeof(IChunkCache), this); // export service.
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            // import required services.
            this._chunkStorage = (IChunkStorage) this.Game.Services.GetService(typeof (IChunkStorage));
            this._camera = (ICamera) this.Game.Services.GetService(typeof (ICamera));
            this._player = (IPlayer) this.Game.Services.GetService(typeof (IPlayer));
            this._fogger = (IFogger) this.Game.Services.GetService(typeof (IFogger));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            this._blockEffect = Game.Content.Load<Effect>("Effects\\BlockEffect");
            this._blockTextureAtlas = Game.Content.Load<Texture2D>("Textures\\terrain");
            this._crackTextureAtlas = Game.Content.Load<Texture2D>("Textures\\cracks");
        }

        // Returns the chunk in given x-z position.
        public Chunk GetChunk(int x, int z)
        {
            return !this._chunkStorage.ContainsKey(x / Chunk.WidthInBlocks, z / Chunk.LenghtInBlocks) ? null : this._chunkStorage[x / Chunk.WidthInBlocks, z / Chunk.LenghtInBlocks];
        }

        public bool IsChunkInViewRange(Chunk chunk)
        {
            if ((chunk.WorldPosition.X > this._player.Position.X + ViewRangeWidthInBlocks) ||
                (chunk.WorldPosition.X < this._player.Position.X - ViewRangeWidthInBlocks))
                return false;

            if ((chunk.WorldPosition.Z > this._player.Position.Z + ViewRangeLenghtInBlocks) ||
                (chunk.WorldPosition.Z < this._player.Position.Z - ViewRangeLenghtInBlocks))
                return false;

            return true;
        }

        public bool IsChunkInCacheRange(Chunk chunk)
        {
            if ((chunk.WorldPosition.X > this._player.Position.X + CacheRangeWidthInBlocks) ||
                (chunk.WorldPosition.X < this._player.Position.X - CacheRangeWidthInBlocks))
                return false;

            if ((chunk.WorldPosition.Z > this._player.Position.Z + CacheRangeLenghtInBlocks) ||
                (chunk.WorldPosition.Z < this._player.Position.Z - CacheRangeLenghtInBlocks))
                return false;

            return true;
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

        public void ToggleInfinitiveWorld()
        {
            this.IsInfinitive = !this.IsInfinitive;
        }
    }
}
