/*
 * Copyright (C) 2011 - Hüseyin Uslu shalafiraistlin@gmail.com
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
