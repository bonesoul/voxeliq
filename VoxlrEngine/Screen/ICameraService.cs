using Microsoft.Xna.Framework;

namespace VolumetricStudios.VoxlrEngine.Screen
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

    /// <summary>
    /// Interface that allows control of the camera.
    /// </summary>
    public interface ICameraControlService
    {
        /// <summary>
        /// Rotates camera in x-axis.
        /// </summary>
        /// <param name="step">The amount to rotate.</param>
        void RotateCamera(float step);

        /// <summary>
        /// Elevates camera in y-axis.
        /// </summary>
        /// <param name="step">The amount to elavate</param>
        void ElevateCamera(float step);

        void LookAt(Vector3 target);
    }
}
