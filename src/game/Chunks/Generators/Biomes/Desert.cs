/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using VolumetricStudios.VoxeliqGame.Blocks;

namespace VolumetricStudios.VoxeliqGame.Chunks.Generators.Biomes
{
    public sealed class Desert : BiomeGenerator
    {
        public override void ApplyBiome(Chunk chunk)
        {
            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (byte z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    int offset = BlockStorage.BlockIndexByRelativePosition(chunk, x, z);

                    for (byte y = chunk.HighestSolidBlockOffset; y >= 0; y--)
                    {
                        if (!BlockStorage.Blocks[offset + y - 1].Exists)
                            continue;

                        BlockStorage.Blocks[offset + y].Type = BlockType.Sand;

                        break;
                    }
                }
            }
        }
    }
}
