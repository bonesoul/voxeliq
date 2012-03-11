/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using VolumetricStudios.VoxeliqGame.Blocks;
using VolumetricStudios.VoxeliqGame.Chunks;
using VolumetricStudios.VoxeliqGame.Generators.Biomes;
using VolumetricStudios.VoxeliqGame.Utils.Algorithms;

namespace VolumetricStudios.VoxeliqGame.Generators.Terrain
{
    public class ValleyTerrain : BasicTerrain
    {
        public ValleyTerrain(BiomeGenerator biomeGenerator)
            :base(biomeGenerator)
        { }

        protected override void GenerateTerrain(Chunk chunk, byte x, byte z, int worldPositionX, int worldPositionZ, int seededWorldPositionX)
        {
            this.RockHeight = this.GetRockHeight(seededWorldPositionX, worldPositionZ);
            this.DirtHeight = this.GetDirtHeight(seededWorldPositionX, worldPositionZ, RockHeight);
            
            //int offset = x * Chunk.FlattenOffset + z * Chunk.HeightInBlocks;
            var offset = BlockStorage.BlockIndexByWorldPosition(worldPositionX, worldPositionZ);

            for (int y = Chunk.MaxHeightIndexInBlocks; y >= 0; y--)
            {
                BlockType blockType;

                if (y > DirtHeight) // air
                    blockType = BlockType.None;
                else if (y > RockHeight) // dirt
                {
                    var valleyNoise = this.GenerateValleyNoise(worldPositionX, worldPositionZ, y);
                    blockType = valleyNoise> 0.2f ? BlockType.None : BlockType.Dirt;
                }
                else // rock level
                    blockType = BlockType.Rock;

                switch (blockType)
                {
                    case BlockType.None: if (chunk.LowestEmptyBlockOffset > y) chunk.LowestEmptyBlockOffset = (byte)y; break;
                    default: if (y > chunk.HighestSolidBlockOffset) chunk.HighestSolidBlockOffset = (byte)y; break;
                }
                
                //chunk.Blocks[offset + y] = new Block(blockType);
                BlockStorage.Blocks[offset + y] = new Block(blockType);
            }
        }

        protected virtual float GenerateValleyNoise(int worldPositionX, int worldPositionZ, int blockY)
        {
            float caveNoise = PerlinSimplexNoise.noise(worldPositionX * 0.01f, worldPositionZ * 0.01f, blockY * 0.01f) * (0.015f * blockY) + 0.1f;
            caveNoise += PerlinSimplexNoise.noise(worldPositionX * 0.01f, worldPositionZ * 0.01f, blockY * 0.1f) * 0.06f + 0.1f;
            caveNoise += PerlinSimplexNoise.noise(worldPositionX * 0.2f, worldPositionZ * 0.2f, blockY * 0.2f) * 0.03f + 0.01f;

            return caveNoise;
        }

        protected override int GetDirtHeight(int blockX, int blockZ, float rockHeight)
        {
            float octave1 = PerlinSimplexNoise.noise((blockX + 100) * 0.001f, blockZ * 0.001f) * 0.5f;
            float octave2 = PerlinSimplexNoise.noise((blockX + 100) * 0.002f, blockZ * 0.002f) * 0.25f;
            float octave3 = PerlinSimplexNoise.noise((blockX + 100) * 0.01f, blockZ * 0.01f) * 0.25f;
            float octaveSum = octave1 + octave2 + octave3;

            return (int)(octaveSum * (Chunk.HeightInBlocks / 2f)) + (int)(rockHeight);
        }

        protected override float GetRockHeight(int blockX, int blockZ)
        {
            int minimumGroundheight = Chunk.HeightInBlocks / 4;
            int minimumGroundDepth = (int)(Chunk.HeightInBlocks * 0.5f);

            float octave1 = PerlinSimplexNoise.noise(blockX * 0.0001f, blockZ * 0.0001f) * 0.5f;
            float octave2 = PerlinSimplexNoise.noise(blockX * 0.0005f, blockZ * 0.0005f) * 0.35f;
            float octave3 = PerlinSimplexNoise.noise(blockX * 0.02f, blockZ * 0.02f) * 0.15f;
            float lowerGroundHeight = octave1 + octave2 + octave3;

            lowerGroundHeight = lowerGroundHeight * minimumGroundDepth + minimumGroundheight;

            return lowerGroundHeight;
        }
    }
}
