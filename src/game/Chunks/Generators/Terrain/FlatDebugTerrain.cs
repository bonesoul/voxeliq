/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using System;
using System.Diagnostics;
using VolumetricStudios.VoxeliqGame.Blocks;

namespace VolumetricStudios.VoxeliqGame.Chunks.Generators.Terrain
{
    /// <summary>
    /// Flat terrain that can be used for debugging purposes.
    /// </summary>
    public sealed class FlatDebugTerrain : TerrainGenerator
    {
        protected override void GenerateChunk(Chunk chunk)
        {
            byte height = 5;
            for (byte x = 0; x < Chunk.WidthInBlocks; x++) // iterate through all point in x-z plane.
            {
                for (byte z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    var offset = BlockStorage.BlockIndexByRelativePosition(chunk, x, z);

                    for (int y = 0; y < height; y++)
                    {
                        BlockStorage.Blocks[offset + y] = y == height - 1 ? new Block(BlockType.Grass) : new Block(BlockType.Dirt);
                    }
                }
            }

            chunk.HighestSolidBlockOffset = (byte) (height - 1);
            chunk.LowestEmptyBlockOffset = 1;

            this.GeneratePlants(chunk);
        }

        private void GeneratePlants(Chunk chunk)
        {
            var r = new Random(this.Seed);

            for (byte x = 0; x < Chunk.WidthInBlocks; x++) // iterate through all point in x-z plane.
            {
                for (byte z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    var offset = BlockStorage.BlockIndexByRelativePosition(chunk, x, z);

                    if(r.Next(250) == 1)
                    {
                        this.BuildTree(chunk, offset, x, z);
                    }
                }
            }

            chunk.HighestSolidBlockOffset += 8; // add trunk height.
        }

        private  void BuildTree(Chunk chunk, int offset, byte relativeX, byte relativeZ)
        {
            var r = new Random(this.Seed);

            // trunk
            var trunkHeight = 8;
            var trunkBottomY = (byte) (chunk.HighestSolidBlockOffset + 1);

            if (trunkBottomY >= Chunk.HeightInBlocks) return;

            for (byte y = trunkBottomY; y < trunkBottomY + trunkHeight; y++)
            {
                BlockStorage.Blocks[offset + y] = new Block(BlockType.Tree);
            }

            // Foliage
            int radius = 3 + r.Next(2);
            byte nY = (byte) (trunkBottomY + trunkHeight);
            for (int i = 0; i < 40 + r.Next(4); i++)
            {
                byte lx = (byte) (relativeX + r.Next(radius) - r.Next(radius));
                byte ly = (byte) (nY + r.Next(radius) - r.Next(radius));
                byte lz = (byte) (relativeZ + r.Next(radius) - r.Next(radius));

                var leafBlockOffset = BlockStorage.BlockIndexByRelativePosition(chunk, lx, ly, lz);
                BlockStorage.Blocks[leafBlockOffset] = new Block(BlockType.Leaves);
            }
        }
    }
}