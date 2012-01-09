using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqGame.Blocks;
using VolumetricStudios.VoxeliqGame.Utils.Vector;

namespace VolumetricStudios.VoxeliqGame.Chunks
{
    public interface IChunkCache
    {
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
    }

    /// <summary>
    /// The chunk cache & manager.
    /// </summary>
    public class ChunkCache: DrawableGameComponent, IChunkCache
    {
        public const byte ViewRange = 6; // View range for the world.

        public BoundingBox BoundingBox { get; set; } // Bounding box for the cache.

        // required services.
        private IChunkStorage _chunkStorage;

        public ChunkCache(Game game) 
            : base(game)
        {
            this.Game.Services.AddService(typeof(IChunkCache), this); // export service.
        }

        public override void Initialize()
        {
            // import required services.
            this._chunkStorage = (IChunkStorage) this.Game.Services.GetService(typeof (IChunkStorage));
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
    }
}
