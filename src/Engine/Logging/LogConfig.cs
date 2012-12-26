/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using VoxeliqEngine.Configuration;

namespace VoxeliqEngine.Logging
{
    public sealed class LogConfig : Config
    {
        public string LoggingRoot
        {
            get { return this.GetString("Root", @"logs"); }
            set { this.Set("Root", value); }
        }

        public LogTargetConfig[] Targets = new[] { new LogTargetConfig("ConsoleLog") };

        public static LogConfig Instance
        {
            get { return _instance; }
        }

        private static readonly LogConfig _instance = new LogConfig();

        private LogConfig()
            : base("Logging")
        { }
    }

    public class LogTargetConfig : Config
    {
        public bool Enabled
        {
            get { return this.GetBoolean("Enabled", true); }
            set { this.Set("Enabled", value); }
        }

        public string Target
        {
            get { return this.GetString("Target", "Console"); }
            set { this.GetString("Target", value); }
        }

        public bool IncludeTimeStamps
        {
            get { return this.GetBoolean("IncludeTimeStamps", false); }
            set { this.Set("IncludeTimeStamps", value); }
        }

        public string FileName
        {
            get { return this.GetString("FileName", ""); }
            set { this.GetString("FileName", value); }
        }

        public Logger.Level MinimumLevel
        {
            get { return (Logger.Level)(this.GetInt("MinimumLevel", (int)Logger.Level.Info, true)); }
            set { this.Set("MinimumLevel", (int)value); }
        }

        public Logger.Level MaximumLevel
        {
            get { return (Logger.Level)(this.GetInt("MaximumLevel", (int)Logger.Level.Fatal, true)); }
            set { this.Set("MaximumLevel", (int)value); }
        }

        public bool ResetOnStartup
        {
            get { return this.GetBoolean("ResetOnStartup", false); }
            set { this.Set("ResetOnStartup", value); }
        }

        public LogTargetConfig(string loggerName)
            : base(loggerName)
        { }
    }
}