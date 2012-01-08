/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using System.Collections.Generic;
using VolumetricStudios.VoxeliqEngine.Utils;
using VolumetricStudios.VoxeliqEngine.Utils.Vector;

namespace VolumetricStudios.VoxeliqEngine.Chunks
{
    /// <summary>
    /// Chunk manager that can load & save chunks to disk.
    /// </summary>
    public class ChunkStorage
    {
        private readonly DoubleIndexedDictionary<Chunk> _dictionary = new DoubleIndexedDictionary<Chunk>();

        public Vector2Int SouthWestEdge;
        public Vector2Int NorthEastEdge;

        public Chunk this[int x, int z]
        {
            get { return this._dictionary[x, z]; }
            set { this._dictionary[x, z] = value; }
        }

        public Chunk Remove(int x, int z)
        {
            return this._dictionary.Remove(x, z);
        }

        public bool ContainsKey(int x, int z)
        {
            return this._dictionary.ContainsKey(x, z);
        }

        public int Count
        {
            get { return this._dictionary.Count; }
        }

        public IEnumerable<Chunk> Values
        {
            get { return this._dictionary.Values; }
        }
    }
}
