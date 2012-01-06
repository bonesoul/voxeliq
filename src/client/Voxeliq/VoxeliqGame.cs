using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VolumetricStudios.Voxeliq.Environment;
using VolumetricStudios.Voxeliq.Graphics;
using VolumetricStudios.Voxeliq.Input;
using VolumetricStudios.VoxeliqEngine.Logging;

namespace VolumetricStudios.Voxeliq
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class VoxeliqGame : Game
    {        
        // principal stuff.
        private readonly GraphicsDeviceManager _graphicsDeviceManager; // graphics device manager.     
        private GraphicsManager _graphicsManager; // graphics manager.
        public Rasterizer Rasterizer { get; private set; } // the rasterizer.

        // misc stuff.
        private static readonly Logger Logger = LogManager.CreateLogger(); // loggin-facility.

        public VoxeliqGame()
        {
            this.Content.RootDirectory = "Content"; // the content root directory.
            this._graphicsDeviceManager = new GraphicsDeviceManager(this); // the graphics device manager.
            this.Rasterizer = new Rasterizer(); // start the rasterizer.
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Logger.Trace("init()");

            this.Window.Title = "Voxeliq Client " + Assembly.GetExecutingAssembly().GetName().Version;
            this._graphicsManager = new GraphicsManager(this, this._graphicsDeviceManager); // start graphics-manager.
            this.AddGameComponents(); // add game-components.

            base.Initialize();
        }

        private void AddGameComponents()
        {
            this.Components.Add(new InputManager(this) {UpdateOrder = 0});
            this.Components.Add(new Player(this) {UpdateOrder = 1});
            this.Components.Add(new Camera(this) {UpdateOrder = 2});
            this.Components.Add(new Sky(this) { UpdateOrder = 3, DrawOrder = 0 });
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.RasterizerState = this.Rasterizer.State;
            base.Draw(gameTime);
        }
    }
}
