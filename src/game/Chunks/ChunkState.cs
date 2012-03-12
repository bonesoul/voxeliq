/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

namespace VolumetricStudios.VoxeliqGame.Chunks
{
    /// <summary>
    /// The chunk state.
    /// </summary>
    public enum ChunkState
    {
        /// <summary>
        /// Chunk awaits initial generation.
        /// </summary>
        AwaitingGenerate,

        /// <summary>
        /// Chunk is being generated.
        /// </summary>
        Generating,

        /// <summary>
        /// Chunk awaits initial lighting.
        /// </summary>
        AwaitingLighting,

        /// <summary>
        /// Chunk is being lightened.
        /// </summary>
        Lighting,

        /// <summary>
        /// Chunk awaits initial build.
        /// </summary>
        AwaitingBuild,

        /// <summary>
        /// Chunk is being built.
        /// </summary>
        Building,

        /// <summary>
        /// Chunk is all clean and ready for rendering.
        /// </summary>
        Ready,

        /// <summary>
        /// Chunks awaits a new relighting,.
        /// </summary>
        AwaitingRelighting,

        /// <summary>
        /// Chunks awaits a rebuild.
        /// </summary>
        AwaitingRebuild,

        /// <summary>
        /// Chunk awaits for removal.
        /// </summary>
        AwaitingRemoval
    }
}