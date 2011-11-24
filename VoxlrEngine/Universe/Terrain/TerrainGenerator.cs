/*    
 * Copyright (C) 2011, Hüseyin Uslu
 *  
 */

namespace VolumetricStudios.VoxlrEngine.Universe.Terrain
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
