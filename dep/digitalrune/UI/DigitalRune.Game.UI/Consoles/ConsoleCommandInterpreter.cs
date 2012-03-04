using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using DigitalRune.Collections;


namespace DigitalRune.Game.UI.Consoles
{
  /// <summary>
  /// Handles game console commands.
  /// </summary>
  public class ConsoleCommandInterpreter
  {
    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------

    /// <summary>
    /// Gets the <see cref="IConsole"/>.
    /// </summary>
    /// <value>The <see cref="IConsole"/>.</value>
    public IConsole Console { get; private set; }


    /// <summary>
    /// Gets the commands.
    /// </summary>
    /// <value>The commands.</value>
    /// <remarks>
    /// New custom commands can be added. If the commands throw exceptions, the exceptions are 
    /// caught and displayed in the console.
    /// </remarks>
    public NamedObjectCollection<ConsoleCommand> Commands { get; private set; }
    #endregion
      
      
    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleCommandInterpreter"/> class.
    /// </summary>
    /// <param name="console">The <see cref="IConsole"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="console"/> is <see langword="null"/>.
    /// </exception>
    public ConsoleCommandInterpreter (IConsole console)
    {
      if (console == null)
        throw new ArgumentNullException("console");

      Console = console;
      Commands = new NamedObjectCollection<ConsoleCommand>();

      // Add default built-in commands.
      //Commands.Add(new ConsoleCommand("add", "Prints the sum of two variables: add <value1> <value2>.", Add));
      Commands.Add(new ConsoleCommand("clear", "Clears the console.", Clear));
      Commands.Add(new ConsoleCommand("gc", "Forces a full garbage collection.", CollectGarbage));
      Commands.Add(new ConsoleCommand("parse", "Prints the command and its arguments (for debugging the console).", ParseCommand));
    }
    #endregion
      
      
    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <summary>
    /// Handles the specified command. 
    /// </summary>
    /// <param name="eventArgs">
    /// The <see cref="ConsoleCommandEventArgs"/> instance containing the event data.
    /// </param>
    /// <remarks>
    /// Types that implementing <see cref="IConsole"/> should call this method after the 
    /// <see cref="IConsole.CommandEntered"/> event was executed.
    /// </remarks>
    public void Interpret(ConsoleCommandEventArgs eventArgs)
    {
      // Abort if eventArgs does not contain useful arguments. The first parameter must be a
      // command name.
      if (eventArgs == null)
        return;

      var args = eventArgs.Args;
      if (args == null || args.Length == 0)
        return;      

      try
      {
        var commandName = args[0].ToLowerInvariant();
        if (commandName == "help")
        {
          // ----- Help command.

          // If the previous command handlers have handled this command then we write
          // a newline to create some space between the help texts.
          if (eventArgs.Handled)
            Console.WriteLine();

          if (args.Length == 1)
            WriteHelp();                    // Write general help.
          else if (!eventArgs.Handled)
            WriteCommandHelp(args[1]);      // help <command> --> Write specific help.
          return;
        }

        // Abort if command (other than "help") was already handled.
        if (eventArgs.Handled)
          return;

        ConsoleCommand command;
        bool found = Commands.TryGet(commandName, out command);
        if (!found)
          throw new ConsoleCommandException(ConsoleCommandException.ErrorInvalidCommand, args[0]);

        if (command.Execute != null)
          command.Execute(args);
      }
      catch (Exception exception)
      {
        // We catch all exceptions and print them.
        Console.WriteLine(exception.Message);
      }
    }


    /// <summary>
    /// Displays a general help text.
    /// </summary>
    private void WriteHelp()
    {
      Console.WriteLine("General Console Help:");
      Console.WriteLine("---------------------");
      Console.WriteLine("To enter \" use following escape sequence: \\\"");
      Console.WriteLine("To get help for a command type: help <command>");
      Console.WriteLine();
      Console.WriteLine("List of commands:");

      foreach(var name in Commands.Select(c => c.Name).OrderBy(n => n))
        Console.WriteLine(name);
    }


    /// <summary>
    /// Displays a description of a single command.
    /// </summary>
    /// <param name="commandName">Name of the command.</param>
    private void WriteCommandHelp(string commandName)
    {
      if (!Commands.Contains(commandName))
        throw new ConsoleCommandException(ConsoleCommandException.ErrorInvalidArgument, commandName);

      var command = Commands[commandName];
      Console.WriteLine(command.Description);
    }


    //private void Add(string[] args)
    //{
    //  if (args.Length != 3)
    //    throw new ConsoleCommandException(ConsoleCommandException.ErrorInvalidNumberOfArguments);

    //  float value1 = float.Parse(args[1], CultureInfo.InvariantCulture);
    //  float value2 = float.Parse(args[2], CultureInfo.InvariantCulture);

    //  Console.WriteLine((value1 + value2).ToString(CultureInfo.InvariantCulture));
    //}


    /// <summary>
    /// Clears the console.
    /// </summary>
    private void Clear(string[] args)
    {
      Console.Clear();
    }


    /// <summary>
    /// Calls the garbage collector and displays the collection time.
    /// </summary>
    private void CollectGarbage(string[] args)
    {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      GC.Collect();
      stopwatch.Stop();
      Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Performed full garbage collection ({0} ms)", stopwatch.Elapsed.TotalMilliseconds));
    }


    /// <summary>
    /// Displays the given arguments (for debugging the console itself).
    /// </summary>
    /// <param name="args">The arguments.</param>
    private void ParseCommand(string[] args)
    {
      foreach (var arg in args)
        Console.WriteLine(arg);
    }
    #endregion
  }
}
