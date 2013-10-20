/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

/* Code based on: http://code.google.com/p/xnagameconsole/ */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Debugging.Console
{
    public class GameConsole
    {
        public GameConsoleOptions Options { get { return GameConsoleOptions.Options; } }
        public bool Enabled { get; set; }

        /// <summary>
        /// Indicates whether the console is currently opened
        /// </summary>
        public bool Opened { get { return console.IsOpen; } }

        private readonly GameConsoleComponent console;

        public GameConsole(Game game, SpriteBatch spriteBatch) :this(game,spriteBatch,new GameConsoleOptions()){}

        public GameConsole(Game game, SpriteBatch spriteBatch, GameConsoleOptions options) 
        {
            if (options.Font == null)
            {
                options.Font = game.Content.Load<SpriteFont>("ConsoleFont");
            }
            options.RoundedCorner = game.Content.Load<Texture2D>(@"Textures/roundedCorner");
            GameConsoleOptions.Options = options;
            Enabled = true;
            console = new GameConsoleComponent(this, game, spriteBatch);
            game.Services.AddService(typeof(GameConsole), this);
            game.Components.Add(console); 
        }

        /// <summary>
        /// Write directly to the output stream of the console
        /// </summary>
        /// <param name="text"></param>
        public void WriteLine(string text)
        {
            console.WriteLine(text);
        }
    }
}
