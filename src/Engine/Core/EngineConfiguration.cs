/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;

namespace VoxeliqEngine.Core
{
    /// <summary>
    /// Holds the configuration parameters to be used by the engine.
    /// </summary>
    public class EngineConfiguration
    {
        /// <summary>
        /// Holds the chunk configuration parameters.
        /// </summary>
        public ChunkConfig ChunkConfiguration { get; private set; }

        /// <summary>
        /// Holds the chunk cache configuration parameters.
        /// </summary>
        public CacheConfig CacheConfiguration { get; private set; }

        /// <summary>
        /// Creates a new instance of engine configuration.
        /// </summary>
        public EngineConfiguration()
        {
            _instance = this;
            this.ChunkConfiguration = new ChunkConfig();
            this.CacheConfiguration = new CacheConfig();
        }

        /// <summary>
        /// Validates the configuration.
        /// </summary>
        /// <returns></returns>
        internal bool Validate()
        {
            if (!this.ChunkConfiguration.Validate())
                return false;

            if (!this.CacheConfiguration.Validate())
                return false;

            return true;
        }

        /// <summary>
        /// Returns the memory instance of EngineConfiguration.
        /// </summary>
        internal static EngineConfiguration _instance { get; set; }

        /// <summary>
        /// Contains configuration parameters for chunks.
        /// </summary>
        public class ChunkConfig
        {
            #region configurable parameters

            /// <summary>
            /// Chunk width in blocks.
            /// </summary>
            public byte WidthInBlocks { get; set; }

            /// <summary>
            /// Chunk height in blocks.
            /// </summary>
            public byte HeightInBlocks { get; set; }

            /// <summary>
            /// Chunk lenght in blocks.
            /// </summary>
            public byte LenghtInBlocks { get; set; }

            #endregion

            #region automatically calculated parameters

            /// <summary>
            /// The chunk volume in blocks.
            /// </summary>
            public int Volume { get; private set; }

            /// <summary>
            /// Maximum width index in blocks for chunk.
            /// </summary>
            public byte MaxWidthInBlocks { get; private set; }

            /// <summary>
            /// Maximum width index in blocks for chunk.
            /// </summary>
            public byte MaxHeightInBlocks { get; private set; }

            /// <summary>
            /// Maximum width index in blocks for chunk.
            /// </summary>
            public byte MaxLenghtInBlocks { get; private set; }

            #endregion

            /// <summary>
            /// Creates a new instance of chunk config.
            /// </summary>
            internal ChunkConfig()
            {
                this.WidthInBlocks = 16;
                this.HeightInBlocks = 128;
                this.LenghtInBlocks = 16;
            }

            /// <summary>
            /// Sets up automatically calculated parameters.
            /// </summary>
            private void Setup()
            {
                this.Volume = WidthInBlocks * HeightInBlocks * LenghtInBlocks;
                this.MaxWidthInBlocks = (byte)(this.WidthInBlocks - 1);
                this.MaxHeightInBlocks = (byte)(this.HeightInBlocks - 1);
                this.MaxLenghtInBlocks = (byte)(this.LenghtInBlocks - 1);
            }

            /// <summary>
            /// Validates the configuration.
            /// </summary>
            /// <returns></returns>
            internal bool Validate()
            {
                this.Setup(); // setup automatically calculated values.

                if (WidthInBlocks == 0)
                    throw new ChunkConfigurationException("Chunk width in blocks can not be set to zero!");

                if (HeightInBlocks == 0)
                    throw new ChunkConfigurationException("Chunk height in blocks can not be set to zero!");

                if (LenghtInBlocks == 0)
                    throw new ChunkConfigurationException("Chunk lenght in blocks can not be set to zero!");

                return true;
            }
        }

        /// <summary>
        /// Contains configuration parameters for chunk-cache.
        /// </summary>
        public class CacheConfig
        {
            #region configurable parameters

            /// <summary>
            /// The amount of chunks in view range.
            /// <remarks>The actual number of chunks in view will be (ViewRange * 2 + 1) </remarks>
            /// </summary>
            public byte ViewRange { get; set; }

            /// <summary>
            /// The amount of chunks in cache range.
            /// <remarks>The actual number of chunks in cache will be (CacheRange * 2 + 1) </remarks>
            /// </summary>
            public byte CacheRange { get; set; }

            /// <summary>
            /// Tells the engine to cache extra chunks than the actual view range.
            /// </summary>
            public bool CacheExtraChunks { get; set; }

            #endregion

            #region automatically calculated parameters

            public int CacheWidthInBlocks { get; private set; }
            public int CacheHeightInBlocks { get; private set; }
            public int CacheLenghtInBlocks { get; private set; }

            #endregion

            /// <summary>
            /// Creates a new instance of cache config.
            /// </summary>
            internal CacheConfig()
            {
                this.CacheExtraChunks = true;
                this.ViewRange = 3;
                this.CacheRange = 5;
            }

            /// <summary>
            /// Sets up automatically calculated parameters.
            /// </summary>
            private void Setup()
            {
                this.CacheWidthInBlocks = (this.CacheRange*2 + 1)* _instance.ChunkConfiguration.WidthInBlocks;
                this.CacheHeightInBlocks = EngineConfiguration._instance.ChunkConfiguration.HeightInBlocks;
                this.CacheLenghtInBlocks = (this.CacheRange * 2 + 1) * _instance.ChunkConfiguration.LenghtInBlocks;
            }

            /// <summary>
            /// Validates the configuration.
            /// </summary>
            /// <returns></returns>
            internal bool Validate()
            {
                this.Setup(); // setup automatically calculated values.

                if (ViewRange == 0)
                    throw new CacheConfigurationException("View range can not be set to zero!");

                if (CacheRange == 0)
                    throw new CacheConfigurationException("Cache range can not be set to zero!");

                if (ViewRange > CacheRange)
                    throw new CacheConfigurationException("View range can not be larger than cache range!");

                if (!CacheExtraChunks && CacheRange != ViewRange)
                    throw new CacheConfigurationException("Cache range can not be different than view range when CacheExtraChunk option is set to false.");

                if (CacheExtraChunks && CacheRange <= ViewRange)
                    throw new CacheConfigurationException("Cache range must be greater view range when CacheExtraChunk option is set to true.");

                return true;
            }
        }
    }

    /// <summary>
    /// Chunk configuration exception.
    /// </summary>
    public class ChunkConfigurationException : Exception
    {
        public ChunkConfigurationException(string message)
            : base(message)
        { }
    }

    /// <summary>
    /// Cache configuration exception.
    /// </summary>
    public class CacheConfigurationException : Exception
    {
        public CacheConfigurationException(string message)
            : base(message)
        { }
    }
}
