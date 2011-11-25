/*
 * Copyright (C) 2011 voxeliq project 
 *
 */

using VolumetricStudios.VoxeliqEngine.Blocks;

namespace VolumetricStudios.VoxeliqEngine.Universe.Terrain.Generators.Biomes
{
    /// <summary>
    /// Rain forest generator.
    /// </summary>
    public sealed class RainForest : BiomedTerrain
    {
        protected override void ApplyBiome(Chunk chunk)
        {
            for (int x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (int z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    int offset = x * Chunk.FlattenOffset + z * Chunk.HeightInBlocks;
                    for(int y=chunk.HighestSolidBlockOffset; y>= 0 ;y--)
                    {
                        if (!chunk.Blocks[offset + y - 1].Exists) continue;
                        chunk.Blocks[offset+y].Type= BlockType.Grass;
                        break;        
                    }
                }
            }
        }
    }
}
