/*
 * Copyright (C) 2011 voxeliq project 
 *
 */

using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VolumetricStudios.VoxeliqClient.Screen;
using VolumetricStudios.VoxeliqClient.States;
using VolumetricStudios.VoxeliqEngine.Common.Logging;

namespace VolumetricStudios.VoxeliqClient
{
    /// <summary>
    /// The game client.
    /// </summary>
    public class Client : Game
    {
        /// <summary>
        /// Graphics device manager.
        /// </summary>
        private readonly GraphicsDeviceManager _graphicsDeviceManager;       

        /// <summary>
        /// Wire-framed rasterizer.
        /// </summary>
        private readonly RasterizerState _wireframedRaster = new RasterizerState() { CullMode = CullMode.CullCounterClockwiseFace, FillMode = FillMode.WireFrame };

        /// <summary>
        /// Normal rasterizer.
        /// </summary>
        private readonly RasterizerState _normalRaster = new RasterizerState() { CullMode = CullMode.CullCounterClockwiseFace, FillMode = FillMode.Solid };

        /// <summary>
        /// Sets if rendering mode is wire-framed.
        /// </summary>
        public bool Wireframed { get; private set; }

        public ScreenManager ScreenManager { get; private set; }

        private static readonly Logger Logger = LogManager.CreateLogger();

        /// <summary>
        /// Creates a new VoxlrClient.
        /// </summary>
        public Client()
        {
            this.Wireframed = false;
            this.Content.RootDirectory = "Content";
            this._graphicsDeviceManager = new GraphicsDeviceManager(this);
        }

        /// <summary>
        /// Initializes the game.
        /// </summary>
        protected override void Initialize()
        {
            Logger.Info("Game:Initialize()");
            this.Window.Title = "Voxeliq Client " + Assembly.GetExecutingAssembly().GetName().Version;
            
            this.ScreenManager = new ScreenManager(this._graphicsDeviceManager, this); // startup the screen manager.
            this.Components.Add(new StateManager(this) { UpdateOrder = 0, ActiveState = new LoadingState(this) });

            base.Initialize();
        }

        /// <summary>
        /// Main logic update.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) 
                this.Exit(); // exit the game on escepa key.

            base.Update(gameTime);
        }

        /// <summary>
        /// Draw function.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.RasterizerState = !this.Wireframed ? this._normalRaster : this._wireframedRaster; // set the rasterizer state.

            base.Draw(gameTime);
        }

        public void ToggleRasterMode()
        {
            this.Wireframed = !this.Wireframed;
        }
    }
}
