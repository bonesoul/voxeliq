using System;
using Microsoft.Xna.Framework.Input;

#if WINDOWS
using System.Windows.Forms;
#endif

#if USE_DIGITALRUNE_MATHEMATICS
using DigitalRune.Mathematics.Algebra;
#else
using Vector2F = Microsoft.Xna.Framework.Vector2;
using Vector3F = Microsoft.Xna.Framework.Vector3;
#endif


namespace DigitalRune.Game.Input
{
  /// <summary>
  /// Defines settings for the input service.
  /// </summary>
  public class InputSettings
  {
    /// <summary>
    /// Gets or sets the double click time interval which defines the time that is allowed between
    /// two clicks to still count as a double click.
    /// </summary>
    /// <value>
    /// The double click time interval. The default value is the same as 
    /// <strong>SystemInformation.DoubleClickTime</strong> in Windows and 500 ms on all other 
    /// platforms.
    /// </value>
    public TimeSpan DoubleClickTime { get; set; }


    /// <summary>
    /// Gets or sets the mouse center for the mouse centering.
    /// </summary>
    /// <value>The mouse center in pixels. The default values (300, 300).</value>
    /// <remarks>
    /// If <see cref="IInputService.EnableMouseCentering"/> is <see langword="true"/>, the input
    /// service will reset the mouse position to <see cref="MouseCenter"/> in each frame. This is 
    /// necessary, for example, for first-person shooters that need only relative mouse input.
    /// </remarks>
    public Vector2F MouseCenter { get; set; }


    /// <summary>
    /// Gets or sets the repetition start delay for virtual key or button presses.
    /// </summary>
    /// <value>The repetition start delay. The default value is 500 ms.</value>
    /// <remarks>
    /// If a key or button is held down for longer than the <see cref="RepetitionDelay"/>, the input 
    /// service will start to generate virtual key/button presses at a rate defined by 
    /// <see cref="RepetitionInterval"/>. (See <see cref="IInputService"/> for more info.)
    /// </remarks>
    public TimeSpan RepetitionDelay { get; set; }


    /// <summary>
    /// Gets or sets the repetition interval for virtual key or button presses.
    /// </summary>
    /// <value>The repetition interval. The default value is 100 ms.</value>
    /// <remarks>
    /// If a key or button is held down for longer than the <see cref="RepetitionDelay"/>,
    /// the input service will start to generate virtual key/button presses at a rate defined by 
    /// <see cref="RepetitionInterval"/>. (See <see cref="IInputService"/> for more info.)
    /// </remarks>
    public TimeSpan RepetitionInterval { get; set; }


    /// <summary>
    /// Gets or sets the thumbstick threshold for detecting thumbstick button presses.
    /// </summary>
    /// <value>
    /// The thumbstick threshold value in the range [0, 1]. A thumbstick axis counts as "down" if 
    /// its absolute value exceeds the threshold value. The default value is 0.5.
    /// </value>
    public float ThumbstickThreshold { get; set; }


    /// <summary>
    /// Gets or sets the trigger threshold for detecting button presses.
    /// </summary>
    /// <value>
    /// The trigger threshold value in the range [0, 1]. A trigger counts as "down" if its value 
    /// exceeds the threshold value. The default value is 0.2.
    /// </value>
    public float TriggerThreshold { get; set; }


    /// <summary>
    /// Gets or sets the type of gamepad dead zone processing that is used for analog sticks
    /// of the gamepads.
    /// </summary>
    /// <value>
    /// The type of dead zone processing. The default value is 
    /// <strong>GamePadDeadZone.IndependentAxes</strong>.
    /// </value>
    public GamePadDeadZone GamePadDeadZone { get; set; }


    /// <summary>
    /// Initializes a new instance of the <see cref="InputSettings"/> class.
    /// </summary>
    public InputSettings()
    {
#if WINDOWS
      DoubleClickTime = new TimeSpan(0, 0, 0, 0, SystemInformation.DoubleClickTime);
#else
      DoubleClickTime = new TimeSpan(0, 0, 0, 0, 500);    // 500 ms
#endif
      MouseCenter = new Vector2F(300, 300);
      RepetitionDelay = new TimeSpan(0, 0, 0, 0, 500);    // 500 ms
      RepetitionInterval = new TimeSpan(0, 0, 0, 0, 100); // 100 ms
      ThumbstickThreshold = 0.5f;
      TriggerThreshold = 0.2f;
      GamePadDeadZone = GamePadDeadZone.IndependentAxes;
    }
  }
}
