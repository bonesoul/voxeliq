/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using Microsoft.Xna.Framework;
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
        /// Flatten offset x step to advance next block in x direction.
        /// </summary>
        public static readonly int XStep = CacheLenghtInBlocks*Chunk.HeightInBlocks;

        /// <summary>
        /// Flatten offset z step to advance next block in z direction.
        /// </summary>
        public static readonly int ZStep = Chunk.HeightInBlocks;

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

        #region block accessors

        /// <summary>
        /// Gets a block by given world position.
        /// </summary>
        /// <param name="x">Block's x world position.</param>
        /// <param name="y">Block's y world position.</param>
        /// <param name="z">Block's z world position.</param>
        /// <returns>Copy of <see cref="Block"/></returns>
        public static Block BlockAt(int x, int y, int z)
        {
            // make sure given coordinates are in chunk cache's bounds.
            if (!ChunkCache.IsInBounds(x, y, z))
                return Block.Empty; // if it's out of bounds, just return an empty block.

            // wrap x coordinate.
            var wrapX = x % CacheWidthInBlocks;
            if (wrapX < 0)
                wrapX += CacheWidthInBlocks;

            // wrap z coordinate.
            var wrapZ = z % CacheLenghtInBlocks;
            if (wrapZ < 0)
                wrapZ += CacheLenghtInBlocks;

            // calculate the flatten index.
            var flattenIndex = wrapX * XStep + wrapZ * ZStep + y;

            // return block copy.
            return Blocks[flattenIndex];
        }

        /// <summary>
        /// Gets a block by given world position.
        /// </summary>
        /// <param name="position">Point/block position.</param>
        /// <returns>Copy of <see cref="Block"/></returns>
        public static Block BlockAt(Vector3 position)
        {
            return BlockAt((int) position.X, (int) position.Y, (int) position.Z);
        }

        /// <summary>
        /// Gets a block by given world position.
        /// </summary>
        /// <param name="x">Block's x world position.</param>
        /// <param name="y">Block's y world position.</param>
        /// <param name="z">Block's z world position.</param>
        /// <returns>Copy of <see cref="Block"/></returns>
        /// <remarks>As <see cref="Block"/> is a struct, the returned block will be a copy of original one.</remarks>
        /// <remarks>This method will not check if given point/block coordinates are in chunk-cache's bounds. If you need a reliable & safe way, use <see cref="BlockAt"/> instead.</remarks>
        public static Block FastBlockAt(int x, int y, int z)
        {
            // wrap x coordinate.
            var wrapX = x%CacheWidthInBlocks;
            if (wrapX < 0)
                wrapX += CacheWidthInBlocks;

            // wrap z coordinate.
            var wrapZ = z%CacheLenghtInBlocks;
            if (wrapZ < 0)
                wrapZ += CacheLenghtInBlocks;

            // calculate the flatten index.
            var flattenIndex = wrapX * XStep + wrapZ * ZStep + y;

            // return block copy.
            return Blocks[flattenIndex];
        }

        #endregion

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
            if (wrapX < 0)
                wrapX += CacheWidthInBlocks;

            var wrapZ = z%CacheLenghtInBlocks;
            if (wrapZ < 0)
                wrapZ += CacheLenghtInBlocks;

            var flattenIndex = wrapX * XStep + wrapZ * ZStep + y;

            Blocks[flattenIndex] = value;
        }

        #region block index queries using world positions

        /// <summary>
        /// Returns block index by world position of block.
        /// </summary>
        /// <param name="x">Block's world position's X-coordinate</param>
        /// <param name="z">Block's world position's Z-coordinate</param>
        /// <returns></returns>
        public static int BlockIndexByWorldPosition(int x, int z)
        {
            var wrapX = x%CacheWidthInBlocks;
            if (wrapX < 0)
                wrapX += CacheWidthInBlocks;

            var wrapZ = z%CacheLenghtInBlocks;
            if (wrapZ < 0)
                wrapZ += CacheLenghtInBlocks;

            var flattenIndex = wrapX * XStep + wrapZ * ZStep;
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
            if (wrapX < 0)
                wrapX += CacheWidthInBlocks;

            var wrapZ = z%CacheLenghtInBlocks;
            if (wrapZ < 0)
                wrapZ += CacheLenghtInBlocks;

            var flattenIndex = wrapX * XStep + wrapZ * ZStep + y;
            return flattenIndex;
        }

        #endregion

        #region block index queries using relative positions

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
            if (wrapX < 0)
                wrapX += CacheWidthInBlocks;

            var wrapZ = zIndex%CacheLenghtInBlocks;
            if (wrapZ < 0)
                wrapZ += CacheLenghtInBlocks;

            var flattenIndex = wrapX * XStep + wrapZ * ZStep;
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
            if (wrapX < 0)
                wrapX += CacheWidthInBlocks;

            var wrapZ = zIndex%CacheLenghtInBlocks;
            if (wrapZ < 0)
                wrapZ += CacheLenghtInBlocks;

            var flattenIndex = wrapX * XStep + wrapZ * ZStep + y;
            return flattenIndex;
        }

        #endregion
    }
}