namespace DigitalRune.Game.UI.Controls
{
  /// <summary>
  /// Represents an item of <see cref="DropDownButton"/> control.
  /// </summary>
  public class DropDownItem : ButtonBase
  {
    // This DropDownItem is like a ButtonBase - but with a different style.

    /// <summary>
    /// Initializes static members of the <see cref="DropDownItem"/> class.
    /// </summary>
    static DropDownItem()
    {
      // Per default, the item should be focused when the mouse moves over the item.
      OverrideDefaultValue(typeof(DropDownItem), FocusWhenMouseOverPropertyId, true);
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="DropDownItem"/> class.
    /// </summary>
    public DropDownItem()
    {
      Style = "DropDownItem";
    }
  }
}
