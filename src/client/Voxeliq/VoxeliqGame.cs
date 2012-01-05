using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        private GraphicsDeviceManager _graphicsDeviceManager; // graphics device manager.     
        private GraphicsManager _graphicsManager; // graphics manager.

        private static readonly Logger Logger = LogManager.CreateLogger(); // loggin-facility.

        public VoxeliqGame()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            this._graphicsManager = new GraphicsManager(this, this._graphicsDeviceManager);
            this.AddGameComponents(); // add game-components.

            base.Initialize();
        }

        private void AddGameComponents()
        {
            this.Components.Add(new InputManager(this) {UpdateOrder = 0});
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        { }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.WhiteSmoke);

            base.Draw(gameTime);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        { }
    }
}
