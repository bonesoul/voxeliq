using System;

#if WINDOWS
using System.Runtime.Serialization;
#endif


namespace DigitalRune.Game.UI
{
  /// <summary>
  /// Occurs when an exception in the game UI occurs.
  /// </summary>
  [Serializable]
  public class UIException : Exception
  {
    /// <overloads>
    /// <summary>
    /// Initializes a new instance of the <see cref="UIException"/> class.
    /// </summary>
    /// </overloads>
    /// <summary>
    /// Initializes a new instance of the <see cref="UIException"/> class.
    /// </summary>
    public UIException()
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="UIException"/> class with a specified 
    /// error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public UIException(string message)
      : base(message)
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="UIException"/> class with a specified 
    /// error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or <see langword="null"/> if no 
    /// inner exception is specified.
    /// </param>
    public UIException(string message, Exception innerException)
      : base(message, innerException)
    {
    }


#if WINDOWS
    /// <summary>
    /// Initializes a new instance of the <see cref="UIException"/> class with serialized 
    /// data.
    /// </summary>
    /// <param name="info">
    /// The <see cref="SerializationInfo"/> that holds the serialized object data about the 
    /// exception being thrown.
    /// </param>
    /// <param name="context">
    /// The <see cref="StreamingContext"/> that contains contextual information about the source or 
    /// destination.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="info"/> parameter is <see langword="null"/>.
    /// </exception>
    /// <exception cref="SerializationException">
    /// The class name is <see langword="null"/> or <see cref="Exception.HResult"/> is zero (0).
    /// </exception>
    protected UIException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
#endif
  }
}
