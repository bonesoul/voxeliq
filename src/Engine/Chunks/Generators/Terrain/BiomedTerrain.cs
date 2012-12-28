﻿/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using VoxeliqEngine.Blocks;
using VoxeliqEngine.Chunks.Generators.Biomes;
using VoxeliqEngine.Common.Randomization.Procedural;

namespace VoxeliqEngine.Chunks.Generators.Terrain
{
    /// <summary>
    /// A basic terrain generator that supports biomes.
    /// </summary>
    public class BiomedTerrain : TerrainGenerator
    {
        /// <summary>
        /// Sets or gets assigned biome generator.
        /// </summary>
        protected BiomeGenerator BiomeGenerator { get; private set; }       

        /// <summary>
        /// Creates a new biomed terrain instance with given biome generator.
        /// </summary>
        /// <param name="biomeGenerator"><see cref="BiomeGenerator"/></param>
        public BiomedTerrain(BiomeGenerator biomeGenerator)
        {
            this.BiomeGenerator = biomeGenerator;
        }

        protected override void GenerateChunkTerrain(Chunk chunk)
        {
            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                var worldPositionX = chunk.WorldPosition.X + x;

                for (byte z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    int worldPositionZ = chunk.WorldPosition.Z + z;
                    this.GenerateBlocks(chunk, worldPositionX, worldPositionZ);
                }
            }
        }

        protected virtual void GenerateBlocks(Chunk chunk, int worldPositionX, int worldPositionZ)
        {
            var rockHeight = this.GetRockHeight(worldPositionX, worldPositionZ);
            var dirtHeight = this.GetDirtHeight(worldPositionX, worldPositionZ, rockHeight);

            var offset = BlockStorage.BlockIndexByWorldPosition(worldPositionX, worldPositionZ);

            for (int y = Chunk.MaxHeightIndexInBlocks; y >= 0; y--)
            {
                if (y > dirtHeight) // air
                {
                    BlockStorage.Blocks[offset + y] = new Block(BlockType.None);
                    if (chunk.LowestEmptyBlockOffset > y) chunk.LowestEmptyBlockOffset = (byte)y;
                }
                else if (y > rockHeight) // dirt level
                {
                    BlockStorage.Blocks[offset + y] = new Block(BlockType.Dirt);
                    if (y > chunk.HighestSolidBlockOffset) chunk.HighestSolidBlockOffset = (byte)y;
                }
                else // rock level
                {
                    BlockStorage.Blocks[offset + y] = new Block(BlockType.Rock);
                    if (y > chunk.HighestSolidBlockOffset) chunk.HighestSolidBlockOffset = (byte)y;
                }
            }

            // apply the biome generator on x-z column.
            this.BiomeGenerator.ApplyBiome(chunk, dirtHeight, offset + dirtHeight, worldPositionX, worldPositionZ);
        }

        protected virtual int GetDirtHeight(int blockX, int blockZ, float rockHeight)
        {
            blockX += this.Seed;

            float octave1 = SimplexNoise.noise((blockX + 100) * 0.001f, this.Seed, blockZ * 0.001f) * 0.5f;
            float octave2 = SimplexNoise.noise((blockX + 100) * 0.002f, this.Seed, blockZ * 0.002f) * 0.25f;
            float octave3 = SimplexNoise.noise((blockX + 100) * 0.01f, this.Seed, blockZ * 0.01f) * 0.25f;
            float octaveSum = octave1 + octave2 + octave3;

            return (int)(octaveSum * (Chunk.HeightInBlocks / 8)) + (int)(rockHeight);
        }

        protected virtual float GetRockHeight(int blockX, int blockZ)
        {
            blockX += this.Seed;

            int minimumGroundheight = Chunk.HeightInBlocks/2;
            int minimumGroundDepth = (int) (Chunk.HeightInBlocks*0.4f);

            float octave1 = SimplexNoise.noise(blockX * 0.0001f, this.Seed, blockZ * 0.0001f) * 0.5f;
            float octave2 = SimplexNoise.noise(blockX * 0.0005f, this.Seed, blockZ * 0.0005f) * 0.35f;
            float octave3 = SimplexNoise.noise(blockX * 0.02f, this.Seed, blockZ * 0.02f) * 0.15f;
            float lowerGroundHeight = octave1 + octave2 + octave3;

            lowerGroundHeight = lowerGroundHeight*minimumGroundDepth + minimumGroundheight;

            return lowerGroundHeight;
        }

        //public const int WaterLevel = 99;

        //protected  virtual void GenerateWater(Chunk chunk) - TODO: not working yet.
        //{
        //    for (int x = 0; x < Chunk.WidthInBlocks; x++)
        //    {
        //        for (int z = 0; z < Chunk.LenghtInBlocks; z++)
        //        {
        //            int offset = x * Chunk.XStep + z * Chunk.HeightInBlocks;
        //            for (int y=WaterLevel + 3; y>= RockHeight; y--)
        //            {
        //                if (chunk.Blocks[offset + y].Type == BlockType.None)
        //                    chunk.Blocks[offset + y] = new Block(BlockType.Water);
        //            }
        //        }
        //    }
        //}
    }
}