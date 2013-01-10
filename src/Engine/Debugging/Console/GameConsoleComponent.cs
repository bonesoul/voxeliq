/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 * 
 * Code based on: http://code.google.com/p/xnagameconsole/
 */


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxeliqEngine.Debugging.Console.Commands;
using VoxeliqEngine.Debugging.Console.KeyboardCapture;

namespace VoxeliqEngine.Debugging.Console
{
    class GameConsoleComponent : DrawableGameComponent
    {
        public bool IsOpen
        {
            get
            {
                return renderer.IsOpen;
            }
        }
        private readonly GameConsole console;
        private readonly SpriteBatch spriteBatch;
        private readonly InputProcessor inputProcesser;
        private readonly Renderer renderer;

        public GameConsoleComponent(GameConsole console, Game game, SpriteBatch spriteBatch)
            : base(game)
        {
            this.console = console;
            EventInput.Initialize(game.Window);
            this.spriteBatch = spriteBatch;
            inputProcesser = new InputProcessor(new CommandProcesser());
            inputProcesser.Open += (s, e) => renderer.Open();
            inputProcesser.Close += (s, e) => renderer.Close();

            renderer = new Renderer(game, spriteBatch, inputProcesser);
            var inbuiltCommands = new IConsoleCommand[] {new ClearScreenCommand(inputProcesser),new ExitCommand(game),new HelpCommand()};
            GameConsoleOptions.Commands.AddRange(inbuiltCommands);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!console.Enabled)
            {
                return;
            }
            spriteBatch.Begin();
            renderer.Draw(gameTime);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (!console.Enabled)
            {
                return;
            }
            renderer.Update(gameTime);
            base.Update(gameTime);
        }

        public void WriteLine(string text)
        {
            inputProcesser.AddToOutput(text);
        }
    }
}