/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using VolumetricStudios.VoxeliqGame.Chunks;

namespace VolumetricStudios.VoxeliqGame.Generators
{
    public class TerrainGenerator
    {
        protected int Seed;

        public TerrainGenerator()
            : this(56) { }

        public TerrainGenerator(int seed)
        {
            this.Seed = seed;
        }

        public void Generate(Chunk chunk)
        {
            /* The chunk should be in queued state, if not just ignore the generate request */

            if (chunk.Generated) return;

            this.GenerateChunk(chunk);
            chunk.Generated = true;
        }

        protected virtual void GenerateChunk(Chunk chunk) { }
    }
}
