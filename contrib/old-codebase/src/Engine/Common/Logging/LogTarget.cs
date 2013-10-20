/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;

namespace Engine.Common.Logging
{
    /// <summary>
    /// Log target.
    /// </summary>
    public class LogTarget
    {
        /// <summary>
        /// Minimum level of messages to emit.
        /// </summary>
        public Logger.Level MinimumLevel { get; protected set; }

        /// <summary>
        /// Maximum level of messages to emit.
        /// </summary>
        public Logger.Level MaximumLevel { get; protected set; }

        /// <summary>
        /// Include timestamps in log?
        /// </summary>
        public bool IncludeTimeStamps { get; protected set; }

        /// <summary>
        /// Logs a message to log-target.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="logger">Source of the log message.</param>
        /// <param name="message">Log message.</param>
        public virtual void LogMessage(Logger.Level level, string logger, string message)
        {
            throw new NotSupportedException("Vanilla log-targets are not supported! Instead use a log-target implementation.");
        }

        /// <summary>
        /// Logs a message and an exception to log-target.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="logger">Source of the log message.</param>
        /// <param name="message">Log message.</param>
        /// <param name="exception">Exception to be included with log message.</param>
        public virtual void LogException(Logger.Level level, string logger, string message, Exception exception)
        {
            throw new NotSupportedException("Vanilla log-targets are not supported! Instead use a log-target implementation.");
        }
    }
}