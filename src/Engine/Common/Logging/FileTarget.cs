/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.IO;

namespace Engine.Common.Logging
{
    /// <summary>
    /// Logs messages to a log file.
    /// </summary>
    public class FileTarget : LogTarget, IDisposable
    {
        private const string LogRoot = "logs";

        private readonly string _fileName; // log-file's filename.
        private readonly string _filePath; // log-file's full path.
        private FileStream _fileStream; // filestream pointing to logfile.
        private StreamWriter _logStream; // stream-writer for flushing logs to disk.

        /// <summary>
        /// Creates a new log file target.
        /// </summary>
        /// <param name="fileName">Filename of the logfile.</param>
        /// <param name="minLevel">Minimum level of messages to emit</param>
        /// <param name="maxLevel">Maximum level of messages to emit</param>
        /// <param name="includeTimeStamps">Include timestamps in log?</param>
        /// <param name="reset">Reset log file on application startup?</param>
        public FileTarget(string fileName, Logger.Level minLevel, Logger.Level maxLevel, bool includeTimeStamps, bool reset = false)
        {
            this.MinimumLevel = minLevel;
            this.MaximumLevel = maxLevel;
            this.IncludeTimeStamps = includeTimeStamps;
            this._fileName = fileName;
            this._filePath = string.Format("{0}/{1}", LogRoot, _fileName); // construct the full path using LoggingRoot defined in config.ini

            if (!Directory.Exists(LogRoot)) // create logging directory if it does not exist yet.
                Directory.CreateDirectory(LogRoot);

            this._fileStream = new FileStream(_filePath, reset ? FileMode.Create : FileMode.Append, FileAccess.Write, FileShare.Read); // init the file stream.
            this._logStream = new StreamWriter(this._fileStream) { AutoFlush = true }; // init the stream writer.
        }

        /// <summary>
        /// Logs a message to a log-file.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="logger">Source of the log message.</param>
        /// <param name="message">Log message.</param>
        public override void LogMessage(Logger.Level level, string logger, string message)
        {
            lock (this) // we need this here until we seperate gs / moonet /raist
            {
                var timeStamp = this.IncludeTimeStamps ? "[" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "] " : "";

                if (!this._disposed) // make sure we're not disposed.
                    this._logStream.WriteLine(string.Format("{0}[{1}] [{2}]: {3}", timeStamp, level.ToString().PadLeft(5), logger, message));
            }
        }

        /// <summary>
        /// Logs a message and an exception to a log-file.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="logger">Source of the log message.</param>
        /// <param name="message">Log message.</param>
        /// <param name="exception">Exception to be included with log message.</param>
        public override void LogException(Logger.Level level, string logger, string message, Exception exception)
        {
            lock (this) // we need this here until we seperate gs / moonet /raist
            {
                var timeStamp = this.IncludeTimeStamps ? "[" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "] " : "";

                if (!this._disposed) // make sure we're not disposed.
                    this._logStream.WriteLine(string.Format("{0}[{1}] [{2}]: {3} - [Exception] {4}", timeStamp, level.ToString().PadLeft(5), logger, message, exception));
            }
        }

        #region de-ctor

        // IDisposable pattern: http://msdn.microsoft.com/en-us/library/fs2xkftw%28VS.80%29.aspx

        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Take object out the finalization queue to prevent finalization code for it from executing a second time.
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed) return; // if already disposed, just return

            if (disposing) // only dispose managed resources if we're called from directly or in-directly from user code.
            {
                this._logStream.Close();
                this._logStream.Dispose();
                this._fileStream.Close();
                this._fileStream.Dispose();
            }

            this._logStream = null;
            this._fileStream = null;

            _disposed = true;
        }

        ~FileTarget() { Dispose(false); } // finalizer called by the runtime. we should only dispose unmanaged objects and should NOT reference managed ones.

        #endregion
    }
}