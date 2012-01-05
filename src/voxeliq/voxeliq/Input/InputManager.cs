using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VolumetricStudios.VoxeliqEngine.Logging;

namespace VolumetricStudios.Voxeliq.Input
{
    /// <summary>
    /// Input Manager interface.
    /// </summary>
    public interface IInputManager
    { }

    /// <summary>
    /// The input manager.
    /// </summary>
    public class InputManager : GameComponent, IInputManager
    {
        /// <summary>
        /// The last known keyboard state.
        /// </summary>
        private KeyboardState _previousKeyboardState;

        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility.

        public InputManager(Game game) : base(game)
        {
            this.Game.Services.AddService(typeof(IInputManager), this); // export the service
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            #if DEBUG
                this.PrintDebugKeys();
            #endif

            // import the required services
        }

        public override void Update(GameTime gameTime)
        {
            this.ProcessMouse();
            this.ProcessKeyboard(gameTime);
        }

        private void ProcessKeyboard(GameTime gameTime)
        {
            var currentState = Keyboard.GetState();

            if (currentState.IsKeyDown(Keys.Up) || currentState.IsKeyDown(Keys.W)) { }
            if (currentState.IsKeyDown(Keys.Down) || currentState.IsKeyDown(Keys.S)) { }
            if (currentState.IsKeyDown(Keys.Left) || currentState.IsKeyDown(Keys.A)) { }
            if (currentState.IsKeyDown(Keys.Right) || currentState.IsKeyDown(Keys.D)) { }
            if (_previousKeyboardState.IsKeyUp(Keys.Space) && currentState.IsKeyDown(Keys.Space)) { }

            if (_previousKeyboardState.IsKeyUp(Keys.F1) && currentState.IsKeyDown(Keys.F1)) { }
            if (_previousKeyboardState.IsKeyUp(Keys.F2) && currentState.IsKeyDown(Keys.F2)) { }
            if (_previousKeyboardState.IsKeyUp(Keys.F3) && currentState.IsKeyDown(Keys.F3)) { }
            if (_previousKeyboardState.IsKeyUp(Keys.F4) && currentState.IsKeyDown(Keys.F4)) { }
            if (_previousKeyboardState.IsKeyUp(Keys.F5) && currentState.IsKeyDown(Keys.F5)) { }

            if (_previousKeyboardState.IsKeyUp(Keys.F10) && currentState.IsKeyDown(Keys.F9)) { }
            if (_previousKeyboardState.IsKeyUp(Keys.F11) && currentState.IsKeyDown(Keys.F10)) { }
            if (_previousKeyboardState.IsKeyUp(Keys.F12) && currentState.IsKeyDown(Keys.F11)) { } 

            this._previousKeyboardState = currentState;
        }

        private void ProcessMouse()
        {
            
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
            Debug.WriteLine("F9: In-game Debugger: On/Off.");
            Debug.WriteLine("F10: Frame-limiter: On/Off.");
            Debug.WriteLine("F12: Wireframe mode: On/Off.");
            Debug.WriteLine("-----------------------------");
        }
    }
}
