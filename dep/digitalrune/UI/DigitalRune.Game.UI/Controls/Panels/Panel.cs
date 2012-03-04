using DigitalRune.Collections;


namespace DigitalRune.Game.UI.Controls
{
  /// <summary>
  /// Provides a base class for all <see cref="Panel"/> controls. Use panels to position and 
  /// arrange child controls.
  /// </summary>
  public abstract class Panel : UIControl
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------
    #endregion
      
      
    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------

    /// <summary>
    /// Gets the children.
    /// </summary>
    /// <value>The children.</value>
    public NotifyingCollection<UIControl> Children { get; private set; }
    #endregion
      
      
    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="Panel"/> class.
    /// </summary>
    protected Panel()
    {
      Style = "Panel";

      Children = new NotifyingCollection<UIControl>(false, false);
      Children.CollectionChanged += OnChildrenChanged;
    }
    #endregion
      
      
    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    private void OnChildrenChanged(object sender, CollectionChangedEventArgs<UIControl> eventArgs)
    {
      if (eventArgs.Action == CollectionChangedAction.Move)
      {
        // Move visual children too.
        VisualChildren.Move(eventArgs.OldItemsIndex, eventArgs.NewItemsIndex);
        return;
      }

      // Remove old items from VisualChildren too.
      foreach (var oldItem in eventArgs.OldItems)
        VisualChildren.Remove(oldItem);

      // Add new items to VisualChildren.
      var newItemsIndex = eventArgs.NewItemsIndex;
      if (newItemsIndex == -1)
      {
        // Append items.
        foreach (var newItem in eventArgs.NewItems)
          VisualChildren.Add(newItem);
      }
      else
      {
        // Make sure that the same order is used in both collections.
        foreach (var newItem in eventArgs.NewItems)
        {
          VisualChildren.Insert(newItemsIndex, newItem);
          newItemsIndex++;
        }        
      }

      InvalidateMeasure();
    }
    #endregion
  }
}
