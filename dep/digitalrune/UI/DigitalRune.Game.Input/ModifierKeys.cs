using System;


namespace DigitalRune.Game.Input
{
  /// <summary>
  /// Describes the pressed modifier keys.
  /// </summary>
  [Flags]
  public enum ModifierKeys
  {
    /// <summary>
    /// No modifier key is pressed.
    /// </summary>
    None = 0,

    /// <summary>
    /// Shift is pressed.
    /// </summary>
    Shift = 1,

    /// <summary>
    /// Control is pressed.
    /// </summary>
    Control = 2,

    /// <summary>
    /// Alt is pressed.
    /// </summary>
    Alt = 4,

    /// <summary>
    /// ChatPadGreen is pressed.
    /// </summary>
    ChatPadGreen = 8,

    /// <summary>
    /// ChatPadOrange is pressed.
    /// </summary>
    ChatPadOrange = 16,

    /// <summary>
    /// Shift and Alt are pressed.
    /// </summary>
    ShiftAlt = Shift | Alt,

    /// <summary>
    /// Control and Alt are pressed.
    /// </summary>
    ControlAlt = Control | Alt,

    /// <summary>
    /// Control and Shift are pressed.
    /// </summary>
    ControlShift = Control | Shift,

    /// <summary>
    /// Control, Shift and Alt are pressed.
    /// </summary>
    ControlShiftAlt = Control | Shift | Alt,
  }
}
