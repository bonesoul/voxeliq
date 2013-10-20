/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Engine.Blocks;
using Engine.Chunks.Generators.Biomes;
using Engine.Common.Noise;

namespace Engine.Chunks.Generators.Terrain
{
    public class ValleyTerrain : BiomedTerrain
    {
        public ValleyTerrain(BiomeGenerator biomeGenerator)
            : base(biomeGenerator)
        { }

        protected override void GenerateBlocks(Chunk chunk, int worldPositionX, int worldPositionZ)
        {
            var rockHeight = this.GetRockHeight(worldPositionX, worldPositionZ);
            var dirtHeight = this.GetDirtHeight(worldPositionX, worldPositionZ, rockHeight);

            var offset = BlockStorage.BlockIndexByWorldPosition(worldPositionX, worldPositionZ);

            for (int y = Chunk.MaxHeightIndexInBlocks; y >= 0; y--)
            {
                if (y > dirtHeight) // air
                {
                    BlockStorage.Blocks[offset + y] = new Block(BlockType.None);
                }
                else if (y > rockHeight) // dirt
                {
                    var valleyNoise = this.GenerateValleyNoise(worldPositionX, worldPositionZ, y);
                    BlockStorage.Blocks[offset + y] = new Block(valleyNoise > 0.2f ? BlockType.None : BlockType.Dirt);
                }
                else // rock level
                {
                    BlockStorage.Blocks[offset + y] = new Block(BlockType.Rock);
                }
            }

            // apply the biome generator on x-z column.
            this.BiomeGenerator.ApplyBiome(chunk, dirtHeight, offset + dirtHeight, worldPositionX + this.Seed, worldPositionZ);
        }

        protected virtual float GenerateValleyNoise(int worldPositionX, int worldPositionZ, int blockY)
        {
            worldPositionX += this.Seed;

            float caveNoise = SimplexNoise.noise(worldPositionX*0.01f, worldPositionZ*0.01f, blockY*0.01f)* (0.015f*blockY) + 0.1f;
            caveNoise += SimplexNoise.noise(worldPositionX*0.01f, worldPositionZ*0.01f, blockY*0.1f)*0.06f + 0.1f;
            caveNoise += SimplexNoise.noise(worldPositionX*0.2f, worldPositionZ*0.2f, blockY*0.2f)*0.03f + 0.01f;

            return caveNoise;
        }

        protected override int GetDirtHeight(int blockX, int blockZ, float rockHeight)
        {
            blockX += this.Seed;

            float octave1 = SimplexNoise.noise((blockX + 100)*0.001f, blockZ*0.001f)*0.5f;
            float octave2 = SimplexNoise.noise((blockX + 100)*0.002f, blockZ*0.002f)*0.25f;
            float octave3 = SimplexNoise.noise((blockX + 100)*0.01f, blockZ*0.01f)*0.25f;
            float octaveSum = octave1 + octave2 + octave3;

            return (int) (octaveSum*(Chunk.HeightInBlocks/2f)) + (int) (rockHeight);
        }

        protected override float GetRockHeight(int blockX, int blockZ)
        {
            blockX += this.Seed;

            int minimumGroundheight = Chunk.HeightInBlocks/4;
            int minimumGroundDepth = (int) (Chunk.HeightInBlocks*0.5f);

            float octave1 = SimplexNoise.noise(blockX*0.0001f, blockZ*0.0001f)*0.5f;
            float octave2 = SimplexNoise.noise(blockX*0.0005f, blockZ*0.0005f)*0.35f;
            float octave3 = SimplexNoise.noise(blockX*0.02f, blockZ*0.02f)*0.15f;
            float lowerGroundHeight = octave1 + octave2 + octave3;

            lowerGroundHeight = lowerGroundHeight*minimumGroundDepth + minimumGroundheight;

            return lowerGroundHeight;
        }
    }
}