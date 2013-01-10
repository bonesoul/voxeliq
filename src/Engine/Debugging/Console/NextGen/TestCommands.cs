using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxeliqEngine.Debugging.Console.NextGen
{
    [CommandGroup("test", "Renders statistics.\nUsage: stats [system].")]
    public class TestCommands : CommandGroup
    {
        [DefaultCommand]
        public string Stats(string[] @params)
        {
            return string.Format("test");
        }
    }
}
