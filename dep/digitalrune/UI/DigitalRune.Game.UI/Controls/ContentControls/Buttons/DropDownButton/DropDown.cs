using System;
using System.ComponentModel;
using DigitalRune.Game.Input;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework.Input;


namespace DigitalRune.Game.UI.Controls
{
  /// <summary>
  /// Represents a popup that displays the <see cref="DropDownItem"/>s of a 
  /// <see cref="DropDownButton"/>.
  /// </summary>
  public class DropDown : ContentControl
  {
    // Very similar to ContextMenu, which is also a kind of "popup". See comments in 
    // ContextMenu code.
    // On Windows Phone the DropDown fills the entire screen and displays a title.

    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------

    private StackPanel _itemsPanel;
    private ScrollViewer _scrollViewer;
    #endregion


    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------

    /// <summary>
    /// Gets the <see cref="DropDownButton"/> that owns this <see cref="DropDown"/>.
    /// </summary>
    /// <value>The <see cref="DropDownButton"/> that owns this <see cref="DropDown"/>.</value>
    public DropDownButton Owner { get; private set; }
    #endregion


    //--------------------------------------------------------------
    #region Game Object Properties & Events
    //--------------------------------------------------------------

    /// <summary> 
    /// The ID of the <see cref="DropDownItemStyle"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int DropDownItemStylePropertyId = CreateProperty(
      typeof(DropDown), "DropDownItemStyle", GamePropertyCategories.Style, null, "DropDownItem", 
      UIPropertyOptions.None);

    /// <summary>
    /// Gets or sets the style that is applied to <see cref="DropDownItem"/>s. 
    /// This is a game object property.
    /// </summary>
    /// <value>
    /// The style that is applied to <see cref="DropDownItem"/>s. 
    /// </value>
    public string DropDownItemStyle
    {
      get { return GetValue<string>(DropDownItemStylePropertyId); }
      set { SetValue(DropDownItemStylePropertyId, value); }
    }    


