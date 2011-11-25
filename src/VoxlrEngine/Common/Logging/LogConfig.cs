/*
 * Copyright (C) 2011 voxlr project
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace VolumetricStudios.VoxlrEngine.Common.Logging
{
    public sealed class LogConfig : Config.Config
    {
        public string LoggingRoot { get { return this.GetString("Root", @"logs"); } set { this.Set("Root", value); } }

        public LogTargetConfig[] Targets = new[] { new LogTargetConfig("ConsoleLog") };

        private static readonly LogConfig _instance = new LogConfig();
        public static LogConfig Instance { get { return _instance; } }
        private LogConfig() : base("Logging") { }
    }

    public class LogTargetConfig : Config.Config
    {
        public bool Enabled { get { return this.GetBoolean("Enabled", true); } set { this.Set("Enabled", value); } }
        public string Target { get { return this.GetString("Target", "Console"); } set { this.GetString("Target", value); } }
        public bool IncludeTimeStamps { get { return this.GetBoolean("IncludeTimeStamps", false); } set { this.Set("IncludeTimeStamps", value); } }
        public string FileName { get { return this.GetString("FileName", ""); } set { this.GetString("FileName", value); } }
        public Logger.Level MinimumLevel { get { return (Logger.Level)(this.GetInt("MinimumLevel", (int)Logger.Level.Info, true)); } set { this.Set("MinimumLevel", (int)value); } }
        public Logger.Level MaximumLevel { get { return (Logger.Level)(this.GetInt("MaximumLevel", (int)Logger.Level.Fatal, true)); } set { this.Set("MaximumLevel", (int)value); } }
        public bool ResetOnStartup { get { return this.GetBoolean("ResetOnStartup", false); } set { this.Set("ResetOnStartup", value); } }

        public LogTargetConfig(string loggerName) : base(loggerName) { }
    }
}
