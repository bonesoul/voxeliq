/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

namespace Engine.Core.Config
{
    /// <summary>
    /// Contains graphics related configuration parameters.
    /// </summary>
    public class GraphicsConfig
    {
        #region configurable parameters

        /// <summary>
        /// Gets or sets the screen width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the screen height.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Is full screen enabled?
        /// </summary>
        public bool FullScreenEnabled { get; set; }

        /// <summary>
        /// Is vsync enabled?
        /// </summary>
        public bool VerticalSyncEnabled { get; set; }

        /// <summary>
        /// Is fixed time steps enabled?
        /// </summary>
        public bool FixedTimeStepsEnabled { get; set; }

        #endregion

        /// <summary>
        /// Creates a new instance of graphics config.
        /// </summary>
        internal GraphicsConfig()
        {
            // set the defaults.
            this.Width = 1280;
            this.Height = 720;
            this.FullScreenEnabled = false;
            this.VerticalSyncEnabled = false;
            this.FixedTimeStepsEnabled = false;
        }

        /// <summary>
        /// Validates the configuration.
        /// </summary>
        /// <returns></returns>
        internal bool Validate()
        {
            return true;
        }
    }
}
