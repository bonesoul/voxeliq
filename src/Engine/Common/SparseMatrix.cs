/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System.Collections.Generic;

namespace Engine.Common
{
    public class SparseMatrix<T>
    {
        // Master dictionary hold rows of column dictionary
        protected Dictionary<uint, Dictionary<uint, T>> _rows;

        /// <summary>
        /// Constructs a SparseMatrix instance.
        /// </summary>
        public SparseMatrix()
        {
            _rows = new Dictionary<uint, Dictionary<uint, T>>();
        }

        /// <summary>
        /// Gets or sets the value at the specified matrix position.
        /// </summary>
        /// <param name="row">Matrix row</param>
        /// <param name="col">Matrix column</param>
        public T this[uint row, uint col]
        {
            get { return GetAt(row, col); }
            set { SetAt(row, col, value); }
        }

        /// <summary>
        /// Gets the value at the specified matrix position.
        /// </summary>
        /// <param name="row">Matrix row</param>
        /// <param name="col">Matrix column</param>
        /// <returns>Value at the specified position</returns>
        public T GetAt(uint row, uint col)
        {
            Dictionary<uint, T> cols;
            if (_rows.TryGetValue(row, out cols))
            {
                T value = default(T);
                if (cols.TryGetValue(col, out value))
                    return value;
            }
            return default(T);
        }

        /// <summary>
        /// Sets the value at the specified matrix position.
        /// </summary>
        /// <param name="row">Matrix row</param>
        /// <param name="col">Matrix column</param>
        /// <param name="value">New value</param>
        public void SetAt(uint row, uint col, T value)
        {
            if (EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                // Remove any existing object if value is default(T)
                RemoveAt(row, col);
            }
            else
            {
                // Set value
                Dictionary<uint, T> cols;
                if (!_rows.TryGetValue(row, out cols))
                {
                    cols = new Dictionary<uint, T>();
                    _rows.Add(row, cols);
                }
                cols[col] = value;
            }
        }

        /// <summary>
        /// Removes the value at the specified matrix position.
        /// </summary>
        /// <param name="row">Matrix row</param>
        /// <param name="col">Matrix column</param>
        public void RemoveAt(uint row, uint col)
        {
            Dictionary<uint, T> cols;
            if (_rows.TryGetValue(row, out cols))
            {
                // Remove column from this row
                cols.Remove(col);
                // Remove entire row if empty
                if (cols.Count == 0)
                    _rows.Remove(row);
            }
        }

        /// <summary>
        /// Returns all items in the specified row.
        /// </summary>
        /// <param name="row">Matrix row</param>
        public IEnumerable<T> GetRowData(uint row)
        {
            Dictionary<uint, T> cols;
            if (_rows.TryGetValue(row, out cols))
            {
                foreach (KeyValuePair<uint, T> pair in cols)
                {
                    yield return pair.Value;
                }
            }
        }

        public void removeRow(uint row)
        {
            _rows.Remove(row);
        }

        public void removeColumn(uint col)
        {
            foreach (KeyValuePair<uint, Dictionary<uint, T>> rowdata in _rows)
            {
                rowdata.Value.Remove(col);
            }
        }

        /// <summary>
        /// Returns the number of items in the specified row.
        /// </summary>
        /// <param name="row">Matrix row</param>
        public int GetRowDataCount(uint row)
        {
            Dictionary<uint, T> cols;
            if (_rows.TryGetValue(row, out cols))
            {
                return cols.Count;
            }
            return 0;
        }

        /// <summary>
        /// Returns all items in the specified column.
        /// This method is less efficent than GetRowData().
        /// </summary>
        /// <param name="col">Matrix column</param>
        /// <returns></returns>
        public IEnumerable<T> GetColumnData(uint col)
        {
            foreach (KeyValuePair<uint, Dictionary<uint, T>> rowdata in _rows)
            {
                T result;
                if (rowdata.Value.TryGetValue(col, out result))
                    yield return result;
            }
        }

        /// <summary>
        /// Returns the number of items in the specified column.
        /// This method is less efficent than GetRowDataCount().
        /// </summary>
        /// <param name="col">Matrix column</param>
        public uint GetColumnDataCount(uint col)
        {
            uint result = 0;

            foreach (KeyValuePair<uint, Dictionary<uint, T>> cols in _rows)
            {
                if (cols.Value.ContainsKey(col))
                    result++;
            }
            return result;
        }
    }
}