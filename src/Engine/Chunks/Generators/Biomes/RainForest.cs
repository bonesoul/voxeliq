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
    public class RainForest : BiomeGenerator
    {
        private readonly Random _treePlanter = new Random(TerrainGenerator.DefaultSeed);

        public override void ApplyBiome(Chunk chunk, int groundLevel, int groundOffset, int worldPositionX, int worldPositionZ)
        {
            BlockStorage.Blocks[groundOffset + 1].Type = BlockType.Grass;

            if (groundLevel + 1 > chunk.HighestSolidBlockOffset)
                chunk.HighestSolidBlockOffset = (byte)(groundLevel + 1);

            bool plantTree = worldPositionX == 5 && worldPositionZ == 5;

            if (plantTree)
                this.PlantTree(chunk, groundLevel + 1, groundOffset + 1, worldPositionX, worldPositionZ);
        }

        private void PlantTree(Chunk chunk, int grassLevel, int grassOffset, int worldPositionX, int worldPositionZ)
        {
            // based on: http://techcraft.codeplex.com/SourceControl/changeset/view/5c1888588e5d#TechCraft%2fNewTake%2fNewTake%2fmodel%2fterrain%2fSimpleTerrain.cs

            var trunkHeight = 1;

            BlockStorage.SetBlockAt(worldPositionX,grassLevel,worldPositionZ,new Block(BlockType.Iron));

            var foliage = 1;
            for (int x = -foliage; x <=  foliage; x++)
            {
                if (x == 0)
                    continue;
                for (int z = -foliage; z <= foliage; z++)
                {
                    if (z == 0)
                        continue;

                    var offset = BlockStorage.BlockIndexByWorldPosition(worldPositionX + x, worldPositionZ + z);
                    for (int y = grassLevel; y < grassLevel + trunkHeight; y++ )
                    {
                        BlockStorage.SetBlockAt(worldPositionX+x, y, worldPositionZ+z, new Block(BlockType.Iron));
                        //BlockStorage.Blocks[offset + y] = new Block(BlockType.Gold);
                    }
                }
            }

            BlockStorage.SetBlockAt(5, 1, 0, new Block(BlockType.Brick));
            BlockStorage.SetBlockAt(5, 1, 1, new Block(BlockType.Brick));
            BlockStorage.SetBlockAt(5, 1, 2, new Block(BlockType.Brick));
            BlockStorage.SetBlockAt(5, 1, 3, new Block(BlockType.Brick));
            BlockStorage.SetBlockAt(5, 1, 4, new Block(BlockType.Brick));
            BlockStorage.SetBlockAt(5, 1, 5, new Block(BlockType.Brick));
            Console.WriteLine("5,1,5:{0}", BlockStorage.BlockIndexByWorldPosition(5, 1, 5));
            BlockStorage.SetBlockAt(5, 1, 6, new Block(BlockType.Brick));
            Console.WriteLine("5,1,6:{0}", BlockStorage.BlockIndexByWorldPosition(5, 1, 6));
            BlockStorage.SetBlockAt(5, 1, 7, new Block(BlockType.Brick));
            BlockStorage.SetBlockAt(5, 1, 8, new Block(BlockType.Brick));
            BlockStorage.SetBlockAt(5, 1, 9, new Block(BlockType.Brick));
            BlockStorage.SetBlockAt(5, 1, 10, new Block(BlockType.Brick));
            BlockStorage.SetBlockAt(5, 1, 11, new Block(BlockType.Brick));
            BlockStorage.SetBlockAt(5, 1, 12, new Block(BlockType.Brick));
            BlockStorage.SetBlockAt(5, 1, 13, new Block(BlockType.Brick));
            BlockStorage.SetBlockAt(5, 1, 14, new Block(BlockType.Brick));
            BlockStorage.SetBlockAt(5, 1, 15, new Block(BlockType.Brick));
            //BlockStorage.SetBlockAt(6, 1, 6, new Block(BlockType.Leaves));
            //BlockStorage.SetBlockAt(5, 1, 5, new Block(BlockType.Gold));
            //BlockStorage.SetBlockAt(4, 1, 4, new Block(BlockType.Gravel));
            //BlockStorage.SetBlockAt(4, 1, 6, new Block(BlockType.Gravel));

            if (grassLevel + trunkHeight > chunk.HighestSolidBlockOffset)
                chunk.HighestSolidBlockOffset = 127;
        }
    }
}