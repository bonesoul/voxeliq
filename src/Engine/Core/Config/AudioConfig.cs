using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxeliqEngine.Core.Config
{
    public class AudioConfig
    {
        #region configurable parameters

        /// <summary>
        /// Is audio enabled?
        /// </summary>
        public bool Enabled { get; set; }

        #endregion

        /// <summary>
        /// Creates a new instance of audio config.
        /// </summary>
        internal AudioConfig()
        {
            // set the defaults.
            this.Enabled = false;
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
