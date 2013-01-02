using NUnit.Framework;
using VoxeliqEngine.Blocks;

namespace EngineTests.Blocks
{
    [TestFixture]
    public class BlockTests
    {
        private Block _emptyBlock;

        [SetUp]
        public void Init()
        {
            _emptyBlock = Block.Empty;
        }

        [Test]
        [Description("Tests blocks.")]
        public void TestChunkStorage()
        {
            Assert.AreEqual(_emptyBlock.Type, BlockType.None); // check if we correctly get the empty block.
            Assert.IsFalse(_emptyBlock.Exists); // check if exists works.
        }
    }
}
