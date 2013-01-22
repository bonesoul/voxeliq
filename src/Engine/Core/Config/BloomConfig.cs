﻿/*
 * Copyright (C) 2011 - 2013 Int6 Studios - http://www.int6.org,
 * Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using VoxeliqEngine.Graphics.Effects.PostProcessing.Bloom;

namespace VoxeliqEngine.Core.Config
{
    public class BloomConfig
    {
        #region configurable parameters

        /// <summary>
        /// Is bloom enabled?
        /// </summary>
        public bool Enabled { get; set; }

        public BloomState State { get; set; }

        #endregion

        #region togglers

        /// <summary>
        /// Sets bloom mode.
        /// </summary>
        /// <param name="state"><see cref="BloomState"/></param>
        public void SetMode(BloomState state)
        {
            this.State = state;
        }

        #endregion

        /// <summary>
        /// Creates a new instance of audio config.
        /// </summary>
        internal BloomConfig()
        {
            // set the defaults.
            this.Enabled = false;
            this.State = BloomState.Default;
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
