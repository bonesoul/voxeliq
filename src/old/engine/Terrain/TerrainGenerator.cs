/*
 * Copyright (C) 2011 voxeliq project 
 *
 */

using VolumetricStudios.VoxeliqEngine.Chunks;

namespace VolumetricStudios.VoxeliqEngine.Terrain
{
    public class TerrainGenerator
    {
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
