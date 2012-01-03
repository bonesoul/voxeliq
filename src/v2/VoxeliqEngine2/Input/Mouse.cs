using System;
using System.Collections.Generic;
using SlimDX;
using SlimDX.Multimedia;
using SlimDX.RawInput;

namespace VolumetricStudios.VoxeliqEngine.Input
{
    public static class Mouse
    {
        private static MouseState _state;

        internal static void Initialize()
        {
            _state=new MouseState();
            Device.RegisterDevice(UsagePage.Generic, UsageId.Mouse, DeviceFlags.None);
            Device.MouseInput += OnInput;
        }

        public static MouseState GetState()
        {
            return new MouseState(_state);
        }

        public static void SetPosition(int x, int y)
        {
            throw new NotImplementedException();
        }

        private static void OnInput(object sender, MouseInputEventArgs e)
        {
            if ((e.ButtonFlags & MouseButtonFlags.LeftDown) == MouseButtonFlags.LeftDown)
                _state.LeftButton = ButtonState.Pressed;
            else if ((e.ButtonFlags & MouseButtonFlags.LeftUp) == MouseButtonFlags.LeftUp)
                _state.LeftButton = ButtonState.Released;

            if ((e.ButtonFlags & MouseButtonFlags.RightDown) == MouseButtonFlags.RightDown)
                _state.RightButton = ButtonState.Pressed;
            else if ((e.ButtonFlags & MouseButtonFlags.RightUp) == MouseButtonFlags.RightUp)
                _state.RightButton = ButtonState.Released;

            if ((e.ButtonFlags & MouseButtonFlags.MiddleDown) == MouseButtonFlags.MiddleDown)
                _state.MiddleButton = ButtonState.Pressed;
            else if ((e.ButtonFlags & MouseButtonFlags.MiddleUp) == MouseButtonFlags.MiddleUp)
                _state.MiddleButton = ButtonState.Released;

            _state.Position.X += e.X;
            _state.Position.Y += e.Y;
        }
    }

    public class MouseState
    {
        public Vector2 Position;
        public ButtonState LeftButton;
        public ButtonState RightButton;
        public ButtonState MiddleButton;

        public MouseState()
        {
            this.Position = Vector2.Zero;
            this.LeftButton = ButtonState.Released;
            this.RightButton = ButtonState.Released;
            this.MiddleButton = ButtonState.Released;
        }

        public MouseState(MouseState state)
        {
            this.Position = new Vector2(state.Position.X, state.Position.Y);
            this.LeftButton = state.LeftButton;
            this.RightButton = state.RightButton;
            this.MiddleButton = state.MiddleButton;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (MouseState)) return false;
            return Equals((MouseState) obj);
        }

        public bool Equals(MouseState other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Position.Equals(Position) && Equals(other.LeftButton, LeftButton) && Equals(other.RightButton, RightButton) && Equals(other.MiddleButton, MiddleButton);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = Position.GetHashCode();
                result = (result*397) ^ LeftButton.GetHashCode();
                result = (result*397) ^ RightButton.GetHashCode();
                result = (result*397) ^ MiddleButton.GetHashCode();
                return result;
            }
        }
    }

    public enum ButtonState : byte
    {
        Released = 0,
        Pressed = 1,
    }
}
