#if USE_DIGITALRUNE_MATHEMATICS
using DigitalRune.Mathematics.Algebra;
#else
using Vector2F = Microsoft.Xna.Framework.Vector2;
using Vector3F = Microsoft.Xna.Framework.Vector3;
#endif

#if WINDOWS_PHONE
using System.Diagnostics;
using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework.Input;
#endif


namespace DigitalRune.Game.Input
{
  partial class InputManager
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------

    private bool _isAccelerometerInitialized;

#if WINDOWS_PHONE
    // The accelerometer sensor on the device.
    private readonly Accelerometer _accelerometer = new Accelerometer();

    // Value is set asynchronously in callback.    
    private Vector3F _accelerometerCallbackValue;  
    private readonly object _syncRoot = new object();
#endif
    #endregion


    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------   

    /// <inheritdoc/>
    public bool IsAccelerometerActive { get; private set; }


    /// <inheritdoc/>
    public Vector3F AccelerometerValue
    {
      get
      {
        if (!_isAccelerometerInitialized)
        {
          _isAccelerometerInitialized = true;

#if WINDOWS_PHONE
          // Try to start the sensor only on devices, catching the exception if it fails.
          if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Device)
          {
            try
            {
              _accelerometer.ReadingChanged += OnAccelerometerReadingChanged;
              _accelerometer.Start();
              IsAccelerometerActive = true;
            }
            catch (AccelerometerFailedException)
            {
              IsAccelerometerActive = false;
            }
          }
          else
          {
            // We always return IsAccelerometerActive on emulator because we use the arrow keys for 
            // simulation which is always available.
            IsAccelerometerActive = true;
          }
#endif
        }

        return _accelerometerValue;
      }
    }
    private static Vector3F _accelerometerValue = new Vector3F(0, 0, -1);
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

#if WINDOWS_PHONE
    // Accelerometer callback.
    private void OnAccelerometerReadingChanged(object sender, AccelerometerReadingEventArgs eventArgs)
    {
      // Store the accelerometer value in our variable to be used on the next Update. This callback
      // can come from another thread!
      lock (_syncRoot)
      {
        _accelerometerCallbackValue = new Vector3F((float)eventArgs.X, (float)eventArgs.Y, (float)eventArgs.Z);
      }
    }
#endif


    private void UpdateAccelerometer()
    {
#if WINDOWS_PHONE
      if (_isAccelerometerInitialized)
      {
        if (IsAccelerometerActive)
        {
          if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Device)
          {
            // If we're on device, we'll just grab our latest reading from the accelerometer.
            lock (_syncRoot)
            {
              _accelerometerValue = _accelerometerCallbackValue;
            }
          }
          else
          {
            // Emulator
            // If we're in the emulator, we'll generate a fake acceleration values using the arrow 
            // keys and the Space key. Press the Pause/Break key or PageUp and PageDown keys to 
            // toggle keyboard input for the emulator.
            _accelerometerValue = Vector3F.Zero;

            if (_newKeyboardState.IsKeyDown(Keys.Left))
              _accelerometerValue.X--;
            if (_newKeyboardState.IsKeyDown(Keys.Right))
              _accelerometerValue.X++;
            if (_newKeyboardState.IsKeyDown(Keys.Up))
              _accelerometerValue.Y++;
            if (_newKeyboardState.IsKeyDown(Keys.Down))
              _accelerometerValue.Y--;
            if (_newKeyboardState.IsKeyDown(Keys.Space))
              _accelerometerValue.Z++;
            else
              _accelerometerValue.Z--;

            // We can safely normalize, because the accelerometerValue will not be a zero vector!
            Debug.Assert(_accelerometerValue != Vector3F.Zero, "_accelerometerValue must not be 0.");
            _accelerometerValue.Normalize();
          }
        }
      }
#endif
    }
    #endregion
  }
}
