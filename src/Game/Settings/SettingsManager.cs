/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using Engine.Common.Helpers.IO;
using Engine.Common.Logging;
using Nini.Config;

namespace Client.Settings
{
    public sealed class SettingsManager
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        private static readonly IniConfigSource Parser; // the ini parser.
        private static readonly string ConfigFile;
        private static bool _fileExists = false; // does the ini file exists?

        static SettingsManager()
        {
            try
            {
                ConfigFile = string.Format("{0}/{1}", FileHelpers.AssemblyRoot, "config.ini"); // the config file's location.
                Parser = new IniConfigSource(ConfigFile); // see if the file exists by trying to parse it.
                _fileExists = true;
            }
            catch (Exception e)
            {
                Parser = new IniConfigSource(); // initiate a new .ini source.
                _fileExists = false;
                Logger.WarnException(e, "Error loading settings config.ini, will be using default settings. Exception thrown: ");
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