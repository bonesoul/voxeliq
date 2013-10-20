/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Microsoft.Xna.Framework;

namespace Engine.Platforms.Config
{
    public class ScreenConfig
    {
        /// <summary>
        /// Screen width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Screen height.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Is full screen enabled?
        /// </summary>
        public bool IsFullScreen { get; set; }

        /// <summary>
        /// Supported orientations.
        /// </summary>
        public DisplayOrientation SupportedOrientations { get; set; }

        public ScreenConfig()
        {
            // set defaults.
            this.Width = 0; // make default resolution 0x0, so that it'll be only set if a platform requires so and sets the values - otherwise the system default will be used.
            this.Height = 0;
            this.IsFullScreen = false;
            this.SupportedOrientations = DisplayOrientation.Default;
        }
    }
}