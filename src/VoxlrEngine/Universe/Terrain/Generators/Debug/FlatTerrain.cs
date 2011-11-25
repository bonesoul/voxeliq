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

namespace VolumetricStudios.VoxlrEngine.Universe.Terrain.Generators.Debug
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
