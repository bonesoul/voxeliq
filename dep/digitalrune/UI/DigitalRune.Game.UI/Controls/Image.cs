using System.ComponentModel;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace DigitalRune.Game.UI.Controls
{
  /// <summary>
  /// Represents a control that displays an image.
  /// </summary>
  /// <remarks>
  /// The <see cref="UIControl.Foreground"/> color can be used to tint the image. 
  /// (The default value is <see cref="Color.White"/>.)
  /// </remarks>
  public class Image : UIControl
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------
    #endregion
      
      
    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------
    #endregion


    //--------------------------------------------------------------
    #region Game Object Properties & Events
    //--------------------------------------------------------------

    /// <summary> 
    /// The ID of the <see cref="Texture"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int TexturePropertyId = CreateProperty<Texture2D>(
      typeof(Image), "Texture", GamePropertyCategories.Appearance, null, null,
      UIPropertyOptions.AffectsMeasure);

    /// <summary>
    /// Gets or sets the texture with the image that should be displayed. 
    /// This is a game object property.
    /// </summary>
    /// <value>The texture with the image that should be displayed.</value>
    public Texture2D Texture
    {
      get { return GetValue<Texture2D>(TexturePropertyId); }
      set { SetValue(TexturePropertyId, value); }
    }


    /// <summary> 
    /// The ID of the <see cref="SourceRectangle"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int SourceRectanglePropertyId = CreateProperty<Rectangle?>(
      typeof(Image), "SourceRectangle", GamePropertyCategories.Appearance, null, null,
      UIPropertyOptions.AffectsMeasure);

    /// <summary>
    /// Gets or sets the source rectangle that defines the region of the <see cref="Texture"/>
    /// that should be displayed. This is a game object property.
    /// </summary>
    /// <value>
    /// The source rectangle. Can be <see langword="null"/> if the whole texture should be
    /// displayed.
    /// </value>
    public Rectangle? SourceRectangle
    {
      get { return GetValue<Rectangle?>(SourceRectanglePropertyId); }
      set { SetValue(SourceRectanglePropertyId, value); }
    }
    #endregion    
      
      
    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    /// <summary>
    /// Initializes static members of the <see cref="Image"/> class.
    /// </summary>
    static Image()
    {
      // The default Foreground color must be white because the Foreground color is used for
      // tinting.
      OverrideDefaultValue(typeof(Image), ForegroundPropertyId, Color.White);
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Image"/> class.
    /// </summary>
    public Image()
    {
      Style = "Image";
    }
    #endregion
      
      
    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <inheritdoc/>
    protected override Vector2F OnMeasure(Vector2F availableSize)
    {
      // If nothing else is set, the desired size is determined by the SourceRectangle or 
      // the whole texture.

      var result = base.OnMeasure(availableSize);

      if (Texture == null)
        return result;

      float width = Width;
      float height = Height;
      Vector4F padding = Padding;
      Vector2F desiredSize = Vector2F.Zero;

      if (Numeric.IsPositiveFinite(width))
      {
        desiredSize.X = width;
      }
      else
      {
        var imageWidth = (SourceRectangle != null) ? SourceRectangle.Value.Width : Texture.Width;
        desiredSize.X = padding.X + padding.Z + imageWidth;
      }

      if (Numeric.IsPositiveFinite(height))
      {
        desiredSize.Y = height;
      }
      else
      {
        var imageHeight = (SourceRectangle != null) ? SourceRectangle.Value.Height : Texture.Height;
        desiredSize.Y = padding.Y + padding.W + imageHeight;
      }

      return desiredSize;
    }
    #endregion
  }
}
