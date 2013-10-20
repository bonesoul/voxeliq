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
        public byte LengthInBlocks { get; set; }

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
        public byte MaxLengthInBlocks { get; private set; }

        #endregion

        /// <summary>
        /// Creates a new instance of chunk config.
        /// </summary>
        internal ChunkConfig()
        {
            this.WidthInBlocks = 16;
            this.HeightInBlocks = 128;
            this.LengthInBlocks = 16;
        }

        /// <summary>
        /// Sets up automatically calculated parameters.
        /// </summary>
        private void Setup()
        {
            this.Volume = WidthInBlocks * HeightInBlocks * LengthInBlocks;
            this.MaxWidthInBlocks = (byte)(this.WidthInBlocks - 1);
            this.MaxHeightInBlocks = (byte)(this.HeightInBlocks - 1);
            this.MaxLengthInBlocks = (byte)(this.LengthInBlocks - 1);
        }

        /// <summary>
        /// Validates the configuration.
        /// </summary>
        /// <returns></returns>
        internal bool Validate()
        {
            this.Setup(); // setup automatically calculated values.

            if (WidthInBlocks == 0)
                throw new ChunkConfigException("Chunk width in blocks can not be set to zero!");

            if (HeightInBlocks == 0)
                throw new ChunkConfigException("Chunk height in blocks can not be set to zero!");

            if (LengthInBlocks == 0)
                throw new ChunkConfigException("Chunk lenght in blocks can not be set to zero!");

            return true;
        }
    }

    /// <summary>
    /// Chunk configuration exception.
    /// </summary>
    public class ChunkConfigException : Exception
    {
        public ChunkConfigException(string message)
            : base(message)
        { }
    }
}
