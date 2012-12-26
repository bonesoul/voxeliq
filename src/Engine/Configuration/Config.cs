﻿/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using Nini.Config;

namespace VoxeliqEngine.Configuration
{
    public class Config
    {
        private readonly IConfig _section;

        public Config(string sectionName)
        {
            this._section = ConfigManager.Section(sectionName) ?? ConfigManager.AddSection(sectionName);
        }

        public void Save()
        {
            ConfigManager.Save();
        }

        protected bool GetBoolean(string key, bool defaultValue)
        {
            return this._section.GetBoolean(key, defaultValue);
        }

        protected double GetDouble(string key, double defaultValue)
        {
            return this._section.GetDouble(key, defaultValue);
        }

        protected float GetFloat(string key, float defaultValue)
        {
            return this._section.GetFloat(key, defaultValue);
        }

        protected int GetInt(string key, int defaultValue)
        {
            return this._section.GetInt(key, defaultValue);
        }

        protected int GetInt(string key, int defaultValue, bool fromAlias)
        {
            return this._section.GetInt(key, defaultValue, fromAlias);
        }

        protected long GetLong(string key, long defaultValue)
        {
            return this._section.GetLong(key, defaultValue);
        }

        protected string GetString(string key, string defaultValue)
        {
            return this._section.Get(key, defaultValue);
        }

        protected string[] GetEntryKeys()
        {
            return this._section.GetKeys();
        }

        protected void Set(string key, object value)
        {
            this._section.Set(key, value);
        }
    }
}