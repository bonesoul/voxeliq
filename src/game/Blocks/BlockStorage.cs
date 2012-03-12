/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using VolumetricStudios.VoxeliqGame.Chunks;
using VolumetricStudios.VoxeliqGame.Common.Logging;

// http://stackoverflow.com/questions/8162100/2d-array-with-wrapped-edges-in-c-sharp
// http://www.voxeliq.org/page/story/_/devlog/optimizing-the-engine-i-r175

namespace VolumetricStudios.VoxeliqGame.Blocks
{
    /// <summary>
    /// Stores all blocks in viewable chunks in a single huge array.
    /// </summary>
    public static class BlockStorage
    {
        /// <summary>
        /// The single huge block array.
        /// </summary>
        public static Block[] Blocks;

        /// <summary>
        /// View cache width in blocks.
        /// </summary>
        public static int CacheWidthInBlocks = ((ChunkCache.ViewRange*2) + 1)*Chunk.WidthInBlocks;

        /// <summary>
        /// View cache lenght in blocks.
        /// </summary>
        public static int CacheLenghtInBlocks = ((ChunkCache.ViewRange*2) + 1)*Chunk.LenghtInBlocks;

        /// <summary>
        /// Flatten offset to access flatten array.
        /// </summary>
        public static readonly int FlattenOffset = CacheLenghtInBlocks*Chunk.HeightInBlocks;

        /// <summary>
        /// Logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.CreateLogger();

        static BlockStorage()
        {
            Logger.Trace("init()");

            // init the block array.
            Blocks = new Block[CacheWidthInBlocks*CacheLenghtInBlocks*Chunk.HeightInBlocks];
        }


        /// <summary>
        /// Gets a block by given world position.
        /// </summary>
        /// <param name="x">Block's x world position.</param>
        /// <param name="y">Block's y world position.</param>
        /// <param name="z">Block's z world position.</param>
        /// <returns></returns>
        public static Block GetByWorldPosition(int x, int y, int z)
        {
            var wrapX = x%CacheWidthInBlocks;
            var wrapZ = z%CacheLenghtInBlocks;
            var flattenIndex = wrapX*FlattenOffset + wrapZ*Chunk.HeightInBlocks + y;

            return Blocks[flattenIndex];
        }

        /// <summary>
        /// Sets a block by given world position.
        /// </summary>
        /// <param name="x">Block's x world position.</param>
        /// <param name="y">Block's y world position.</param>
        /// <param name="z">Block's z world position.</param>
        /// <param name="value">Block value to set.</param>
        /// <returns></returns>
        public static void SetByWorldPosition(int x, int y, int z, Block value)
        {
            var wrapX = x%CacheWidthInBlocks;
            var wrapZ = z%CacheLenghtInBlocks;
            var flattenIndex = wrapX*FlattenOffset + wrapZ*Chunk.HeightInBlocks + y;

            Blocks[flattenIndex] = value;
        }

        /// <summary>
        /// Returns block index by world position of block.
        /// </summary>
        /// <param name="x">Block's world position's X-coordinate</param>
        /// <param name="z">Block's world position's Z-coordinate</param>
        /// <returns></returns>
        public static int BlockIndexByWorldPosition(int x, int z)
        {
            var wrapX = x%CacheWidthInBlocks;
            var wrapZ = z%CacheLenghtInBlocks;

            var flattenIndex = wrapX*FlattenOffset + wrapZ*Chunk.HeightInBlocks;
            return flattenIndex;
        }

        /// <summary>
        /// Returns block index by world position of block.
        /// </summary>
        /// <param name="x">Block's world position's X-coordinate</param>
        /// <param name="y">Block's world position's Y-coordinate</param>
        /// <param name="z">Block's world position's Z-coordinate</param>
        /// <returns></returns>
        public static int BlockIndexByWorldPosition(int x, byte y, int z)
        {
            var wrapX = x%CacheWidthInBlocks;
            var wrapZ = z%CacheLenghtInBlocks;

            var flattenIndex = wrapX*FlattenOffset + wrapZ*Chunk.HeightInBlocks + y;
            return flattenIndex;
        }

        /// <summary>
        /// Returns block index by relative position of block in chunk.
        /// </summary>
        /// <param name="chunk">The chunk block belongs to.</param>
        /// <param name="x">Block's relative x position in chunk.</param>
        /// <param name="z">Block's relative x position in chunk.</param>
        /// <returns></returns>
        public static int BlockIndexByRelativePosition(Chunk chunk, byte x, byte z)
        {
            var xIndex = chunk.WorldPosition.X + x;
            var zIndex = chunk.WorldPosition.Z + z;

            var wrapX = xIndex%CacheWidthInBlocks;
            var wrapZ = zIndex%CacheLenghtInBlocks;

            var flattenIndex = wrapX*FlattenOffset + wrapZ*Chunk.HeightInBlocks;
            return flattenIndex;
        }

        /// <summary>
        /// Returns block index by relative position of block in chunk.
        /// </summary>
        /// <param name="chunk">The chunk block belongs to.</param>
        /// <param name="x">Block's relative x position in chunk.</param>
        /// <param name="y">Block's y position in chunk.</param>
        /// <param name="z">Block's relative x position in chunk.</param>
        /// <returns></returns>
        public static int BlockIndexByRelativePosition(Chunk chunk, byte x, byte y, byte z)
        {
            var xIndex = chunk.WorldPosition.X + x;
            var zIndex = chunk.WorldPosition.Z + z;

            var wrapX = xIndex%CacheWidthInBlocks;
            var wrapZ = zIndex%CacheLenghtInBlocks;

            var flattenIndex = wrapX*FlattenOffset + wrapZ*Chunk.HeightInBlocks + y;
            return flattenIndex;
        }
    }
}