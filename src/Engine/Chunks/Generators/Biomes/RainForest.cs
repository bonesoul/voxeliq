/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using VoxeliqEngine.Blocks;
using VoxeliqEngine.Chunks.Generators.Terrain;

namespace VoxeliqEngine.Chunks.Generators.Biomes
{
    /// <summary>
    /// Rain forest generator.
    /// </summary>
    public sealed class RainForest : BiomeGenerator
    {
        private readonly Random _treePlanter = new Random(TerrainGenerator.DefaultSeed);

        public override void ApplyBiome(Chunk chunk, int groundLevel, int groundOffset, int worldPositionX, int worldPositionZ)
        {
            bool plantTree = _treePlanter.Next(1000) == 1;

            BlockStorage.Blocks[groundOffset + 1].Type = BlockType.Grass;

            if (groundLevel + 1 > chunk.HighestSolidBlockOffset)
                chunk.HighestSolidBlockOffset = (byte)(groundLevel + 1);

            if (plantTree)
            {
                this.PlantTree(chunk, groundLevel + 1, groundOffset + 1, worldPositionX, worldPositionZ);
            }
        }

        private void PlantTree(Chunk chunk, int grassLevel, int grassOffset, int worldPositionX, int worldPositionZ)
        {
            // based on: http://techcraft.codeplex.com/SourceControl/changeset/view/5c1888588e5d#TechCraft%2fNewTake%2fNewTake%2fmodel%2fterrain%2fSimpleTerrain.cs

            var trunkHeight = (byte) (5 + (byte) _treePlanter.Next(10));

            Console.WriteLine("tree location: {0},{1},{2}", worldPositionX, grassLevel, worldPositionZ);
            // trunk
            for (byte y = 1; y <= trunkHeight; y++)
            {
                BlockStorage.Blocks[grassOffset + y].Type = BlockType.Tree;

                // set the foliage.
                int radius = 2;


                Console.WriteLine("foliage: {0}-{1}, {2}, {3}", worldPositionX - radius, worldPositionX + radius, grassLevel + y, worldPositionZ);
                for (int x = worldPositionX - radius; x <= worldPositionX + radius; x++)
                {
                    //for (int z = worldPositionZ - radius; z <= worldPositionZ + radius; z++)
                    //{
                    if (!BlockStorage.BlockAt(x, grassLevel + y, worldPositionZ).Exists)
                    {
                        Console.Write("leave ");
                        BlockStorage.SetBlockAt(x, grassLevel + y, worldPositionZ, new Block(BlockType.Leaves));
                    }
                    else
                    {
                        Console.Write("solid ");
                    }                    
                    //}
                }

                Console.WriteLine();
            }

            if (grassLevel + trunkHeight > chunk.HighestSolidBlockOffset)
                chunk.HighestSolidBlockOffset = (byte) (grassLevel + trunkHeight + 1);

        }
    }
}