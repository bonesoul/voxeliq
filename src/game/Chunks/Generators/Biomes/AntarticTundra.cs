/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using VolumetricStudios.VoxeliqGame.Blocks;

namespace VolumetricStudios.VoxeliqGame.Chunks.Generators.Biomes
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
                    byte snowDepth = 5;
                    var offset = BlockStorage.BlockIndexByRelativePosition(chunk, (byte) x, (byte) z);

                    for (int y = chunk.HighestSolidBlockOffset; y > 0; y--)
                    {
                        if (!BlockStorage.Blocks[offset + y - 1].Exists)
                            continue;

                        BlockStorage.Blocks[offset + y].Type = BlockType.Grass;

                        snowDepth--;
                        if (snowDepth == 0) break;
                    }
                }
            }
        }
    }
}