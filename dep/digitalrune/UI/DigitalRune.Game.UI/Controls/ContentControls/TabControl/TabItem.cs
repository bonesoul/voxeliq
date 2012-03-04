using System.ComponentModel;
using DigitalRune.Game.Input;


namespace DigitalRune.Game.UI.Controls
{
  /// <summary>
  /// Represents a selectable item inside a <see cref="TabControl"/>. 
  /// </summary>
  /// <remarks>
  /// <para>
  /// The <see cref="TabItem"/> is the clickable tab that the user clicks to switch to this tab.
  /// If this <see cref="TabItem"/> is selected, the <see cref="TabPage"/> of this control is
  /// displayed in the <see cref="TabControl"/>.
  /// </para>
  /// <para>
  /// <strong>Visual States:</strong> The <see cref="VisualState"/>s of this control are:
  /// "Disabled", "Default", "MouseOver", "Selected"
  /// </para>
  /// </remarks>
  public class TabItem : ContentControl
  {
    // TabItem defines the appearance of a tab. It has a TabPage property, which defines the
    // content of the TabControl when this TabItem is selected.

    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------
    #endregion


    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------

    /// <summary>
    /// Gets or sets the control that should be displayed as <see cref="TabControl"/> content.
    /// </summary>
    /// <value>The control that should be displayed as <see cref="TabControl"/> content..</value>
    public UIControl TabPage { get; set; }


    /// <summary>
    /// Gets the <see cref="TabControl"/>.
    /// </summary>
    /// <value>The <see cref="TabControl"/>.</value>
    /// <remarks>
    /// This property is automatically set when the <see cref="TabItem"/> is added to the 
    /// <see cref="Controls.TabControl.Items"/> of a <see cref="Controls.TabControl"/>.
    /// </remarks>
    public TabControl TabControl { get; internal set; }


    /// <inheritdoc/>
    public override string VisualState
    {
      get
      {
        if (!ActualIsEnabled)
          return "Disabled";

        if (IsSelected)
          return "Selected";

        if (IsMouseOver)
          return "MouseOver";

        return "Default";
      }
    }
    #endregion


    //--------------------------------------------------------------
    #region Game Object Properties & Events
    //--------------------------------------------------------------

    /// <summary> 
    /// The ID of the <see cref="IsSelected"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int IsSelectedPropertyId = CreateProperty(
      typeof(TabItem), "IsSelected", GamePropertyCategories.Common, null, false, 
      UIPropertyOptions.AffectsRender);

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="TabItem"/> is selected in the
    /// <see cref="TabControl"/>. This is a game object property.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if this <see cref="TabItem"/> is selected in the 
    /// <see cref="TabControl"/>; otherwise, <see langword="false"/>.
    /// </value>
    public bool IsSelected
    {
      get { return GetValue<bool>(IsSelectedPropertyId); }
      set { SetValue(IsSelectedPropertyId, value); }
    }
    #endregion


    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="TabItem"/> class.
    /// </summary>
    public TabItem()
    {
      Style = "TabItem";

      // The user might change the IsSelected property directly. For this case we catch the
      // Changed event of the IsSelected property.
      var isSelected = Properties.Get<bool>(IsSelectedPropertyId);
      isSelected.Changed += (s, e) =>
                            {
                              // The user has set IsSelected directly:
                              if (IsSelected && TabControl != null)
                                TabControl.Select(this);
                            };
    }
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <inheritdoc/>
    protected override void OnHandleInput(InputContext context)
    {
      base.OnHandleInput(context);

      if (!IsLoaded)
        return;

      var inputService = InputService;

      // When clicked, call TabControl.Select().      
      if (IsMouseOver 
          && !inputService.IsMouseOrTouchHandled 
          && inputService.IsPressed(MouseButtons.Left, false))
      {
        inputService.IsMouseOrTouchHandled = true;
        if (TabControl != null)
          TabControl.Select(this);
      }
    }
    #endregion
  }
}
