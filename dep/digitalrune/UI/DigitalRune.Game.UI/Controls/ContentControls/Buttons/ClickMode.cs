namespace DigitalRune.Game.UI.Controls
{
  /// <summary>
  /// Specifies when the <see cref="ButtonBase.Click"/> event should be raised for a control. 
  /// </summary>
  public enum ClickMode
  {
    /// <summary>
    /// Specifies that the <see cref="ButtonBase.Click"/> event should be raised when the button is
    /// pressed and released and the control is focused.
    /// </summary>
    Release,

    /// <summary>
    /// Specifies that the <see cref="ButtonBase.Click"/> event should be raised when the button is
    /// pressed the control is focused.
    /// </summary>
    Press,

    //Hover,
  }
}
