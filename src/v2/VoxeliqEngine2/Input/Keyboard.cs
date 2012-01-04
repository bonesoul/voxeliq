using System.Collections.Generic;
using System.Windows.Forms;
using SlimDX.Multimedia;
using SlimDX.RawInput;

namespace VolumetricStudios.VoxeliqEngine.Input
{
    public static class Keyboard
    {
        private static KeyboardState _state;

        internal static void Initialize()
        {
            _state = new KeyboardState();
            Device.RegisterDevice(UsagePage.Generic, UsageId.Keyboard, DeviceFlags.None);
            Device.KeyboardInput += OnInput;
        }

        public static KeyboardState GetState()
        {
            return new KeyboardState(_state);
        }

        private static void OnInput(object sender, KeyboardInputEventArgs e)
        {
            var isKeyDown = (e.State == KeyState.Pressed || e.State == KeyState.SystemKeyPressed);
            _state.KeyPressed(e.Key, isKeyDown);
        }
    }

    public class KeyboardState
    {
        public readonly Dictionary<int, bool> Keys;

        public KeyboardState()
        {
            this.Keys = new Dictionary<int, bool>();
        }

        public KeyboardState(KeyboardState state)
        {
            this.Keys = new Dictionary<int, bool>(state.Keys);
        }

        public void KeyPressed(Keys key, bool isKeyDown)
        {
            Keys[(int)key] = isKeyDown;
        }

        public bool IsKeyDown(Keys key)
        {
            return Keys.ContainsKey((int)key) && Keys[(int)key];
        }

        public bool IsKeyUp(Keys key)
        {
            return !Keys.ContainsKey((int) key) || !Keys[(int) key];
        }
    }
}
