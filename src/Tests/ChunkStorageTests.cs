using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Voxeliq.Game;
using VoxeliqEngine.Chunks;
using VoxeliqEngine.Common.Vector;

namespace Tests
{
    [TestClass]
    public class ChunkStorageTests
    {
        [TestMethod]
        [Description("Tests the internal dictionary used by the chunk storage.")]
        public void TestChunkStorage()
        {
            var game = new SampleGame();
            var chunkStorage = new ChunkStorage(game);
            var chunk = new Chunk(new Vector2Int(0, 0));

            chunkStorage[0, 0] = chunk;

            Assert.IsTrue(chunkStorage.ContainsKey(0,0));
            Assert.AreEqual(chunk, chunkStorage[0, 0]);

            chunkStorage.Remove(0, 0);

            Assert.IsFalse(chunkStorage.ContainsKey(0, 0));

            chunkStorage[-1, -1] = new Chunk(new Vector2Int(-1, -1));

            Assert.IsTrue(chunkStorage.ContainsKey(-1, -1));
        }
    }
}
