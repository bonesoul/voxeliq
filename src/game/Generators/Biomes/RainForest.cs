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
            for (int x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (int z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    //int offset = x * Chunk.FlattenOffset + z * Chunk.HeightInBlocks;
                    for(int y=chunk.HighestSolidBlockOffset; y>= 0 ;y--)
                    {
                        //if (!chunk.Blocks[offset + y - 1].Exists) continue;                       
                        if (!chunk.BlockAt(x, y - 1, z).Exists)
                            continue;
                            
                        // struct is byval so access block directly when we want to change it's data!
                        var blockIndex = BlockCache.GetBlockIndex(chunk, (byte) x, (byte) y, (byte) z);
                        BlockCache.Instance.Blocks[blockIndex].Type = BlockType.Grass;                            

                        //chunk.Blocks[offset+y].Type = BlockType.Grass;                        
                        //var block = chunk.BlockAt(x, y, z);
                        //var block2 = chunk.BlockAt(x, y, z);


                        //block.SetType(BlockType.Grass);

                        

                        break;        
                    }
                }
            }
        }
    }
}
