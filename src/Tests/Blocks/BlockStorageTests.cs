/*
 * Copyright (C) 2011 - 2013 Int6 Studios - http://www.int6.org,
 * Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Collections.Generic;
using Client;
using Engine.Blocks;
using Engine.Core.Config;
using NUnit.Framework;

namespace EngineTests.Blocks
{
    [TestFixture]
    public class BlockStorageTests
    {
        private GameClient _game;
        private EngineConfig _config;
        private Engine.Core.Engine _engine;

        private int _cacheXStartIndex;
        private int _cacheXEndIndex;
        private int _cacheZStartIndex;
        private int _cacheZEndIndex;

        private Dictionary<int, BlockType> _directlyIndexedValidationDictionary;

        [SetUp]
        public void Init()
        {
            _game = new GameClient();
            this._config = new EngineConfig();
            this._engine = new Engine.Core.Engine(this._game, this._config);

            var cacheWidthInBlocks = ((_config.Cache.CacheRange * 2) + 1) * _config.Chunk.WidthInBlocks;
            var cacheLenghtInBlocks = ((_config.Cache.CacheRange*2) + 1) * _config.Chunk.LenghtInBlocks;

            this._cacheXStartIndex = -cacheWidthInBlocks/2;
            this._cacheXEndIndex = cacheWidthInBlocks / 2;

            this._cacheZStartIndex = -cacheLenghtInBlocks / 2;
            this._cacheZEndIndex = cacheLenghtInBlocks / 2;

            this._directlyIndexedValidationDictionary = new Dictionary<int, BlockType>();

            // set the initial values.
            for (var x = this._cacheXStartIndex; x < this._cacheXEndIndex; x++)
            {
                for (var z = this._cacheZStartIndex; z < this._cacheZEndIndex; z++)
                {
                    var offset = BlockStorage.BlockIndexByWorldPosition(x, z);

                    for (var y = 0; y < _config.Chunk.HeightInBlocks; y++)
                    {
                        var index = offset + y;
                        var block = new Block().RandomizeType();

                        this._directlyIndexedValidationDictionary.Add(index, block.Type);

                        BlockStorage.Blocks[index] = block;
                    }
                }
            }

            // check if validationDictionaries item count is equal to CacheRange's volume.
            Assert.AreEqual(this._directlyIndexedValidationDictionary.Values.Count, _config.Cache.CacheRangeVolume);
        }

        [Test]
        [Description("Tests BlockStorage.")]
        public void TestAllBlocksInCacheRange()
        {
            // read them back
            for (var x = this._cacheXStartIndex; x < this._cacheXEndIndex; x++)
            {
                for (var z = this._cacheZStartIndex; z < this._cacheZEndIndex; z++)
                {
                    var offset = BlockStorage.BlockIndexByWorldPosition(x, z);

                    for (var y = 0; y < _config.Chunk.HeightInBlocks; y++)
                    {
                        var index = offset + y;

                        var expectedType = this._directlyIndexedValidationDictionary[index];

                        var blockIndexed = BlockStorage.Blocks[index];
                        Assert.AreEqual(expectedType, blockIndexed.Type);

                        var blockFastAt = BlockStorage.FastBlockAt(x, y, z);
                        Assert.AreEqual(expectedType, blockFastAt.Type);
                    }
                }
            }            
        }

        // TODO: add landscape generated tests.
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
