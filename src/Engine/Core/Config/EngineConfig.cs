﻿/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;

namespace VoxeliqEngine.Core.Config
{
    /// <summary>
    /// Holds the configuration parameters to be used by the engine.
    /// </summary>
    public class EngineConfig
    {
        /// <summary>
        /// Holds the chunk configuration parameters.
        /// </summary>
        public ChunkConfig Chunk { get; private set; }

        /// <summary>
        /// Holds the chunk cache configuration parameters.
        /// </summary>
        public CacheConfig Cache { get; private set; }

        /// <summary>
        /// Holds graphics related configuration parameters.
        /// </summary>
        public GraphicsConfig Graphics { get; private set; }

        /// <summary>
        /// Holds the audio related configuration parameters.
        /// </summary>
        public AudioConfig Audio { get; private set; }

        /// <summary>
        /// Creates a new instance of engine configuration.
        /// </summary>
        public EngineConfig()
        {
            this.Chunk = new ChunkConfig();
            this.Cache = new CacheConfig();
            this.Graphics = new GraphicsConfig();
            this.Audio = new AudioConfig();
        }

        /// <summary>
        /// Validates the configuration.
        /// </summary>
        /// <returns></returns>
        internal bool Validate()
        {
            if (!this.Chunk.Validate())
                return false;

            if (!this.Cache.Validate())
                return false;

            if (!this.Graphics.Validate())
                return false;

            if (!this.Audio.Validate())
                return false;

            return true;
        }           
    }
}