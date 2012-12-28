﻿/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using VoxeliqEngine.Common;
using VoxeliqEngine.Common.Vector;

namespace VoxeliqEngine.Chunks
{
    /// <summary>
    /// Allows interaction with the chunk storage service.
    /// </summary>
    public interface IChunkStorage
    {
        /// <summary>
        /// Returns the chunk in given x-z coordinate.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        Chunk this[int x, int z] { get; set; }

        /// <summary>
        /// Removes the chunk at given x-z coordinate.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        Chunk Remove(int x, int z);

        /// <summary>
        /// Returns true if a chunk exists at given x-z coordinate.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
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
    /// Chunk storage that stores chunks over memory and can load & save chunks to disk.
    /// </summary>
    public class ChunkStorage : GameComponent, IChunkStorage
    {
        /// <summary>
        /// The memory instance of ChunkStorage.
        /// </summary>
        private static ChunkStorage _instance;

        /// <summary>
        /// The memory instance of ChunkStorage.
        /// </summary>
        public static ChunkStorage Instance
        {
            get { return _instance; }
        }


        private readonly DoubleIndexedDictionary<Chunk> _dictionary = new DoubleIndexedDictionary<Chunk>();

        public Vector2Int SouthWestEdge;
        public Vector2Int NorthEastEdge;

        public ChunkStorage(Game game)
            : base(game)
        {
            _instance = this;
            this.Game.Services.AddService(typeof (IChunkStorage), this); // export service.
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        // Returns the chunk in given x-z coordinate.
        public Chunk this[int x, int z]
        {
            get { return this._dictionary[x, z]; }
            set { this._dictionary[x, z] = value; }
        }

        // Removes the chunk at given x-z coordinate.
        public Chunk Remove(int x, int z)
        {
            return this._dictionary.Remove(x, z);
        }

        // Returns true if a chunk exists at given x-z coordinate.
        public bool ContainsKey(int x, int z)
        {
            return this._dictionary.ContainsKey(x, z);
        }

        // Returns total count of chunk stored.
        public int Count
        {
            get { return this._dictionary.Count; }
        }

        // Returns an enumarable list of chunks stored.
        public IEnumerable<Chunk> Values
        {
            get { return this._dictionary.Values; }
        }
    }
}