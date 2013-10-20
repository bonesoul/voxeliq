/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Linq;

namespace Engine.Common.Logging
{
    /// <summary>
    /// LogRouter class that routes messages to appropriate log-targets.
    /// </summary>
    internal static class LogRouter
    {
        /// <summary>
        /// Routes a message to appropriate log-targets.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="logger">Source of the log message.</param>
        /// <param name="message">Log message.</param>
        public static void RouteMessage(Logger.Level level, string logger, string message)
        {
            if (!LogManager.Enabled) // if we logging is not enabled,
                return; // just skip.

            if (LogManager.Targets.Count == 0) // if we don't have any active log-targets,
                return; // just skip

            // loop through all available logs targets and route the messages that meet the filters.
            foreach (var target in LogManager.Targets.Where(target => level >= target.MinimumLevel && level <= target.MaximumLevel))
            {
                target.LogMessage(level, logger, message);
            }
        }

        /// <summary>
        /// Routes a message to appropriate log-targets.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="logger">Source of the log message.</param>
        /// <param name="message">Log message.</param>
        /// <param name="exception">Exception to be included with log message.</param>
        public static void RouteException(Logger.Level level, string logger, string message, Exception exception)
        {
            if (!LogManager.Enabled) // if we logging is not enabled,
                return; // just skip.

            if (LogManager.Targets.Count == 0) // if we don't have any active log-targets,
                return; // just skip

            // loop through all available logs targets and route the messages that meet the filters.
            foreach (var target in LogManager.Targets.Where(target => level >= target.MinimumLevel && level <= target.MaximumLevel))
            {
                target.LogException(level, logger, message, exception);
            }
        }
    }
}