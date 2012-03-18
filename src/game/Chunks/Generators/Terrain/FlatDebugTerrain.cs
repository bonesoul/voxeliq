/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using VolumetricStudios.VoxeliqGame.Blocks;
using VolumetricStudios.VoxeliqGame.Chunks.Generators.Biomes;

namespace VolumetricStudios.VoxeliqGame.Chunks.Generators.Terrain
{
    /// <summary>
    /// Flat terrain that can be used for debugging purposes.
    /// </summary>
    public sealed class FlatDebugTerrain : BiomedTerrain
    {
        public FlatDebugTerrain(BiomeGenerator biomeGenerator) 
            : base(biomeGenerator)
        { }

        protected override void GenerateBlock(Chunk chunk, int worldPositionX, int worldPositionZ)
        {
            byte height = 10;
            var offset = BlockStorage.BlockIndexByWorldPosition(worldPositionX, worldPositionZ);

            for (int y = Chunk.MaxHeightIndexInBlocks; y >= 0; y--)
            {
                BlockStorage.Blocks[offset + y] = new Block(BlockType.Grass);
            }

            chunk.HighestSolidBlockOffset = height;
            chunk.LowestEmptyBlockOffset = 0;
        }     
    }
}