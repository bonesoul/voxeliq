using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VolumetricStudios.VoxeliqEngine.Config;

namespace VolumetricStudios.VoxeliqClient
{
    public sealed class ScreenConfig : Config
    {
        /// <summary>
        /// Is full screen enabled?
        /// </summary>
        public bool FullScreenEnabled { get { return this.GetBoolean("FullScreen", true); } set { this.Set("FullScreen", value); } }

        /// <summary>
        /// Sets the screen width.
        /// </summary>
        public int ScreenWidth { get { return this.GetInt("Width", 1280); } set { this.Set("ScreenWidth", value); } }

        /// <summary>
        /// Sets the screen height.
        /// </summary>
        public int ScreenHeight { get { return this.GetInt("Height", 720); } set { this.Set("ScreenHeight", value); } }

        /// <summary>
        /// The memory instance of ScreenConfig.
        /// </summary>
        private static readonly ScreenConfig _instance = new ScreenConfig();

        /// <summary>
        /// The memory instance of ScreenConfig.
        /// </summary>
        public static ScreenConfig Instance { get { return _instance; } }

        /// <summary>
        /// Creates a new ScreenConfig instance.
        /// </summary>
        private ScreenConfig()
            : base("Screen") { }
    }
}
