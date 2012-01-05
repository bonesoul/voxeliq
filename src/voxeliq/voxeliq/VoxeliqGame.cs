using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;
using VolumetricStudios.Voxeliq.Environment;
using VolumetricStudios.Voxeliq.Input;
using VolumetricStudios.VoxeliqEngine.Logging;

namespace VolumetricStudios.Voxeliq
{
    public class VoxeliqGame : Game
    {
        // The SunBurn lighting system.
        LightingSystemManager lightingSystemManager;
        FrameBuffers frameBuffers;
        SplashScreenGameComponent splashScreenGameComponent;

        // Scene related members.
        public SceneState SceneState { get; private set; }

        SceneInterface sceneInterface;
        ContentRepository contentRepository;
        SceneEnvironment environment;

        // Default XNA members.
        GraphicsDeviceManager graphics;

        // Controller related.
        const float moveScale = 100.0f;

        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility.

        public VoxeliqGame()
        {
            // Default XNA setup.
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;

            // Used for advanced edge cleanup.
            graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;

            // Required for lighting system.
            splashScreenGameComponent = new SplashScreenGameComponent(this, graphics);
            Components.Add(splashScreenGameComponent);

            // Create the lighting system.
            lightingSystemManager = new LightingSystemManager(Services, Content);
            SceneState = new SceneState();

            // Create the scene interface. Acts as a service provider containing all scene managers
            // and returning them by type (including custom managers). Also acts as a component
            // container where calls to manager methods on the SceneInterface (such as BeginFrameRendering,
            // Unload, ...) are automatically called on all contained managers.
            //
            // This design allows managers to be plugged-in like modular components and for managers
            // to easily be added, removed, or replaced with custom implementations.
            //
            sceneInterface = new SceneInterface(graphics);
            sceneInterface.CreateDefaultManagers(false);

            // Create the frame buffers used for rendering (sized to the backbuffer) and
            // assign them to the ResourceManager so we don't have to worry about cleanup.
            frameBuffers = new FrameBuffers(graphics, DetailPreference.High, DetailPreference.Medium);
            sceneInterface.ResourceManager.AssignOwnership(frameBuffers);

            // Post console messages letting the user know how to open the SunBurn Editor.
            sceneInterface.ShowConsole = true;
            sceneInterface.ShowStatistics = true;
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

            var title = "Voxeliq " + Assembly.GetExecutingAssembly().GetName().Version;
            this.Window.Title = title;
            SystemConsole.AddMessage(title,5);

            this.Components.Add(new InputManager(this));
            this.Components.Add(new Sky(this));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load the content repository, which stores all assets imported via the editor.
            // This must be loaded before any other assets.
            contentRepository = Content.Load<ContentRepository>("Content");

            // Add objects and lights to the ObjectManager and LightManager. They accept
            // objects and lights in several forms:
            //
            //   -As scenes containing both dynamic (movable) and static objects and lights.
            //
            //   -As SceneObjects and lights, which can be dynamic or static, and
            //    (in the case of objects) are created from XNA Models or custom vertex / index buffers.
            //
            //   -As XNA Models, which can only be static.
            //

            // Load the scene and add it to the managers.
            Scene scene = Content.Load<Scene>("Scenes/Scene");

            sceneInterface.Submit(scene);

            // Load the scene environment settings.
            environment = Content.Load<SceneEnvironment>("Environment/Environment");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

            // Cleanup any used resources.
            sceneInterface.Unload();
            lightingSystemManager.Unload();

            environment = null;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Enables game input / control when not editing the scene (the editor provides its own control).
            if (!sceneInterface.Editor.EditorAttached)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    this.Exit();

                // Calculate the view.
                view = ProcessCameraInput(gameTime);
            }

