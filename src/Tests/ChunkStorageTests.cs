using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using VoxeliqGame;
using VoxeliqEngine.Chunks;
using VoxeliqEngine.Common.Vector;

namespace Tests
{
    [TestFixture]
    public class ChunkStorageTests
    {
        private Game _game;
        private ChunkStorage _chunkStorage;
        private Chunk _chunk;

        [SetUp]
        public void Init()
        {
            _game = new Game();
            _chunkStorage = new ChunkStorage(_game);
            _chunk = new Chunk(new Vector2Int(0, 0));
        }

        [Test]
        [Description("Tests the internal dictionary used by the chunk storage.")]
        public void TestChunkStorage()
        {
            _chunkStorage[0, 0] = _chunk;

            Assert.IsTrue(_chunkStorage.ContainsKey(0,0));
            Assert.AreEqual(_chunk, _chunkStorage[0, 0]);

            _chunkStorage.Remove(0, 0);

            Assert.IsFalse(_chunkStorage.ContainsKey(0, 0));

            _chunkStorage[-1, -1] = new Chunk(new Vector2Int(-1, -1));

            Assert.IsTrue(_chunkStorage.ContainsKey(-1, -1));
        }
    }
}
