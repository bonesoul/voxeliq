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
        private const byte RockHeight = 1;

        public FlatDebugTerrain(BiomeGenerator biomeGenerator) 
            : base(biomeGenerator)
        { }

        protected override void GenerateBlocks(Chunk chunk, int worldPositionX, int worldPositionZ)
        {
            var offset = BlockStorage.BlockIndexByWorldPosition(worldPositionX, worldPositionZ);

            for (int y = Chunk.MaxHeightIndexInBlocks; y >= 0; y--)
            {
                if (y >= RockHeight)
                    BlockStorage.Blocks[offset + y] = new Block(BlockType.None);
                else
                    BlockStorage.Blocks[offset + y] = new Block(BlockType.Rock);
            }

            chunk.HighestSolidBlockOffset = RockHeight;
            chunk.LowestEmptyBlockOffset = 0;
        }     
    }
}