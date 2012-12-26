/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using VoxeliqEngine.Blocks;
using VoxeliqEngine.Chunks.Generators.Biomes;

namespace VoxeliqEngine.Chunks.Generators.Terrain
{
    /// <summary>
    /// Flat terrain that can be used for debugging purposes.
    /// </summary>
    public sealed class FlatDebugTerrain : BiomedTerrain
    {
        private const byte DirtHeight = 1;

        public FlatDebugTerrain(BiomeGenerator biomeGenerator) 
            : base(biomeGenerator)
        { }

        protected override void GenerateBlocks(Chunk chunk, int worldPositionX, int worldPositionZ)
        {
            var offset = BlockStorage.BlockIndexByWorldPosition(worldPositionX, worldPositionZ);

            for (int y = Chunk.MaxHeightIndexInBlocks; y >= 0; y--)
            {
                if (y >= DirtHeight)
                    BlockStorage.Blocks[offset + y] = new Block(BlockType.None);
                else
                    BlockStorage.Blocks[offset + y] = new Block(BlockType.Dirt);
            }

            chunk.HighestSolidBlockOffset = DirtHeight - 1;
            chunk.LowestEmptyBlockOffset = 0;

            // apply the biome generator on x-z column.
            this.BiomeGenerator.ApplyBiome(chunk, DirtHeight - 1, offset + DirtHeight - 1, worldPositionX + this.Seed, worldPositionZ);
        }     
    }
}