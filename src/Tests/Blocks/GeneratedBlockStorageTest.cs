using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using VoxeliqEngine.Chunks;
using VoxeliqEngine.Core;
using VoxeliqEngine.Graphics;
using VoxeliqEngine.Universe;
using VoxeliqGame;

namespace EngineTests.Blocks
{
    [TestFixture]
    public class GeneratedBlockStorageTest
    {
        private Game _game;
        private EngineConfiguration _config;
        private VoxeliqEngine.Core.Engine _engine;
        
        private ChunkStorage _chunkStorage;
        private Camera _camera;
        private Player _player;
        private ChunkCacheTester _chunkCacheTester;
        private WorldTester _worldTester;

        [SetUp]
        public void Init()
        {
            this._game = new Game();
            this._config = new EngineConfiguration();
            this._engine = new VoxeliqEngine.Core.Engine(this._game, this._config);

            this._chunkStorage=new ChunkStorage(this._game);
            this._game.Components.Add(this._chunkStorage);

            this._worldTester = new WorldTester(this._game, this._chunkStorage, this._chunkCacheTester);
            this._game.Components.Add(this._worldTester);

            this._camera=new Camera(this._game);
            this._game.Components.Add(this._camera);

            this._player = new Player(this._game, this._worldTester);
            this._game.Components.Add(this._player);

            this._chunkCacheTester = new ChunkCacheTester(this._game);
            this._game.Components.Add(_chunkCacheTester);

            this._chunkCacheTester.Init();
            this._worldTester.Init();
        }

        [Test]
        [Description("Tests BlockStorage.")]
        public void TestAllBlocksInCacheRange()
        {
            this._chunkCacheTester.CalculateBoundingBoxes();
            this._chunkCacheTester.StartTerrainGeneration();
        }
    }

    public class ChunkCacheTester:ChunkCache
    {
        public ChunkCacheTester(Microsoft.Xna.Framework.Game game)
            : base(game)
        { }

        public void Init()
        {
            this.Initialize();
        }

        public void CalculateBoundingBoxes()
        {
            this.UpdateBoundingBoxes();
        }

        public void StartTerrainGeneration()
        {
            this.Process();
        }
    }

    public class WorldTester : World
    {
        public WorldTester(Microsoft.Xna.Framework.Game game, ChunkStorage chunkStorage, ChunkCache chunkCache)
            : base(game, chunkStorage, chunkCache)
        { }

        public void Init()
        {
            this.Initialize();
        }
    }
}
