/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using VolumetricStudios.VoxeliqGame.Blocks;
using VolumetricStudios.VoxeliqGame.Chunks;

namespace VolumetricStudios.VoxeliqGame.Generators.Debug
{
    /// <summary>
    /// Flat terrain that can be used for debugging purposes.
    /// </summary>
    public sealed class FlatTerrain : TerrainGenerator
    {
        protected override void GenerateChunk(Chunk chunk)
        {
            byte height = 5;
            for (int x = 0; x < Chunk.WidthInBlocks; x++) // iterate through all point in x-z plane.
            {
                for (int z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    int offset = x * Chunk.FlattenOffset + z * Chunk.HeightInBlocks;
                    for (int y = 0; y < height; y++)
                    {
                        chunk.Blocks[offset + y] = y == height - 1 ? new Block(BlockType.Grass) : new Block(BlockType.Dirt);
                    }
                }
            }

            chunk.HighestSolidBlockOffset = height;
            chunk.LowestEmptyBlockOffset = 1;
        }
    }
}
