/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using System;
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
            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (byte z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    //int offset = x * Chunk.FlattenOffset + z * Chunk.HeightInBlocks;
                    int offset = BlockCache.BlockIndexByRelativePosition(chunk, x, z);

                    for (byte y = chunk.HighestSolidBlockOffset; y >= 0; y--)
                    {
                        //if (!chunk.Blocks[offset + y - 1].Exists);                       
                        if (!BlockCache.Blocks[offset + y - 1].Exists)
                            continue;
                            
                        BlockCache.Blocks[offset + y].Type = BlockType.Grass;                            

                        //chunk.Blocks[offset+y].Type = BlockType.Grass;                        
                       
                        break;        
                    }
                }
            }
        }
    }
}
