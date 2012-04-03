/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using VoxeliqStudios.Voxeliq.Blocks;

namespace VoxeliqStudios.Voxeliq.Chunks.Generators.Biomes
{
    public sealed class Desert : BiomeGenerator
    {
        public override void ApplyBiome(Chunk chunk, int groundLevel, int groundOffset, int worldPositionX, int worldPositionZ)
        {
            BlockStorage.Blocks[groundOffset + 1].Type = BlockType.Sand;

            if (groundLevel + 1 > chunk.HighestSolidBlockOffset)
                chunk.HighestSolidBlockOffset = (byte)(groundLevel + 1);
        }
    }
}
