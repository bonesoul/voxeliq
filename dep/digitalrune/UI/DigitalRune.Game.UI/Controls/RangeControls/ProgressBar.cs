using System;
using System.ComponentModel;


namespace DigitalRune.Game.UI.Controls
{
  /// <summary>
  /// Indicates the progress of an operation. 
  /// </summary>
  /// <remarks>
  /// The progress is usually indicated as the length of a bar (determined by the 
  /// <see cref="RangeBase.Value"/>. If the mode is set to <see cref="IsIndeterminate"/>, the value 
  /// automatically cycles between 0 and 100 and an animation based on this value is rendered.
  /// </remarks>
  public class ProgressBar : RangeBase
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------

    // Current direction of cycling value in Indeterminate mode.
    private bool _forward = true;
    #endregion
      
      
    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------
    #endregion


    //--------------------------------------------------------------
    #region Game Object Properties & Events
    //--------------------------------------------------------------

    /// <summary> 
    /// The ID of the <see cref="IsIndeterminate"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int IsIndeterminatePropertyId = CreateProperty(
      typeof(ProgressBar), "IsIndeterminate", GamePropertyCategories.Behavior, null, false, 
      UIPropertyOptions.AffectsRender);

    /// <summary>
    /// Gets or sets a value that indicates whether the progress bar reports generic progress with
    /// a repeating pattern or reports progress based on the <see cref="RangeBase.Value"/> property. 
    /// This is a game object property.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the progress bar reports generic progress with a repeating 
    /// pattern; <see langword="false"/> if the progress bar reports progress based on the 
    /// <see cref="RangeBase.Value"/> property. The default is <see langword="false"/>. 
    /// </value>
    public bool IsIndeterminate
    {
      get { return GetValue<bool>(IsIndeterminatePropertyId); }
      set { SetValue(IsIndeterminatePropertyId, value); }
    }


    /// <summary> 
    /// The ID of the <see cref="IsIndeterminate"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int IndeterminateCycleTimePropertyId = CreateProperty(
      typeof(ProgressBar), "IndeterminateCycleTime", GamePropertyCategories.Appearance, null, 
      TimeSpan.FromSeconds(4), UIPropertyOptions.AffectsRender);

    /// <summary>
    /// Gets or sets a value indicating how long an animation cycle in <see cref="IsIndeterminate"/> 
    /// mode takes. This is a game object property.
    /// </summary>
    /// <value>
    /// A value indicating how long an animation cycle in <see cref="IsIndeterminate"/> mode takes. 
    /// The default value is 4 seconds.
    /// </value>
    public TimeSpan IndeterminateCycleTime
    {
      get { return GetValue<TimeSpan>(IndeterminateCycleTimePropertyId); }
      set { SetValue(IndeterminateCycleTimePropertyId, value); }
    }
    #endregion
          
      
    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    /// <summary>
    /// Initializes static members of the <see cref="ProgressBar"/> class.
    /// </summary>
    static ProgressBar()
    {
      OverrideDefaultValue(typeof(ProgressBar), IndeterminateCycleTimePropertyId, TimeSpan.FromSeconds(4));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressBar"/> class.
    /// </summary>
    public ProgressBar()
    {
      Style = "ProgressBar";
    }
    #endregion
      
      
    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <inheritdoc/>
    protected override void OnUpdate(TimeSpan deltaTime)
    {
      // Animate indeterminate progress bar.
      if (IsIndeterminate && IsEnabled && IsVisible)
      {
        // Change current value.
        var period = (float)IndeterminateCycleTime.TotalSeconds;
        var t = (float)deltaTime.TotalSeconds;
        float change = 100 / period * 2 * t;
        float newValue;
        if (_forward)
          newValue = Value + change;
        else
          newValue = Value - change;

        // Change bar movement direction at Minimum and Maximum.
        if (newValue < Minimum)
        {
          newValue = Math.Abs(newValue);
          _forward = true;
        }
        else if (newValue > Maximum)
        {
          newValue = Maximum - (newValue - Maximum);
          _forward = false;
        }

        Value = newValue;
      }

      base.OnUpdate(deltaTime);
    }
    #endregion
  }
}
