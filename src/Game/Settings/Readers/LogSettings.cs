/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Engine.Common.Logging;

namespace Client.Settings.Readers
{
    /// <summary>
    /// Holds configuration info for log manager.
    /// </summary>
    public sealed class LogSettings : SettingsReader
    {
        /// <summary>
        /// Available log target configs.
        /// </summary>
        public LogTargetConfig[] Targets = new[]
        {
            new LogTargetConfig("ConsoleLog"), 
            new LogTargetConfig("EngineLog"), 
        };

        /// <summary>
        /// Creates a new log config.
        /// </summary>
        internal LogSettings() :
            base("Logging") // Call the base ctor with section name 'Logging'.
        { }
    }

    /// <summary>
    /// Holds configuration of a log target.
    /// </summary>
    public class LogTargetConfig : SettingsReader
    {
        /// <summary>
        /// Is enabled?
        /// </summary>
        public bool Enabled
        {
            get { return this.GetBoolean("Enabled", true); }
            set { this.Set("Enabled", value); }
        }

        /// <summary>
        /// Target type. Valid values are file and console.
        /// </summary>
        public string Target
        {
            get { return this.GetString("Target", "Console"); }
            set { this.GetString("Target", value); }
        }

        /// <summary>
        /// Include timestamps in logs?
        /// </summary>
        public bool IncludeTimeStamps
        {
            get { return this.GetBoolean("IncludeTimeStamps", false); }
            set { this.Set("IncludeTimeStamps", value); }
        }

        /// <summary>
        /// Filename if logtarget is a file based one.
        /// </summary>
        public string FileName
        {
            get { return this.GetString("FileName", ""); }
            set { this.GetString("FileName", value); }
        }

        /// <summary>
        /// Minimum level of messages to emit.
        /// </summary>
        public Logger.Level MinimumLevel
        {
            get { return (Logger.Level)(this.GetInt("MinimumLevel", (int)Logger.Level.Info, true)); }
            set { this.Set("MinimumLevel", (int)value); }
        }

        /// <summary>
        /// Maximum level of messages to emit
        /// </summary>
        public Logger.Level MaximumLevel
        {
            get { return (Logger.Level)(this.GetInt("MaximumLevel", (int)Logger.Level.Fatal, true)); }
            set { this.Set("MaximumLevel", (int)value); }
        }

        /// <summary>
        /// Reset log file on startup?
        /// </summary>
        public bool ResetOnStartup
        {
            get { return this.GetBoolean("ResetOnStartup", false); }
            set { this.Set("ResetOnStartup", value); }
        }

        public LogTargetConfig(string loggerName)
            : base(loggerName) { }
    }
}