            // Calculate the projection.
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70.0f),
                GraphicsDevice.Viewport.AspectRatio, 0.1f, environment.VisibleDistance);

            // Update all contained managers.
            sceneInterface.Update(gameTime);

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Check to see if the splash screen is finished.
            if (!SplashScreenGameComponent.DisplayComplete)
            {
                base.Draw(gameTime);
                return;
            }

            // Render the scene.
            SceneState.BeginFrameRendering(view, projection, gameTime, environment, frameBuffers, true);
            sceneInterface.BeginFrameRendering(SceneState);

            // Add custom rendering that should occur before the scene is rendered.

            sceneInterface.RenderManager.Render();

             //Add custom rendering that should occur after the scene is rendered.

            sceneInterface.EndFrameRendering();
            SceneState.EndFrameRendering();

            base.Draw(gameTime);
        }


        #region Controller code

        // Scene/camera supporting members.
        bool firstMouseSample = true;
        Vector3 viewPosition = new Vector3(86.5f, 11.2f, 57.0f);
        Vector3 viewRotation = new Vector3(-2.2f, 0.16f, 0.0f);
        Matrix view = Matrix.Identity;
        Matrix projection = Matrix.Identity;

        /// <summary>
        /// Handles controller input.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public Matrix ProcessCameraInput(GameTime gameTime)
        {
            if (IsActive)
            {
                GamePadState gamepad = GamePad.GetState(PlayerIndex.One);
                KeyboardState keyboard = Keyboard.GetState();
                MouseState mouse = Mouse.GetState();

                float timescale = (float)gameTime.ElapsedGameTime.TotalSeconds;
                float rotatescale = 3.0f * timescale;
                float movescale = timescale * moveScale;

                // Get the right trigger, which affects speed.
                if (gamepad.IsConnected)
                {
                    rotatescale *= (1.0f - gamepad.Triggers.Right * 0.5f);
                    movescale *= (1.0f - gamepad.Triggers.Right * 0.5f);
                }
                else if (mouse.RightButton == ButtonState.Pressed)
                    movescale *= 0.25f;

                // If the gamepad is connected use its input instead of the mouse and keyboard.
                if (gamepad.IsConnected)
                    viewRotation -= new Vector3(gamepad.ThumbSticks.Right.X * rotatescale, gamepad.ThumbSticks.Right.Y * rotatescale, 0.0f);
                else
                {
                    GraphicsDevice device = GraphicsDevice;
                    int halfx = device.Viewport.Width / 2;
                    int halfy = device.Viewport.Height / 2;

                    if (!firstMouseSample)
                    {
                        // Convert the amount the mouse was moved into camera rotation.
                        viewRotation.X += MathHelper.ToRadians((float)(halfx - mouse.X) * rotatescale * 1.5f);
                        viewRotation.Y -= MathHelper.ToRadians((float)(halfy - mouse.Y) * rotatescale * 1.5f);
                    }
                    else
                        firstMouseSample = false;

                    Mouse.SetPosition(halfx, halfy);
                }

                if (viewRotation.Y > MathHelper.PiOver2 - 0.01f)
                    viewRotation.Y = MathHelper.PiOver2 - 0.01f;
                else if (viewRotation.Y < -MathHelper.PiOver2 + 0.01f)
                    viewRotation.Y = -MathHelper.PiOver2 + 0.01f;

                Quaternion rot = Quaternion.CreateFromYawPitchRoll(viewRotation.X, viewRotation.Y, viewRotation.Z);

                // Now apply the camera movement based on either the gamepad or keyboard input.
                if (gamepad.IsConnected)
                {
                    viewPosition += Vector3.Transform(new Vector3(movescale, 0, movescale) * new Vector3(
                        -gamepad.ThumbSticks.Left.X, 0,
                        gamepad.ThumbSticks.Left.Y), rot);
                }
                else
                {
                    Vector3 move = new Vector3();
                    if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
                        move.Z += 1.0f;
                    if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
                        move.Z -= 1.0f;
                    if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
                        move.X += 1.0f;
                    if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
                        move.X -= 1.0f;
                    viewPosition += Vector3.Transform(new Vector3(movescale, 0, movescale) * move, rot);
                }
            }

            // mouse visibility...
            if (!IsActive || GamePad.GetState(PlayerIndex.One).IsConnected)
                IsMouseVisible = true;
            else
                IsMouseVisible = false;

            // Convert the camera rotation and movement into a view transform.
            return GetViewMatrix();
        }

        /// <summary>
        /// Convert the camera rotation and movement into a view transform.
        /// </summary>
        /// <returns></returns>
        private Matrix GetViewMatrix()
        {
            Matrix rotation = Matrix.CreateFromYawPitchRoll(viewRotation.X, viewRotation.Y, viewRotation.Z);
            Vector3 target = viewPosition + Vector3.Transform(Vector3.Backward, rotation);
            return Matrix.CreateLookAt(viewPosition, target, Vector3.Up);
        }

        #endregion
    }
}
