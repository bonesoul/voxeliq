/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using VolumetricStudios.VoxeliqGame.Blocks;
using VolumetricStudios.VoxeliqGame.Chunks;

namespace VolumetricStudios.VoxeliqGame.Generators.Terrain
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
                        BlockStorage.Blocks[offset + y] = y == height - 1
                                                              ? new Block(BlockType.Grass)
                                                              : new Block(BlockType.Dirt);
                    }
                }
            }

            chunk.HighestSolidBlockOffset = (byte) (height - 1);
            chunk.LowestEmptyBlockOffset = 1;
        }
    }
}