﻿/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

namespace VoxeliqEngine.Core
{
    /// <summary>
    /// Gets or sets engine specific settings.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// World related setings.
        /// </summary>
        public static class World
        {
            /// <summary>
            /// Is the world infinitive?
            /// </summary>
            public static bool IsInfinitive { get; set; }

            /// <summary>
            /// Static world constructor.
            /// </summary>
            static World()
            {
                IsInfinitive = true;
            }
        }

        /// <summary>
        /// Debugging related settings.
        /// </summary>
        public static class Debugging
        {
            /// <summary>
            /// Gets or sets if fps-graph is enabled.
            /// </summary>
            public static bool DebugGraphsEnabled { get; set; }

            /// <summary>
            /// Static Debugging constructor.
            /// </summary>
            static Debugging()
            {
                DebugGraphsEnabled = true;
            }
        }

        static Settings()
        { }
    }
}
