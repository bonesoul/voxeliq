/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Client.Settings.Readers;
using Engine.Common.Logging;
using Engine.Core.Config;
using Engine.Debugging.Timing;
using Engine.Graphics;
using Engine.Graphics.Effects.PostProcessing.Bloom;
using Engine.Platforms;
using Microsoft.Xna.Framework;

namespace Client
{
    /// <summary>
    /// The game client.
    /// </summary>
    public class GameClient : Game
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
        public GameClient()
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
            this.Window.Title = string.Format("Voxeliq [{0}/{1}]", PlatformManager.GameFramework, PlatformManager.GraphicsApi); // set the window title.

            this.IsMouseVisible = false;

            // read settings.
            var audioSettings = new AudioSettings();
            var graphicsSettings = new GraphicsSettings();

            // create a new engine configuration.
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
                World =
                {
                    IsInfinitive = true,
                },
                Debugging =
                {
                    GraphsEnabled = true,
                },
                Bloom =
                {
                    Enabled = false,
                    State = BloomState.Saturated,
                },
                Audio =
                {
                    Enabled = audioSettings.Enabled,
                }
            };

            var engine = new Engine.Core.Engine(this, config);
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
 
			var skyColor = new Color(128, 173, 254);
            this.GraphicsDevice.Clear(skyColor);

            this.GraphicsDevice.RasterizerState = Engine.Core.Engine.Instance.Rasterizer.State;

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