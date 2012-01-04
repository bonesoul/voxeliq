using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VolumetricStudios.VoxeliqEngine.Core
{
    public class GameTime
    {
        /// <summary>
        /// Gets the elapsed game time, in seconds.
        /// </summary>
        public TimeSpan ElapsedGameTime { get; internal set; }

        /// <summary>
        /// Gets the total game time in seconds.
        /// </summary>
        public TimeSpan TotalGameTime { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this instance is running slowly.
        /// </summary>
        public bool IsRunningSlowly { get; internal set; }

        /// <summary>
        /// Gets the current frames-per-second measure.
        /// </summary>
        public float FramesPerSecond { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameTime"/> class.
        /// </summary>
        public GameTime()
        {
            this.ElapsedGameTime = TimeSpan.Zero;
            this.TotalGameTime = TimeSpan.Zero;
            this.IsRunningSlowly = false;
            this.FramesPerSecond = 0.0f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameTime"/> class.
        /// </summary>
        public GameTime(TimeSpan totalGameTime, TimeSpan elapsedGameTime, bool isRunningSlowly)
        {
            this.TotalGameTime = totalGameTime;
            this.ElapsedGameTime = elapsedGameTime;
            this.IsRunningSlowly = isRunningSlowly;
        }
    }
}
