/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 * 
 * Code based on: http://code.google.com/p/xnagameconsole/
 */

using System;
using VoxeliqEngine.Graphics;

namespace VoxeliqEngine.Debugging.Console.Commands
{
    class RasterizerCommand : IConsoleCommand
    {
        public string Name
        {
            get { return "rasterizer"; }
        }

        public string Description
        {
            get { return "Toggles the rasterizer mode."; }
        }

        public RasterizerCommand()
        { }

        public string Execute(string[] arguments)
        {
            if (arguments.Length == 0)
                return "Missing argument rasterizer mode.";
            else if (arguments[0]=="wireframed")
                Rasterizer.Instance.ActivateWireframedMode();
            else if(arguments[0]=="normal")
                Rasterizer.Instance.ActivateNormalMode();
            return "";
        }
    }
}