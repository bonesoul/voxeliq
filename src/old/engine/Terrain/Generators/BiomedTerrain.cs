/*
 * Copyright (C) 2011 voxeliq project 
 *
 */

using VolumetricStudios.VoxeliqEngine.Blocks;
using VolumetricStudios.VoxeliqEngine.Chunks;
using VolumetricStudios.VoxeliqEngine.Utils.Algorithms;

namespace VolumetricStudios.VoxeliqEngine.Terrain.Generators
{
    /// <summary>
    /// Biomed terrain generators.
    /// </summary>
    public class BiomedTerrain:TerrainGenerator
    {
        protected int Seed;

        public BiomedTerrain():this(56) { }

        public BiomedTerrain(int seed)
        {
            this.Seed = seed;
        }

        protected override void GenerateChunk(Chunk chunk)
        {
            for (int x = 0; x < Chunk.WidthInBlocks; x++) 
            {
                int worldPositionX = chunk.WorldPosition.X + x + this.Seed; 

                for (int z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    int worldPositionZ = chunk.WorldPosition.Z + z;
                    GenerateTerrain(chunk, x, z, worldPositionX, worldPositionZ);
                }
            }

            ApplyBiome(chunk);
        }

        protected void GenerateTerrain(Chunk chunk, int x, int z, int worldPositionX, int worldPositionZ) 
        {
            float rockHeight = GetRockHeight(worldPositionX, worldPositionZ);
            int dirtHeight = GetBiomeHeight(worldPositionX, worldPositionZ, rockHeight);
            int offset = x*Chunk.FlattenOffset + z*Chunk.HeightInBlocks;

            for (int y = Chunk.MaxHeightInBlocks; y >= 0; y--)
            {
                BlockType blockType;
                
                if (y > dirtHeight) // air
                {
                    blockType = BlockType.None; // everything above upper-ground height is empty-space -- air.
                }
                else if (y > rockHeight) // dirt
                {
                    blockType = BlockType.Dirt;
                }
                else // rock level
                {
                    blockType = BlockType.Rock;
                }

                switch (blockType)
                {
                    case BlockType.None: if (chunk.LowestEmptyBlockOffset > y) chunk.LowestEmptyBlockOffset = (byte)y; break;
                    default: if (y > chunk.HighestSolidBlockOffset) chunk.HighestSolidBlockOffset = (byte)y; break;
                }

                chunk.Blocks[offset + y] = new Block(blockType);
            }
        }

        protected virtual void ApplyBiome(Chunk chunk) { }

        protected static int GetBiomeHeight(int blockX, int blockY, float rockHeight)
        {
            float octave1 = PerlinSimplexNoise.noise((blockX + 100) * 0.001f, blockY * 0.001f) * 0.5f;
            float octave2 = PerlinSimplexNoise.noise((blockX + 100) * 0.002f, blockY * 0.002f) * 0.25f;
            float octave3 = PerlinSimplexNoise.noise((blockX + 100) * 0.01f, blockY * 0.01f) * 0.25f;
            float octaveSum = octave1 + octave2 + octave3;

            return (int)(octaveSum * (Chunk.HeightInBlocks / 8)) + (int)(rockHeight);
        }

        protected static float GetRockHeight(int blockX, int blockY)
        {
            int minimumGroundheight = Chunk.HeightInBlocks / 2;
            int minimumGroundDepth = (int)(Chunk.HeightInBlocks * 0.4f);

            float octave1 = PerlinSimplexNoise.noise(blockX * 0.0001f, blockY * 0.0001f) * 0.5f;
            float octave2 = PerlinSimplexNoise.noise(blockX * 0.0005f, blockY * 0.0005f) * 0.35f;
            float octave3 = PerlinSimplexNoise.noise(blockX * 0.02f, blockY * 0.02f) * 0.15f;
            float lowerGroundHeight = octave1 + octave2 + octave3;

            lowerGroundHeight = lowerGroundHeight * minimumGroundDepth + minimumGroundheight;

            return lowerGroundHeight;
        }
    }
}
