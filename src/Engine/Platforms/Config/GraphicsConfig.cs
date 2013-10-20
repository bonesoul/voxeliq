/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

namespace Engine.Platforms.Config
{
    public class GraphicsConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use fixed time steps.
        /// </summary>
        public bool IsFixedTimeStep { get; set; }

        /// <summary>
        /// Enables or disables vsync.
        /// </summary>
        public bool IsVsyncEnabled { get; set; }

        // Is post-processing enabled?
        public bool PostprocessEnabled { get; set; }

        /// <summary>
        /// Gets or sets if custom shaders are enabled for platform.
        /// </summary>
        public bool ExtendedEffects { get; set; }

        /// <summary>
        /// Creates a new instance of graphics-config.
        /// </summary>
        public GraphicsConfig()
        {
            // set defaults.
            this.IsFixedTimeStep = false;
            this.IsVsyncEnabled = false;
            this.PostprocessEnabled = true;
            this.ExtendedEffects = false;
        }
    }
}