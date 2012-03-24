/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

#if XNA

using DigitalRune.Game.Input;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Game.UI.Rendering;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace VolumetricStudios.VoxeliqGame.UI
{
    public class GameScreenOverlay : DrawableGameComponent
    {
        private IInputService _inputService;
        private IUIService _uiService;
        private UIScreen _screen;

        public GameScreenOverlay(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            // Get the services that this component needs regularly.
            this._inputService = (IInputService)this.Game.Services.GetService(typeof(IInputService));
            _uiService = (IUIService)this.Game.Services.GetService(typeof(IUIService));

            // Load a UI theme, which defines the appearance and default values of UI controls.
            var content = new ContentManager(Game.Services, "BlendBlueTheme");
            var theme = content.Load<Theme>("Theme");

            // Create a UI renderer, which uses the theme info to renderer UI controls.
            UIRenderer renderer = new UIRenderer(Game, theme);

            // Create a UIScreen and add it to the UI service. The screen is the root of the 
            // tree of UI controls. Each screen can have its own renderer.
            _screen = new UIScreen("Overlay", renderer)
                          {
                              Background = new Color(0, 0, 0, 0),
                              // Make the screen transparent.
                          };

            _uiService.Screens.Add(_screen);

            var progressbar = new ProgressBar()
                                  {
                                      Value = 50,
                                      Margin = new Vector4F(4),
                                      Padding = new Vector4F(6),
                                      HorizontalAlignment = HorizontalAlignment.Center,
                                  };


            var button4 = new Button
                              {
                                  Content = new TextBlock { Text = "Sample #5: Console" },
                                  Margin = new Vector4F(4),
                                  Padding = new Vector4F(6),
                                  HorizontalAlignment = HorizontalAlignment.Center
                              };
            button4.Click += (s, e) =>
                                 {
                                     var consoleWindow = new SampleConsole();
                                     consoleWindow.Show(_screen);
                                 };

            //var stackPanel = new StackPanel { Margin = new Vector4F(40) };
            //stackPanel.Children.Add(button4);
            //_screen.Children.Add(stackPanel);

            //_screen.Children.Add(button4);
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw the screen. UIScreen.Draw() must be called manually. (If we want, we could also
            // render the screen into an offscreen render target.)
            _screen.Draw(gameTime);
        }
    }
}

#endif