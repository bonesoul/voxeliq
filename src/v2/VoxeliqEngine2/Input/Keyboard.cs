using System.Collections.Generic;
using System.Windows.Forms;
using SlimDX.Multimedia;
using SlimDX.RawInput;

namespace VolumetricStudios.VoxeliqEngine.Input
{
    public static class Keyboard
    {
        public static KeyboardState State { get; private set; }

        internal static void Initialize()
        {
            State = new KeyboardState();
            Device.RegisterDevice(UsagePage.Generic, UsageId.Keyboard, DeviceFlags.None);
            Device.KeyboardInput += OnInput;
        }

        private static void OnInput(object sender, KeyboardInputEventArgs e)
        {
            var isKeyDown = (e.State == KeyState.Pressed || e.State == KeyState.SystemKeyPressed);
            State.KeyPressed(e.Key, isKeyDown);
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
}