    /// <summary> 
    /// The ID of the <see cref="TitleTextBlockStyle"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int TitleTextBlockStylePropertyId = CreateProperty(
      typeof(DropDown), "TitleTextBlockStyle", GamePropertyCategories.Style, null,
      "TitleTextBlock", UIPropertyOptions.None);

    /// <summary>
    /// Gets or sets the style that is applied to the <see cref="DropDownButton.Title"/>
    /// (only on Windows Phone 7). This is a game object property.
    /// </summary>
    /// <value>
    /// The style that is applied to the <see cref="DropDownButton.Title"/>.
    /// Can be <see langword="null"/> or an empty string to hide the title.
    /// </value>
    public string TitleTextBlockStyle
    {
      get { return GetValue<string>(TitleTextBlockStylePropertyId); }
      set { SetValue(TitleTextBlockStylePropertyId, value); }
    }
    #endregion


    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    /// <summary>
    /// Initializes static members of the <see cref="DropDown"/> class.
    /// </summary>
    static DropDown()
    {
      // TODO: Is this needed?
      OverrideDefaultValue(typeof(DropDown), IsFocusScopePropertyId, true);
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="DropDown"/> class.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="owner"/> is <see langword="null"/>.
    /// </exception>
    public DropDown(DropDownButton owner)
    {
      if (owner == null)
        throw new ArgumentNullException("owner");

      Owner = owner;
      Style = "DropDown";
    }
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------    

    /// <inheritdoc/>
    protected override void OnLoad()
    {
      base.OnLoad();

      // _panel and _scrollViewer are only initialized once and reused. They are not destroyed
      // in Unload().
      if (_itemsPanel != null)
        return;

      string scrollViewerStyle = ContentStyle;
      if (string.IsNullOrEmpty(scrollViewerStyle))
        scrollViewerStyle = "ScrollViewer";

      _itemsPanel = new StackPanel
      {
        HorizontalAlignment = HorizontalAlignment.Stretch,
        Orientation = Orientation.Vertical,
      };

      _scrollViewer = new ScrollViewer
      {
        Style = scrollViewerStyle,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        VerticalAlignment = VerticalAlignment.Stretch,
        HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
        VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
        Content = _itemsPanel,
      };

#if WINDOWS_PHONE
      // On Windows Phone the DropDown fills the whole screen and a title is displayed on top.
      var titleTextBlockStyle = TitleTextBlockStyle;
      if (!string.IsNullOrEmpty(titleTextBlockStyle))
      {
        var title = new TextBlock
        {
          Style = titleTextBlockStyle,
          Text = Owner.Title,
        };

        // Connect DropDownButton.Title with title.Text.
        var titleProperty = Owner.Properties.Get<string>(DropDownButton.TitlePropertyId);
        var textProperty = title.Properties.Get<string>(TextBlock.TextPropertyId);
        titleProperty.Changed += textProperty.Change;

        var outerPanel = new StackPanel
        {
          HorizontalAlignment = HorizontalAlignment.Stretch,
          VerticalAlignment = VerticalAlignment.Stretch,
          Orientation = Orientation.Vertical,
        };
        outerPanel.Children.Add(title);
        outerPanel.Children.Add(_scrollViewer);
        Content = outerPanel;

        // The ContentStyle should be applied to the scroll viewer - not the outer panel:
        outerPanel.Style = "StackPanel";
        _scrollViewer.Style = ContentStyle;
      }
      else
      {
        Content = _scrollViewer;
      }
#else
      Content = _scrollViewer;
#endif
    }


    /// <inheritdoc/>
    protected override void OnHandleInput(InputContext context)
    {
      base.OnHandleInput(context);

      if (!IsLoaded)
        return;

      var screen = Screen;
      var inputService = InputService;

      // ESC --> Close drop-down.
      // Note: We do not check the InputService.IsHandled flags because the popup closes 
      // when ESC is pressed - even if the ESC was caught by another game component.
      if (inputService.IsDown(Keys.Escape))
      {
        inputService.IsKeyboardHandled = true;
        Close();
      }

      // BACK on gamepad --> Close drop-down.
      if (inputService.IsPressed(Buttons.Back, false, context.AllowedPlayer))
      {
        inputService.SetGamePadHandled(context.AllowedPlayer, true);

#if WINDOWS_PHONE
        // Special: The SelectedIndex needs to be set to the item that has focus. 
        // (Only on Windows Phone 7.)
        var focusedControl = Screen.FocusManager.FocusedControl;
        var index = _itemsPanel.Children.IndexOf(focusedControl);
        if (index >= 0)
          Owner.SelectedIndex = index;
#endif

        Close();
      }

      // B on gamepad --> Close drop-down.
      if (inputService.IsPressed(Buttons.B, false, context.AllowedPlayer))
      {
        inputService.SetGamePadHandled(context.AllowedPlayer, true);
        Close();
      }

      // If another control is opened above this popup, then this popup closes. 
      // Exception: Tooltips are okay above the popup.
      if (screen.Children[screen.Children.Count - 1] != this)
      {
        if (screen.Children[screen.Children.Count - 1] != screen.ToolTipManager.ToolTipControl
            || screen.Children[screen.Children.Count - 2] != this)
        {
          Close();
        }
      }

      // If mouse is pressed somewhere else, we close the drop-down.
      if (!IsMouseOver    // If mouse is pressed over drop-down, we still have to wait for MouseUp.
          && (inputService.IsPressed(MouseButtons.Left, false)
              || inputService.IsPressed(MouseButtons.Right, false)))
      {
        Close();
      }

      // Like a normal window: mouse does not act through this popup.
      if (IsMouseOver)
        inputService.IsMouseOrTouchHandled = true;      
    }


    /// <summary>
    /// Opens this <see cref="DropDown"/> (adds it to the <see cref="UIScreen"/>).
    /// </summary>
    /// <exception cref="UIException">
    /// <see cref="Owner"/> is not loaded. The owner must be loaded before the 
    /// <see cref="DropDown"/> can be shown.
    /// </exception>
    public void Open()
    {
      // Close if already opened.
      Close();

      var screen = Owner.Screen;
      if (screen == null)
        throw new UIException("Cannot show drop-down. The owner must be loaded first.");

      // Make visible and add to screen.
      IsVisible = true;
      screen.Children.Add(this);

      // Rebuild the panel with the newest DropDownItems.
      _itemsPanel.Children.Clear();
      var dropDownItemStyle = string.IsNullOrEmpty(DropDownItemStyle) ? "DropDownItem" : DropDownItemStyle;
      foreach (var item in Owner.Items)
      {
        var dropDownItem = new DropDownItem
                           {
                             Style = dropDownItemStyle,
                             Content = Owner.CreateControl(item),          
                           };        
        _itemsPanel.Children.Add(dropDownItem);

        var dropDownItemClick = dropDownItem.Events.Get<EventArgs>(ButtonBase.ClickEventId);
        dropDownItemClick.Event += OnDropDownItemClick;
      }      

      UpdatePosition();

      // Set focus to the currently selected DropDownItem.
      if (0 <= Owner.SelectedIndex && Owner.SelectedIndex <= _itemsPanel.Children.Count)
        screen.FocusManager.Focus(_itemsPanel.Children[Owner.SelectedIndex]);
      else
        screen.FocusManager.Focus(this);
    }


    private void UpdatePosition()
    {
#if WINDOWS_PHONE
      // Fill entire screen.
      HorizontalAlignment = HorizontalAlignment.Stretch;
      VerticalAlignment = VerticalAlignment.Stretch;
#else
      // Choose position near owner.
      var screen = Screen;

      float x = Owner.ActualX;
      float y = Owner.ActualY + Owner.ActualHeight;
      float width = Owner.ActualWidth;

      // Measure height.
      _scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
      Height = float.NaN;
      Measure(new Vector2F(float.PositiveInfinity));
      float height = Math.Min(DesiredHeight, Owner.MaxDropDownHeight);
      _scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

      // The DropDown is under the DropDownButton per default. Unless there is not enough
      // space then it is displayed on top if there is enough room.
      if (y + height > screen.ActualY + screen.ActualHeight)
      {
        if (Owner.ActualY - height >= screen.ActualY)
        {
          y = Owner.ActualY - height;
        }
      }

      X = x;
      Y = y;
      Width = width;
      Height = height;
#endif
    }


    /// <summary>
    /// Closes this <see cref="DropDown"/> (removes it from the <see cref="UIScreen"/>).
    /// </summary>
    public void Close()
    {
      var screen = Screen;
      if (screen != null)
      {
        if (IsFocusWithin)
        {
          // Move focus back to owner.
          screen.FocusManager.Focus(Owner);
        }

        screen.Children.Remove(this);
      }
    }


    private void OnDropDownItemClick(object sender, EventArgs eventArgs)
    {
      // Set SelectedIndex of DropDownButton to the index of the clicked item and close the
      // DropDown.
      var dropDownItem = (DropDownItem)sender;
      var index = _itemsPanel.Children.IndexOf(dropDownItem);
      Owner.SelectedIndex = index;
      Close();
    }
    #endregion
  }
}
