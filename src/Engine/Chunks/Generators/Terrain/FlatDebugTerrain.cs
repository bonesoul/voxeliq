/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Engine.Blocks;
using Engine.Chunks.Generators.Biomes;

namespace Engine.Chunks.Generators.Terrain
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

            // apply the biome generator on x-z column.
            this.BiomeGenerator.ApplyBiome(chunk, DirtHeight - 1, offset + DirtHeight - 1, worldPositionX + this.Seed, worldPositionZ);
        }
    }
}