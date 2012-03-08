/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using VolumetricStudios.VoxeliqGame.Blocks;
using VolumetricStudios.VoxeliqGame.Chunks;

namespace VolumetricStudios.VoxeliqGame.Generators.Biomes
{
    /// <summary>
    /// Rain forest generator.
    /// </summary>
    public sealed class RainForest : BiomeGenerator
    {
        public override void ApplyBiome(Chunk chunk)
        {
            for (int x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (int z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    //int offset = x * Chunk.FlattenOffset + z * Chunk.HeightInBlocks;
                    for(int y=chunk.HighestSolidBlockOffset; y>= 0 ;y--)
                    {
                        var block = chunk.BlockAt(x, y, z);

                        //if (!chunk.Blocks[offset + y - 1].Exists) continue;                       
                        if(!block.Exists)
                            continue;
                        
                        //chunk.Blocks[offset+y].Type = BlockType.Grass;                        
                        block.Type = BlockType.Grass;
                        break;        
                    }
                }
            }
        }
    }
}
