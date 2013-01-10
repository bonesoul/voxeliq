/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 * 
 * Code based on: http://code.google.com/p/xnagameconsole/
 */

using Microsoft.Xna.Framework;

namespace VoxeliqEngine.Debugging.Console.Commands
{
    class ExitCommand : IConsoleCommand
    {
        public string Name
        {
            get
            {
                return "exit";
            }
        }
        public string Description
        {
            get
            {
                return "Forcefully exists the game";
            }
        }

        private readonly Game game;
        public ExitCommand(Game game)
        {
            this.game = game;
        }
        public string Execute(string[] arguments)
        {
            game.Exit();
            return "Exiting the game";
        }
    }
}