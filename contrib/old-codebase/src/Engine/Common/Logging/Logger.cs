/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Globalization;

namespace Engine.Common.Logging
{
    /// <summary>
    /// Logger class that can log messages and exceptions to available log-targets.
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// Name of the logger.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Creates a new logger with given name.
        /// </summary>
        /// <param name="name">Name of the logger.</param>
        public Logger(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Available log levels.
        /// </summary>
        public enum Level
        {
            /// <summary>
            /// Trace messages.
            /// </summary>
            Trace,
            /// <summary>
            /// Debug messages.
            /// </summary>
            Debug,
            /// <summary>
            /// Info messages.
            /// </summary>
            Info,
            /// <summary>
            /// Warning messages.
            /// </summary>
            Warn,
            /// <summary>
            /// Error messages.
            /// </summary>
            Error,
            /// <summary>
            /// Fatal error messages.
            /// </summary>
            Fatal,
            /// <summary>
            /// Packet dumps.
            /// </summary>
            PacketDump,
        }

        #region message loggers

        /// <summary>
        /// Logs a trace message.
        /// </summary>
        /// <param name="message">The log message.</param>
        public void Trace(string message) { Log(Level.Trace, message, null); }

        /// <summary>
        /// Logs a trace message.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="args">Additional arguments.</param>
        public void Trace(string message, params object[] args) { Log(Level.Trace, message, args); }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The log message.</param>
        public void Debug(string message) { Log(Level.Debug, message, null); }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="args">Additional arguments.</param>
        public void Debug(string message, params object[] args) { Log(Level.Debug, message, args); }

        /// <summary>
        /// Logs an info message.
        /// </summary>
        /// <param name="message">The log message.</param>
        public void Info(string message) { Log(Level.Info, message, null); }

        /// <summary>
        /// Logs an info message.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="args">Additional arguments.</param>
        public void Info(string message, params object[] args) { Log(Level.Info, message, args); }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The log message.</param>
        public void Warn(string message) { Log(Level.Warn, message, null); }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="args">Additional arguments.</param>
        public void Warn(string message, params object[] args) { Log(Level.Warn, message, args); }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The log message.</param>
        public void Error(string message) { Log(Level.Error, message, null); }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="args">Additional arguments.</param>
        public void Error(string message, params object[] args) { Log(Level.Error, message, args); }

        /// <summary>
        /// Logs an fatal error message.
        /// </summary>
        /// <param name="message">The log message.</param>
        public void Fatal(string message) { Log(Level.Fatal, message, null); }

        /// <summary>
        /// Logs an fatal error message.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="args">Additional arguments.</param>
        public void Fatal(string message, params object[] args) { Log(Level.Fatal, message, args); }

        #endregion

        #region message loggers with additional exception info included

        /// <summary>
        /// Logs a trace message with an exception included.
        /// </summary>
        /// <param name="exception">The exception to include in log line.</param>
        /// <param name="message">The log message.</param>
        public void TraceException(Exception exception, string message) { LogException(Level.Trace, message, null, exception); }

        /// <summary>
        /// Logs a trace message with an exception included.
        /// </summary>
        /// <param name="exception">The exception to include in log line.</param>
        /// <param name="message">The log message.</param>
        /// <param name="args">Additional arguments.</param>
        public void TraceException(Exception exception, string message, params object[] args) { LogException(Level.Trace, message, args, exception); }

        /// <summary>
        /// Logs a debug message with an exception included.
        /// </summary>
        /// <param name="exception">The exception to include in log line.</param>
        /// <param name="message">The log message.</param>
        public void DebugException(Exception exception, string message) { LogException(Level.Debug, message, null, exception); }

        /// <summary>
        /// Logs a debug message with an exception included.
        /// </summary>
        /// <param name="exception">The exception to include in log line.</param>
        /// <param name="message">The log message.</param>
        /// <param name="args">Additional arguments.</param>
        public void DebugException(Exception exception, string message, params object[] args) { LogException(Level.Debug, message, args, exception); }

        /// <summary>
        /// Logs an info message with an exception included.
        /// </summary>
        /// <param name="exception">The exception to include in log line.</param>
        /// <param name="message">The log message.</param>
        public void InfoException(Exception exception, string message) { LogException(Level.Info, message, null, exception); }

        /// <summary>
        /// Logs an info message with an exception included.
        /// </summary>
        /// <param name="exception">The exception to include in log line.</param>
        /// <param name="message">The log message.</param>
        /// <param name="args">Additional arguments.</param>
        public void InfoException(Exception exception, string message, params object[] args) { LogException(Level.Info, message, args, exception); }

        /// <summary>
        /// Logs a warning message with an exception included.
        /// </summary>
        /// <param name="exception">The exception to include in log line.</param>
        /// <param name="message">The log message.</param>
        public void WarnException(Exception exception, string message) { LogException(Level.Warn, message, null, exception); }

        /// <summary>
        /// Logs a warning message with an exception included.
        /// </summary>
        /// <param name="exception">The exception to include in log line.</param>
        /// <param name="message">The log message.</param>
        /// <param name="args">Additional arguments.</param>
        public void WarnException(Exception exception, string message, params object[] args) { LogException(Level.Warn, message, args, exception); }

        /// <summary>
        /// Logs an error message with an exception included.
        /// </summary>
        /// <param name="exception">The exception to include in log line.</param>
        /// <param name="message">The log message.</param>
        public void ErrorException(Exception exception, string message) { LogException(Level.Error, message, null, exception); }

        /// <summary>
        /// Logs an error message with an exception included.
        /// </summary>
        /// <param name="exception">The exception to include in log line.</param>
        /// <param name="message">The log message.</param>
        /// <param name="args">Additional arguments.</param>
        public void ErrorException(Exception exception, string message, params object[] args) { LogException(Level.Error, message, args, exception); }

        /// <summary>
        /// Logs a fatal error message with an exception included.
        /// </summary>
        /// <param name="exception">The exception to include in log line.</param>
        /// <param name="message">The log message.</param>
        public void FatalException(Exception exception, string message) { LogException(Level.Fatal, message, null, exception); }

        /// <summary>
        /// Logs a fatal error message with an exception included.
        /// </summary>
        /// <param name="exception">The exception to include in log line.</param>
        /// <param name="message">The log message.</param>
        /// <param name="args">Additional arguments.</param>
        public void FatalException(Exception exception, string message, params object[] args) { LogException(Level.Fatal, message, args, exception); }

        #endregion

        #region utility functions

        private void Log(Level level, string message, object[] args) // sends logs to log-router.
        {
            LogRouter.RouteMessage(level, this.Name, args == null ? message : string.Format(CultureInfo.InvariantCulture, message, args));
        }

        private void LogException(Level level, string message, object[] args, Exception exception) // sends logs to log-router.
        {
            LogRouter.RouteException(level, this.Name, args == null ? message : string.Format(CultureInfo.InvariantCulture, message, args), exception);
        }

        #endregion
    }
}