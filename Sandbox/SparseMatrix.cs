using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blocksGame.VoxelEngine
{
    //http://www.blackbeltcoder.com/Articles/algorithms/creating-a-sparse-matrix-in-net
    public class SparseMatrix<T>
    {
        // Master dictionary hold rows of column dictionary
        protected Dictionary<int, Dictionary<int, T>> _rows;

        /// <summary>
        /// Constructs a SparseMatrix instance.
        /// </summary>
        public SparseMatrix()
        {
            _rows = new Dictionary<int, Dictionary<int, T>>();
        }

        /// <summary>
        /// Gets or sets the value at the specified matrix position.
        /// </summary>
        /// <param name="row">Matrix row</param>
        /// <param name="col">Matrix column</param>
        public T this[int row, int col]
        {
            get
            {
                return GetAt(row, col);
            }
            set
            {
                SetAt(row, col, value);
            }
        }

        /// <summary>
        /// Gets the value at the specified matrix position.
        /// </summary>
        /// <param name="row">Matrix row</param>
        /// <param name="col">Matrix column</param>
        /// <returns>Value at the specified position</returns>
        public T GetAt(int row, int col)
        {
            Dictionary<int, T> cols;
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
        public void SetAt(int row, int col, T value)
        {
            if (EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                // Remove any existing object if value is default(T)
                RemoveAt(row, col);
            }
            else
            {
                // Set value
                Dictionary<int, T> cols;
                if (!_rows.TryGetValue(row, out cols))
                {
                    cols = new Dictionary<int, T>();
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
        public void RemoveAt(int row, int col)
        {
            Dictionary<int, T> cols;
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
        public IEnumerable<T> GetRowData(int row)
        {
            Dictionary<int, T> cols;
            if (_rows.TryGetValue(row, out cols))
            {
                foreach (KeyValuePair<int, T> pair in cols)
                {
                    yield return pair.Value;
                }
            }
        }

        /// <summary>
        /// Returns the number of items in the specified row.
        /// </summary>
        /// <param name="row">Matrix row</param>
        public int GetRowDataCount(int row)
        {
            Dictionary<int, T> cols;
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
        public IEnumerable<T> GetColumnData(int col)
        {
            foreach (KeyValuePair<int, Dictionary<int, T>> rowdata in _rows)
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
        public int GetColumnDataCount(int col)
        {
            int result = 0;

            foreach (KeyValuePair<int, Dictionary<int, T>> cols in _rows)
            {
                if (cols.Value.ContainsKey(col))
                    result++;
            }
            return result;
        }
    }
}
