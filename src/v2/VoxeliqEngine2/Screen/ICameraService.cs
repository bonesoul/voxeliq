using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace VolumetricStudios.VoxeliqEngine.Screen
{
    /// <summary>
    /// Interface that provides camera information.
    /// </summary>
    public interface ICameraService
    {
        /// <summary>
        /// Camera's projection matrix (lens).
        /// </summary>
        Matrix Projection { get; }

        /// <summary>
        /// Cameras view matrix (position).
        /// </summary>
        Matrix View { get; }

        /// <summary>
        /// The world matrix.
        /// </summary>
        Matrix World { get; }

        /// <summary>
        /// Camera's position.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// Current camera elevation.
        /// </summary>
        float CurrentElevation { get; }

        /// <summary>
        /// Current camera rotation.
        /// </summary>
        float CurrentRotation { get; }
    }
}
