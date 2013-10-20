/*
 * Copyright (C) 2011 - 2013 Int6 Studios - http://www.int6.org,
 * Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Client;
using Engine.Core.Config;
using NUnit.Framework;

namespace EngineTests.Config
{
    [TestFixture]
    public class EngineConfigurationTests
    {
        [Test]
        [Description("Tests default engine configurations.")]
        public void TestDefaultValidConfig()
        {
            var game = new GameClient();
            var config = new EngineConfig();
            var engine = new Engine.Core.Engine(game, config);

            Assert.IsTrue(config.Validate()); // expect config validation to succeed.

            Assert.IsNotNull(config.Chunk); // chunk configuration should exist.
            Assert.IsNotNull(config.Cache); // cache configuratio should exists.

            #region validate chunk configuration 

            // make sure default dimensions are valid.
            Assert.Greater(config.Chunk.WidthInBlocks, 0);
            Assert.Greater(config.Chunk.HeightInBlocks, 0);
            Assert.Greater(config.Chunk.LenghtInBlocks, 0);

            // calculate expected chunk volume in blocks
            var expectedChunkVolumeInBlocks = config.Chunk.WidthInBlocks*
                                              config.Chunk.HeightInBlocks*
                                              config.Chunk.LenghtInBlocks;
            Assert.AreEqual(config.Chunk.Volume, expectedChunkVolumeInBlocks);

            // make sure max-width-index is valid.
            Assert.AreEqual(config.Chunk.MaxWidthInBlocks,
                            config.Chunk.WidthInBlocks - 1);

            // make sure max-height-index is valid.
            Assert.AreEqual(config.Chunk.MaxHeightInBlocks,
                            config.Chunk.HeightInBlocks - 1);

            // make sure max-lenght-index is valid.
            Assert.AreEqual(config.Chunk.MaxLenghtInBlocks,
                            config.Chunk.LenghtInBlocks - 1);

            #endregion

            #region validate cache configuration

            // make sure default dimensions are valid.
            Assert.Greater(config.Cache.ViewRange, 0);
            Assert.Greater(config.Cache.CacheRange, 0);

            // make sure cache-dimension in blocks are calculated correctly.
            var expectedCacheWidthInBlocks = (config.Cache.CacheRange * 2 + 1) *
                                             config.Chunk.WidthInBlocks;

            var expectedCacheHeightInBlocks = config.Chunk.HeightInBlocks;

            var expectedCacheLenghtInBlocks = (config.Cache.CacheRange * 2 + 1) *
                                              config.Chunk.LenghtInBlocks;

            Assert.AreEqual(config.Cache.CacheRangeWidthInBlocks, expectedCacheWidthInBlocks);
            Assert.AreEqual(config.Cache.CacheRangeHeightInBlocks, expectedCacheHeightInBlocks);
            Assert.AreEqual(config.Cache.CacheRangeLenghtInBlocks, expectedCacheLenghtInBlocks);

            // if by default, cache-extra-chunks option is set to true, make sure that default cache-range > default view-range.
            if (config.Cache.CacheExtraChunks)
                Assert.Greater(config.Cache.CacheRange, config.Cache.ViewRange,
                    "Cache range must be greater view range when CacheExtraChunk option is set to true.");
            else // if by default, cache-extra-chunks option is set to false, make sure that default cache-range = default view-range.
                Assert.AreEqual(config.Cache.ViewRange, config.Cache.CacheRange,
                    "Cache range can not be different than view range when CacheExtraChunk option is set to false.");

            #endregion
        }

        [Test]
        [Description("Tests invalid chunk configuration where WidthInBlocks value is set to invalid value of zero.")]
        [ExpectedException(typeof (ChunkConfigException),
            ExpectedMessage = "Chunk width in blocks can not be set to zero!", MatchType = MessageMatch.Exact)]
        public void TestInvalidWidthInBlocksValue()
        {
            var config = new EngineConfig {Chunk = {WidthInBlocks = 0}};
            config.Validate();
        }

        [Test]
        [Description("Tests invalid chunk configuration where HeightInBlocks value is set to invalid value of zero.")]
        [ExpectedException(typeof(ChunkConfigException),
            ExpectedMessage = "Chunk height in blocks can not be set to zero!", MatchType = MessageMatch.Exact)]
        public void TestInvalidHeightInBlocksValue()
        {
            var config = new EngineConfig {Chunk = {HeightInBlocks = 0}};
            config.Validate();
        }

        [Test]
        [Description("Tests invalid chunk configuration where LenghtInBlocks value is set to invalid value of zero.")]
        [ExpectedException(typeof(ChunkConfigException),
            ExpectedMessage = "Chunk lenght in blocks can not be set to zero!", MatchType = MessageMatch.Exact)]
        public void TestInvalidLenghtInBlocksValue()
        {
            var config = new EngineConfig {Chunk = {LenghtInBlocks = 0}};
            config.Validate();
        }

        [Test]
        [Description("Tests invalid cache configuration where view-range value is set to invalid value of zero.")]
        [ExpectedException(typeof(CacheConfigException),
            ExpectedMessage = "View range can not be set to zero!", MatchType = MessageMatch.Exact)]
        public void TestInvalidViewRangeValue()
        {
            var config = new EngineConfig {Cache = {ViewRange = 0}};
            config.Validate();
        }

        [Test]
        [Description("Tests invalid cache configuration where cache-range value is set to invalid value of zero.")]
        [ExpectedException(typeof(CacheConfigException),
            ExpectedMessage = "Cache range can not be set to zero!", MatchType = MessageMatch.Exact)]
        public void TestInvalidCacheRangeValue()
        {
            var config = new EngineConfig { Cache = { CacheRange = 0 } };
            config.Validate();
        }

        [Test]
        [Description("Tests invalid cache configuration where view-range is set greater than cache-range.")]
        [ExpectedException(typeof(CacheConfigException),
            ExpectedMessage = "View range can not be larger than cache range!", MatchType = MessageMatch.Exact)]
        public void TestInvalidViewRangeGreaterThanCacheRange()
        {
            var config = new EngineConfig { Cache = { ViewRange = 2, CacheRange = 1 } };
            config.Validate();
        }

        [Test]
        [Description("Tests invalid cache configuration where CacheExtraChunks is off and view-range is not equal to cache-range.")]
        [ExpectedException(typeof(CacheConfigException),
            ExpectedMessage = "Cache range can not be different than view range when CacheExtraChunk option is set to false.", MatchType = MessageMatch.Exact)]
        public void TestInvalidViewRangeNotEqualToCacheRangeWhenCacheExtraChunksIsOff()
        {
            var config = new EngineConfig { Cache = { CacheExtraChunks = false, ViewRange = 1, CacheRange = 2 } };
            config.Validate();
        }

        [Test]
        [Description("Tests invalid cache configuration where CacheExtraChunks is off and view-range is not equal to cache-range.")]
        [ExpectedException(typeof(CacheConfigException),
            ExpectedMessage = "Cache range must be greater view range when CacheExtraChunk option is set to true.", MatchType = MessageMatch.Exact)]
        public void TestInvalidCacheRangeNotGreaterThanViewRangeWhenCacheExtraChunksIsOn()
        {
            var config = new EngineConfig { Cache = { CacheExtraChunks = true, ViewRange = 2, CacheRange = 2 } };
            config.Validate();
        }
    }
}
