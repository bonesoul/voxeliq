/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;

namespace Engine.Debugging.Console
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// Command group's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Help text for command group.
        /// </summary>
        public string Help { get; private set; }

        public CommandAttribute(string name, string help)
        {
            this.Name = name.ToLower();
            this.Help = help;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class SubcommandAttribute : Attribute
    {
        /// <summary>
        /// Command's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Help text for command.
        /// </summary>
        public string Help { get; private set; }

        public SubcommandAttribute(string command, string help)
        {
            this.Name = command.ToLower();
            this.Help = help;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class DefaultCommand : SubcommandAttribute
    {
        public DefaultCommand()
            : base("", "")
        {}
    }
}
