/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

namespace Client.Settings.Readers
{
    public sealed class GraphicsSettings : SettingsReader
    {
        /// <summary>
        /// Sets the screen width.
        /// </summary>
        public int Width
        {
            get { return this.GetInt("Width", 1280); }
            set { this.Set("Width", value); }
        }

        /// <summary>
        /// Sets the screen height.
        /// </summary>
        public int Height
        {
            get { return this.GetInt("Height", 720); }
            set { this.Set("Height", value); }
        }

        /// <summary>
        /// Is full screen enabled?
        /// </summary>
        public bool FullScreenEnabled
        {
            get { return this.GetBoolean("FullScreen", true); }
            set { this.Set("FullScreen", value); }
        }

        /// <summary>
        /// Is vsync enabled?
        /// </summary>
        public bool VerticalSyncEnabled
        {
            get { return this.GetBoolean("VSync", true); }
            set { this.Set("VSync", value); }
        }

        /// <summary>
        /// Is fixed time steps enabled?
        /// </summary>
        public bool FixedTimeStepsEnabled
        {
            get { return this.GetBoolean("FixedTimeSteps", true); }
            set { this.Set("FixedTimeSteps", value); }
        }

        /// <summary>
        /// Creates a new ScreenConfig instance.
        /// </summary>
        internal GraphicsSettings()
            : base("Graphics")
        { }
    }
}