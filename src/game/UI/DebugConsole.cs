using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using DigitalRune.Game.Input;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Consoles;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Game.UI.Rendering;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Console = DigitalRune.Game.UI.Controls.Console;

namespace VolumetricStudios.VoxeliqGame.UI
{
    // Creates a UIScreen for debugging. The screen contains a console, but other controls
    // or text could be added as well.
    // The screen is drawn on top of all other screens and game components.
    public class DebugConsole : DrawableGameComponent
    {
        private readonly IInputService _inputService;
        private readonly IUIService _uiService;

        private UIScreen _screen;
        private Console _console;

        private Task _loadUITask;


        public DebugConsole(Game game)
            : base(game)
        {
            // Get the services that this component needs regularly.
            _inputService = (IInputService)game.Services.GetService(typeof(IInputService));
            _uiService = (IUIService)game.Services.GetService(typeof(IUIService));

            // The debug screen is displayed on top of all other screens and components. It should
            // be updated first and drawn on top of the other content.
            UpdateOrder = -100;   // (Update order is front-to-back.)
            DrawOrder = 100;      // (Draw order is back-to-front.)
        }


        protected override void LoadContent()
        {
            // Load UI in a background task.
            _loadUITask = Parallel.StartBackground(LoadUI);

            base.LoadContent();
        }


        private void LoadUI()
        {
            try
            {
                // Load a UI theme and create a renderer. 
                // We could use the same renderer as the "Default" screen (see StartScreenComponent.cs).
                // But usually, the debug screen will use a more efficient theme (smaller fonts, no
                // fancy graphics). Here, we simply use the Neoforce theme again.
                var uiThemeContent = new ContentManager(Game.Services, "BlendBlueTheme");
                var theme = uiThemeContent.Load<Theme>("Theme");
                UIRenderer renderer = new UIRenderer(Game, theme);

                // Create a UIScreen and add it to the UI service. 
                _screen = new UIScreen("Debug", renderer)
                {
                    // A transparent background.
                    Background = new Color(0, 0, 0, 0),

                    // The z-index is equal to the draw order. The z-index defines in which order the 
                    // screens are updated. The "Debug" screen has a higher z-index and is updated 
                    // before the "Default" screen.
                    ZIndex = DrawOrder,

                    // Hide the screen. The user has to press a button to make the debug screen visible.
                    IsVisible = false,
                };

                // Optional: 
                // The debug screen handles gamepad input first, then the other screens and game components
                // can handle input. We do not want that the game is controllable when the debug screen is
                // visible, therefore we set the IsHandled flags when the screen is finished with the input.
                _screen.InputProcessed += (s, e) => _inputService.SetGamePadHandled(LogicalPlayerIndex.Any, true);

                // Add a console control on the left.
                _console = new Console
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Top,
                    Height = 250,
                };
                _screen.Children.Add(_console);

                // Print a few info messages in the console.
                _console.WriteLine("voxeliq " + Assembly.GetExecutingAssembly().GetName().Version + " debug console..");
                _console.WriteLine("Enter 'help' to view console commands.");

                // Add a custom command:
                _console.Interpreter.Commands.Add(new ConsoleCommand("greet", "greet [<name>] ... Prints a greeting message.", Greet));


                _console.Interpreter.Commands.Add(new ConsoleCommand("version", "Displays engine version", callback => _console.WriteLine(Assembly.GetExecutingAssembly().GetName().Version.ToString())));

                // Add the screen to the UI service. We must lock the collection because the UI service
                // runs in a parallel thread.
                lock (((ICollection)_uiService.Screens).SyncRoot)
                {
                    _uiService.Screens.Add(_screen);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Exception in LoadUI():\n" + exception);
                throw;
            }
        }


        /// <summary>
        /// Handles the "greet" console command.
        /// </summary>
        /// <param name="args">
        /// The command arguments entered by the user. The first argument is the command name.
        /// Other arguments are optional.
        /// </param>
        private void Greet(string[] args)
        {
            if (args.Length > 1)
                _console.WriteLine("Hello " + args[1] + "!");
            else
                _console.WriteLine("Hello!");
        }


        public override void Update(GameTime gameTime)
        {
            if (_loadUITask.IsComplete)
            {
                if (!_inputService.IsKeyboardHandled &&  (_inputService.IsPressed(Keys.Tab, false)
                        || _inputService.IsPressed(Keys.F8, false)))
                {
                    _inputService.IsKeyboardHandled = true;

                    // Toggle visibility of screen when TAB or ChatPadGreen is pressed.
                    _screen.IsVisible = !_screen.IsVisible;

                    // If the screen becomes visible, make sure that the console has the input focus.
                    if (_screen.IsVisible)
                        _console.Focus();
                }
            }

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            if (_loadUITask.IsComplete)
            {
                // Draw the screen. This method does nothing if _screen.IsVisible is false.
                _screen.Draw(gameTime);
            }

            base.Draw(gameTime);
        }
    }
}
