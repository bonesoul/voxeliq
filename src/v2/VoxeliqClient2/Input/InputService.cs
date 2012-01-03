using VolumetricStudios.VoxeliqEngine.Core;
using VolumetricStudios.VoxeliqEngine.Input;
using VolumetricStudios.VoxeliqEngine.Utils.Logging;

namespace VolumetricStudios.VoxeliqClient.Input
{    
    public class InputService : GameComponent, IGameService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private KeyboardState _previousKeyboardState = new KeyboardState();
        private MouseState _previousMouseState = new MouseState();

        public InputService(Game game)
        {
            game.AddService(typeof (InputService), this);
        }

        public override void Initialize()
        {
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
    }
}
