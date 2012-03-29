/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using System.Reflection;
using Microsoft.Xna.Framework;
using VolumetricStudios.LibVolumetric.Logging;
using VolumetricStudios.VoxeliqGame.Chunks;
using VolumetricStudios.VoxeliqGame.Chunks.Processors;
using VolumetricStudios.VoxeliqGame.Debugging;
using VolumetricStudios.VoxeliqGame.Effects.PostProcessing.Bloom;
using VolumetricStudios.VoxeliqGame.Graphics;
using VolumetricStudios.VoxeliqGame.Interface;
using VolumetricStudios.VoxeliqGame.Managers;
using VolumetricStudios.VoxeliqGame.Universe;
using InputManager = VolumetricStudios.VoxeliqGame.Input.InputManager;

#if XNA
using DigitalRune.Game.Input;
using DigitalRune.Game.UI;
using VolumetricStudios.VoxeliqGame.UI;
#endif

namespace VolumetricStudios.VoxeliqGame
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
        /// Rasterizer helper.
        /// </summary>
        public readonly Rasterizer Rasterizer;

        /// <summary>
        /// Screen manager.
        /// </summary>
        public GraphicsManager ScreenManager { get; private set; }

        #if XNA
        private DigitalRune.Game.Input.InputManager _inputManager;
        private UIManager _uiManager;
        #endif

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
            this.Rasterizer = new Rasterizer();
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

            this.Components.Add(new Sky(this));
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
            Components.Add(bloom);
            #endif

            this.Components.Add(new Camera(this));
            this.Components.Add(new UserInterface(this));

            this.Components.Add(new InGameDebugger(this));
            this.Components.Add(new Statistics(this));
            this.Components.Add(new StatisticsGraphs(this));

            #if XNA
            this.Components.Add(new MusicManager(this));

            this._inputManager = new DigitalRune.Game.Input.InputManager(false);
            Services.AddService(typeof(IInputService), this._inputManager);

            this._uiManager = new UIManager(this, _inputManager);
            Services.AddService(typeof(IUIService), this._uiManager);

            this.Components.Add(new GameScreenOverlay(this));

            // The component that shows a debugging console.
            this.Components.Add(new DebugConsole(this));
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

            var deltaTime = gameTime.ElapsedGameTime;

            #if XNA
            // Update input manager. The input manager gets the device states and performs other work.
            this._inputManager.Update(deltaTime);

            // Update UI manager. The UI manager updates all registered UIScreens.
            this._uiManager.Update(deltaTime);
            #endif

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

            this.GraphicsDevice.RasterizerState = this.Rasterizer.State;

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