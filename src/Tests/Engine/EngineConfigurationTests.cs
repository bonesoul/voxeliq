using NUnit.Framework;
using VoxeliqEngine.Engine;

namespace EngineTests.Engine
{
    [TestFixture]
    public class EngineConfigurationTests
    {
        [Test]
        [Description("Tests default engine configurations.")]
        public void TestDefaultConfig()
        {
            Assert.IsTrue(EngineConfiguration.Instance.Validate()); // expect config validation to succeed.

            #region validate chunk configuration 

            Assert.IsNotNull(EngineConfiguration.Instance.ChunkConfiguration); // chunk configuration should exist.
            Assert.IsNotNull(EngineConfiguration.Instance.CacheConfiguration); // cache configuratio should exists.

            // make sure default dimensions are valid.
            Assert.Greater(EngineConfiguration.Instance.ChunkConfiguration.WidthInBlocks, 0);
            Assert.Greater(EngineConfiguration.Instance.ChunkConfiguration.HeightInBlocks, 0);
            Assert.Greater(EngineConfiguration.Instance.ChunkConfiguration.LenghtInBlocks, 0);

            // calculate expected chunk volume in blocks
            var expectedChunkVolumeInBlocks = EngineConfiguration.Instance.ChunkConfiguration.WidthInBlocks*
                                      EngineConfiguration.Instance.ChunkConfiguration.HeightInBlocks*
                                      EngineConfiguration.Instance.ChunkConfiguration.LenghtInBlocks;
            Assert.AreEqual(EngineConfiguration.Instance.ChunkConfiguration.Volume, expectedChunkVolumeInBlocks);

            // make sure max-width-index is valid.
            Assert.AreEqual(EngineConfiguration.Instance.ChunkConfiguration.MaxWidthInBlocks,
                            EngineConfiguration.Instance.ChunkConfiguration.WidthInBlocks - 1);

            // make sure max-height-index is valid.
            Assert.AreEqual(EngineConfiguration.Instance.ChunkConfiguration.MaxHeightInBlocks,
                            EngineConfiguration.Instance.ChunkConfiguration.HeightInBlocks - 1);

            // make sure max-lenght-index is valid.
            Assert.AreEqual(EngineConfiguration.Instance.ChunkConfiguration.MaxLenghtInBlocks,
                            EngineConfiguration.Instance.ChunkConfiguration.LenghtInBlocks - 1);

            #endregion

            #region validate cache configuration

            // make sure default dimensions are valid.
            Assert.Greater(EngineConfiguration.Instance.CacheConfiguration.ViewRange, 0);
            Assert.Greater(EngineConfiguration.Instance.CacheConfiguration.CacheRange, 0);

            // make sure cache-dimension in blocks are calculated correctly.
            var expectedCacheWidthInBlocks = (EngineConfiguration.Instance.CacheConfiguration.CacheRange*2 + 1)*
                                             EngineConfiguration.Instance.ChunkConfiguration.WidthInBlocks;

            var expectedCacheHeightInBlocks =  EngineConfiguration.Instance.ChunkConfiguration.HeightInBlocks;

            var expectedCacheLenghtInBlocks = (EngineConfiguration.Instance.CacheConfiguration.CacheRange*2 + 1)*
                                              EngineConfiguration.Instance.ChunkConfiguration.LenghtInBlocks;

            Assert.AreEqual(EngineConfiguration.Instance.CacheConfiguration.CacheWidthInBlocks, expectedCacheWidthInBlocks);
            Assert.AreEqual(EngineConfiguration.Instance.CacheConfiguration.CacheHeightInBlocks, expectedCacheHeightInBlocks);
            Assert.AreEqual(EngineConfiguration.Instance.CacheConfiguration.CacheLenghtInBlocks, expectedCacheLenghtInBlocks);

            // if by default, cache-extra-chunks option is set to true, make sure that default cache-range > default view-range.
            if (EngineConfiguration.Instance.CacheConfiguration.CacheExtraChunks)
                Assert.Greater(EngineConfiguration.Instance.CacheConfiguration.CacheRange, EngineConfiguration.Instance.CacheConfiguration.ViewRange,
                    "Cache range must be greater view range when CacheExtraChunk option is set to true.");
            else // if by default, cache-extra-chunks option is set to false, make sure that default cache-range = default view-range.
                Assert.AreEqual(EngineConfiguration.Instance.CacheConfiguration.ViewRange, EngineConfiguration.Instance.CacheConfiguration.CacheRange,
                    "Cache range can not be different than view range when CacheExtraChunk option is set to false.");

            #endregion
        }

        [Test]
        [Description("Tests invalid engine configurations.")]
        public void TestInvalidConfig()
        {
            
        }

        [Test]
        [Description("Tests valid engine configurations.")]
        public void TestValidConfig()
        {

        }
    }
}
