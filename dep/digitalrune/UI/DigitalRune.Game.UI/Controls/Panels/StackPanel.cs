using System;
using System.ComponentModel;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;


namespace DigitalRune.Game.UI.Controls
{
  /// <summary>
  /// Arranges child elements into a single line that can be oriented horizontally or vertically. 
  /// </summary>
  public class StackPanel : Panel
  {
    //--------------------------------------------------------------
    #region Game Object Properties & Events
    //--------------------------------------------------------------

    /// <summary> 
    /// The ID of the <see cref="Orientation"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int OrientationPropertyId = CreateProperty(
      typeof(StackPanel), "Orientation", GamePropertyCategories.Layout, null, Orientation.Vertical,
      UIPropertyOptions.AffectsMeasure);

    /// <summary>
    /// Gets or sets the orientation of the stack panel. 
    /// This is a game object property.
    /// </summary>
    /// <value>The orientation of the stack panel.</value>
    public Orientation Orientation
    {
      get { return GetValue<Orientation>(OrientationPropertyId); }
      set { SetValue(OrientationPropertyId, value); }
    }
    #endregion


    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="StackPanel"/> class.
    /// </summary>
    public StackPanel()
    {
      Style = "StackPanel";
    }
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <inheritdoc/>
    protected override Vector2F OnMeasure(Vector2F availableSize)
    {
      // Similar to UIControl.OnMeasure, but we sum up the desired sizes in the stack panel 
      // orientation. In the other direction we take the maximum of the children - unless a 
      // Width or Height was set explicitly. If there are VisualChildren that are not Children, 
      // they do not contribute to the DesiredSize.

      float width = Width;
      float height = Height;
      bool hasWidth = Numeric.IsPositiveFinite(width);
      bool hasHeight = Numeric.IsPositiveFinite(height);

      if (hasWidth && width < availableSize.X)
        availableSize.X = width;
      if (hasHeight && height < availableSize.Y)
        availableSize.Y = height;

      foreach (var child in VisualChildren)
        child.Measure(availableSize);

      if (hasWidth && hasHeight)
        return new Vector2F(width, height);

      var desiredSize = new Vector2F(width, height);

      float maxWidth = 0;
      float sumWidth = 0;
      float maxHeight = 0;
      float sumHeight = 0;

      // Sum up widths and heights.
      foreach (var child in Children)
      {
        float childWidth = child.DesiredWidth;
        float childHeight = child.DesiredHeight;
        maxWidth = Math.Max(maxWidth, childWidth);
        maxHeight = Math.Max(maxHeight, childHeight);
        sumWidth += childWidth;
        sumHeight += childHeight;
      }

      if (!hasWidth)
      {
        if (Orientation == Orientation.Horizontal)
          desiredSize.X = sumWidth;
        else
          desiredSize.X = maxWidth;
      }

      if (!hasHeight)
      {
        if (Orientation == Orientation.Vertical)
          desiredSize.Y = sumHeight;
        else
          desiredSize.Y = maxHeight;
      }

      return desiredSize;
    }


    /// <inheritdoc/>
    protected override void OnArrange(Vector2F position, Vector2F size)
    {
      // Get extreme positions of arrange area.
      float left = position.X;
      float top = position.Y;
      float right = left + size.X;
      float bottom = top + size.Y;

      if (Orientation == Orientation.Horizontal)
      {
        // ----- Horizontal: 
        // Each child gets its desired width or the rest of the available space.
        foreach (var child in VisualChildren)
        {
          float availableSize = Math.Max(0.0f, right - left);
          float sizeX = Math.Min(availableSize, child.DesiredWidth);
          float sizeY = size.Y;
          Arrange(child, new Vector2F(left, top), new Vector2F(sizeX, sizeY));
          left += sizeX;
        }
      }
      else
      {
        // ----- Vertical
        // Each child gets its desired height or the rest of the available space.
        foreach (var child in VisualChildren)
        {
          float sizeX = size.X;
          float availableSize = Math.Max(0.0f, bottom - top);
          float sizeY = Math.Min(availableSize, child.DesiredHeight);
          Arrange(child, new Vector2F(left, top), new Vector2F(sizeX, sizeY));
          top += sizeY;
        }
      }
    }
    #endregion
  }
}
