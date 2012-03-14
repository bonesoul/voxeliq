/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using VolumetricStudios.VoxeliqGame.Common.Config;
using VolumetricStudios.VoxeliqGame.Graphics;

namespace VolumetricStudios.VoxeliqGame.Managers
{
    public sealed class AudioConfig : Config
    {
        /// <summary>
        /// Is full screen enabled?
        /// </summary>
        public bool Enabled
        {
            get { return this.GetBoolean("Enabled", true); }
            set { this.Set("Enabled", value); }
        }

        /// <summary>
        /// The memory instance of ScreenConfig.
        /// </summary>
        private static readonly AudioConfig _instance = new AudioConfig();

        /// <summary>
        /// The memory instance of ScreenConfig.
        /// </summary>
        public static AudioConfig Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Creates a new ScreenConfig instance.
        /// </summary>
        private AudioConfig()
            : base("Audio")
        {
        }
    }
}