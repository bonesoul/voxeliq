using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxeliqEngine.Debugging.Console.NextGen
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandGroupAttribute : Attribute
    {
        /// <summary>
        /// Command group's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Help text for command group.
        /// </summary>
        public string Help { get; private set; }

        public CommandGroupAttribute(string name, string help)
        {
            this.Name = name.ToLower();
            this.Help = help;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// Command's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Help text for command.
        /// </summary>
        public string Help { get; private set; }

        public CommandAttribute(string command, string help)
        {
            this.Name = command.ToLower();
            this.Help = help;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class DefaultCommand : CommandAttribute
    {
        public DefaultCommand()
            : base("", "")
        {}
    }
}
