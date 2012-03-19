/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using System;
using VolumetricStudios.VoxeliqGame.Blocks;
using VolumetricStudios.VoxeliqGame.Utils.Algorithms;

namespace VolumetricStudios.VoxeliqGame.Chunks.Generators.Biomes
{
    /// <summary>
    /// Rain forest generator.
    /// </summary>
    public sealed class RainForest : BiomeGenerator
    {
        private float TreeNoise(int blockX, int blockZ, float grassHeight)
        {
            float octave1 = SimplexNoise.noise((blockX + 100) * 0.001f, blockZ * 0.001f) * 0.5f;
            float octave2 = SimplexNoise.noise((blockX + 100) * 0.002f, blockZ * 0.002f) * 0.25f;
            float octave3 = SimplexNoise.noise((blockX + 100) * 0.01f, blockZ * 0.01f) * 0.25f;
            float octaveSum = octave1 + octave2 + octave3;

            return (int)(octaveSum * (Chunk.HeightInBlocks / 8)) + (int)(grassHeight);
        }

        public override void ApplyBiome(Chunk chunk, int groundLevel, int groundOffset, int worldPositionX, int worldPositionZ)
        {
            BlockStorage.Blocks[groundOffset + 1].Type = BlockType.Grass;

            //var treeNoise = TreeNoise(worldPositionX, worldPositionZ, groundLevel + 1);
            //Console.WriteLine(treeNoise);

            if (groundLevel + 1 > chunk.HighestSolidBlockOffset)
                chunk.HighestSolidBlockOffset = (byte)(groundLevel + 1);
        }
    }
}