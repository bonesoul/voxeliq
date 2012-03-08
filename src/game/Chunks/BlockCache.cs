/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqGame.Blocks;
using VolumetricStudios.VoxeliqGame.Common.Logging;

namespace VolumetricStudios.VoxeliqGame.Chunks
{
    // http://stackoverflow.com/questions/8162100/2d-array-with-wrapped-edges-in-c-sharp

    public static class BlockCache
    {
        public static Block[] Blocks;

        public static int CacheWidthInBlocks = ((ChunkCache.ViewRange * 2) + 1) * Chunk.WidthInBlocks;
        public static int CacheLenghtInBlocks = ((ChunkCache.ViewRange * 2) + 1) * Chunk.LenghtInBlocks;

        public static readonly int FlattenOffset = CacheLenghtInBlocks * Chunk.HeightInBlocks;

        private static readonly Logger Logger = LogManager.CreateLogger();

        static BlockCache() 
        {
            Logger.Trace("init()");
            InitStorage();
        }

        private static void InitStorage()
        {
            //Console.WriteLine("init array");
            Blocks = new Block[CacheWidthInBlocks*CacheLenghtInBlocks*Chunk.HeightInBlocks];

            //Console.WriteLine("empty blocks");
            for (int x = 0; x < CacheWidthInBlocks; x++)
            {
                for (int z = 0; z < CacheLenghtInBlocks; z++)
                {
                    int offset = x * FlattenOffset + z * Chunk.HeightInBlocks;
                    for (byte y = 0; y < Chunk.HeightInBlocks; y++)
                    {
                        Blocks[offset + y] = Block.Empty;
                    }
                }
            }
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
            var flattenIndex = wrapX * FlattenOffset + wrapZ * Chunk.HeightInBlocks + y;

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
            var wrapX = x % CacheWidthInBlocks;
            var wrapZ = z % CacheLenghtInBlocks;
            var flattenIndex = wrapX * FlattenOffset + wrapZ * Chunk.HeightInBlocks + y;

            Blocks[flattenIndex] = value;
        }

        public static int BlockIndexByWorldPosition(byte x, byte z)
        {
            var wrapX = x % CacheWidthInBlocks;
            var wrapZ = x % CacheLenghtInBlocks;

            var flattenIndex = wrapX * FlattenOffset + wrapZ * Chunk.HeightInBlocks;
            return flattenIndex;
        }

        public static int BlockIndexByWorldPosition(byte x, byte y, byte z)
        {
            var wrapX = x % CacheWidthInBlocks;
            var wrapZ = x % CacheLenghtInBlocks;

            var flattenIndex = wrapX * FlattenOffset + wrapZ * Chunk.HeightInBlocks + y;
            return flattenIndex;
        }

        /// <summary>
        /// Returns block index by relative position of block in chunk.
        /// </summary>
        /// <param name="chunk">The chunk block belongs to.</param>
        /// <param name="x">Blocks relative x position in chunk.</param>
        /// <param name="z">Blocks relative x position in chunk.</param>
        /// <returns></returns>
        public static int BlockIndexByRelativePosition(Chunk chunk, byte x, byte z)
        {
            var xIndex = chunk.WorldPosition.X + x;
            var zIndex = chunk.WorldPosition.Z + z;

            var wrapX = xIndex % CacheWidthInBlocks;
            var wrapZ = zIndex % CacheLenghtInBlocks;

            var flattenIndex = wrapX*FlattenOffset + wrapZ*Chunk.HeightInBlocks;
            return flattenIndex;
        }

        /// <summary>
        /// Returns block index by relative position of block in chunk.
        /// </summary>
        /// <param name="chunk">The chunk block belongs to.</param>
        /// <param name="x">Blocks relative x position in chunk.</param>
        /// <param name="y">Blocks y position in chunk. </param>
        /// <param name="z">Blocks relative x position in chunk.</param>
        /// <returns></returns>
        public static int BlockIndexByRelativePosition(Chunk chunk, byte x, byte y, byte z)
        {
            var xIndex = chunk.WorldPosition.X + x;
            var zIndex = chunk.WorldPosition.Z + z;

            var wrapX = xIndex % CacheWidthInBlocks;
            var wrapZ = zIndex % CacheLenghtInBlocks;

            var flattenIndex = wrapX * FlattenOffset + wrapZ * Chunk.HeightInBlocks + y;
            return flattenIndex;
        }
    }
}
