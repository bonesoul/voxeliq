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
    /// Logs messages to console.
    /// </summary>
    public class ConsoleTarget : LogTarget
    {
        /// <summary>
        /// Creates a new console target.
        /// </summary>
        /// <param name="minLevel">Minimum level of messages to emit</param>
        /// <param name="maxLevel">Maximum level of messages to emit</param>
        /// <param name="includeTimeStamps">Include timestamps in log?</param>
        public ConsoleTarget(Logger.Level minLevel, Logger.Level maxLevel, bool includeTimeStamps)
        {
            MinimumLevel = minLevel;
            MaximumLevel = maxLevel;
            this.IncludeTimeStamps = includeTimeStamps;
        }

        /// <summary>
        /// Logs a message to console.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="logger">Source of the log message.</param>
        /// <param name="message">Log message.</param>
        public override void LogMessage(Logger.Level level, string logger, string message)
        {
            var timeStamp = this.IncludeTimeStamps ? "[" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "] " : "";
            SetConsoleForegroundColor(level);
            Console.WriteLine(string.Format("{0}[{1}] [{2}]: {3}", timeStamp, level.ToString().PadLeft(5), logger, message));
        }

        /// <summary>
        /// Logs a message and an exception to console.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="logger">Source of the log message.</param>
        /// <param name="message">Log message.</param>
        /// <param name="exception">Exception to be included with log message.</param>
        public override void LogException(Logger.Level level, string logger, string message, Exception exception)
        {
            var timeStamp = this.IncludeTimeStamps ? "[" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "] " : "";
            SetConsoleForegroundColor(level);
            Console.WriteLine(string.Format("{0}[{1}] [{2}]: {3} - [Exception] {4}", timeStamp, level.ToString().PadLeft(5), logger, message, exception));
        }

        /// <summary>
        /// Sets console's foreground color.
        /// </summary>
        /// <param name="level"></param>
        private static void SetConsoleForegroundColor(Logger.Level level)
        {
            switch (level)
            {
                case Logger.Level.Trace:
                case Logger.Level.PacketDump:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case Logger.Level.Debug:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case Logger.Level.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case Logger.Level.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case Logger.Level.Error:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case Logger.Level.Fatal:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    break;
            }
        }
    }
}