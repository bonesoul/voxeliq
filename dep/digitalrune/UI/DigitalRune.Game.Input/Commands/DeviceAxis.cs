namespace DigitalRune.Game.Input
{
  /// <summary>
  /// Describes an analog input source, like an axis of joystick.
  /// </summary>
  public enum DeviceAxis
  {
    /// <summary>
    /// The x-coordinate of the absolute mouse position.
    /// </summary>
    MouseXAbsolute,

    /// <summary>
    /// The y-coordinate of the absolute mouse position.
    /// </summary>
    MouseYAbsolute,

    /// <summary>
    /// The x-coordinate of the mouse position change since the last frame.
    /// </summary>
    MouseXRelative,

    /// <summary>
    /// The y-coordinate of the mouse position change since the last frame.
    /// </summary>
    MouseYRelative,
    
    /// <summary>
    /// The value of the mouse wheel.
    /// </summary>
    MouseWheel,

    /// <summary>
    /// The horizontal axis of the left thumb stick on a gamepad.
    /// </summary>
    GamePadStickLeftX,

    /// <summary>
    /// The vertical axis of the left thumb stick on a gamepad.
    /// </summary>
    GamePadStickLeftY,

    /// <summary>
    /// The horizontal axis of the right thumb stick on a gamepad.
    /// </summary>
    GamePadStickRightX,

    /// <summary>
    /// The vertical axis of the right thumb stick on a gamepad.
    /// </summary>
    GamePadStickRightY,

    /// <summary>
    /// The value of the left trigger button on a gamepad.
    /// </summary>
    GamePadTriggerLeft,

    /// <summary>
    /// The value of the right trigger button on a gamepad.
    /// </summary>
    GamePadTriggerRight,
  }
}
