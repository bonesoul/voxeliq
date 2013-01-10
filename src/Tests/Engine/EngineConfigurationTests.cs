/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using NUnit.Framework;
using VoxeliqEngine.Core;

namespace EngineTests.Engine
{
    [TestFixture]
    public class EngineConfigurationTests
    {

        [Test]
        [Description("Tests default engine configurations.")]
        public void TestDefaultValidConfig()
        {
            var config = new EngineConfiguration();

            Assert.IsTrue(config.Validate()); // expect config validation to succeed.

            Assert.IsNotNull(config.ChunkConfiguration); // chunk configuration should exist.
            Assert.IsNotNull(config.CacheConfiguration); // cache configuratio should exists.

            #region validate chunk configuration 

            // make sure default dimensions are valid.
            Assert.Greater(config.ChunkConfiguration.WidthInBlocks, 0);
            Assert.Greater(config.ChunkConfiguration.HeightInBlocks, 0);
            Assert.Greater(config.ChunkConfiguration.LenghtInBlocks, 0);

            // calculate expected chunk volume in blocks
            var expectedChunkVolumeInBlocks = config.ChunkConfiguration.WidthInBlocks*
                                              config.ChunkConfiguration.HeightInBlocks*
                                              config.ChunkConfiguration.LenghtInBlocks;
            Assert.AreEqual(config.ChunkConfiguration.Volume, expectedChunkVolumeInBlocks);

            // make sure max-width-index is valid.
            Assert.AreEqual(config.ChunkConfiguration.MaxWidthInBlocks,
                            config.ChunkConfiguration.WidthInBlocks - 1);

            // make sure max-height-index is valid.
            Assert.AreEqual(config.ChunkConfiguration.MaxHeightInBlocks,
                            config.ChunkConfiguration.HeightInBlocks - 1);

            // make sure max-lenght-index is valid.
            Assert.AreEqual(config.ChunkConfiguration.MaxLenghtInBlocks,
                            config.ChunkConfiguration.LenghtInBlocks - 1);

            #endregion

            #region validate cache configuration

            // make sure default dimensions are valid.
            Assert.Greater(config.CacheConfiguration.ViewRange, 0);
            Assert.Greater(config.CacheConfiguration.CacheRange, 0);

            // make sure cache-dimension in blocks are calculated correctly.
            var expectedCacheWidthInBlocks = (config.CacheConfiguration.CacheRange * 2 + 1) *
                                             config.ChunkConfiguration.WidthInBlocks;

            var expectedCacheHeightInBlocks = config.ChunkConfiguration.HeightInBlocks;

            var expectedCacheLenghtInBlocks = (config.CacheConfiguration.CacheRange * 2 + 1) *
                                              config.ChunkConfiguration.LenghtInBlocks;

            Assert.AreEqual(config.CacheConfiguration.CacheRangeWidthInBlocks, expectedCacheWidthInBlocks);
            Assert.AreEqual(config.CacheConfiguration.CacheRangeHeightInBlocks, expectedCacheHeightInBlocks);
            Assert.AreEqual(config.CacheConfiguration.CacheRangeLenghtInBlocks, expectedCacheLenghtInBlocks);

            // if by default, cache-extra-chunks option is set to true, make sure that default cache-range > default view-range.
            if (config.CacheConfiguration.CacheExtraChunks)
                Assert.Greater(config.CacheConfiguration.CacheRange, config.CacheConfiguration.ViewRange,
                    "Cache range must be greater view range when CacheExtraChunk option is set to true.");
            else // if by default, cache-extra-chunks option is set to false, make sure that default cache-range = default view-range.
                Assert.AreEqual(config.CacheConfiguration.ViewRange, config.CacheConfiguration.CacheRange,
                    "Cache range can not be different than view range when CacheExtraChunk option is set to false.");

            #endregion
        }

