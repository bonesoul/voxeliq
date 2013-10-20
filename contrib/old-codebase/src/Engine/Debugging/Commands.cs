/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Engine.Debugging.Console;

namespace Engine.Debugging
{
    [Command("debug-graphs", "Sets the debug graphs mode.\nusage: debug-graphs [on|off]")]
    public class VSyncCommand:Command
    {
        [DefaultCommand]
        public string Default(string[] @params)
        {
            return string.Format("Debug-graphs are currently {0}.\nusage: debug-graphs [on|off].",
                                 Core.Engine.Instance.Configuration.Debugging.GraphsEnabled
                                     ? "on"
                                     : "off");
        }

        [Subcommand("on", "Sets debug-graphs on.")]
        public string On(string[] @params)
        {
            Core.Engine.Instance.Configuration.Debugging.GraphsEnabled = true;
            return "Debug-graphs on.";
        }

        [Subcommand("off", "Sets debug-graphs off.")]
        public string Off(string[] @params)
        {
            Core.Engine.Instance.Configuration.Debugging.GraphsEnabled = false;
            return "Debug-graphs off.";
        }
    }
}
