/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

namespace VoxeliqGame.Settings.Readers
{
    public sealed class AudioSettings : SettingsReader
    {
        /// <summary>
        /// Is audio enabled?
        /// </summary>
        public bool Enabled
        {
            get { return this.GetBoolean("Enabled", true); }
            set { this.Set("Enabled", value); }
        }

        /// <summary>
        /// The memory instance of ScreenConfig.
        /// </summary>
        private static readonly AudioSettings _instance = new AudioSettings();

        /// <summary>
        /// The memory instance of ScreenConfig.
        /// </summary>
        public static AudioSettings Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Creates a new ScreenConfig instance.
        /// </summary>
        private AudioSettings()
            : base("Audio")
        {
        }
    }
}