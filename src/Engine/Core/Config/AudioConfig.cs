/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

namespace VoxeliqEngine.Core.Config
{
    public class AudioConfig
    {
        #region configurable parameters

        /// <summary>
        /// Is audio enabled?
        /// </summary>
        public bool Enabled { get; set; }

        #endregion

        /// <summary>
        /// Creates a new instance of audio config.
        /// </summary>
        internal AudioConfig()
        {
            // set the defaults.
            this.Enabled = false;
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
