/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 * 
 * Code based on: http://code.google.com/p/xnagameconsole/
 */

using System;

namespace VoxeliqEngine.Debugging.Console.Commands
{
    class CustomCommand:IConsoleCommand
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        private Func<string[], string> action;

        public CustomCommand(string name, Func<string[], string> action, string description)
        {
            Name = name;
            Description = description;
            this.action = action;
        }
        public string Execute(string[] arguments)
        {
            return action(arguments);
        }
    }
}
