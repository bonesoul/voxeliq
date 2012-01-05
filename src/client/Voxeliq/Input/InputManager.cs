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
        public bool CaptureMouse { get; private set; } // Should the game capture mouse?
        public bool CenterCursor { get; private set; } // Should the mouse cursor centered on screen?
        private KeyboardState _previosuKeyboardState; // previous keyboard-state.
        private MouseState _previousMouseState; // previous mouse-state.

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

            #if DEBUG
                this.PrintDebugKeys();
            #endif
        }

        public override void Update(GameTime gameTime)
        {
            this.ProcessKeyboard(gameTime);
            this.ProcessMouse();
        }

        private void ProcessKeyboard(GameTime gameTime)
        {
            var currentState = Keyboard.GetState();

            if (currentState.IsKeyDown(Keys.Escape) && this._previosuKeyboardState.IsKeyUp(Keys.Escape)) // allows quick exiting the game.
                this.Game.Exit();

            if (currentState.IsKeyDown(Keys.Up) || currentState.IsKeyDown(Keys.W)) {}
            if (currentState.IsKeyDown(Keys.Down) || currentState.IsKeyDown(Keys.S)) { }
            if (currentState.IsKeyDown(Keys.Left) || currentState.IsKeyDown(Keys.A)) { }
            if (currentState.IsKeyDown(Keys.Right) || currentState.IsKeyDown(Keys.D)) { }
            if (_previosuKeyboardState.IsKeyUp(Keys.Space) && currentState.IsKeyDown(Keys.Space)) { }

            if (currentState.IsKeyDown(Keys.F1) && _previosuKeyboardState.IsKeyUp(Keys.F1)) { }
            if (currentState.IsKeyDown(Keys.F2) && _previosuKeyboardState.IsKeyUp(Keys.F2)) { }
            if (currentState.IsKeyDown(Keys.F3) && _previosuKeyboardState.IsKeyUp(Keys.F3)) { }
            if (currentState.IsKeyDown(Keys.F4) && _previosuKeyboardState.IsKeyUp(Keys.F4)) { }
            if (currentState.IsKeyDown(Keys.F5) && _previosuKeyboardState.IsKeyUp(Keys.F5)) { }
            if (currentState.IsKeyDown(Keys.F10) && _previosuKeyboardState.IsKeyUp(Keys.F10)) { }
            if (currentState.IsKeyDown(Keys.F11) && _previosuKeyboardState.IsKeyUp(Keys.F11)) { }
            if (currentState.IsKeyDown(Keys.F12) && _previosuKeyboardState.IsKeyUp(Keys.F12)) { }

            this._previosuKeyboardState = currentState;
        }

        private void ProcessMouse()
        {
            var currentState = Mouse.GetState();

            if (currentState == this._previousMouseState || !this.CaptureMouse) // if there's no mouse-state change or if it's not captured, just return.
                return;

            float rotation = currentState.X - this._previousMouseState.X;
            if (rotation != 0) { }

            float elevation = currentState.Y - this._previousMouseState.Y;
            if (elevation != 0) { }

            if (currentState.LeftButton == ButtonState.Pressed && this._previousMouseState.LeftButton == ButtonState.Released) { }
            if (currentState.RightButton == ButtonState.Pressed && this._previousMouseState.RightButton == ButtonState.Released) { }

            this._previousMouseState = currentState;
            if (CenterCursor) CenterMouseCursor();
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
