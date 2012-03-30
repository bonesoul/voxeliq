/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using System;
using VoxeliqStudios.Voxeliq.Blocks;
using VoxeliqStudios.Voxeliq.Chunks.Generators.Terrain;

namespace VoxeliqStudios.Voxeliq.Chunks.Generators.Biomes
{
    /// <summary>
    /// Rain forest generator.
    /// </summary>
    public sealed class RainForest : BiomeGenerator
    {
        private readonly Random _treePlanter = new Random(TerrainGenerator.DefaultSeed);

        public override void ApplyBiome(Chunk chunk, int groundLevel, int groundOffset, int worldPositionX, int worldPositionZ)
        {
            bool plantTree = _treePlanter.Next(700) == 1;

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
            var trunkHeight = (byte) (5 + (byte) _treePlanter.Next(10));

            // trunk
            for (int y = 1; y <= trunkHeight; y++)
            {
                BlockStorage.Blocks[grassOffset + y].Type = BlockType.Tree;
            }

            // foliage
            int radius = 3 + _treePlanter.Next(2);

            for (int i = 0; i < 40 + _treePlanter.Next(4); i++)
            {
                int lx = worldPositionX + _treePlanter.Next(radius) - _treePlanter.Next(radius);
                int ly = grassLevel + trunkHeight + _treePlanter.Next(radius) - _treePlanter.Next(radius);
                int lz = worldPositionZ + _treePlanter.Next(radius) - _treePlanter.Next(radius);

                // http://techcraft.codeplex.com/SourceControl/changeset/view/5c1888588e5d#TechCraft%2fNewTake%2fNewTake%2fmodel%2fterrain%2fSimpleTerrain.cs
            }

            if (grassLevel + trunkHeight > chunk.HighestSolidBlockOffset)
                chunk.HighestSolidBlockOffset = (byte) (grassLevel + trunkHeight);

        }
    }
}