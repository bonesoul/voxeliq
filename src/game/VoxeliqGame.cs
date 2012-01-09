/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using System.Reflection;
using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqEngine;
using VolumetricStudios.VoxeliqEngine.Chunks;
using VolumetricStudios.VoxeliqEngine.Common.Logging;
using VolumetricStudios.VoxeliqGame.Debugging;
using VolumetricStudios.VoxeliqGame.Environment;
using VolumetricStudios.VoxeliqGame.Graphics;
using VolumetricStudios.VoxeliqGame.Input;
using VolumetricStudios.VoxeliqGame.Interface;
using VolumetricStudios.VoxeliqGame.Worlds;

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
            this.Components.Add(new InputManager(this) { UpdateOrder = 0 });
            this.Components.Add(new Sky(this) { UpdateOrder = 1, DrawOrder = 0 });                       
            this.Components.Add(new Fogger(this) { UpdateOrder = 2 });
            
            var chunkStorage = new ChunkStorage(this) {UpdateOrder = 3};
            this.Components.Add(chunkStorage);

            var chunkCache = new ChunkCache(this) {UpdateOrder = 4, DrawOrder = 1};
            this.Components.Add(chunkCache);

            var world = new GameWorld(this, chunkStorage, chunkCache) { UpdateOrder = 5, DrawOrder = 2 };
            this.Components.Add(world);

            this.Components.Add(new Player(this, world) { UpdateOrder = 6, DrawOrder = 3 });
            this.Components.Add(new Camera(this) { UpdateOrder = 7 });                                                    
            this.Components.Add(new UserInterface(this) { UpdateOrder = 8, DrawOrder = 4 });
            this.Components.Add(new InGameDebugger(this) { UpdateOrder = 9, DrawOrder = 5 });
            this.Components.Add(new Statistics(this) { UpdateOrder = 10, DrawOrder = 6 });
            this.Components.Add(new StatisticsGraphs(this) {UpdateOrder = 11, DrawOrder = 7 });
        }

        /// <summary>
        /// Draws the scene.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.RasterizerState = this.Rasterizer.State;
                
            base.Draw(gameTime);
        }
    }
}
