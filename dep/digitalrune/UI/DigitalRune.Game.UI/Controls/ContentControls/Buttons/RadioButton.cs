namespace DigitalRune.Game.UI.Controls
{
  /// <summary>
  /// Enables the user to select a single option from a of choices when paired with other 
  /// <see cref="RadioButton"/> controls. 
  /// </summary>
  /// <remarks>
  /// Radio buttons can be grouped into different groups. Radio buttons belong together if they
  /// have the same parent and the same <see cref="GroupName"/>.
  /// </remarks>
  public class RadioButton : ToggleButton
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------
    #endregion
      
      
    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------

    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    /// <value>The name of the group.</value>
    public string GroupName { get; set; }
    #endregion
      
      
    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="RadioButton"/> class.
    /// </summary>
    public RadioButton()
    {
      Style = "RadioButton";
    }
    #endregion
      
      
    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <inheritdoc/>
    protected override void OnToggle()
    {
      if (IsChecked)
      {
        // A checked radio button is clicked again --> nothing to do.
        return;
      }

      // Uncheck all other radio buttons of the parent and the same Group.
      var parent = VisualParent;
      if (parent != null)
      {
        foreach (var child in parent.VisualChildren)
        {
          if (child != this)
          {
            RadioButton radioButton = child as RadioButton;
            if (radioButton != null && radioButton.GroupName == GroupName)
            {
              radioButton.IsChecked = false;
            }
          }
        }
      }

      // Check this radio button.
      IsChecked = true;
    }
    #endregion
  }
}
