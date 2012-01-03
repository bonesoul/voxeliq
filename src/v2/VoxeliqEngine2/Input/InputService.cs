using System.Collections.Generic;
using System.Windows.Forms;
using Ninject.Modules;
using SlimDX;
using SlimDX.Multimedia;
using SlimDX.RawInput;
using VolumetricStudios.VoxeliqEngine.Core;
using VolumetricStudios.VoxeliqEngine.Utils.Logging;

namespace VolumetricStudios.VoxeliqEngine.Input
{
    public interface IInputService : IService
    { }

    // http://code.google.com/p/adtengine/source/browse/trunk/Engine/Input.cs?r=2  
    public sealed class InputService :  IInputService
    {
        public MouseState MouseState;
        public KeyboardState KeyboardState;

        private static readonly Logger Logger = LogManager.CreateLogger();

        public InputService()
        {
            this.MouseState = new MouseState();
            this.KeyboardState = new KeyboardState();

            Device.RegisterDevice(UsagePage.Generic, UsageId.Keyboard, DeviceFlags.None);
            Device.RegisterDevice(UsagePage.Generic, UsageId.Mouse, DeviceFlags.None);

            Device.KeyboardInput += Device_KeyboardInput;
            Device.MouseInput += Device_MouseInput;
        }

        public void Update()
        { }

        private void Device_KeyboardInput(object sender, KeyboardInputEventArgs e)
        {
            var isKeyDown = (e.State == KeyState.Pressed || e.State == KeyState.SystemKeyPressed);
            this.KeyboardState.KeyPressed(e.Key, isKeyDown);
        }

        private void Device_MouseInput(object sender, MouseInputEventArgs e)
        {
            if ((e.ButtonFlags & MouseButtonFlags.LeftDown) == MouseButtonFlags.LeftDown)
                this.MouseState.ButtonPressed(MouseButton.Left);
            else if ((e.ButtonFlags & MouseButtonFlags.LeftUp) == MouseButtonFlags.LeftUp)
                this.MouseState.ButtonReleased(MouseButton.Left);

            if ((e.ButtonFlags & MouseButtonFlags.RightDown) == MouseButtonFlags.RightDown)
                this.MouseState.ButtonPressed(MouseButton.Right);
            else if ((e.ButtonFlags & MouseButtonFlags.RightUp) == MouseButtonFlags.RightUp)
                this.MouseState.ButtonReleased(MouseButton.Right);

            if ((e.ButtonFlags & MouseButtonFlags.MiddleDown) == MouseButtonFlags.MiddleDown)
                this.MouseState.ButtonPressed(MouseButton.Middle);
            else if ((e.ButtonFlags & MouseButtonFlags.MiddleUp) == MouseButtonFlags.MiddleUp)
                this.MouseState.ButtonReleased(MouseButton.Middle);

            if ((e.ButtonFlags & MouseButtonFlags.Button4Down) == MouseButtonFlags.Button4Down)
                this.MouseState.ButtonPressed(MouseButton.Button4);
            else if ((e.ButtonFlags & MouseButtonFlags.Button4Up) == MouseButtonFlags.Button4Up)
                this.MouseState.ButtonReleased(MouseButton.Button4);

            if ((e.ButtonFlags & MouseButtonFlags.Button5Down) == MouseButtonFlags.Button5Down)
                this.MouseState.ButtonPressed(MouseButton.Button5);
            else if ((e.ButtonFlags & MouseButtonFlags.Button5Up) == MouseButtonFlags.Button5Up)
                this.MouseState.ButtonReleased(MouseButton.Button5);

            this.MouseState.Position.X += e.X;
            this.MouseState.Position.Y += e.Y;
        }
    }

    public class KeyboardState
    {
        private readonly Dictionary<int, bool> _keysPressed = new Dictionary<int, bool>();

        public void KeyPressed(Keys key, bool isKeyDown)
        {
            _keysPressed[(int)key] = isKeyDown;
        }

        public bool IsKeyDown(Keys key)
        {
            bool isKeyDown;
            return _keysPressed.TryGetValue((int)key, out isKeyDown) && isKeyDown;
        }
    }

    public class MouseState
    {
        public Vector2 Position;

        private readonly Dictionary<MouseButton, bool> _buttonsPressed = new Dictionary<MouseButton, bool>(); // pressed buttons.

        public void ButtonPressed(MouseButton button)
        {
            _buttonsPressed[button] = true;
        }

        public void ButtonReleased(MouseButton button)
        {
            _buttonsPressed[button] = false;
        }

        public bool IsButtonDown(MouseButton button)
        {
            bool isButtonDown;
            return _buttonsPressed.TryGetValue(button, out isButtonDown) && isButtonDown;
        }
    }

    public enum MouseButton : byte
    {
        Left = 0,
        Right = 1,
        Middle = 2,
        Button4 = 3,
        Button5 = 4
    }
}
