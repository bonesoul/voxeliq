/*
 * Copyright (C) 2011 voxlr project 
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using VolumetricStudios.VoxlrEngine.Blocks;
using VolumetricStudios.VoxlrEngine.Utils.Algorithms;

namespace VolumetricStudios.VoxlrEngine.Universe.Terrain.Generators
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
            int minimumGroundheight = Chunk.HeightInBlocks / 20;
            int minimumGroundDepth = (int)(Chunk.HeightInBlocks * 0.2f);

            float octave1 = PerlinSimplexNoise.noise(blockX * 0.0001f, blockY * 0.0001f) * 0.5f;
            float octave2 = PerlinSimplexNoise.noise(blockX * 0.0005f, blockY * 0.0005f) * 0.35f;
            float octave3 = PerlinSimplexNoise.noise(blockX * 0.02f, blockY * 0.02f) * 0.15f;
            float lowerGroundHeight = octave1 + octave2 + octave3;

            lowerGroundHeight = lowerGroundHeight * minimumGroundDepth + minimumGroundheight;

            return lowerGroundHeight;
        }
    }
}
