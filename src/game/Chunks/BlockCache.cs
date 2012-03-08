using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqGame.Blocks;
using VolumetricStudios.VoxeliqGame.Common.Logging;

namespace VolumetricStudios.VoxeliqGame.Chunks
{
    // http://stackoverflow.com/questions/8162100/2d-array-with-wrapped-edges-in-c-sharp

    public interface IBlockCache
    {
        Block this[int x, int y, int z] { get; set; }
    }

    public class BlockCache : GameComponent, IBlockCache
    {
        private static BlockCache _instance;
        public static BlockCache Instance { get { return _instance; } }

        public Block[] Blocks;

        public static int CacheWidthInBlocks = ((ChunkCache.ViewRange * 2) + 1) * Chunk.WidthInBlocks;
        public static int CacheLenghtInBlocks = ((ChunkCache.ViewRange * 2) + 1) * Chunk.LenghtInBlocks;

        public static readonly int FlattenOffset = CacheLenghtInBlocks * Chunk.HeightInBlocks;

        private static readonly Logger Logger = LogManager.CreateLogger();

        public BlockCache(Game game) 
            : base(game)
        {
            _instance = this;
            this.Game.Services.AddService(typeof(IBlockCache), this); // export service.
        }

        public override void Initialize()
        {
            Logger.Trace("init()");
            this.InitStorage();
        }

        private void InitStorage()
        {
            //Console.WriteLine("init array");
            this.Blocks = new Block[CacheWidthInBlocks*CacheLenghtInBlocks*Chunk.HeightInBlocks];

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

        public static Block Get(int x, int y, int z)
        {
            return _instance[x, y, z];
        }

        public static void Set(int x, int y, int z, Block value)
        {
            _instance[x, y, z] = value;
        }

        public static int GetBlockIndex(Chunk chunk, byte x, byte y, byte z)
        {
            var xIndex = chunk.WorldPosition.X + x;
            var zIndex = chunk.WorldPosition.Z + z;

            var wrapX = xIndex % CacheWidthInBlocks;
            var wrapZ = zIndex % CacheLenghtInBlocks;

            var flattenIndex = wrapX * FlattenOffset + wrapZ * Chunk.HeightInBlocks + y;
            return flattenIndex;
        }

        public Block this[int x, int y, int z]
        {
            get
            {
                var wrapX = x%CacheWidthInBlocks;
                var wrapZ = z%CacheLenghtInBlocks;
                var flattenIndex = wrapX * FlattenOffset + wrapZ * Chunk.HeightInBlocks + y;

                return this.Blocks[flattenIndex];
            }
            set
            {
                var wrapX = x % CacheWidthInBlocks;
                var wrapZ = z % CacheLenghtInBlocks;
                var flattenIndex = wrapX * FlattenOffset + wrapZ * Chunk.HeightInBlocks + y;

                this.Blocks[flattenIndex] = value;
            }
        }
    }
}
