/*
 * Copyright (C) 2011 - 2013 Int6 Studios - http://www.int6.org,
 * Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

namespace VoxeliqEngine.Core.Config
{
    public class DebugConfig
    {
        #region configurable parameters

        /// <summary>
        /// Gets or sets if debug graphs are enabled.
        /// </summary>
        public bool GraphsEnabled { get; set; }

        #endregion

        /// <summary>
        /// Creates a new instance of audio config.
        /// </summary>
        internal DebugConfig()
        {
            // set the defaults.
            this.GraphsEnabled = true;
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
