/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using System;
using System.Collections.Concurrent;

namespace VolumetricStudios.VoxeliqEngine.Utils
{
    public class DoubleIndexedDictionary<T> : ConcurrentDictionary<long, T>
    {
        const long RowSize = Int32.MaxValue;

        public T this[int x, int z]
        {
            get
            {
                T @out= default(T);
                TryGetValue(x + (z*RowSize), out @out);
                return @out;
            }
            set { this[x + (z*RowSize)] = value; }
        }

        public bool ContainsKey(int x,int z)
        {
            return ContainsKey(x + (z*RowSize));
        }

        public T Remove(int x, int z)
        {
            T removed;
            TryRemove(x + (z*RowSize), out removed);
            return removed;
        }
    }
}
