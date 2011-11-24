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

using System;
using System.Collections.Concurrent;

namespace VolumetricStudios.VoxlrEngine.Utils
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
