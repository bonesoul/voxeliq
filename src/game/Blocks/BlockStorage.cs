/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using System;
using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqGame.Chunks;
using VolumetricStudios.VoxeliqGame.Common.Logging;
using VolumetricStudios.VoxeliqGame.Utils.Vector;

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

        /// <summary>
        /// Gets a block by given world position.
        /// </summary>
        /// <param name="x">Block's x world position.</param>
        /// <param name="y">Block's y world position.</param>
        /// <param name="z">Block's z world position.</param>
        /// <returns></returns>
        public static Block GetBlockByWorldPosition(int x, int y, int z)
        {
            var wrapX = x%CacheWidthInBlocks;
            if (wrapX < 0)
                wrapX += CacheWidthInBlocks;

            var wrapZ = z%CacheLenghtInBlocks;
            if (wrapZ < 0)
                wrapZ += CacheLenghtInBlocks;

            var flattenIndex = wrapX * XStep + wrapZ * ZStep + y;

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
        public static void SetBlockByWorldPosition(int x, int y, int z, Block value)
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

        /// <summary>
        /// Gets a neighboring block's index
        /// </summary>
        /// <param name="blockIndex"></param>
        /// <param name="xFace"></param>
        /// <param name="zFace"></param>
        /// <param name="yFace"></param>
        /// <returns></returns>
        public static Block GetNeighborBlock(int blockIndex, Vector3Int worldPosition, YFace yFace = YFace.None, ZFace zFace = ZFace.None, XFace xFace = XFace.None)
        {
            if (yFace == YFace.Top)
            {
                blockIndex++;
                worldPosition.Y++;
            }
            else if (yFace == YFace.Bottom)
            {
                blockIndex--;
                worldPosition.Y--;
            }

            if (zFace == ZFace.North)
            {
                blockIndex -= ZStep;
                worldPosition.Z--;
            }
            else if (zFace == ZFace.South)
            {
                blockIndex += ZStep;
                worldPosition.Z++;
            }

            if (xFace == XFace.East)
            {
                blockIndex += XStep;
                worldPosition.X++;
            }
            else if (xFace == XFace.West)
            {
                blockIndex -= XStep;
                worldPosition.X--;
            }

            if(ChunkCache.BoundingBox.Contains(worldPosition.AsVector3())==ContainmentType.Disjoint)
                return Block.Empty;

            if(blockIndex< 0 || blockIndex>= Blocks.Length)
                return Block.Empty;

            return Blocks[blockIndex];

            //// make sure block index is within view range.
            //blockIndex = blockIndex % Blocks.Length;

            //if (blockIndex < 0)
            //    blockIndex += Blocks.Length;

            //return blockIndex;
        }

        public enum XFace
        {
            None,
            East,
            West
        }

        public enum ZFace
        {
            None,
            North,
            South
        }

        public enum YFace
        {
            None,
            Top,
            Bottom
        }
    }
}