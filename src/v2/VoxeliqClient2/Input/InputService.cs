using System.Diagnostics;
using System.Windows.Forms;
using VolumetricStudios.VoxeliqEngine.Core;
using VolumetricStudios.VoxeliqEngine.Input;
using VolumetricStudios.VoxeliqEngine.Utility.Logging;
using ButtonState = VolumetricStudios.VoxeliqEngine.Input.ButtonState;

namespace VolumetricStudios.VoxeliqClient.Input
{    
    public interface IInputService
    { }

    public class InputService : GameComponent, IInputService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private KeyboardState _previousKeyboardState = new KeyboardState();
        private MouseState _previousMouseState = new MouseState();

        public InputService(Game game)
            :base(game)
        {
            game.AddService(typeof(IInputService), this);
        }

        public override void Initialize()
        {
            this.PrintDebugKeys();
            Logger.Trace("init()");
        }

        public override void Update(GameTime gameTime)
        {
            Logger.Trace(gameTime.FramesPerSecond + " || " + gameTime.TotalGameTime.ToString());
            this.ProcessKeyboard();
            this.ProcessMouse();
        }

        private void ProcessKeyboard()
        {
            var currentState = Keyboard.GetState();

            if(currentState.IsKeyDown(Keys.Up) || currentState.IsKeyDown(Keys.W)) Logger.Trace("up");
            if (currentState.IsKeyDown(Keys.Down) || currentState.IsKeyDown(Keys.S)) Logger.Trace("down");
            if (currentState.IsKeyDown(Keys.Left) || currentState.IsKeyDown(Keys.A)) Logger.Trace("left");
            if (currentState.IsKeyDown(Keys.Right) || currentState.IsKeyDown(Keys.D)) Logger.Trace("right");

            if(this._previousKeyboardState.IsKeyUp(Keys.Space) && currentState.IsKeyDown(Keys.Space)) Logger.Trace("Space!");

            if (_previousKeyboardState.IsKeyUp(Keys.F1) && currentState.IsKeyDown(Keys.F1)) Logger.Trace("F1");
            if (_previousKeyboardState.IsKeyUp(Keys.F2) && currentState.IsKeyDown(Keys.F2)) Logger.Trace("F2");
            if (_previousKeyboardState.IsKeyUp(Keys.F3) && currentState.IsKeyDown(Keys.F3)) Logger.Trace("F3");
            if (_previousKeyboardState.IsKeyUp(Keys.F4) && currentState.IsKeyDown(Keys.F4)) Logger.Trace("F4");
            if (_previousKeyboardState.IsKeyUp(Keys.F5) && currentState.IsKeyDown(Keys.F5)) Logger.Trace("F5");
            if (_previousKeyboardState.IsKeyUp(Keys.F10) && currentState.IsKeyDown(Keys.F10)) Logger.Trace("F10");
            if (_previousKeyboardState.IsKeyUp(Keys.F11) && currentState.IsKeyDown(Keys.F11)) Logger.Trace("F11");
            if (_previousKeyboardState.IsKeyUp(Keys.F12) && currentState.IsKeyDown(Keys.F12)) Logger.Trace("F12");

            this._previousKeyboardState = currentState;
        }

        private void ProcessMouse()
        {
            var currentState = Mouse.GetState();

            if (currentState == this._previousMouseState)
                return;

            float rotation = currentState.Position.X - this._previousMouseState.Position.X;
            float elevation = currentState.Position.Y - this._previousMouseState.Position.Y;

            if (currentState.LeftButton == ButtonState.Pressed && this._previousMouseState.LeftButton == ButtonState.Released)
                Logger.Trace("LeftButton");

            if (currentState.RightButton == ButtonState.Pressed && this._previousMouseState.RightButton == ButtonState.Released)
                Logger.Trace("RightButton");

            if (currentState.MiddleButton == ButtonState.Pressed && this._previousMouseState.MiddleButton == ButtonState.Released)
                Logger.Trace("MiddleButton");

            this._previousMouseState = currentState;
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
