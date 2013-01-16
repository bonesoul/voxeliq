/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

namespace VoxeliqEngine.Core.Config
{
    public class WorldConfig
    {
        #region configurable parameters

        /// <summary>
        /// Is the world infinitive?
        /// </summary>
        public bool IsInfinitive { get; set; }

        #endregion

        #region togglers

        /// <summary>
        /// Toggles infinitive world on or off.
        /// </summary>
        public void ToggleInfinitiveWorld()
        {
            IsInfinitive = !IsInfinitive;
        }

        #endregion

        /// <summary>
        /// Creates a new instance of audio config.
        /// </summary>
        internal WorldConfig()
        {
            // set the defaults.
            this.IsInfinitive = true;
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
