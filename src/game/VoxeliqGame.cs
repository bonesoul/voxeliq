/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using System.Reflection;
using DigitalRune.Game.Input;
using DigitalRune.Game.UI;
using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqGame.Chunks;
using VolumetricStudios.VoxeliqGame.Chunks.Processors;
using VolumetricStudios.VoxeliqGame.Common.Logging;
using VolumetricStudios.VoxeliqGame.Debugging;
using VolumetricStudios.VoxeliqGame.Effects.PostProcess.Bloom;
using VolumetricStudios.VoxeliqGame.Graphics;
using VolumetricStudios.VoxeliqGame.Interface;
using VolumetricStudios.VoxeliqGame.Managers;
using VolumetricStudios.VoxeliqGame.UI;
using VolumetricStudios.VoxeliqGame.Universe;
using InputManager = VolumetricStudios.VoxeliqGame.Input.InputManager;

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

        private DigitalRune.Game.Input.InputManager _inputManager;
        private UIManager _uiManager;

        BloomComponent bloom;

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

            this.AddComponents(); // add the main compontents.

            base.Initialize();
        }

        /// <summary>
        /// Adds game-components.
        /// </summary>
        private void AddComponents()
        {

            //bloom = new BloomComponent(this);

            //Components.Add(bloom);

            this.Components.Add(new InputManager(this) { UpdateOrder = 0 });
            this.Components.Add(new Sky(this) { UpdateOrder = 1, DrawOrder = 0 });
            this.Components.Add(new Fogger(this) { UpdateOrder = 2 });

            var chunkStorage = new ChunkStorage(this) { UpdateOrder = 3 };
            this.Components.Add(chunkStorage);

            var vertexBuilder = new VertexBuilder(this);
            this.Components.Add(vertexBuilder);

            var chunkCache = new ChunkCache(this) { UpdateOrder = 4, DrawOrder = 1 };
            this.Components.Add(chunkCache);

            var world = new World(this, chunkStorage, chunkCache) { UpdateOrder = 5, DrawOrder = 2 };
            this.Components.Add(world);

            this.Components.Add(new Player(this, world) { UpdateOrder = 6, DrawOrder = 3 });
            this.Components.Add(new Camera(this) { UpdateOrder = 7 });
            this.Components.Add(new UserInterface(this) { UpdateOrder = 8, DrawOrder = 4 });

            this.Components.Add(new InGameDebugger(this) { UpdateOrder = 9, DrawOrder = 5 });
            this.Components.Add(new Statistics(this) { UpdateOrder = 10, DrawOrder = 6 });
            this.Components.Add(new StatisticsGraphs(this) { UpdateOrder = 11, DrawOrder = 7 });

            this.Components.Add(new MusicManager(this) { UpdateOrder = 12 });

            this._inputManager = new DigitalRune.Game.Input.InputManager(false);
            Services.AddService(typeof(IInputService), this._inputManager);

            this._uiManager = new UIManager(this, _inputManager);
            Services.AddService(typeof(IUIService), this._uiManager);

            this.Components.Add(new GameScreenOverlay(this) { UpdateOrder = 13, DrawOrder = 8 });

            // The component that shows a debugging console.
            this.Components.Add(new DebugConsole(this));

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.ElapsedGameTime;

            // Update input manager. The input manager gets the device states and performs other work.
            this._inputManager.Update(deltaTime);

            // Update UI manager. The UI manager updates all registered UIScreens.
            this._uiManager.Update(deltaTime);

            // Update game components.
            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the scene.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            //this.GraphicsDevice.Clear(Color.Black);

            this.GraphicsDevice.RasterizerState = this.Rasterizer.State;

            base.Draw(gameTime);
        }
    }
}