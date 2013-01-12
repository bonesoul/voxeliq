using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                                 Settings.Debugging.DebugGraphsEnabled
                                     ? "on"
                                     : "off");
        }

        [Subcommand("on", "Sets debug-graphs on.")]
        public string On(string[] @params)
        {
            Settings.Debugging.DebugGraphsEnabled = true;
            return "Debug-graphs on.";
        }

        [Subcommand("off", "Sets debug-graphs off.")]
        public string Off(string[] @params)
        {
            Settings.Debugging.DebugGraphsEnabled = false;
            return "Debug-graphs off.";
        }
    }
}
