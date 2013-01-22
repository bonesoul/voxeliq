/*
 * Copyright (C) 2011 - 2013 Int6 Studios - http://www.int6.org,
 * Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using VoxeliqEngine.Blocks;

namespace VoxeliqEngine.Chunks.Generators.Biomes
{
    public sealed class Desert : BiomeGenerator
    {
        public override void ApplyBiome(Chunk chunk, int groundLevel, int groundOffset, int worldPositionX, int worldPositionZ)
        {
            BlockStorage.Blocks[groundOffset + 1].Type = BlockType.Sand;
        }
    }
}
