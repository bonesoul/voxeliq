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

        protected override void GenerateBlocks(Chunk chunk, int worldPositionX, int worldPositionZ)
        {
            byte height = 2;
            var offset = BlockStorage.BlockIndexByWorldPosition(worldPositionX, worldPositionZ);

            // fix for full height!;
            for (int y = 0; y < height; y++)
            {
                BlockStorage.Blocks[offset + y]=new Block(BlockType.Dirt);
            }

            chunk.HighestSolidBlockOffset = Chunk.MaxHeightIndexInBlocks;
            chunk.LowestEmptyBlockOffset = 0;
        }     
    }
}