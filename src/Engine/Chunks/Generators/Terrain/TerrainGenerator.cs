/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

namespace Engine.Chunks.Generators.Terrain
{
    /// <summary>
    /// A terrain generator.
    /// </summary>
    public class TerrainGenerator
    {
        /// <summary>
        /// A default seed for terrain generator if no seed is assigned.
        /// </summary>
        public const int DefaultSeed = 0;

        /// <summary>
        /// Gets or sets the current seed.
        /// </summary>
        protected int Seed { get; private set; }

        /// <summary>
        /// Creates a new terrain generator with given seed.
        /// </summary>
        /// <param name="seed"></param>
        public TerrainGenerator(int seed)
        {
            this.Seed = seed;
        }

        /// <summary>
        /// Creates a new terrain generator with default seed.
        /// </summary>
        public TerrainGenerator()
            : this(DefaultSeed)
        { }        

        /// <summary>
        /// Generates the terrain for given chunk.
        /// </summary>
        /// <param name="chunk">The chunk to generate terrain for.</param>
        public void Generate(Chunk chunk)
        {
            if (chunk.ChunkState != ChunkState.AwaitingGenerate) // if chunk is not awating generation
                return; // then just pass it.

            chunk.ChunkState = ChunkState.Generating; // set chunk state to generating.

            this.GenerateChunkTerrain(chunk); // generate the chunk terrain.

            chunk.ChunkState = ChunkState.AwaitingLighting; // chunk should be lighten now.            
        }

        /// <summary>
        /// Generates chunk terrain for given chunk.
        /// </summary>
        /// <param name="chunk">The chunk to generate terrain for.</param>
        protected virtual void GenerateChunkTerrain(Chunk chunk)
        { }
    }
}