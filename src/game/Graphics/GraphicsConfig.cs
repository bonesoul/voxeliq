/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using VolumetricStudios.VoxeliqGame.Common.Config;

namespace VolumetricStudios.VoxeliqGame.Graphics
{
    public sealed class GraphicsConfig : Config
    {
        /// <summary>
        /// Is full screen enabled?
        /// </summary>
        public bool FullScreenEnabled
        {
            get { return this.GetBoolean("FullScreen", true); }
            set { this.Set("FullScreen", value); }
        }

        /// <summary>
        /// Sets the screen width.
        /// </summary>
        public int Width
        {
            get { return this.GetInt("Width", 1280); }
            set { this.Set("Width", value); }
        }

        /// <summary>
        /// Sets the screen height.
        /// </summary>
        public int Height
        {
            get { return this.GetInt("Height", 720); }
            set { this.Set("Height", value); }
        }

        /// <summary>
        /// The memory instance of ScreenConfig.
        /// </summary>
        private static readonly GraphicsConfig _instance = new GraphicsConfig();

        /// <summary>
        /// The memory instance of ScreenConfig.
        /// </summary>
        public static GraphicsConfig Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Creates a new ScreenConfig instance.
        /// </summary>
        private GraphicsConfig()
            : base("Graphics")
        {
        }
    }
}