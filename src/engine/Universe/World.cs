/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxeliqEngine.Blocks;
using VolumetricStudios.VoxeliqEngine.Chunks;
using VolumetricStudios.VoxeliqEngine.Utils.Vector;

namespace VolumetricStudios.VoxeliqEngine.Universe
{
    /// <summary>
    /// World service interface.
    /// </summary>
    public interface IWorldService
    {
        ChunkStorage Chunks { get; }
        //void ToggleFog();
        void ToggleInfinitiveWorld();
        void SetBlock(int x, int y, int z, Block block);
        void SetBlock(Vector3Int position, Block block);
        Chunk GetChunk(int x, int z);
    }

    /// <summary>
    /// World.
    /// </summary>
    public class World : DrawableGameComponent, IWorldService
    {
        public ChunkStorage Chunks { get; private set; }
        public ChunkCache ChunkCache { get; private set; }

        /// <summary>
        /// Bounding box for the world.
        /// </summary>
        public BoundingBox BoundingBox { get; set; }

        /// <summary>
        /// View range for the world.
        /// </summary>
        public const byte ViewRange = 6;

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
        /// Creates a new world.
        /// </summary>
        /// <param name="game"> </param>
        /// <param name="isInfinitive"></param>
        /// <param name="chunkStorage"> </param>
        /// <param name="chunkCache"> </param>
        public World(Game game, bool isInfinitive, ChunkStorage chunkStorage, ChunkCache chunkCache)
            :base(game)
        {
            this.Chunks = chunkStorage;
            this.ChunkCache = chunkCache;
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
