/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

namespace VoxeliqStudios.Voxeliq.Engine
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
            /// Toggles infinitive world on or off.
            /// </summary>
            public static void ToggleInfinitiveWorld()
            {
                IsInfinitive = !IsInfinitive;
            }

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
            public static bool FPSGraphEnabled { get; set; }

            /// <summary>
            /// Toggles fps-graph on or off.
            /// </summary>
            public static void ToggleFPSGraph()
            {
                FPSGraphEnabled = !FPSGraphEnabled;
            }

            /// <summary>
            /// Static Debugging constructor.
            /// </summary>
            static Debugging()
            {
                FPSGraphEnabled = false;
            }
        }

        static Settings()
        { }
    }
}
