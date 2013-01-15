/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Microsoft.Xna.Framework;
using VoxeliqEngine.Common.Logging;
using VoxeliqEngine.Common.Versions;
using VoxeliqEngine.Core;
using VoxeliqEngine.Core.Config;
using VoxeliqEngine.Debugging.Timing;
using VoxeliqEngine.Graphics;
using VoxeliqEngine.Graphics.Effects.PostProcessing.Bloom;
using VoxeliqGame.Settings.Readers;

namespace VoxeliqGame
{
    /// <summary>
    /// The game client.
    /// </summary>
    public class SampleGame : Game
    {
        /// <summary>
        /// Graphics device manager.
        /// </summary>
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        /// <summary>
        /// Screen manager.
        /// </summary>
        public GraphicsManager ScreenManager { get; private set; }

        private TimeRuler _timeRuler;

        private BloomComponent _bloomComponent;

        /// <summary>
        /// Logging facility.
        /// </summary>
        private static readonly Logger Logger = LogManager.CreateLogger();

        /// <summary>
        /// Creates a new game instance.
        /// </summary>
        public SampleGame()
        {
            this.Content.RootDirectory = "Content"; // set content root directory.
            this._graphicsDeviceManager = new GraphicsDeviceManager(this);
        }

        /// <summary>
        /// Initializes the game.
        /// </summary>
        protected override void Initialize()
        {
            Logger.Trace("init()"); // log the init.
            this.Window.Title = string.Format("Voxeliq [{0}/{1}]", VersionInfo.GameFramework, VersionInfo.GraphicsApi); // set the window title.

            this.IsMouseVisible = false;

            // read settings.
            var audioSettings = new AudioSettings();
            var graphicsSettings = new GraphicsSettings();

            // create a new EngineConfiguration instance.
            var config = new EngineConfig
            {
                Chunk =
                {
                    WidthInBlocks = 16,
                    HeightInBlocks = 128,
                    LenghtInBlocks = 16,
                },
                Cache =
                {
                    CacheExtraChunks = true,
                    ViewRange = 8,
                    CacheRange = 12,
                },
                Graphics =
                {
                    Width = graphicsSettings.Width,
                    Height = graphicsSettings.Height,
                    FullScreenEnabled = graphicsSettings.FullScreenEnabled,
                    VerticalSyncEnabled = graphicsSettings.VerticalSyncEnabled,
                    FixedTimeStepsEnabled = graphicsSettings.FixedTimeStepsEnabled,
                },
                Audio =
                {
                    Enabled = audioSettings.Enabled,
                }
            };

            var engine = new Engine(this, config);
            this.ScreenManager = new GraphicsManager(this._graphicsDeviceManager, this); // start the screen manager.

            engine.EngineStart += OnEngineStart;

            engine.Run();

            base.Initialize();
        }

        private void OnEngineStart(object sender, System.EventArgs e)
        {
            this._timeRuler = new TimeRuler(this) { Visible = true, ShowLog = true };
            this.Components.Add(this._timeRuler);

#if XNA
            this._bloomComponent = new BloomComponent(this);
            this.Components.Add(this._bloomComponent);
#endif
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
            this._bloomComponent.BeginDraw();
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