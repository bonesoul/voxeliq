using Microsoft.Xna.Framework;

namespace VolumetricStudios.VoxeliqGame.Worlds.Enviromental
{
    public interface IFogService
    {
        /// <summary>
        /// Fog state.
        /// </summary>
        FogState State { get; }

        /// <summary>
        /// Fog vector value for current fog-state.
        /// </summary>
        Vector2 FogVector { get; }

        /// <summary>
        /// Fog vectors.
        /// </summary>
        void ToggleFog();
    }
}
