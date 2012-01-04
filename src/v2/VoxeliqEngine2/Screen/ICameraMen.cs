using SlimDX;

namespace VolumetricStudios.VoxeliqEngine.Screen
{
    /// <summary>
    /// Interface that allows control of the camera.
    /// </summary>
    public interface ICameraMen
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

        /// <summary>
        /// Makes the camera look within the direction of given vector.
        /// </summary>
        /// <param name="target">The direction to look at.</param>
        void LookAt(Vector3 target);
    }
}
