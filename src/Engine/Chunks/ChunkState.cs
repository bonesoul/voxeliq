/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

namespace VoxeliqEngine.Chunks
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