using Ninject;
using VolumetricStudios.VoxeliqEngine.Core;
using VolumetricStudios.VoxeliqEngine.Input;
using VolumetricStudios.VoxeliqEngine.Utils.Logging;

namespace VolumetricStudios.VoxeliqClient.Input
{    
    public class InputManager : IUpdateable
    {
        private readonly InputService _inputService;
        private static readonly Logger Logger = LogManager.CreateLogger();

        public InputManager()
        {
            var kernel = new StandardKernel(EngineModule.Engine);
            this._inputService = (InputService)kernel.Get<IInputService>();
        }

        public void Update()
        {
            this.ProcessKeyboard();
            this.ProcessMouse();
        }

        private void ProcessKeyboard()
        {
            
        }

        private void ProcessMouse()
        {
            Logger.Trace(this._inputService.MouseState.Position.X + ":" + this._inputService.MouseState.Position.Y + this._inputService.MouseState.IsButtonDown(MouseButton.Left));
        }
    }
}
