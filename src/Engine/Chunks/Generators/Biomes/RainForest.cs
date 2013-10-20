/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using Engine.Blocks;
using Engine.Chunks.Generators.Terrain;
using Engine.Common.Noise;

namespace Engine.Chunks.Generators.Biomes
{
    /// <summary>
    /// Rain forest generator.
    /// </summary>
    public class RainForest : BiomeGenerator
    {
        private readonly Random _treePlanter = new Random(TerrainGenerator.DefaultSeed);

        public override void ApplyBiome(Chunk chunk, int groundLevel, int groundOffset, int worldPositionX, int worldPositionZ)
        {
            BlockStorage.Blocks[groundOffset + 1].Type = BlockType.Grass;

            //var test = GetRockHeight(worldPositionX, worldPositionZ);
            //if (Math.Abs(test - groundLevel) < 1)
            //{
            //    TreePopulator.PopulateTree(chunk.WorldPosition.X + 8, chunk.WorldPosition.Z + 8, 1);
            //    chunk.HighestSolidBlockOffset += 11;
            //}
        }

        protected virtual double GetRockHeight(int blockX, int blockZ)
        {
            return SimplexNoise.noise(blockX * 0.02f, blockZ * 0.02f) * 100;
        }
    }
}