/*
 * Copyright (C) 2011 - 2013 Int6 Studios - http://www.int6.org,
 * Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Engine.Blocks;
using NUnit.Framework;

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
