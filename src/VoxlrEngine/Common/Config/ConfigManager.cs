/*
 * Copyright (C) 2011 mooege project
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

using System;
using Nini.Config;
using VolumetricStudios.VoxlrEngine.Common.Helpers.IO;
using VolumetricStudios.VoxlrEngine.Common.Logging;

namespace VolumetricStudios.VoxlrEngine.Common.Config
{
    public sealed class ConfigurationManager
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        private static readonly IniConfigSource Parser; // the ini parser.
        private static readonly string ConfigFile;
        private static bool _fileExists = false; // does the ini file exists?

        static ConfigurationManager()
        {
            try
            {
                ConfigFile = string.Format("{0}/{1}", FileHelpers.AssemblyRoot, "config.ini"); // the config file's location.
                Parser = new IniConfigSource(ConfigFile); // see if the file exists by trying to parse it.
                _fileExists = true;
            }
            catch (Exception)
            {
                Parser = new IniConfigSource(); // initiate a new .ini source.
                _fileExists = false;
                Logger.Warn("Error loading settings config.ini, will be using default settings.");
            }
            finally
            {
                // adds aliases so we can use On and Off directives in ini files.
                Parser.Alias.AddAlias("On", true);
                Parser.Alias.AddAlias("Off", false);

                // logger level aliases.
                Parser.Alias.AddAlias("MinimumLevel", Logger.Level.Trace);
                Parser.Alias.AddAlias("MaximumLevel", Logger.Level.Trace);
            }

            Parser.ExpandKeyValues();
        }

        static internal IConfig Section(string section) // Returns the asked config section.
        {
            return Parser.Configs[section];
        }

        static internal IConfig AddSection(string section) // Adds a config section.
        {
            return Parser.AddConfig(section);
        }

        static internal void Save() //  Saves the settings.
        {
            if (_fileExists) Parser.Save();
            else
            {
                Parser.Save(ConfigFile);
                _fileExists = true;
            }
        }
    }
}
