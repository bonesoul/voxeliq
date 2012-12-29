﻿/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Collections.Concurrent;

namespace VoxeliqEngine.Common
{
    /// <summary>
    /// A dictionary objects that accepts dual keys for indexing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleIndexedDictionary<T> : ConcurrentDictionary<long, T>
    {
        private const long RowSize = Int32.MaxValue;

        public T this[int x, int z]
        {
            get
            {
                T @out = default(T);
                TryGetValue(x + (z*RowSize), out @out);
                return @out;
            }
            set { this[x + (z*RowSize)] = value; }
        }

        public bool ContainsKey(int x, int z)
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