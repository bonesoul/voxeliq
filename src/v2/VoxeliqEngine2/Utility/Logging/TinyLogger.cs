using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace VolumetricStudios.VoxeliqEngine.Utility.Logging
{
    public static class LogManager
    {
        public static bool Enabled { get; set; }

        internal readonly static List<LogTarget> Targets = new List<LogTarget>();
        internal static readonly Dictionary<string, Logger> Loggers = new Dictionary<string, Logger>();

        public static void AttachLogTarget(LogTarget target)
        {
            Targets.Add(target);
        }

        public static Logger CreateLogger()
        {
            var frame = new StackFrame(1, false);
            var name = frame.GetMethod().DeclaringType.Name;
            if (name == null) throw new Exception("Error getting full name for declaring type.");
            if (!Loggers.ContainsKey(name)) Loggers.Add(name, new Logger(name));
            return Loggers[name];
        }

        public static Logger CreateLogger(string name)
        {
            if (!Loggers.ContainsKey(name)) Loggers.Add(name, new Logger(name));
            return Loggers[name];
        }
    }

    internal static class LogRouter
    {
        public static void RouteMessage(Logger.Level level, string logger, string message)
        {
            if (!LogManager.Enabled) return;
            if (LogManager.Targets.Count == 0) return;

            foreach (var target in LogManager.Targets.Where(target => level >= target.MinimumLevel && level <= target.MaximumLevel))
            {
                target.LogMessage(level, logger, message);
            }
        }

        public static void RouteException(Logger.Level level, string logger, string message, Exception exception)
        {
            if (!LogManager.Enabled) return;
            if (LogManager.Targets.Count == 0) return;

            foreach (var target in LogManager.Targets.Where(target => level >= target.MinimumLevel && level <= target.MaximumLevel))
            {
                target.LogException(level, logger, message, exception);
            }
        }
    }

    public class Logger
    {
        public string Name { get; protected set; }

        public Logger(string name)
        {
            Name = name;
        }

        private void Log(Level level, string message, object[] args)
        {
            LogRouter.RouteMessage(level, this.Name, args == null ? message : string.Format(CultureInfo.InvariantCulture, message, args));
        }

        private void LogException(Level level, string message, object[] args, Exception exception)
        {
            LogRouter.RouteException(level, this.Name, args == null ? message : string.Format(CultureInfo.InvariantCulture, message, args), exception);
        }

        public void Trace(string message) { Log(Level.Trace, message, null); }
        public void Trace(string message, params object[] args) { Log(Level.Trace, message, args); }

        public void Debug(string message) { Log(Level.Debug, message, null); }
        public void Debug(string message, params object[] args) { Log(Level.Debug, message, args); }

        public void Info(string message) { Log(Level.Info, message, null); }
        public void Info(string message, params object[] args) { Log(Level.Info, message, args); }

        public void Warn(string message) { Log(Level.Warn, message, null); }
        public void Warn(string message, params object[] args) { Log(Level.Warn, message, args); }

        public void Error(string message) { Log(Level.Error, message, null); }
        public void Error(string message, params object[] args) { Log(Level.Error, message, args); }

        public void Fatal(string message) { Log(Level.Fatal, message, null); }
        public void Fatal(string message, params object[] args) { Log(Level.Fatal, message, args); }

        public void TraceException(Exception exception, string message) { LogException(Level.Trace, message, null, exception); }
        public void TraceException(Exception exception, string message, params object[] args) { LogException(Level.Trace, message, args, exception); }

        public void DebugException(Exception exception, string message) { LogException(Level.Debug, message, null, exception); }
        public void DebugException(Exception exception, string message, params object[] args) { LogException(Level.Debug, message, args, exception); }

        public void InfoException(Exception exception, string message) { LogException(Level.Info, message, null, exception); }
        public void InfoException(Exception exception, string message, params object[] args) { LogException(Level.Info, message, args, exception); }

        public void WarnException(Exception exception, string message) { LogException(Level.Warn, message, null, exception); }
        public void WarnException(Exception exception, string message, params object[] args) { LogException(Level.Warn, message, args, exception); }

        public void ErrorException(Exception exception, string message) { LogException(Level.Error, message, null, exception); }
        public void ErrorException(Exception exception, string message, params object[] args) { LogException(Level.Error, message, args, exception); }

        public void FatalException(Exception exception, string message) { LogException(Level.Fatal, message, null, exception); }
        public void FatalException(Exception exception, string message, params object[] args) { LogException(Level.Fatal, message, args, exception); }

        public enum Level
        {
            Dump, // used for logging packets.
            Trace,
            Debug,
            Info,
            Warn,
            Error,
            Fatal
        }
    }

    public class LogTarget
    {
        public Logger.Level MinimumLevel { get; protected set; }
        public Logger.Level MaximumLevel { get; protected set; }
        public bool IncludeTimeStamps { get; protected set; }

        public virtual void LogMessage(Logger.Level level, string logger, string message) { throw new NotSupportedException(); }
        public virtual void LogException(Logger.Level level, string logger, string message, Exception exception) { throw new NotSupportedException(); }
    }

    public class FileTarget : LogTarget, IDisposable
    {
        private readonly string _fileName;
        private readonly string _filePath;

        private FileStream _fileStream;
        private StreamWriter _logStream;

        public FileTarget(string fileName, Logger.Level minLevel, Logger.Level maxLevel, bool includeTimeStamps, bool reset = false)
        {
            this._fileName = fileName;
            this._filePath = string.Format("logs/{0}", _fileName);
            this.MinimumLevel = minLevel;
            this.MaximumLevel = maxLevel;
            this.IncludeTimeStamps = includeTimeStamps;

            if (!Directory.Exists("logs")) // create logging directory if it does not exist.
                Directory.CreateDirectory("logs");

            this._fileStream = new FileStream(_filePath, reset ? FileMode.Create : FileMode.Append, FileAccess.Write, FileShare.Read);
            this._logStream = new StreamWriter(this._fileStream) { AutoFlush = true };
        }

        public override void LogMessage(Logger.Level level, string logger, string message)
        {
            lock (this) // we need this here until we seperate gs / moonet /raist
            {
                var timeStamp = this.IncludeTimeStamps
                                    ? "[" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "] "
                                    : "";

                if (!this._disposed)
                    this._logStream.WriteLine(string.Format("{0}[{1}] [{2}]: {3}", timeStamp, level.ToString().PadLeft(5), logger, message));
            }
        }

        public override void LogException(Logger.Level level, string logger, string message, Exception exception)
        {
            lock (this)
            {
                var timeStamp = this.IncludeTimeStamps
                    ? "[" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "] "
                    : "";

                if (!this._disposed)
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

    public class ConsoleTarget : LogTarget
    {
        public ConsoleTarget(Logger.Level minLevel, Logger.Level maxLevel, bool includeTimeStamps)
        {
            MinimumLevel = minLevel;
            MaximumLevel = maxLevel;
            this.IncludeTimeStamps = includeTimeStamps;
        }

        public override void LogMessage(Logger.Level level, string logger, string message)
        {
            var timeStamp = this.IncludeTimeStamps
                                ? "[" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "] "
                                : "";

            SetForeGroundColor(level);
            Console.WriteLine(string.Format("{0}[{1}] [{2}]: {3}", timeStamp, level.ToString().PadLeft(5), logger, message));
        }

        public override void LogException(Logger.Level level, string logger, string message, Exception exception)
        {
            var timeStamp = this.IncludeTimeStamps
                                ? "[" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "] "
                                : "";

            SetForeGroundColor(level);
            Console.WriteLine(string.Format("{0}[{1}] [{2}]: {3} - [Exception] {4}", timeStamp, level.ToString().PadLeft(5), logger, message, exception));
        }

        private static void SetForeGroundColor(Logger.Level level)
        {
            switch (level)
            {
                case Logger.Level.Dump: Console.ForegroundColor = ConsoleColor.DarkGray; break;
                case Logger.Level.Trace: Console.ForegroundColor = ConsoleColor.DarkGray; break;
                case Logger.Level.Debug: Console.ForegroundColor = ConsoleColor.Cyan; break;
                case Logger.Level.Info: Console.ForegroundColor = ConsoleColor.White; break;
                case Logger.Level.Warn: Console.ForegroundColor = ConsoleColor.Yellow; break;
                case Logger.Level.Error: Console.ForegroundColor = ConsoleColor.Magenta; break;
                case Logger.Level.Fatal: Console.ForegroundColor = ConsoleColor.Red; break;
                default: break;
            }
        }
    }
}
