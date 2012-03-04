using System;
using System.ComponentModel;
using DigitalRune.Game.Input;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace DigitalRune.Game.UI.Controls
{
  /// <summary>
  /// Provides the ability to create, configure, show, and manage the lifetime of windows and 
  /// dialog boxes.
  /// </summary>
  /// <remarks>
  /// <para>
  /// A window is <see cref="ContentControl"/>. The <see cref="ContentControl.Content"/> is usually
  /// a <see cref="Panel"/>. Besides the <see cref="ContentControl.Content"/> the window contains
  /// an <see cref="Icon"/>, a <see cref="Title"/> and a Close button.
  /// </para>
  /// <para>
  /// The window can be dragged with the mouse if <see cref="CanDrag"/> is <see langword="true"/>.
  /// A dragging operation starts when the user clicks any part of the window (except over nested
  /// controls). The window can be resized with the mouse if <see cref="CanResize"/> is 
  /// <see langword="true"/>. A resize operation starts when the user clicks the border of the
  /// window. <see cref="ResizeBorder"/> defines the size of the border where resize operations
  /// can start. For windows that can be dragged or resized, use only top/left for the 
  /// <see cref="UIControl.VerticalAlignment"/> and <see cref="UIControl.HorizontalAlignment"/>.
  /// </para>
  /// <para>
  /// <strong>Visual States:</strong> The <see cref="VisualState"/>s of this control are:
  /// "Disabled", "Default", "Active"
  /// </para>
  /// </remarks>
  public class Window : ContentControl
  {
    //--------------------------------------------------------------
    #region Nested Types
    //--------------------------------------------------------------

    // The direction in which we are currently resizing. - Defines also the appearance of the
    // mouse cursor.
    private enum ResizeDirection { None, N, NE, E, SE, S, SW, W, NW }
    #endregion


    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------

    private Image _icon;
    private TextBlock _caption;
    private Button _closeButton;

    // For resizing:
    private bool _isResizing;
    private ResizeDirection _resizeDirection;   // Set if mouse is over resize border or if currently resizing.
    private bool _setSpecialCursor;

    // For dragging:
    private bool _isDragging;

    // For resizing and dragging:
    private Vector2F _mouseStartPosition;
    private Vector2F _startSizeOrPosition;
    #endregion


    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------

    /// <summary>
    /// Gets the owner of this window that was specified in <see cref="Show"/> (typically a 
    /// <see cref="UIScreen"/>).
    /// </summary>
    /// <value>
    /// The owner of this window that was specified in <see cref="Show"/> (typically a 
    /// <see cref="UIScreen"/>).
    /// </value>
    public UIControl Owner { get; private set; }  // The owner gets back the focus when this window is closed.


    /// <inheritdoc/>
    public override string VisualState
    {
      get
      {
        if (!ActualIsEnabled)
          return "Disabled";

        if (IsActive)
          return "Active";

        return "Default";
      }
    }
    #endregion


    //--------------------------------------------------------------
    #region Game Object Properties & Events
    //--------------------------------------------------------------

    /// <summary> 
    /// The ID of the <see cref="CanDrag"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int CanDragPropertyId = CreateProperty(
      typeof(Window), "CanDrag", GamePropertyCategories.Behavior, null, false,
      UIPropertyOptions.AffectsRender);

    /// <summary>
    /// Gets or sets a value indicating whether this window can dragged with the mouse. 
    /// This is a game object property.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if this window can dragged with the mouse; otherwise, 
    /// <see langword="false"/>. The default value is <see langword="false"/>.
    /// </value>
    public bool CanDrag
    {
      get { return GetValue<bool>(CanDragPropertyId); }
      set { SetValue(CanDragPropertyId, value); }
    }


    /// <summary> 
    /// The ID of the <see cref="CanResize"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int CanResizePropertyId = CreateProperty(
      typeof(Window), "CanResize", GamePropertyCategories.Behavior, null, false, 
      UIPropertyOptions.AffectsRender);

    /// <summary>
    /// Gets or sets a value indicating whether this window can be resized with the mouse. 
    /// This is a game object property.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if this window can be resized with the mouse; otherwise, 
    /// <see langword="false"/>. The default value is <see langword="false"/>.
    /// </value>
    public bool CanResize
    {
      get { return GetValue<bool>(CanResizePropertyId); }
      set { SetValue(CanResizePropertyId, value); }
    }


    /// <summary> 
    /// The ID of the <see cref="ResizeBorder"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int ResizeBorderPropertyId = CreateProperty(
      typeof(Window), "ResizeBorder", GamePropertyCategories.Layout, null, new Vector4F(4),
      UIPropertyOptions.AffectsRender);

    /// <summary>
    /// Gets or sets the dimensions of the window border where resize operations can start. 
    /// This is a game object property.
    /// </summary>
    /// <value>
    /// The dimensions of the window border where resize operations can start. The default value
    /// is (4, 4, 4, 4).
    /// </value>
    public Vector4F ResizeBorder
    {
      get { return GetValue<Vector4F>(ResizeBorderPropertyId); }
      set { SetValue(ResizeBorderPropertyId, value); }
    }


    /// <summary> 
    /// The ID of the <see cref="DialogResult"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int DialogResultPropertyId = CreateProperty<bool?>(
      typeof(Window), "DialogResult", GamePropertyCategories.Default, null, null, 
      UIPropertyOptions.None);

    /// <summary>
    /// Gets or sets the dialog result. 
    /// This is a game object property.
    /// </summary>
    /// <value>The dialog result.</value>
    /// <remarks>
    /// This property is set to <see langword="null"/> when <see cref="Show"/> is called. Otherwise
    /// this property is not changed automatically. It is typically expected that OK buttons set
    /// this property to <see langword="true"/> and Cancel buttons set this property to 
    /// <see langword="false"/>.
    /// </remarks>
    public bool? DialogResult
    {
      get { return GetValue<bool?>(DialogResultPropertyId); }
      set { SetValue(DialogResultPropertyId, value); }
    }


    /// <summary> 
    /// The ID of the <see cref="HideOnClose"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int HideOnClosePropertyId = CreateProperty(
      typeof(Window), "HideOnClose", GamePropertyCategories.Behavior, null, false, 
      UIPropertyOptions.None);

    /// <summary>
    /// Gets or sets a value indicating whether a this window is made in visible or totally
    /// removed from the control tree when the window is closed. This is a game object property.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if <see cref="Close"/> hides this window by setting
    /// <see cref="UIControl.IsVisible"/> to <see langword="false"/>; otherwise, 
    /// <see langword="false"/> if <see cref="Close"/> detaches the window from the 
    /// <see cref="UIScreen"/>. The default value is <see langword="false"/>.
    /// </value>
    public bool HideOnClose
    {
      get { return GetValue<bool>(HideOnClosePropertyId); }
      set { SetValue(HideOnClosePropertyId, value); }
    }


    /// <summary> 
    /// The ID of the <see cref="IsActive"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int IsActivePropertyId = CreateProperty(
      typeof(Window), "IsActive", GamePropertyCategories.Common, null, false, 
      UIPropertyOptions.AffectsRender);

    /// <summary>
    /// Gets a value indicating whether this window is the currently active window. 
    /// This is a game object property.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if this window is the currently active window; otherwise, 
    /// <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// Of all visible windows, only one window can be active. If <see cref="Activate"/> is called 
    /// the window is made active and all other windows are made inactive. Do not change 
    /// <see cref="IsActive"/> directly, use <see cref="Activate"/> instead.
    /// </remarks>
    public bool IsActive
    {
      get { return GetValue<bool>(IsActivePropertyId); }
      private set { SetValue(IsActivePropertyId, value); }
    }


    // blocks all input for objects behind the window.
    /// <summary> 
    /// The ID of the <see cref="IsModal"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int IsModalPropertyId = CreateProperty(
      typeof(Window), "IsModal", GamePropertyCategories.Behavior, null, false, 
      UIPropertyOptions.AffectsRender);

    /// <summary>
    /// Gets or sets a value indicating whether this window is a modal dialog. 
    /// This is a game object property.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if this window is a modal dialog; otherwise, <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// A modal window blocks all input from windows that are behind the modal window. The user must
    /// close the modal window before he/she can interact with the other windows. The default value 
    /// is <see langword="false"/>.
    /// </remarks>
    public bool IsModal
    {
      get { return GetValue<bool>(IsModalPropertyId); }
      set { SetValue(IsModalPropertyId, value); }
    }

    
    /// <summary> 
    /// The ID of the <see cref="IconStyle"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int IconStylePropertyId = CreateProperty(
      typeof(Window), "IconStyle", GamePropertyCategories.Style, null, "Icon", 
      UIPropertyOptions.None);

    /// <summary>
    /// Gets or sets the style that is applied to the <see cref="Image"/> control that draws the 
    /// <see cref="Icon"/>. This is a game object property.
    /// </summary>
    /// <value>
    /// The style that is applied to the <see cref="Image"/> control that draws the 
    /// <see cref="Icon"/>. Can be <see langword="null"/> or an empty string to hide the icon. The 
    /// default value is "Icon".
    /// </value>
    public string IconStyle
    {
      get { return GetValue<string>(IconStylePropertyId); }
      set { SetValue(IconStylePropertyId, value); }
    }


    /// <summary> 
    /// The ID of the <see cref="Icon"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int IconPropertyId = CreateProperty<Texture2D>(
      typeof(Window), "Icon", GamePropertyCategories.Appearance, null, null, 
      UIPropertyOptions.AffectsRender);

    /// <summary>
    /// Gets or sets the texture that contains the window icon. 
    /// This is a game object property.
    /// </summary>
    /// <value>
    /// The texture that contains the window icon. The default value is <see langword="null"/>.
    /// </value>
    public Texture2D Icon
    {
      get { return GetValue<Texture2D>(IconPropertyId); }
      set { SetValue(IconPropertyId, value); }
    }


    /// <summary> 
    /// The ID of the <see cref="IconSourceRectangle"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int IconSourceRectanglePropertyId = CreateProperty<Rectangle?>(
      typeof(Window), "IconSourceRectangle", GamePropertyCategories.Appearance, null, null, 
      UIPropertyOptions.AffectsRender);

    /// <summary>
    /// Gets or sets the region of the <see cref="Icon"/> texture that contains the icon. 
    /// This is a game object property.
    /// </summary>
    /// <value>
    /// The region of the <see cref="Icon"/> texture that contains the icon. Can be 
    /// <see langword="null"/> if the whole <see cref="Icon"/> texture should be drawn. 
    /// The default value is <see langword="null"/>.
    /// </value>
    public Rectangle? IconSourceRectangle
    {
      get { return GetValue<Rectangle?>(IconSourceRectanglePropertyId); }
      set { SetValue(IconSourceRectanglePropertyId, value); }
    }


    /// <summary> 
    /// The ID of the <see cref="TitleTextBlockStyle"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int TitleTextBlockStylePropertyId = CreateProperty(
      typeof(Window), "TitleTextBlockStyle", GamePropertyCategories.Style, null, "TitleTextBlock", 
      UIPropertyOptions.None);

    /// <summary>
    /// Gets or sets the style that is applied to the <see cref="TextBlock"/> that draws the window 
    /// <see cref="Title"/>. This is a game object property.
    /// </summary>
    /// <value>
    /// The style that is applied to the <see cref="TextBlock"/> that draws the window 
    /// <see cref="Title"/>. Can be <see langword="null"/> or an empty string to hide the title.
    /// The default value is "TitleTextBlock".
    /// </value>
    public string TitleTextBlockStyle
    {
      get { return GetValue<string>(TitleTextBlockStylePropertyId); }
      set { SetValue(TitleTextBlockStylePropertyId, value); }
    }


    /// <summary> 
    /// The ID of the <see cref="Title"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int TitlePropertyId = CreateProperty(
      typeof(Window), "Title", GamePropertyCategories.Common, null, "Unnamed", 
      UIPropertyOptions.AffectsRender);

    /// <summary>
    /// Gets or sets the window title that is visible in the caption bar. 
    /// This is a game object property.
    /// </summary>
    /// <value>
    /// The window title that is visible in the caption bar. The default value is "Unnamed".
    /// </value>
    public string Title
    {
      get { return GetValue<string>(TitlePropertyId); }
      set { SetValue(TitlePropertyId, value); }
    }


    /// <summary> 
    /// The ID of the <see cref="CloseButtonStyle"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int CloseButtonStylePropertyId = CreateProperty(
      typeof(Window), "CloseButtonStyle", GamePropertyCategories.Style, null, "CloseButton", 
      UIPropertyOptions.None);

    /// <summary>
    /// Gets or sets the style that is applied to the Close button. 
    /// This is a game object property.
    /// </summary>
    /// <value>
    /// The style that is applied to the Close button. Set this property to <see langword="null"/> 
    /// or an empty string to remove the Close button. The default value is "CloseButton".
    /// </value>
    public string CloseButtonStyle
    {
      get { return GetValue<string>(CloseButtonStylePropertyId); }
      set { SetValue(CloseButtonStylePropertyId, value); }
    }


    /// <summary> 
    /// The ID of the <see cref="Closing"/> game object event.
    /// </summary>
    [Browsable(false)]
    public static readonly int ClosingEventId = CreateEvent(
      typeof(Window), "Closing", GamePropertyCategories.Default, null, new CancelEventArgs());

    /// <summary>
    /// Occurs when the window is closing. Allows to cancel the closing operation. 
    /// This is a game object event.
    /// </summary>
    public event EventHandler<CancelEventArgs> Closing
    {
      add
      {
        var closing = Events.Get<CancelEventArgs>(ClosingEventId);
        closing.Event += value;
      }
      remove
      {
        var closing = Events.Get<CancelEventArgs>(ClosingEventId);
        closing.Event -= value;
      }
    }

    
    /// <summary> 
    /// The ID of the <see cref="Closed"/> game object event.
    /// </summary>
    [Browsable(false)]
    public static readonly int ClosedEventId = CreateEvent(
      typeof(Window), "Closed", GamePropertyCategories.Default, null, EventArgs.Empty);

    /// <summary>
    /// Occurs when the window was closed using the <see cref="Close"/> method. 
    /// This is a game object event.
    /// </summary>
    /// <remarks>
    /// Depending on <see cref="HideOnClose"/> "closed" means either removed from the 
    /// <see cref="UIScreen"/> or only hidden (<see cref="UIControl.IsVisible"/> is 
    /// <see langword="false"/>). This event is only called if the window is closed using the 
    /// <see cref="Close"/> method.
    /// </remarks>
    public event EventHandler<EventArgs> Closed
    {
      add
      {
        var closed = Events.Get<EventArgs>(ClosedEventId);
        closed.Event += value;
      }
      remove
      {
        var closed = Events.Get<EventArgs>(ClosedEventId);
        closed.Event -= value;
      }
    }
    #endregion



    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    /// <summary>
    /// Initializes static members of the <see cref="Window"/> class.
    /// </summary>
    static Window()
    {
      // Windows are the standard focus scopes.
      OverrideDefaultValue(typeof(Window), IsFocusScopePropertyId, true);
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Window"/> class.
    /// </summary>
    public Window()
    {
      Style = "Window";
    }
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <inheritdoc/>
    protected override void OnLoad()
    {
      base.OnLoad();

      // Create icon.
      var iconStyle = IconStyle;
      if (!string.IsNullOrEmpty(iconStyle))
      {
        _icon = new Image
        {
          Name = "WindowIcon",
          Style = iconStyle,
          Texture = Icon,
          SourceRectangle = IconSourceRectangle,
        };        
        VisualChildren.Add(_icon);

        // Connect Icon property with Image.Texture.
        GameProperty<Texture2D> icon = Properties.Get<Texture2D>(IconPropertyId);
        GameProperty<Texture2D> imageTexture = _icon.Properties.Get<Texture2D>(Image.TexturePropertyId);
        icon.Changed += imageTexture.Change;

        // Connect IconSourceRectangle property with Image.SourceRectangle.
        GameProperty<Rectangle?> iconSourceRectangle = Properties.Get<Rectangle?>(IconSourceRectanglePropertyId);
        GameProperty<Rectangle?> imageSourceRectangle = _icon.Properties.Get<Rectangle?>(Image.SourceRectanglePropertyId);
        iconSourceRectangle.Changed += imageSourceRectangle.Change;
      }

      // Create text block for title.
      var titleTextBlockStyle = TitleTextBlockStyle;
      if (!string.IsNullOrEmpty(titleTextBlockStyle))
      {
        _caption = new TextBlock
        {
          Name = "WindowTitle",
          Style = titleTextBlockStyle,
          Text = Title,
        };
        VisualChildren.Add(_caption);

        // Connect Title property with TextBlock.Text.
        GameProperty<string> title = Properties.Get<string>(TitlePropertyId);
        GameProperty<string> captionText = _caption.Properties.Get<string>(TextBlock.TextPropertyId);
        title.Changed += captionText.Change;
      }

      // Create Close button.
      var closeButtonStyle = CloseButtonStyle;
      if (!string.IsNullOrEmpty(closeButtonStyle))
      {
        _closeButton = new Button
        {
          Name = "CloseButton",
          Style = closeButtonStyle,
          Focusable = false,
        };
        VisualChildren.Add(_closeButton);

        _closeButton.Click += OnCloseButtonClick;
      }
    }


    /// <inheritdoc/>
    protected override void OnUnload()
    {
      // Clean up and remove controls for icon, title and close button.

      if (_icon != null)
      {
        var icon = Properties.Get<Texture2D>(IconPropertyId);
        var imageTexture = _icon.Properties.Get<Texture2D>(Image.TexturePropertyId);
        icon.Changed -= imageTexture.Change;

        var iconSourceRectangle = Properties.Get<Rectangle?>(IconSourceRectanglePropertyId);
        var imageSourceRectangle = _icon.Properties.Get<Rectangle?>(Image.SourceRectanglePropertyId);
        iconSourceRectangle.Changed -= imageSourceRectangle.Change;

        VisualChildren.Remove(_icon);
        _icon = null;
      }

      if (_caption != null)
      {
        var title = Properties.Get<string>(TitlePropertyId);
        var captionText = _caption.Properties.Get<string>(TextBlock.TextPropertyId);
        title.Changed -= captionText.Change;

        VisualChildren.Remove(_caption);
        _caption = null;
      }

      if (_closeButton != null)
      {
        _closeButton.Click -= OnCloseButtonClick;
        VisualChildren.Remove(_closeButton);
        _closeButton = null;
      }

      Owner = null;
      IsActive = false;
      base.OnUnload();
    }


    private void OnCloseButtonClick(object sender, EventArgs eventArgs)
    {
      Close();
    }


    /// <summary>
    /// Activates this window (and deactivates all other windows).
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if this window was successfully activated; otherwise, 
    /// <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// Activating also brings the window to the front.
    /// </remarks>
    public bool Activate()
    {
      // We must have a screen. Otherwise, we are not loaded.
      var screen = Screen;
      if (screen == null)
        return false;

      // Move focus into this window.
      if (!IsFocusWithin)
      {
        var gotFocus = screen.FocusManager.Focus(this);

        // If we didn't get the focus (maybe this window does not contain focusable elements),
        // at least remove the focus from the other windows.
        if (!gotFocus)
          screen.FocusManager.ClearFocus();
      }

      if (IsActive)
        return true;
      
      // Deactivate all other windows on the screen.
      foreach (var child in screen.Children)
      {
        var window = child as Window;
        if (window != null && window != this)
          window.IsActive = false;
      }

      // Activate this window.
      IsActive = true;
      screen.BringToFront(this);

      return true;
    }


    /// <summary>
    /// Opens a window and returns without waiting for the newly opened window to close.
    /// </summary>
    /// <param name="owner">
    /// The owner of this window. If this window is closed, the focus moves back to the owner. Must
    /// not be <see langword="null"/>.
    /// </param>
    /// <remarks>
    /// The window is added to the <see cref="UIScreen"/> of the <paramref name="owner"/> 
    /// (unless it was already added to a screen) and activated (see <see cref="Activate"/>).
    /// <see cref="DialogResult"/> is reset to <see langword="null"/>.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="owner"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="owner"/> is not loaded. The owner needs to be a visible control.
    /// </exception>
    public void Show(UIControl owner)
    {
      if (owner == null)
        throw new ArgumentNullException("owner");

      var screen = owner.Screen;
      if (screen == null)
        throw new ArgumentException("Invalid owner. Owner must be loaded.", "owner");

      Owner = owner;

      // Make visible and add to screen.
      IsVisible = true;
      if (VisualParent == null)
        screen.Children.Add(this);
      
      Activate();
      DialogResult = null;
    }


    /// <summary>
    /// Closes this window.
    /// </summary>
    /// <remarks>
    /// This method raises the <see cref="Closing"/> and <see cref="Closed"/> events. The close
    /// operation can be canceled in <see cref="Closing"/>. If <see cref="HideOnClose"/> is 
    /// <see langword="true"/>, the window will only be hidden (<see cref="UIControl.IsVisible"/>
    /// is set to <see langword="false"/>). If <see cref="HideOnClose"/> is <see langword="false"/>
    /// the window is removed from the control tree.
    /// </remarks>
    public void Close()
    {
      if (_isResizing || _isDragging)
      {
        if (UIService != null)
          UIService.Cursor = null;

        _setSpecialCursor = false;
        _isResizing = false;
        _isDragging = false;
      }

      // Raise Closing event and check if close should be canceled.
      var eventArgs = new CancelEventArgs();
      var closing = Events.Get<CancelEventArgs>(ClosingEventId);
      closing.Raise(eventArgs);
      if (eventArgs.Cancel)
        return;

      var screen = Screen;

      // Move focus back to owner - but only if owner is not the screen and the focus
      // is currently in the window.
      if (!(Owner is UIScreen) && IsFocusWithin && screen != null)
        screen.FocusManager.Focus(Owner);

      Owner = null;
      IsActive = false;

      // Hide or remove from parent.
      if (HideOnClose)
      {
        IsVisible = false;
      }
      else
      {
        
        if (screen != null)
          screen.Children.Remove(this);
      }

      // Raise Closed event.
      var closed = Events.Get<EventArgs>(ClosedEventId);
      closed.Raise();
    }


    /// <inheritdoc/>
    protected override void OnHandleInput(InputContext context)
    {
      var screen = Screen;
      var inputService = InputService;
      var uiService = UIService;

      bool mouseOrTouchWasHandled = inputService.IsMouseOrTouchHandled;
      UIControl oldFocusedControl = screen.FocusManager.FocusedControl;

      if (mouseOrTouchWasHandled && _setSpecialCursor)
      {
        // This window has changed the cursor in the last frame, but in this frame another
        // window has the mouse.
        // Minor problem: If the other window has also changed the cursor, then we remove
        // its special cursor. But this case should be rare.
        uiService.Cursor = null;
        _setSpecialCursor = false;
      }

      // Continue resizing and dragging if already in progress.
      ContinueResizeAndDrag(context);
      
      base.OnHandleInput(context);

      if (!IsLoaded)
        return;

      // Handling of activation is very complicated. For example: The window is clicked, but 
      // a context menu is opened by this click. And there are other difficult cases... Horror!
      if (!mouseOrTouchWasHandled)
      {
        // The mouse was not handled by any control that handled input before this window;
        if (!screen.IsFocusWithin                                       // Nothing in window is focused.
            || IsFocusWithin                                            // This window is focused.
            || oldFocusedControl == screen.FocusManager.FocusedControl) // The focus was not changed by a visual child. (Don't 
        {                                                               // activate window if focus moved to a new context menu or other popup!!!)
          // Mouse must be over the window and left or right mouse button must be pressed. 
          if (IsMouseOver)
          {
            if ((inputService.IsPressed(MouseButtons.Left, false) 
                || inputService.IsPressed(MouseButtons.Right, false)))
            {
              Activate();
            }
          }
        }
      }

      // If the focus moves into this window, it should become activated.
      if (IsFocusWithin && !IsActive)
        Activate();

      // Check whether to start resizing or dragging.
      StartResizeAndDrag(context);

      // Update mouse cursor.
      if ((uiService.Cursor == null || _setSpecialCursor)          // Cursor of UIService was set by this window.
          && (!inputService.IsMouseOrTouchHandled || _isResizing)) // Mouse was not yet handled or is currently resizing.
      {
        switch (_resizeDirection)
        {
          case ResizeDirection.N:
          case ResizeDirection.S:
            uiService.Cursor = screen.Renderer.GetCursor("SizeNS");
            _setSpecialCursor = true;
            break;
          case ResizeDirection.E:
          case ResizeDirection.W:
            uiService.Cursor = screen.Renderer.GetCursor("SizeWE");
            _setSpecialCursor = true;
            break;
          case ResizeDirection.NE:
          case ResizeDirection.SW:
            uiService.Cursor = screen.Renderer.GetCursor("SizeNESW");
            _setSpecialCursor = true;
            break;
          case ResizeDirection.NW:
          case ResizeDirection.SE:
            uiService.Cursor = screen.Renderer.GetCursor("SizeNWSE");
            _setSpecialCursor = true;
            break;
          default:
            uiService.Cursor = null;
            _setSpecialCursor = false;
            break;
        }
      }
            
      // Mouse cannot act through a window.
      if (IsMouseOver)
        inputService.IsMouseOrTouchHandled = true;

      if (IsModal)
      {
        // Modal windows absorb all input.
        inputService.IsMouseOrTouchHandled = true;
        inputService.SetGamePadHandled(context.AllowedPlayer, true);
        inputService.IsKeyboardHandled = true;
      }
    }


    private void StartResizeAndDrag(InputContext context)
    {
      if (_isResizing || _isDragging)
        return;

      var canDrag = CanDrag;
      var canResize = CanResize;
      if (!canDrag && !canResize)
        return;

      var inputService = InputService;

      // ----- Find out if mouse position is over the border.      
      _resizeDirection = ResizeDirection.None;
      if (!inputService.IsMouseOrTouchHandled   // Mouse is available for resizing.
          && canResize)                         // Window allows resizing.
      {
        // Position relative to window:
        var mousePosition = context.MousePosition - new Vector2F(ActualX, ActualY);

        // Find resize direction.
        if (IsMouseDirectlyOver)
        {
          bool isWest = 0 <= mousePosition.X && mousePosition.X <= ResizeBorder.X;
          bool isEast = ActualWidth - ResizeBorder.Z < mousePosition.X && mousePosition.X < ActualWidth;
          bool isNorth = 0 <= mousePosition.Y && mousePosition.Y < ResizeBorder.Y;
          bool isSouth = ActualHeight - ResizeBorder.W <= mousePosition.Y && mousePosition.Y < ActualHeight;
          if (isSouth && isEast)
            _resizeDirection = ResizeDirection.SE;
          else if (isSouth && isWest)
            _resizeDirection = ResizeDirection.SW;
          else if (isNorth && isEast)
            _resizeDirection = ResizeDirection.NE;
          else if (isNorth && isWest)
            _resizeDirection = ResizeDirection.NW;
          else if (isSouth)
            _resizeDirection = ResizeDirection.S;
          else if (isEast)
            _resizeDirection = ResizeDirection.E;
          else if (isWest)
            _resizeDirection = ResizeDirection.W;
          else if (isNorth)
            _resizeDirection = ResizeDirection.N;
        }
      }

      // ----- Start resizing.
      if (canResize)
      {
        if (_resizeDirection != ResizeDirection.None && inputService.IsPressed(MouseButtons.Left, false))
        {
          _isResizing = true;
          inputService.IsMouseOrTouchHandled = true;
          _mouseStartPosition = context.MousePosition;
          _startSizeOrPosition = new Vector2F(ActualWidth, ActualHeight);
          return;
        }
      }

      // ----- Start dragging.
      if (canDrag)
      {
        // The window can be grabbed on any point that is not a visual child 
        // (except for icon and title).
        var isOverDragArea = IsMouseDirectlyOver
                             || (_icon != null && _icon.IsMouseOver)
                             || (_caption != null && _caption.IsMouseOver);
        if (isOverDragArea && inputService.IsPressed(MouseButtons.Left, false))
        {
          _isDragging = true;
          inputService.IsMouseOrTouchHandled = true;
          _mouseStartPosition = context.ScreenMousePosition;
          _startSizeOrPosition = new Vector2F(ActualX, ActualY);
          return;
        }
      }
    }


    private void ContinueResizeAndDrag(InputContext context)
    {
      if (!_isResizing && !_isDragging)
      {
        // Nothing to do.
        return;
      }

      var screen = Screen;
      var inputService = InputService;

      // ----- Stop dragging/resizing
      if (inputService.IsUp(MouseButtons.Left)    // Mouse button is up.
          || inputService.IsMouseOrTouchHandled   // Mouse was handled by another control.
          || _isResizing && !CanResize            // CanResize has been reset by user during resizing.
          || _isDragging && !CanDrag)             // CanDrag has been reset by user during dragging.
      {
        screen.UIService.Cursor = null;
        _setSpecialCursor = false;
        _isResizing = false;
        _isDragging = false;
        return;
      }

      // ----- Handle ongoing resizing operation.
      if (_isResizing)
      {
        var screenMousePosition = context.ScreenMousePosition;

        // Clamp mouse position to screen. (Only relevant if game runs in windowed-mode.)        
        float left = screen.ActualX;
        float right = left + screen.ActualWidth;
        float top = screen.ActualY;
        float bottom = top + screen.ActualHeight;
        screenMousePosition.X = MathHelper.Clamp(screenMousePosition.X, left, right);
        screenMousePosition.Y = MathHelper.Clamp(screenMousePosition.Y, top, bottom);

        // Resizing is done in local space of the window.
        // Get absolute render transform and transform the mouse position from screen coordinates
        // to local coordinates.
        var transform = RenderTransform;
        var control = VisualParent;
        while (control != null)
        {
          if (control.HasRenderTransform)
            transform = control.RenderTransform * transform;

          control = control.VisualParent;
        }

        var mousePosition = transform.FromRenderPosition(screenMousePosition);

        // Resize window.
        var delta = mousePosition - _mouseStartPosition;
        if (delta != Vector2F.Zero)
        {
          if (_resizeDirection == ResizeDirection.N
               || _resizeDirection == ResizeDirection.NW
               || _resizeDirection == ResizeDirection.NE)
          {
            float height = _startSizeOrPosition.Y - delta.Y;
            if (height < MinHeight)
              height = MinHeight;
            else if (height > MaxHeight)
              height = MaxHeight;

            Y = Y - (height - ActualHeight);
            Height = height;
          }

          if (_resizeDirection == ResizeDirection.S
               || _resizeDirection == ResizeDirection.SW
               || _resizeDirection == ResizeDirection.SE)
          {
            float height = _startSizeOrPosition.Y + delta.Y;
            if (height < MinHeight)
              height = MinHeight;
            else if (height > MaxHeight)
              height = MaxHeight;

            Height = height;
          }

          if (_resizeDirection == ResizeDirection.NW
               || _resizeDirection == ResizeDirection.W
               || _resizeDirection == ResizeDirection.SW)
          {
            float width = _startSizeOrPosition.X - delta.X;
            if (width < MinWidth)
              width = MinWidth;
            else if (width > MaxWidth)
              width = MaxWidth;

            X = X - (width - ActualWidth);
            Width = width;
          }

          if (_resizeDirection == ResizeDirection.NE
               || _resizeDirection == ResizeDirection.E
               || _resizeDirection == ResizeDirection.SE)
          {
            float width = _startSizeOrPosition.X + delta.X;
            if (width < MinWidth)
              width = MinWidth;
            else if (width > MaxWidth)
              width = MaxWidth;

            Width = width;
          }

          InvalidateArrange();
        }

        inputService.IsMouseOrTouchHandled = true;
        return;
      }

      // ----- Handle ongoing dragging operation.
      if (_isDragging)
      {
        // Dragging is done in screen space. Therefore, we need to use 
        // context.ScreenMousePosition.
        var screenMousePosition = context.ScreenMousePosition;

        // Clamp mouse position to screen. (Only relevant if game runs in windowed-mode.)
        float left = screen.ActualX;
        float right = left + screen.ActualWidth;
        float top = screen.ActualY;
        float bottom = top + screen.ActualHeight;
        screenMousePosition.X = MathHelper.Clamp(screenMousePosition.X, left, right);
        screenMousePosition.Y = MathHelper.Clamp(screenMousePosition.Y, top, bottom);

        // Drag window.
        var delta = screenMousePosition - _mouseStartPosition;
        if (delta != Vector2F.Zero)
        {
          X = _startSizeOrPosition.X + delta.X;
          Y = _startSizeOrPosition.Y + delta.Y;
          InvalidateArrange();
        }

        inputService.IsMouseOrTouchHandled = true;
      }
    }


    /// <summary>
    /// Gets the window that contains the given <paramref name="control"/>.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <returns>
    /// The window that contains the <paramref name="control"/>, or <see langword="null"/> if
    /// the control is not part of a window (controls can be direct children of the screen, no 
    /// intermediate window is required).
    /// </returns>
    public static Window GetWindow(UIControl control)
    {
      while (control != null && !(control is Window))
        control = control.VisualParent;

      return control as Window;
    }
    #endregion
  }
}
