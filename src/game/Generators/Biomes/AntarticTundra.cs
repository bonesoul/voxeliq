/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using VolumetricStudios.VoxeliqGame.Blocks;
using VolumetricStudios.VoxeliqGame.Chunks;

namespace VolumetricStudios.VoxeliqGame.Generators.Biomes
{
    /// <summary>
    /// Antartic tundra generator.
    /// </summary>
    public sealed class AntarticTundra : BiomeGenerator
    {
        public override void ApplyBiome(Chunk chunk)
        {
            for (int x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (int z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    //int offset = x * Chunk.FlattenOffset + z * Chunk.HeightInBlocks;
                    byte snowDepth = 5;
                    int chunkIndex = BlockStorage.BlockIndexByRelativePosition(chunk, (byte)x, (byte)z);

                    for(int y=chunk.HighestSolidBlockOffset; y> 0 ;y--)
                    {
                        // if (!chunk.Blocks[offset + y - 1].Exists)
                        //if (!chunk.BlockAt(x, y - 1, z).Exists)
                        if(!BlockStorage.Blocks[chunkIndex + y -1].Exists)
                            continue;

                        //chunk.Blocks[offset+y].Type= BlockType.Snow;
                        //var blockIndex = BlockCache.GetBlockIndex(chunk, (byte)x, (byte)y, (byte)z);

                        BlockStorage.Blocks[chunkIndex + y].Type = BlockType.Grass; 

                        snowDepth--;
                        if(snowDepth==0) break;
                    }
                }
            }
        }
    }
}
