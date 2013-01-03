using NUnit.Framework;
using VoxeliqEngine.Core;
using VoxeliqGame;
using VoxeliqEngine.Chunks;
using VoxeliqEngine.Common.Vector;

namespace EngineTests.Chunks
{
    [TestFixture]
    public class ChunkStorageTests
    {
        private Game _game;
        private EngineConfiguration _config;
        private VoxeliqEngine.Core.Engine _engine;
        private ChunkStorage _chunkStorage;
        private Chunk _chunk;

        [SetUp]
        public void Init()
        {
            _game = new Game();
            this._config = new EngineConfiguration();
            this._engine = new VoxeliqEngine.Core.Engine(this._game, this._config);
            this._chunkStorage = new ChunkStorage(_game);
            this._chunk = new Chunk(new Vector2Int(0, 0));
        }

        [Test]
        [Description("Tests the internal dictionary used by the chunk storage.")]
        public void TestChunkStorage()
        {
            _chunkStorage[0, 0] = _chunk;
            Assert.IsTrue(_chunkStorage.ContainsKey(0,0)); // check if we can index back the chunk.
            Assert.AreEqual(_chunk, _chunkStorage[0, 0]); // check if we can access it back.

            _chunkStorage.Remove(0, 0);
            Assert.IsFalse(_chunkStorage.ContainsKey(0, 0)); // check if it's correctly removed.

            _chunkStorage[-1, -1] = new Chunk(new Vector2Int(-1, -1));
            Assert.IsTrue(_chunkStorage.ContainsKey(-1, -1)); // check if we can index negative coordinates for chunks.
        }
    }
}
