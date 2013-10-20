/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;

namespace Engine.Core.Config
{
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

        public int CacheRangeWidthInBlocks { get; private set; }
        public int CacheRangeHeightInBlocks { get; private set; }
        public int CacheRangeLenghtInBlocks { get; private set; }
        public int ViewRangeWidthInBlocks { get; private set; }
        public int ViewRangeHeightInBlocks { get; private set; }
        public int ViewRangeLenghtInBlocks { get; private set; }
        public int ChunksInCacheRange { get; private set; }
        public int ChunksInViewRange { get; private set; }
        public int CacheRangeVolume { get; private set; }
        public int ViewRangeVolume { get; private set; }

        #endregion

        /// <summary>
        /// Creates a new instance of cache config.
        /// </summary>
        internal CacheConfig()
        {
            // set the defaults.
            this.CacheExtraChunks = true;
            this.ViewRange = 3;
            this.CacheRange = 5;
        }

        /// <summary>
        /// Sets up automatically calculated parameters.
        /// </summary>
        private void Setup()
        {
            this.ChunksInCacheRange = (this.CacheRange * 2 + 1) * (this.CacheRange * 2 + 1);
            this.ChunksInViewRange = (this.ViewRange * 2 + 1) * (this.ViewRange * 2 + 1);

            this.CacheRangeWidthInBlocks = (this.CacheRange * 2 + 1) * Engine.Instance.Configuration.Chunk.WidthInBlocks;
            this.CacheRangeHeightInBlocks = Engine.Instance.Configuration.Chunk.HeightInBlocks;
            this.CacheRangeLenghtInBlocks = (this.CacheRange * 2 + 1) * Engine.Instance.Configuration.Chunk.LenghtInBlocks;
            this.CacheRangeVolume = this.CacheRangeWidthInBlocks * this.CacheRangeHeightInBlocks * this.CacheRangeLenghtInBlocks;

            this.ViewRangeWidthInBlocks = (this.ViewRange * 2 + 1) * Engine.Instance.Configuration.Chunk.WidthInBlocks;
            this.ViewRangeHeightInBlocks = Engine.Instance.Configuration.Chunk.HeightInBlocks;
            this.ViewRangeLenghtInBlocks = (this.ViewRange * 2 + 1) * Engine.Instance.Configuration.Chunk.LenghtInBlocks;
            this.ViewRangeVolume = this.ViewRangeWidthInBlocks * this.ViewRangeHeightInBlocks * this.ViewRangeLenghtInBlocks;
        }

        /// <summary>
        /// Validates the configuration.
        /// </summary>
        /// <returns></returns>
        internal bool Validate()
        {
            this.Setup(); // setup automatically calculated values.

            if (ViewRange == 0)
                throw new CacheConfigException("View range can not be set to zero!");

            if (CacheRange == 0)
                throw new CacheConfigException("Cache range can not be set to zero!");

            if (ViewRange > CacheRange)
                throw new CacheConfigException("View range can not be larger than cache range!");

            if (!CacheExtraChunks && CacheRange != ViewRange)
                throw new CacheConfigException("Cache range can not be different than view range when CacheExtraChunk option is set to false.");

            if (CacheExtraChunks && CacheRange <= ViewRange)
                throw new CacheConfigException("Cache range must be greater view range when CacheExtraChunk option is set to true.");

            return true;
        }
    }

    /// <summary>
    /// Cache configuration exception.
    /// </summary>
    public class CacheConfigException : Exception
    {
        public CacheConfigException(string message)
            : base(message)
        { }
    }
}
