/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Engine.Chunks;
using Engine.Common.Logging;
using Engine.Common.Vector;
using Microsoft.Xna.Framework;

// http://stackoverflow.com/questions/8162100/2d-array-with-wrapped-edges-in-c-sharp
// http://www.voxeliq.org/page/story/_/devlog/optimizing-the-engine-i-r175

namespace Engine.Blocks
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

        // note: below values should be calculated according to cache-range, using view-range will be buggy.

        /// <summary>
        /// Cache width in blocks.
        /// </summary>
        public static int CacheWidthInBlocks = ((ChunkCache.CacheRange*2) + 1)*Chunk.WidthInBlocks;

        /// <summary>
        /// Cache lenght in blocks.
        /// </summary>
        public static int CacheLenghtInBlocks = ((ChunkCache.CacheRange * 2) + 1) * Chunk.LenghtInBlocks;

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
        /// <remarks>As <see cref="Block"/> is a struct, the returned block will be a copy of original one.</remarks>
        /// <remarks>This method will not check if given point/block coordinates are in chunk-cache's bounds. If you need a reliable & safe way, use <see cref="BlockAt"/> instead.</remarks>
        public static Block BlockAt(Vector3 position)
        {
            return BlockAt((int)position.X, (int)position.Y, (int)position.Z);
        }

        /// <summary>
        /// Gets a block by given world position.
        /// </summary>
        /// <param name="position">Point/block position.</param>
        /// <returns>Copy of <see cref="Block"/></returns>
        /// <remarks>As <see cref="Block"/> is a struct, the returned block will be a copy of original one.</remarks>
        /// <remarks>This method will not check if given point/block coordinates are in chunk-cache's bounds. If you need a reliable & safe way, use <see cref="BlockAt"/> instead.</remarks>
        public static Block BlockAt(Vector3Int position)
        {
            return BlockAt(position.X, position.Y, position.Z);
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

        /// <summary>
        /// Gets a block by given world position.
        /// </summary>
        /// <param name="position">Point/block position.</param>
        /// <returns>Copy of <see cref="Block"/></returns>
        /// <remarks>As <see cref="Block"/> is a struct, the returned block will be a copy of original one.</remarks>
        /// <remarks>This method will not check if given point/block coordinates are in chunk-cache's bounds. If you need a reliable & safe way, use <see cref="BlockAt"/> instead.</remarks>
        public static Block FastBlockAt(Vector3 position)
        {
            return FastBlockAt((int)position.X, (int)position.Y, (int)position.Z);
        }

        /// <summary>
        /// Gets a block by given world position.
        /// </summary>
        /// <param name="position">Point/block position.</param>
        /// <returns>Copy of <see cref="Block"/></returns>
        /// <remarks>As <see cref="Block"/> is a struct, the returned block will be a copy of original one.</remarks>        /// <remarks>This method will not check if given point/block coordinates are in chunk-cache's bounds. If you need a reliable & safe way, use <see cref="BlockAt"/> instead.</remarks>
        public static Block FastBlockAt(Vector3Int position)
        {
            return FastBlockAt(position.X, position.Y, position.Z);
        }

        #endregion

        #region block setters

        /// <summary>
        /// Sets a block by given world position.
        /// </summary>
        /// <param name="x">Block's x world position.</param>
        /// <param name="y">Block's y world position.</param>
        /// <param name="z">Block's z world position.</param>
        /// <param name="block">Block to set.</param>
        public static void SetBlockAt(int x, int y, int z, Block block)
        {
            // make sure given coordinates are in chunk cache's bounds.
            if (!ChunkCache.IsInBounds(x, y, z))
                return; // if it's out of bounds, just return;

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

            // set the block
            Blocks[flattenIndex] = block;
        }

        /// <summary>
        /// Sets a block by given world position.
        /// </summary>
        /// <param name="position">Point/block position.</param>
        /// <returns>Copy of <see cref="Block"/></returns>
        /// <param name="block">Block to set.</param>
        public static void SetBlockAt(Vector3 position, Block block)
        {
            SetBlockAt((int)position.X, (int)position.Y, (int)position.Z, block);
        }

        /// <summary>
        /// Sets a block by given world position.
        /// </summary>
        /// <param name="position">Point/block position.</param>
        /// <returns>Copy of <see cref="Block"/></returns>
        /// <param name="block">Block to set.</param>
        public static void SetBlockAt(Vector3Int position, Block block)
        {
            SetBlockAt(position.X, position.Y, position.Z, block);
        }

        /// <summary>
        /// Sets a block by given world position.
        /// </summary>
        /// <param name="x">Block's x world position.</param>
        /// <param name="y">Block's y world position.</param>
        /// <param name="z">Block's z world position.</param>
        /// <param name="block">Block to set.</param>
        /// <remarks>This method will not check if given point/block coordinates are in chunk-cache's bounds. If you need a reliable & safe way, use <see cref="SetBlockAt"/> instead.</remarks>
        public static void FastSetBlockAt(int x, int y, int z, Block block)
        {
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

            // sett the block
            Blocks[flattenIndex] = block;
        }

        /// <summary>
        /// Sets a block by given world position.
        /// </summary>
        /// <param name="position">Point/block position.</param>
        /// <returns>Copy of <see cref="Block"/></returns>
        /// <param name="block">Block to set.</param>
        /// <remarks>This method will not check if given point/block coordinates are in chunk-cache's bounds. If you need a reliable & safe way, use <see cref="SetBlockAt"/> instead.</remarks>
        public static void FastSetBlockAt(Vector3 position, Block block)
        {
            FastSetBlockAt((int)position.X, (int)position.Y, (int)position.Z, block);
        }

        /// <summary>
        /// Sets a block by given world position.
        /// </summary>
        /// <param name="position">Point/block position.</param>
        /// <returns>Copy of <see cref="Block"/></returns>
        /// <param name="block">Block to set.</param>
        /// <remarks>This method will not check if given point/block coordinates are in chunk-cache's bounds. If you need a reliable & safe way, use <see cref="SetBlockAt"/> instead.</remarks>
        public static void FastSetBlockAt(Vector3Int position, Block block)
        {
            FastSetBlockAt(position.X, position.Y, position.Z, block);
        }

        #endregion

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