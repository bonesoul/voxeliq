/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

// Original code by: http://www.deanchalk.me.uk/post/Task-Parallel-Concurrent-List-Implementation.aspx

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Engine.Common.Helpers
{
    public class ConcurrentList<T> : IList<T>, IList
    {
        private readonly List<T> underlyingList = new List<T>();
        private readonly object syncRoot = new object();
        private readonly ConcurrentQueue<T> underlyingQueue;
        private bool requiresSync;
        private bool isDirty;

        public ConcurrentList()
        {
            underlyingQueue = new ConcurrentQueue<T>();
        }

        public ConcurrentList(IEnumerable<T> items)
        {
            underlyingQueue = new ConcurrentQueue<T>(items);
        }

        private void UpdateLists()
        {
            if (!isDirty)
                return;
            lock (syncRoot)
            {
                requiresSync = true;
                T temp;
                while (underlyingQueue.TryDequeue(out temp))
                    underlyingList.Add(temp);
                requiresSync = false;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (syncRoot)
            {
                UpdateLists();
                return underlyingList.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            if (requiresSync)
                lock (syncRoot)
                    underlyingQueue.Enqueue(item);
            else
                underlyingQueue.Enqueue(item);
            isDirty = true;
        }

        public int Add(object value)
        {
            if (requiresSync)
                lock (syncRoot)
                    underlyingQueue.Enqueue((T)value);
            else
                underlyingQueue.Enqueue((T)value);
            isDirty = true;
            lock (syncRoot)
            {
                UpdateLists();
                return underlyingList.IndexOf((T)value);
            }
        }

        public bool Contains(object value)
        {
            lock (syncRoot)
            {
                UpdateLists();
                return underlyingList.Contains((T)value);
            }
        }

        public int IndexOf(object value)
        {
            lock (syncRoot)
            {
                UpdateLists();
                return underlyingList.IndexOf((T)value);
            }
        }

        public void Insert(int index, object value)
        {
            lock (syncRoot)
            {
                UpdateLists();
                underlyingList.Insert(index, (T)value);
            }
        }

        public void Remove(object value)
        {
            lock (syncRoot)
            {
                UpdateLists();
                underlyingList.Remove((T)value);
            }
        }

        public void RemoveAt(int index)
        {
            lock (syncRoot)
            {
                UpdateLists();
                underlyingList.RemoveAt(index);
            }
        }

        T IList<T>.this[int index]
        {
            get
            {
                lock (syncRoot)
                {
                    UpdateLists();
                    return underlyingList[index];
                }
            }
            set
            {
                lock (syncRoot)
                {
                    UpdateLists();
                    underlyingList[index] = value;
                }
            }
        }

        object IList.this[int index]
        {
            get { return ((IList<T>)this)[index]; }
            set { ((IList<T>)this)[index] = (T)value; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public void Clear()
        {
            lock (syncRoot)
            {
                UpdateLists();
                underlyingList.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (syncRoot)
            {
                UpdateLists();
                return underlyingList.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (syncRoot)
            {
                UpdateLists();
                underlyingList.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            lock (syncRoot)
            {
                UpdateLists();
                return underlyingList.Remove(item);
            }
        }

        public void CopyTo(Array array, int index)
        {
            lock (syncRoot)
            {
                UpdateLists();
                underlyingList.CopyTo((T[])array, index);
            }
        }

        public int Count
        {
            get
            {
                lock (syncRoot)
                {
                    UpdateLists();
                    return underlyingList.Count;
                }
            }
        }

        public object SyncRoot
        {
            get { return syncRoot; }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public int IndexOf(T item)
        {
            lock (syncRoot)
            {
                UpdateLists();
                return underlyingList.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (syncRoot)
            {
                UpdateLists();
                underlyingList.Insert(index, item);
            }
        }
    }
}