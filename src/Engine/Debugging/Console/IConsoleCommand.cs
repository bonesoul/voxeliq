/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 * 
 * Code based on: http://code.google.com/p/xnagameconsole/
 */

namespace VoxeliqEngine.Debugging.Console
{
    public interface IConsoleCommand
    {
        /// <summary>
        /// The name of the command; the command will be invoked through this name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The description that is displayed with the 'help' command
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The action of the command.  The return string value is used as output in the console 
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        string Execute(string[] arguments);
    }
}
