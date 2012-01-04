using System.Diagnostics;
using VolumetricStudios.VoxeliqEngine.Core;
using VolumetricStudios.VoxeliqEngine.Input;
using VolumetricStudios.VoxeliqEngine.Utility.Logging;

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

        public override void Update()
        {
            this.ProcessKeyboard();
            this.ProcessMouse();
        }

        private void ProcessKeyboard()
        {
            
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
