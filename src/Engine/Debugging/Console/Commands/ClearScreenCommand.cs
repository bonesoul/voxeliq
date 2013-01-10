/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 * 
 * Code based on: http://code.google.com/p/xnagameconsole/
 */

namespace VoxeliqEngine.Debugging.Console.Commands
{
    class ClearScreenCommand:IConsoleCommand
    {
        public string Name { get
        {
            return "clear";
        } }
        public string Description { get
        {
            return "Clears the console output";
        } }

        private InputProcessor processor;
        public ClearScreenCommand(InputProcessor processor)
        {
            this.processor = processor;
        }
        public string Execute(string[] arguments)
        {
            processor.Out.Clear();
            return "";
        }
    }
}
