/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Vector;
using Microsoft.Xna.Framework;

namespace Engine.Chunks
{
    /// <summary>
    /// Allows interaction with the chunk storage service.
    /// </summary>
    public interface IChunkStorage
    {
        /// <summary>
        /// Returns the chunk in given x-z coordinate.
        /// </summary>
        /// <param name="x">The x coordinate of the chunk.</param>
        /// <param name="z">The z coordinate of the chunk.</param>
        /// <returns><see cref="Chunk"/></returns>
        Chunk this[int x, int z] { get; set; }

        /// <summary>
        /// Removes the chunk at given x-z coordinate.
        /// </summary>
        /// <param name="x">The x coordinate of the chunk.</param>
        /// <param name="z">The z coordinate of the chunk.</param>
        /// <returns><see cref="Chunk"/></returns>
        Chunk Remove(int x, int z);

        /// <summary>
        /// Returns true if a chunk exists at given x-z coordinate.
        /// </summary>
        /// <param name="x">The x coordinate of the chunk.</param>
        /// <param name="z">The z coordinate of the chunk.</param>
        /// <returns><see cref="bool"/></returns>
        bool ContainsKey(int x, int z);

        /// <summary>
        /// Returns total count of chunk stored.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns an enumarable list of chunks stored.
        /// </summary>
        IEnumerable<Chunk> Values { get; }
    }

    /// <summary>
    /// Chunk storage that stores chunks within memory and can load & save chunks to disk.
    /// </summary>
    public class ChunkStorage : GameComponent, IChunkStorage
    {
        private readonly DoubleIndexedDictionary<Chunk> _dictionary = new DoubleIndexedDictionary<Chunk>();

        /// <summary>
        /// South west edge of the current chunk storage.
        /// </summary>
        public Vector2Int SouthWestEdge;

        /// <summary>
        /// North east edge of the current chunk storage.
        /// </summary>
        public Vector2Int NorthEastEdge;

        /// <summary>
        /// Creates a new chunk storage instance which can hold chunks.
        /// </summary>
        /// <param name="game"></param>
        public ChunkStorage(Game game)
            : base(game)
        {
            this.Game.Services.AddService(typeof (IChunkStorage), this); // export service.
        }

        /// <summary>
        /// Returns the chunk in given x-z coordinate.
        /// </summary>
        /// <param name="x">The x coordinate of the chunk.</param>
        /// <param name="z">The z coordinate of the chunk.</param>
        /// <returns><see cref="Chunk"/></returns>
        public Chunk this[int x, int z]
        {
            get { return this._dictionary[x, z]; }
            set { this._dictionary[x, z] = value; }
        }

        /// <summary>
        /// Removes the chunk at given x-z coordinate.
        /// </summary>
        /// <param name="x">The x coordinate of the chunk.</param>
        /// <param name="z">The z coordinate of the chunk.</param>
        /// <returns><see cref="Chunk"/></returns>
        public Chunk Remove(int x, int z)
        {
            return this._dictionary.Remove(x, z);
        }

        /// <summary>
        /// Returns true if a chunk exists at given x-z coordinate.
        /// </summary>
        /// <param name="x">The x coordinate of the chunk.</param>
        /// <param name="z">The z coordinate of the chunk.</param>
        /// <returns><see cref="bool"/></returns>
        public bool ContainsKey(int x, int z)
        {
            return this._dictionary.ContainsKey(x, z);
        }

        /// <summary>
        /// Returns total count of chunk stored.
        /// </summary>
        public int Count
        {
            get { return this._dictionary.Count; }
        }

        /// <summary>
        /// Returns an enumarable list of chunks stored.
        /// </summary>
        public IEnumerable<Chunk> Values
        {
            get { return this._dictionary.Values; }
        }
    }
}