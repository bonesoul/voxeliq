/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

namespace VolumetricStudios.VoxeliqGame.Chunks.Generators.Terrain
{
    public class TerrainGenerator
    {
        protected int Seed { get; private set; }

        public TerrainGenerator()
            : this(56)
        {
        }

        public TerrainGenerator(int seed)
        {
            this.Seed = seed;
        }

        public void Generate(Chunk chunk)
        {
            if (chunk.ChunkState != ChunkState.AwaitingGenerate) // if chunk is not awating generation
                return; // then just pass it.

            chunk.ChunkState = ChunkState.Generating; // set chunk state to generating.

            this.GenerateChunk(chunk); // generate the chunk.

            chunk.ChunkState = ChunkState.AwaitingLighting; // chunk should be lighten now.            
        }

        protected virtual void GenerateChunk(Chunk chunk)
        { }
    }
}