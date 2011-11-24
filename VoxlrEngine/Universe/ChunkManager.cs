/*    
 * Copyright (C) 2011, Hüseyin Uslu
 *  
 */

using System.Collections.Generic;
using VolumetricStudios.VoxlrEngine.Utils;
using VolumetricStudios.VoxlrEngine.Utils.Vector;

namespace VolumetricStudios.VoxlrEngine.Universe
{
    /// <summary>
    /// Chunk manager that can load & save chunks to disk.
    /// </summary>
    public class ChunkManager
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
