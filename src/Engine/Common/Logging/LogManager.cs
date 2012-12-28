﻿/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VoxeliqEngine.Common.Logging
{
    /// <summary>
    /// Log manager class.
    /// </summary>
    public static class LogManager
    {
        /// <summary>
        /// Is logging enabled?
        /// </summary>
        public static bool Enabled { get; set; }

        /// <summary>
        /// Available & configured log targets.
        /// </summary>
        internal readonly static List<LogTarget> Targets = new List<LogTarget>();

        /// <summary>
        /// Available loggers.
        /// </summary>
        internal static readonly Dictionary<string, Logger> Loggers = new Dictionary<string, Logger>();

        /// <summary>
        /// Creates and returns a logger named with declaring type.
        /// </summary>
        /// <returns>A <see cref="Logger"/> instance.</returns>
        public static Logger CreateLogger()
        {
            var frame = new StackFrame(1, false); // read stack frame.
            var name = frame.GetMethod().DeclaringType.Name; // get declaring type's name.

            if (name == null) // see if we got a name.
                throw new Exception("Error getting full name for declaring type.");

            if (!Loggers.ContainsKey(name))  // see if we already have instance for the given name.
                Loggers.Add(name, new Logger(name)); // add it to dictionary of loggers.

            return Loggers[name]; // return the newly created logger.
        }

        /// <summary>
        /// Creates and returns a logger with given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A <see cref="Logger"/> instance.</returns>
        public static Logger CreateLogger(string name)
        {
            if (!Loggers.ContainsKey(name)) // see if we already have instance for the given name.
                Loggers.Add(name, new Logger(name)); // add it to dictionary of loggers.

            return Loggers[name]; // return the newly created logger.
        }

        /// <summary>
        /// Attachs a new log target.
        /// </summary>
        /// <param name="target"></param>
        public static void AttachLogTarget(LogTarget target)
        {
            Targets.Add(target);
        }
    }
}