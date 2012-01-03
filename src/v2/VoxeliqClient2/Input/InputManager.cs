using VolumetricStudios.VoxeliqEngine.Core;

namespace VolumetricStudios.VoxeliqClient.Input
{
    public class InputManager : IUpdateable
    {
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
            //Logger.Trace(this.MouseState.Position.X + ":" + this.MouseState.Position.Y + this.MouseState.IsButtonDown(MouseButton.Left));
        }
    }
}
