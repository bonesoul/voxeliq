using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input.Touch;


namespace DigitalRune.Game.Input
{
  partial class InputManager
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------
    #endregion


    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------   

    /// <inheritdoc/>
    public TouchCollection TouchCollection
    {
      get { return _touchCollection; }
    }
    private TouchCollection _touchCollection;


    /// <inheritdoc/>
    public List<GestureSample> Gestures
    {
      get { return _gestures; }
    }
    private readonly List<GestureSample> _gestures;
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    private void UpdateTouch(TimeSpan deltaTime)
    {
      // Touch input
      _touchCollection = TouchPanel.GetState();

      // Touch gestures
      _gestures.Clear();
      while (TouchPanel.IsGestureAvailable)
        _gestures.Add(TouchPanel.ReadGesture());
    }
    #endregion
  }
}
