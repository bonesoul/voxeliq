/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System.Reflection;
using Microsoft.Xna.Framework;
using VoxeliqEngine;
using VoxeliqEngine.Assets;
using VoxeliqEngine.Audio;
using VoxeliqEngine.Chunks;
using VoxeliqEngine.Chunks.Processors;
using VoxeliqEngine.Debugging;
using VoxeliqEngine.Debugging.Graphs;
using VoxeliqEngine.Effects.PostProcessing.Bloom;
using VoxeliqEngine.Graphics;
using VoxeliqEngine.Input;
using VoxeliqEngine.Interface;
using VoxeliqEngine.Logging;
using VoxeliqEngine.Universe;

namespace VoxeliqStudios.Voxeliq
{
    /// <summary>
    /// The game client.
    /// </summary>
    public class VoxeliqGame : Game
    {
        /// <summary>
        /// Graphics device manager.
        /// </summary>
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        /// <summary>
        /// Screen manager.
        /// </summary>
        public GraphicsManager ScreenManager { get; private set; }

        BloomComponent bloom;

        private TimeRuler _timeRuler;

        /// <summary>
        /// Logging facility.
        /// </summary>
        private static readonly Logger Logger = LogManager.CreateLogger();

        /// <summary>
        /// Creates a new VoxlrClient.
        /// </summary>
        public VoxeliqGame()
        {
            this.Content.RootDirectory = "Content"; // set content root directory.
            this._graphicsDeviceManager = new GraphicsDeviceManager(this);
        }

        /// <summary>
        /// Initializes the game.
        /// </summary>
        protected override void Initialize()
        {
            Logger.Trace("init()");
            this.Window.Title = "Voxeliq Client " + Assembly.GetExecutingAssembly().GetName().Version;

            this.ScreenManager = new GraphicsManager(this._graphicsDeviceManager, this); // start the screen manager.

            this.AddComponents(); // add the game compontents.

            base.Initialize();
        }

        /// <summary>
        /// Adds game-components.
        /// </summary>
        private void AddComponents()
        {
            this.Components.Add(new InputManager(this));

            this.Components.Add(new AssetManager(this));

            #if XNA
            this.Components.Add(new Sky(this));
            #endif

            this.Components.Add(new Fogger(this));

            var chunkStorage = new ChunkStorage(this);
            this.Components.Add(chunkStorage);

            var vertexBuilder = new VertexBuilder(this);
            this.Components.Add(vertexBuilder);

            var chunkCache = new ChunkCache(this);
            this.Components.Add(chunkCache);

            var world = new World(this, chunkStorage, chunkCache);
            this.Components.Add(world);

            this.Components.Add(new Player(this, world));

            #if XNA
            bloom = new BloomComponent(this);
            this.Components.Add(bloom);
            #endif

            this.Components.Add(new Camera(this));
            this.Components.Add(new UserInterface(this));

            this.Components.Add(new InGameDebugger(this));
            this.Components.Add(new Statistics(this));
            this.Components.Add(new GraphManager(this));

            #if XNA
            this.Components.Add(new AudioManager(this));
            #endif

            this._timeRuler = new TimeRuler(this);
            this._timeRuler.Visible = true;
            this._timeRuler.ShowLog = true;
            this.Components.Add(this._timeRuler);
        }

        /// <summary>
        /// Allows the game to run logic.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // tell the TimeRuler that we're starting a new frame. you always want
            // to call this at the start of Update
            this._timeRuler.StartFrame();

            this._timeRuler.BeginMark("Update", Color.Blue);

            // Update game components.
            base.Update(gameTime);

            this._timeRuler.EndMark("Update");
        }

        /// <summary>
        /// Draws the scene.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this._timeRuler.BeginMark("Draw", Color.Yellow);

            #if XNA
            bloom.BeginDraw();
            #endif

            this.GraphicsDevice.Clear(Color.Black);

            this.GraphicsDevice.RasterizerState = Rasterizer.Instance.State;

            base.Draw(gameTime);

            // Stop measuring time for "Draw".
            this._timeRuler.EndMark("Draw");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }
    }
}