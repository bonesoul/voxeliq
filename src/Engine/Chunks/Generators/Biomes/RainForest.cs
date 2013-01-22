﻿/*
 * Copyright (C) 2011 - 2013 Int6 Studios - http://www.int6.org,
 * Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Diagnostics;
using VoxeliqEngine.Blocks;
using VoxeliqEngine.Chunks.Generators.Terrain;
using VoxeliqEngine.Chunks.Populators;
using VoxeliqEngine.Common.Noise;

namespace VoxeliqEngine.Chunks.Generators.Biomes
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