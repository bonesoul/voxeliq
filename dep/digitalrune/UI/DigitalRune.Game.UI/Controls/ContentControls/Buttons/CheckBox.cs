namespace DigitalRune.Game.UI.Controls
{
  /// <summary>
  /// Represents a check box.
  /// </summary>
  public class CheckBox : ToggleButton
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CheckBox"/> class.
    /// </summary>
    public CheckBox()
    {
      Style = "CheckBox";
    }
      
  
    /// <inheritdoc/>
    protected override void OnToggle()
    {
      IsChecked = !IsChecked;      
    }
  }
}
