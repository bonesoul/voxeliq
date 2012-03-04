using System;


namespace DigitalRune.Game.UI.Consoles
{
  /// <summary>
  /// Provides arguments for a <see cref="IConsole.CommandEntered"/> event.
  /// </summary>
  public class ConsoleCommandEventArgs : EventArgs
  {
    /// <summary>
    /// Gets the command arguments.
    /// </summary>
    /// <value>The command arguments.</value>
    /// <remarks>
    /// The first argument is the command name.
    /// </remarks>
    public string[] Args { get; private set; }


    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="ConsoleCommandEventArgs"/> was
    /// handled.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the command was handled; otherwise, <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// Event handler that have handled the command must set this property. 
    /// </remarks>
    public bool Handled { get; set; }


    /// <summary>
    /// Constructs a new instance of the <see cref="ConsoleCommandEventArgs"/> class.
    /// </summary>
    /// <param name="args">The command arguments.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="args"/> is <see langword="null"/>.
    /// </exception>
    public ConsoleCommandEventArgs(string[] args)
    {
      if (args == null)
        throw new ArgumentNullException("args");

      Args = args;
    }
  }
}
