﻿/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using System;
using Nini.Config;
using VoxeliqEngine.IO;
using VoxeliqEngine.Logging;

namespace VoxeliqEngine.Configuration
{
    public sealed class ConfigManager
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        private static readonly IniConfigSource Parser; // the ini parser.
        private static readonly string ConfigFile;
        private static bool _fileExists = false; // does the ini file exists?

        static ConfigManager()
        {
            try
            {
                ConfigFile = string.Format("{0}/{1}", FileHelpers.AssemblyRoot, "config.ini");// the config file's location.
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

        internal static IConfig Section(string section) // Returns the asked config section.
        {
            return Parser.Configs[section];
        }

        internal static IConfig AddSection(string section) // Adds a config section.
        {
            return Parser.AddConfig(section);
        }

        internal static void Save() //  Saves the settings.
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