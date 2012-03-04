using System.Diagnostics;
using DigitalRune.Collections;


namespace DigitalRune.Game.UI.Controls
{
  /// <summary>
  /// Manages a collection of child <see cref="UIControl"/>s for a parent <see cref="UIControl"/>.
  /// </summary>
  public class VisualChildCollection : ChildCollection<UIControl, UIControl>
  {
    // Special tasks:
    // - Calls UIControl.Load/Unload() when needed.
    // - Stores an isChanged flag, so that the owner knows when the collection has changed.
    
    // TODO: Turn into full NotifyingCollection or something else more powerful.


    /// <summary>
    /// Gets or sets a value indicating whether the collection is modified.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if this collection is modified; otherwise, <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// This flag is read and reset by the <see cref="UIControl"/>.
    /// </remarks>
    internal bool IsChanged { get; set; }


    /// <summary>
    /// Initializes a new instance of the <see cref="VisualChildCollection"/> class.
    /// </summary>
    /// <param name="parent">The parent.</param>
    internal VisualChildCollection(UIControl parent) : base(parent)
    {
    }


    /// <inheritdoc/>
    protected override void ClearItems()
    {
      IsChanged = true;
      base.ClearItems();
    }


    /// <inheritdoc/>
    protected override void InsertItem(int index, UIControl child)
    {
      IsChanged = true;
      base.InsertItem(index, child);
    }


    /// <inheritdoc/>
    protected override void RemoveItem(int index)
    {
      IsChanged = true;
      base.RemoveItem(index);
    }


    /// <inheritdoc/>
    protected override void SetItem(int index, UIControl child)
    {
      IsChanged = true;
      base.SetItem(index, child);
    }


    /// <inheritdoc/>
    protected override void MoveItem(int oldIndex, int newIndex)
    {
      IsChanged = true;
      base.MoveItem(oldIndex, newIndex);
    }


    /// <inheritdoc/>
    protected override UIControl GetParent(UIControl child)
    {
      return child.VisualParent;
    }


    /// <inheritdoc/>
    protected override void SetParent(UIControl child, UIControl parent)
    {
      Debug.Assert(child.VisualParent != parent, "SetParent() should not be called if the correct parent is already set.");      

      if (parent == null)
      {
        child.Unload();
        child.VisualParent = null;
      }
      else if (parent.IsLoaded)
      {
        child.VisualParent = parent;
        child.Load();
      }
      else
      {
        child.VisualParent = parent;
      }
    }
  }
}
