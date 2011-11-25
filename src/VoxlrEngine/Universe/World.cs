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

using Microsoft.Xna.Framework;
using VolumetricStudios.VoxlrEngine.Screen;
using VolumetricStudios.VoxlrEngine.Utils.Vector;

namespace VolumetricStudios.VoxlrEngine.Universe
{
    /// <summary>
    /// World service interface.
    /// </summary>
    public interface IWorldService
    {
        ChunkManager Chunks { get; }
        //void ToggleFog();
        void ToggleInfinitiveWorld();
        void SetBlock(int x, int y, int z, Block block);
        void SetBlock(Vector3Int position, Block block);
        Chunk GetChunk(int x, int z);
    }

    /// <summary>
    /// World.
    /// </summary>
    public class World : IWorldService
    {
        /// <summary>
        /// fog vectors.
        /// </summary>
        protected readonly Vector2[] _fogVectors = new[] {new Vector2(0, 0), new Vector2(175, 250), new Vector2(250, 400)};

        /// <summary>
        /// Chunk manager
        /// </summary>
        public ChunkManager Chunks { get; protected set; }

        /// <summary>
        /// Bounding box for the world.
        /// </summary>
        public BoundingBox BoundingBox { get; set; }

        /// <summary>
        /// View range for the world.
        /// </summary>
        public const byte ViewRange = 6;

        /// <summary>
        /// The camera service.
        /// </summary>
        public ICameraService Camera;

        /// <summary>
        /// Chunks drawn statistics.
        /// </summary>
        public int ChunksDrawn { get; protected set; } // chunks drawn statistics.

        /// <summary>
        /// Total chunks
        /// </summary>
        public int TotalChunks { get { return this.Chunks.Count; } }

        /// <summary>
        /// Is the world infinitive?
        /// </summary>
        public bool IsInfinitive { get; private set; }
        
        /// <summary>
        /// TODO: shouldn't be getting game object really and abstract of game.
        /// </summary>
        /// <param name="game"></param>
        public World(bool isInfinitive)
        {
            this.IsInfinitive = isInfinitive;
        }

        public void ToggleInfinitiveWorld()
        {
            this.IsInfinitive = !this.IsInfinitive;
        }

        public bool IsInBounds(int x, int y, int z)
        {
            if (x < this.BoundingBox.Min.X || z < this.BoundingBox.Min.Z || x >= this.BoundingBox.Max.X || z >= this.BoundingBox.Max.Z || y < this.BoundingBox.Min.Y || y >= this.BoundingBox.Max.Y) return false;
            return true;
        }

        public Block BlockAt(Vector3 position)
        {
            return BlockAt((int)position.X, (int)position.Y, (int)position.Z);
        }

        public Block BlockAt(int x, int y, int z)
        {
            if (!IsInBounds(x, y, z)) return Block.Empty;

            if (!this.Chunks.ContainsKey(x / Chunk.WidthInBlocks, z / Chunk.LenghtInBlocks)) 
                return Block.Empty;

            return this.Chunks[x/Chunk.WidthInBlocks, z/Chunk.LenghtInBlocks].BlockAt(x%Chunk.WidthInBlocks, y, z%Chunk.LenghtInBlocks);
        }

        public void SetBlock(Vector3Int position, Block block)
        {
            this.SetBlock(position.X,position.Y,position.Z,block);
        }

        public void SetBlock(int x, int y, int z, Block block)
        {
            var chunk = GetChunk(x, z);
            chunk.SetBlock((byte) (x%Chunk.WidthInBlocks), (byte) y, (byte) (z%Chunk.LenghtInBlocks), block);
        }

        public Chunk GetChunk(int x, int z)
        {
            return !this.Chunks.ContainsKey(x / Chunk.WidthInBlocks, z / Chunk.LenghtInBlocks) ? null : this.Chunks[x / Chunk.WidthInBlocks, z / Chunk.LenghtInBlocks];
        }
    }
}
