﻿/*
 * Copyright (C) 2011 - 2013 Int6 Studios - http://www.int6.org,
 * Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using NUnit.Framework;
using VoxeliqEngine.Core;
using VoxeliqEngine.Core.Config;
using VoxeliqGame;
using VoxeliqEngine.Chunks;
using VoxeliqEngine.Common.Vector;

namespace EngineTests.Chunks
{
    [TestFixture]
    public class ChunkStorageTests
    {
        private SampleGame _game;
        private EngineConfig _config;
        private Engine _engine;
        private ChunkStorage _chunkStorage;
        private Chunk _chunk;

        [SetUp]
        public void Init()
        {
            _game = new SampleGame();
            this._config = new EngineConfig();

            if(Engine.Instance!=null) // if there exists already an engine instance, dispose it first.
                Engine.Instance.Dispose(); 

            this._engine = new Engine(this._game, this._config);
            this._chunkStorage = new ChunkStorage(_game);
            this._chunk = new Chunk(new Vector2Int(0, 0));
        }

        /// <summary>
        /// Teardown method that resets any existing instances.
        /// </summary>
        [TearDown]
        public void Dispose()
        {
            VoxeliqEngine.Core.Engine.Instance.Dispose();
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
