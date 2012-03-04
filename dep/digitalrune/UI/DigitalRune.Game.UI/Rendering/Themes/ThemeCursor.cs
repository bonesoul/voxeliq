using System.Windows.Forms;


namespace DigitalRune.Game.UI.Rendering
{
  /// <summary>
  /// Represents a mouse cursor of the UI theme.
  /// </summary>
  public class ThemeCursor : INamedObject
  {
    /// <summary>
    /// Gets the name of the cursor.
    /// </summary>
    /// <value>The name of the cursor.</value>
    public string Name { get; set; }


    /// <summary>
    /// Gets or sets a value indicating whether this cursor is the default cursor.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if this cursor is the default cursor; otherwise, 
    /// <see langword="false"/>.
    /// </value>
    public bool IsDefault { get; set; }


    /// <summary>
    /// Gets or sets the cursor.
    /// </summary>
    /// <value>The cursor.</value>
    public Cursor Cursor { get; set; }
  }
}
