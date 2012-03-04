namespace DigitalRune.Game.UI.Controls
{
  /// <summary>
  /// Represents an item of a menu (e.g. a <see cref="ContextMenu"/>).
  /// </summary>
  /// <remarks>
  /// A <see cref="MenuItem"/> is basically a button - but with a different style.
  /// </remarks>
  public class MenuItem : ButtonBase
  {
    /// <summary>
    /// Initializes static members of the <see cref="MenuItem"/> class.
    /// </summary>
    static MenuItem()
    {
      // Per default, the item should be focused when the mouse moves over the item.
      OverrideDefaultValue(typeof(MenuItem), FocusWhenMouseOverPropertyId, true);
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItem"/> class.
    /// </summary>
    public MenuItem()
    {
      Style = "MenuItem";
    }
  }
}
