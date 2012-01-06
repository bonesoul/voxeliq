using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VolumetricStudios.VoxeliqEngine.Logging;

namespace VolumetricStudios.Voxeliq.Input
{
    public interface IInputManager
    {
        /// <summary>
        /// Should the game capture mouse?
        /// </summary>
        bool CaptureMouse { get; }

        // Should the mouse cursor centered on screen?
        bool CenterCursor { get; }        
    }

    public class InputManager : GameComponent, IInputManager
    {
        // properties.
        public bool CaptureMouse { get; private set; } // Should the game capture mouse?
        public bool CenterCursor { get; private set; } // Should the mouse cursor centered on screen?

        // previous input states.
        private KeyboardState _previousKeyboardState; // previous keyboard-state.
        private MouseState _previousMouseState; // previous mouse click-state.

        // required services.
        private ICameraController _cameraController;

        // misc.
        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility.

        public InputManager(Game game) : base(game)
        {
            this.Game.Services.AddService(typeof(IInputManager), this); // export the service.

            this.CaptureMouse = true; // capture the mouse by default.
            this.CenterCursor = true; // center the mouse by default.            
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            // import required services.
            this._cameraController = (ICameraController) this.Game.Services.GetService(typeof (ICameraController));

            #if DEBUG // if in debug mode, print debug-keys.
                this.PrintDebugKeys();
            #endif

            // get current mouse & keyboard states.
            this._previousKeyboardState = Keyboard.GetState();
            this._previousMouseState = Mouse.GetState();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            this.ProcessKeyboard(gameTime);
            this.ProcessMouse();
        }

        private void ProcessMouse()
        {
            var currentState = Mouse.GetState();

            if (currentState == this._previousMouseState || !this.CaptureMouse) // if there's no mouse-state change or if it's not captured, just return.
                return;

            float rotation = currentState.X - Graphics.GraphicsConfig.Instance.Width/2;
            if (rotation != 0) this._cameraController.RotateCamera(rotation);

            float elevation = currentState.Y - Graphics.GraphicsConfig.Instance.Height/2;
            if (elevation != 0) this._cameraController.ElevateCamera(elevation);

            if (currentState.LeftButton == ButtonState.Pressed && this._previousMouseState.LeftButton == ButtonState.Released) { }
            if (currentState.RightButton == ButtonState.Pressed && this._previousMouseState.RightButton == ButtonState.Released) { }

            this._previousMouseState = currentState;
            if (CenterCursor) CenterMouseCursor();            
        }

        private void ProcessKeyboard(GameTime gameTime)
        {
            var currentState = Keyboard.GetState();

            if (currentState.IsKeyDown(Keys.Escape) && this._previousKeyboardState.IsKeyUp(Keys.Escape)) // allows quick exiting the game.
                this.Game.Exit();

            if (currentState.IsKeyDown(Keys.Up) || currentState.IsKeyDown(Keys.W)) {}
            if (currentState.IsKeyDown(Keys.Down) || currentState.IsKeyDown(Keys.S)) { }
            if (currentState.IsKeyDown(Keys.Left) || currentState.IsKeyDown(Keys.A)) { }
            if (currentState.IsKeyDown(Keys.Right) || currentState.IsKeyDown(Keys.D)) { }
            if (_previousKeyboardState.IsKeyUp(Keys.Space) && currentState.IsKeyDown(Keys.Space)) { }

            if (currentState.IsKeyDown(Keys.F1) && _previousKeyboardState.IsKeyUp(Keys.F1)) { }
            if (currentState.IsKeyDown(Keys.F2) && _previousKeyboardState.IsKeyUp(Keys.F2)) { }
            if (currentState.IsKeyDown(Keys.F3) && _previousKeyboardState.IsKeyUp(Keys.F3)) { }
            if (currentState.IsKeyDown(Keys.F4) && _previousKeyboardState.IsKeyUp(Keys.F4)) { }
            if (currentState.IsKeyDown(Keys.F5) && _previousKeyboardState.IsKeyUp(Keys.F5)) { }
            if (currentState.IsKeyDown(Keys.F10) && _previousKeyboardState.IsKeyUp(Keys.F10)) { }
            if (currentState.IsKeyDown(Keys.F11) && _previousKeyboardState.IsKeyUp(Keys.F11)) { }
            
            if (currentState.IsKeyDown(Keys.F12) && _previousKeyboardState.IsKeyUp(Keys.F12)) // toggles rasterizer-mode.
                ((VoxeliqGame) this.Game).Rasterizer.ToggleRasterMode();

            this._previousKeyboardState = currentState;
        }

        /// <summary>
        /// Center's the cursor on screen.
        /// </summary>
        private void CenterMouseCursor()
        {
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
        }

        private void PrintDebugKeys()
        {
            Debug.WriteLine("Debug keys:");
            Debug.WriteLine("-----------------------------");
            Debug.WriteLine("F1: Infinitive-world: On/Off.");
            Debug.WriteLine("F2: Fly-mode: On/Off.");
            Debug.WriteLine("F3: Fog-mode: None/Near/Far.");
            Debug.WriteLine("F4: Dynamic Clouds: On/Off.");
            Debug.WriteLine("F5: Window-focus: On/Off.");
            Debug.WriteLine("F10: In-game Debugger: On/Off.");
            Debug.WriteLine("F11: Frame-limiter: On/Off.");
            Debug.WriteLine("F12: Wireframe mode: On/Off.");
            Debug.WriteLine("-----------------------------");
        }
    }
}
