/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

namespace Engine.Core.Config
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
        /// Holds the world related configuration parameters.
        /// </summary>
        public WorldConfig World { get; private set; }

        /// <summary>
        /// Holds the debugging related configuration parameters.
        /// </summary>
        public DebugConfig Debugging { get; private set; }

        /// <summary>
        /// Holds the bloom related configuration parameters.
        /// </summary>
        public BloomConfig Bloom { get; private set; }

        /// <summary>
        /// Creates a new instance of engine configuration.
        /// </summary>
        public EngineConfig()
        {
            this.Chunk = new ChunkConfig();
            this.Cache = new CacheConfig();
            this.Graphics = new GraphicsConfig();
            this.Audio = new AudioConfig();
            this.World = new WorldConfig();
            this.Debugging = new DebugConfig();
            this.Bloom = new BloomConfig();
        }

        /// <summary>
        /// Validates the configuration.
        /// </summary>
        /// <returns></returns>
        internal bool Validate()
        {
            // valide all the subconfigurations. 
            if (!this.Chunk.Validate())
                return false;

            if (!this.Cache.Validate())
                return false;

            if (!this.Graphics.Validate())
                return false;

            if (!this.Audio.Validate())
                return false;

            if (!this.World.Validate())
                return false;

            if (!this.Debugging.Validate())
                return false;

            if (!this.Bloom.Validate())
                return false;

            return true;
        }           
    }
}
