using NUnit.Framework;
using VoxeliqEngine.Blocks;

namespace EngineTests.Blocks
{
    [TestFixture]
    public class BlockTypeTests
    {
        private Block _emptyBlock;

        [SetUp]
        public void Init()
        {
            _emptyBlock = Block.Empty;
        }

        [Test]
        [Description("Tests blocks.")]
        public void TestEmptyBlock()
        {
            Assert.AreEqual(_emptyBlock.Type, BlockType.None); // check if we correctly get the empty block.
            Assert.IsFalse(_emptyBlock.Exists); // check if exists works.
        }
    }
}