        [Test]
        [Description("Tests invalid chunk configuration where WidthInBlocks value is set to invalid value of zero.")]
        [ExpectedException(typeof (ChunkConfigurationException),
            ExpectedMessage = "Chunk width in blocks can not be set to zero!", MatchType = MessageMatch.Exact)]
        public void TestInvalidWidthInBlocksValue()
        {
            var config = new EngineConfiguration {ChunkConfiguration = {WidthInBlocks = 0}};
            config.Validate();
        }

        [Test]
        [Description("Tests invalid chunk configuration where HeightInBlocks value is set to invalid value of zero.")]
        [ExpectedException(typeof(ChunkConfigurationException),
            ExpectedMessage = "Chunk height in blocks can not be set to zero!", MatchType = MessageMatch.Exact)]
        public void TestInvalidHeightInBlocksValue()
        {
            var config = new EngineConfiguration {ChunkConfiguration = {HeightInBlocks = 0}};
            config.Validate();
        }

        [Test]
        [Description("Tests invalid chunk configuration where LenghtInBlocks value is set to invalid value of zero.")]
        [ExpectedException(typeof(ChunkConfigurationException),
            ExpectedMessage = "Chunk lenght in blocks can not be set to zero!", MatchType = MessageMatch.Exact)]
        public void TestInvalidLenghtInBlocksValue()
        {
            var config = new EngineConfiguration {ChunkConfiguration = {LenghtInBlocks = 0}};
            config.Validate();
        }

        [Test]
        [Description("Tests invalid cache configuration where view-range value is set to invalid value of zero.")]
        [ExpectedException(typeof(CacheConfigurationException),
            ExpectedMessage = "View range can not be set to zero!", MatchType = MessageMatch.Exact)]
        public void TestInvalidViewRangeValue()
        {
            var config = new EngineConfiguration {CacheConfiguration = {ViewRange = 0}};
            config.Validate();
        }

        [Test]
        [Description("Tests invalid cache configuration where cache-range value is set to invalid value of zero.")]
        [ExpectedException(typeof(CacheConfigurationException),
            ExpectedMessage = "Cache range can not be set to zero!", MatchType = MessageMatch.Exact)]
        public void TestInvalidCacheRangeValue()
        {
            var config = new EngineConfiguration { CacheConfiguration = { CacheRange = 0 } };
            config.Validate();
        }

        [Test]
        [Description("Tests invalid cache configuration where view-range is set greater than cache-range.")]
        [ExpectedException(typeof(CacheConfigurationException),
            ExpectedMessage = "View range can not be larger than cache range!", MatchType = MessageMatch.Exact)]
        public void TestInvalidViewRangeGreaterThanCacheRange()
        {
            var config = new EngineConfiguration { CacheConfiguration = { ViewRange = 2, CacheRange = 1 } };
            config.Validate();
        }

        [Test]
        [Description("Tests invalid cache configuration where CacheExtraChunks is off and view-range is not equal to cache-range.")]
        [ExpectedException(typeof(CacheConfigurationException),
            ExpectedMessage = "Cache range can not be different than view range when CacheExtraChunk option is set to false.", MatchType = MessageMatch.Exact)]
        public void TestInvalidViewRangeNotEqualToCacheRangeWhenCacheExtraChunksIsOff()
        {
            var config = new EngineConfiguration { CacheConfiguration = { CacheExtraChunks = false, ViewRange = 1, CacheRange = 2 } };
            config.Validate();
        }

        [Test]
        [Description("Tests invalid cache configuration where CacheExtraChunks is off and view-range is not equal to cache-range.")]
        [ExpectedException(typeof(CacheConfigurationException),
            ExpectedMessage = "Cache range must be greater view range when CacheExtraChunk option is set to true.", MatchType = MessageMatch.Exact)]
        public void TestInvalidCacheRangeNotGreaterThanViewRangeWhenCacheExtraChunksIsOn()
        {
            var config = new EngineConfiguration { CacheConfiguration = { CacheExtraChunks = true, ViewRange = 2, CacheRange = 2 } };
            config.Validate();
        }
    }
}
