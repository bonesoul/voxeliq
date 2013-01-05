using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;
using VoxeliqEngine.Blocks;
using VoxeliqEngine.Chunks;
using VoxeliqEngine.Chunks.Generators.Biomes;
using VoxeliqEngine.Chunks.Generators.Terrain;
using VoxeliqEngine.Common.Vector;
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
        private BiomedTerrainTester _terrainGenerator;
        private Dictionary<Tuple<int,int>, Chunk> _chunkValidationDictionary;
        private Dictionary<int, BlockType> _blockValidationDictionary;

        private int _cacheXStartIndex;
        private int _cacheXEndIndex;
        private int _cacheZStartIndex;
        private int _cacheZEndIndex;

        [SetUp]
        public void Init()
        {
            this._game = new Game();
            this._config = new EngineConfiguration();
            this._engine = new VoxeliqEngine.Core.Engine(this._game, this._config);

            this._chunkStorage = new ChunkStorage(this._game);
            this._game.Components.Add(this._chunkStorage);

            this._worldTester = new WorldTester(this._game, this._chunkStorage, this._chunkCacheTester);
            this._game.Components.Add(this._worldTester);

            this._camera = new Camera(this._game);
            this._game.Components.Add(this._camera);

            this._player = new Player(this._game, this._worldTester);
            this._game.Components.Add(this._player);

            this._chunkCacheTester = new ChunkCacheTester(this._game);
            this._game.Components.Add(_chunkCacheTester);

            var cacheWidthInBlocks = ((_config.CacheConfiguration.CacheRange * 2) + 1) * _config.ChunkConfiguration.WidthInBlocks;
            var cacheLenghtInBlocks = ((_config.CacheConfiguration.CacheRange * 2) + 1) * _config.ChunkConfiguration.LenghtInBlocks;

            this._cacheXStartIndex = -cacheWidthInBlocks / 2;
            this._cacheXEndIndex = cacheWidthInBlocks / 2;

            this._cacheZStartIndex = -cacheLenghtInBlocks / 2;
            this._cacheZEndIndex = cacheLenghtInBlocks / 2;

            this._chunkCacheTester.Init();
            this._worldTester.Init();

            this._chunkCacheTester.CalculateBoundingBoxes();
            this._chunkCacheTester.StartTerrainGeneration();

            this.InitValidationDictionary();
        }

        [Test]
        [Description("Tests BlockStorage.")]
        public void TestAllBlocksInCacheRange()
        {
            Assert.AreEqual(this._chunkStorage.Count,this._chunkValidationDictionary.Values.Count);
            Assert.AreEqual(BlockStorage.Blocks.Length, this._blockValidationDictionary.Values.Count);
            Assert.AreEqual(this._blockValidationDictionary.Values.Count, _config.CacheConfiguration.CacheRangeVolume);

            // read them back
            for (var x = this._cacheXStartIndex; x < this._cacheXEndIndex; x++)
            {
                for (var z = this._cacheZStartIndex; z < this._cacheZEndIndex; z++)
                {
                    var offset = BlockStorage.BlockIndexByWorldPosition(x, z);

                    for (var y = 0; y < _config.ChunkConfiguration.HeightInBlocks; y++)
                    {
                        var index = offset + y;

                        var expectedType = this._blockValidationDictionary[index];

                        var blockIndexed = BlockStorage.Blocks[index];
                        Assert.AreEqual(expectedType, blockIndexed.Type);

                        var blockFastAt = BlockStorage.FastBlockAt(x, y, z);
                        Assert.AreEqual(expectedType, blockFastAt.Type);
                    }
                }
            }     
        }

        private void InitValidationDictionary()
        {
            this._chunkValidationDictionary = new Dictionary<Tuple<int, int>, Chunk>();
            this._blockValidationDictionary = new Dictionary<int, BlockType>();

            var cacheConfig = VoxeliqEngine.Core.Engine.Instance.Configuration.CacheConfiguration;
            var spawnPoint = new Vector2Int(0, 0);

            for (int z = -cacheConfig.CacheRange; z <= cacheConfig.CacheRange; z++)
            {
                for (int x = -cacheConfig.CacheRange; x <= cacheConfig.CacheRange; x++)
                {
                    var chunk = new Chunk(new Vector2Int(spawnPoint.X + x, spawnPoint.Z + z));
                    this._chunkValidationDictionary.Add( new Tuple<int, int>(chunk.RelativePosition.X, chunk.RelativePosition.Z), chunk);
                }
            }

            this._terrainGenerator = new BiomedTerrainTester(new RainForestTester(this._blockValidationDictionary),this._blockValidationDictionary);

            foreach (var chunk in this._chunkValidationDictionary.Values)
            {
                if (!this._chunkCacheTester.IsChunkInCacheRange(chunk)) 
                    continue;

                if (chunk.ChunkState != ChunkState.AwaitingGenerate)
                    continue;

                this._terrainGenerator.Generate(chunk);
            }
        }
    }

    public class BiomedTerrainTester:BiomedTerrain
    {
        private readonly Dictionary<int, BlockType> _blockValidationDictionary; 

        public BiomedTerrainTester(BiomeGenerator biomeGenerator, Dictionary<int, BlockType> blockValidationDictionary)
            : base(biomeGenerator)
        {
            this._blockValidationDictionary = blockValidationDictionary;
        }

        protected override void GenerateBlocks(Chunk chunk, int worldPositionX, int worldPositionZ)
        {
            var rockHeight = this.GetRockHeight(worldPositionX, worldPositionZ);
            var dirtHeight = this.GetDirtHeight(worldPositionX, worldPositionZ, rockHeight);

            var offset = BlockStorage.BlockIndexByWorldPosition(worldPositionX, worldPositionZ);

            for (int y = Chunk.MaxHeightIndexInBlocks; y >= 0; y--)
            {
                if (y > dirtHeight) // air
                {
                    this._blockValidationDictionary.Add(offset + y, BlockType.None);
                }
                else if (y > rockHeight) // dirt level
                {
                    this._blockValidationDictionary.Add(offset + y, BlockType.Dirt);
                }
                else // rock level
                {
                    this._blockValidationDictionary.Add(offset + y, BlockType.Rock);
                }
            }

            // apply the biome generator on x-z column.
            this.BiomeGenerator.ApplyBiome(chunk, dirtHeight, offset + dirtHeight, worldPositionX, worldPositionZ);
        }
    }

    public class RainForestTester:RainForest
    {
        private readonly Dictionary<int, BlockType> _blockValidationDictionary; 

        public RainForestTester(Dictionary<int, BlockType> blockValidationDictionary)
        {
            this._blockValidationDictionary = blockValidationDictionary;
        }

        public override void ApplyBiome(Chunk chunk, int groundLevel, int groundOffset, int worldPositionX, int worldPositionZ)
        {
            this._blockValidationDictionary[groundOffset + 1] = BlockType.Grass;
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
