using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;
using VoxeliqEngine.Blocks;
using VoxeliqEngine.Chunks;
using VoxeliqEngine.Core;
using VoxeliqGame;

namespace EngineTests.Blocks
{
    [TestFixture]
    public class BlockStorageTests
    {
        private Game _game;
        private EngineConfiguration _config;
        private VoxeliqEngine.Core.Engine _engine;

        /// <summary>
        /// Flatten offset x step to advance next block in x direction.
        /// </summary>
        private int _xStep;

        /// <summary>
        /// Flatten offset z step to advance next block in z direction.
        /// </summary>
        private int _zStep;

        private Dictionary<int, BlockType> _validationDictionary;

        private Random _random;
        private int _seedX;
        private int _seedZ;

        [SetUp]
        public void Init()
        {
            _game = new Game();
            this._config = new EngineConfiguration();
            this._config.CacheConfiguration.CacheRange = 12;
            this._config.CacheConfiguration.ViewRange = 8;
            this._engine = new VoxeliqEngine.Core.Engine(this._game, this._config);

            var cacheLenghtInBlocks = ((_config.CacheConfiguration.CacheRange*2) + 1) * _config.ChunkConfiguration.LenghtInBlocks;
            this._xStep =  cacheLenghtInBlocks *_config.ChunkConfiguration.HeightInBlocks;
            this._zStep = _config.ChunkConfiguration.HeightInBlocks;

            this._validationDictionary = new Dictionary<int, BlockType>();

            this._random=new Random();
            this._seedX = this._random.Next(120);
            this._seedZ = this._random.Next(120);

            // set the initial values.
            for (var x = 0; x < _config.ChunkConfiguration.WidthInBlocks; x++)
            {
                for (var z = 0; z < _config.ChunkConfiguration.LenghtInBlocks; z++)
                {
                    var seededX = x - _seedX;
                    var seededZ = z - _seedZ;
                    var offset = BlockStorage.BlockIndexByWorldPosition(seededX, seededZ);

                    for (var y = 0; y < _config.ChunkConfiguration.HeightInBlocks; y++)
                    {
                        var index = offset + y;
                        var block = new Block().RandomizeType();

                        this._validationDictionary.Add(index, block.Type);

                        BlockStorage.Blocks[index] = block;
                    }
                }
            }
        }

        [Test]
        [Description("Tests BlockStorage.")]
        public void BlockReadTests()
        {
            // read them back
            for (var x = 0; x < _config.ChunkConfiguration.WidthInBlocks; x++)
            {
                for (var z = 0; z < _config.ChunkConfiguration.LenghtInBlocks; z++)
                {
                    var seededX = x - _seedX;
                    var seededZ = z - _seedZ;
                    var offset = BlockStorage.BlockIndexByWorldPosition(seededX, seededZ);

                    for (var y = 0; y < _config.ChunkConfiguration.HeightInBlocks; y++)
                    {
                        var index = offset + y;

                        var expectedType = this._validationDictionary[index];

                        var blockIndexed = BlockStorage.Blocks[index];
                        Assert.AreEqual(expectedType, blockIndexed.Type);

                        var blockFastAt = BlockStorage.FastBlockAt(seededX, y, seededZ);
                        Assert.AreEqual(expectedType, blockFastAt.Type);
                    }
                }
            }            
        }

        // TODO: add landscape generated tests.
        // TODO: add positive, middle and negative seeded tests.
        // TODO: add SetBlock tests.
    }

    public static class BlockTestExtensions
    {
        private static readonly Array BlockTypes = Enum.GetValues(typeof(BlockType));
        private static readonly Random Random=new Random();

        public static Block RandomizeType(this Block block)
        {
            return new Block((BlockType) BlockTypes.GetValue(Random.Next(BlockTypes.Length - 1))); // we substract 1 because we dont wan't BlockType.Maximum.
        }
    }
}
