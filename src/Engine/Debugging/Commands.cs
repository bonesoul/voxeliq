/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using VoxeliqEngine.Core;
using VoxeliqEngine.Debugging.Console;

namespace VoxeliqEngine.Debugging
{
    [Command("debug-graphs", "Sets the debug graphs mode.\nusage: debug-graphs [on|off]")]
    public class VSyncCommand:Command
    {
        [DefaultCommand]
        public string Default(string[] @params)
        {
            return string.Format("Debug-graphs are currently {0}.\nusage: debug-graphs [on|off].",
                                 Engine.Instance.Configuration.Debugging.GraphsEnabled
                                     ? "on"
                                     : "off");
        }

        [Subcommand("on", "Sets debug-graphs on.")]
        public string On(string[] @params)
        {
            Engine.Instance.Configuration.Debugging.GraphsEnabled = true;
            return "Debug-graphs on.";
        }

        [Subcommand("off", "Sets debug-graphs off.")]
        public string Off(string[] @params)
        {
            Engine.Instance.Configuration.Debugging.GraphsEnabled = false;
            return "Debug-graphs off.";
        }
    }
}
