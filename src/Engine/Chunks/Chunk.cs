﻿/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxeliqEngine.Blocks;
using VoxeliqEngine.Common.Vector;
using VoxeliqEngine.Core;
using VoxeliqEngine.Debugging;
using VoxeliqEngine.Debugging.Ingame;
using VoxeliqEngine.Graphics;
using VoxeliqEngine.Universe;

namespace VoxeliqEngine.Chunks
{
    /// <summary>
    /// Unit drawable chunk of blocks.
    /// </summary>
    public sealed class Chunk : IInGameDebuggable
    {
        /// <summary>
        /// Maximum sun value.
        /// </summary>
        public static byte MaxSunValue = 16;

        /// <summary>
        /// Chunk width in blocks.
        /// </summary>
        public static byte WidthInBlocks = Engine.Instance.Configuration.ChunkConfiguration.WidthInBlocks;

        /// <summary>
        /// Chunk lenght in blocks
        /// </summary>
        public static byte LenghtInBlocks = Engine.Instance.Configuration.ChunkConfiguration.LenghtInBlocks;

        /// <summary>
        /// Chunk height in blocks.
        /// </summary>
        public static byte HeightInBlocks = Engine.Instance.Configuration.ChunkConfiguration.HeightInBlocks;

        /// <summary>
        /// Maximum height index in blocks for chunk.
        /// </summary>
        public static byte MaxHeightIndexInBlocks = Engine.Instance.Configuration.ChunkConfiguration.MaxHeightInBlocks;

        /// <summary>
        /// The chunks world position.
        /// </summary>
        public Vector2Int WorldPosition { get; private set; }

        /// <summary>
        /// The chunks relative position.
        /// </summary>
        public Vector2Int RelativePosition { get; private set; }

        /// <summary>
        /// The bounding box for the chunk.
        /// </summary>
        public BoundingBox BoundingBox { get; set; }

        /// <summary>
        /// The chunk state.
        /// </summary>
        public ChunkState ChunkState { get; set; }

        // note: we keep track of highset solid blocks offset and lowest empty block offset
        // so that we can only generate the mesh for those blocks.

        /// <summary>
        /// Highest solid block's offset in chunk.
        /// </summary>
        public byte HighestSolidBlockOffset;

        /// <summary>
        /// Lowest empty block's offset in chunk.
        /// </summary>
        public byte LowestEmptyBlockOffset = HeightInBlocks;

        /// <summary>
        /// Vertex buffer for chunk's blocks.
        /// </summary>
        public VertexBuffer VertexBuffer { get; set; }

        /// <summary>
        /// Index buffer for chunk's blocks.
        /// </summary>
        public IndexBuffer IndexBuffer { get; set; }

        /// <summary>
        /// The vertex list.
        /// </summary>
        public List<BlockVertex> VertexList;

        /// <summary>
        /// The index list.
        /// </summary>
        public List<short> IndexList;

        /// <summary>
        /// TODO: fix comment.
        /// </summary>
        public short Index;

        /// <summary>
        /// Is the region disposed already?
        /// </summary>
        public bool Disposed = false;

        /// <summary>
        /// Creates a new chunk instance.
        /// </summary>
        /// <param name="relativePosition">The relative position of the chunk.</param>
        public Chunk(Vector2Int relativePosition)
        {
            this.ChunkState = ChunkState.AwaitingGenerate; // set initial state to awaiting generation.
            this.RelativePosition = relativePosition; // set the relative position.

            // calculate the real world position.
            this.WorldPosition = new Vector2Int(this.RelativePosition.X*WidthInBlocks,
                                                this.RelativePosition.Z*LenghtInBlocks);

            // calculate bounding-box.
            this.BoundingBox = new BoundingBox(new Vector3(WorldPosition.X, 0, WorldPosition.Z),
                                               new Vector3(this.WorldPosition.X + WidthInBlocks, HeightInBlocks,
                                                           this.WorldPosition.Z + LenghtInBlocks));

            // create vertex & index lists.
            this.VertexList = new List<BlockVertex>();
            this.IndexList = new List<short>();
        }

        /// <summary>
        /// Returns if given coordinates are in chunk's bounds.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public bool IsInBounds(float x, float z)
        {
            if (x < this.BoundingBox.Min.X || z < this.BoundingBox.Min.Z || x >= this.BoundingBox.Max.X ||
                z >= this.BoundingBox.Max.Z) return false;

            return true;
        }

        /// <summary>
        /// Gets a block by given world position.
        /// </summary>
        /// <param name="x">Block's x world position.</param>
        /// <param name="y">Block's y world position.</param>
        /// <param name="z">Block's z world position.</param>
        /// <returns>Copy of <see cref="Block"/></returns>
        public Block BlockAt(int x, int y, int z)
        {
            return BlockStorage.BlockAt(this.WorldPosition.X + x, y, this.WorldPosition.Z + z);
        }

        /// <summary>
        /// Gets a block by given world position.
        /// </summary>
        /// <param name="x">Block's x world position.</param>
        /// <param name="y">Block's y world position.</param>
        /// <param name="z">Block's z world position.</param>
        /// <returns>Copy of <see cref="Block"/></returns>
        /// <remarks>As <see cref="Block"/> is a struct, the returned block will be a copy of original one.</remarks>
        /// <remarks>This method will not check if given point/block coordinates are in chunk-cache's bounds. If you need a reliable & safe way, use <see cref="BlockAt"/> instead.</remarks>
        public Block FastBlockAt(int x, int y, int z)
        {
            return BlockStorage.FastBlockAt(this.WorldPosition.X + x, y, this.WorldPosition.Z + z);
        }

        /// <summary>
        /// Sets a block by given relative position.
        /// </summary>
        /// <param name="x">Block's x world position.</param>
        /// <param name="y">Block's y world position.</param>
        /// <param name="z">Block's z world position.</param>
        /// <param name="block">Block to set.</param>        
        /// <remarks>This method will not check if given point/block coordinates are in chunk-cache's bounds. If you need a reliable & safe way, use <see cref="SetBlockAt"/> instead.</remarks>
        public void FastSetBlockAt(sbyte x, sbyte y, sbyte z, Block block)
        {
            if (x < 0)
                x += (sbyte)Chunk.WidthInBlocks;
            if (z < 0)
                z += (sbyte) Chunk.LenghtInBlocks;

            switch (block.Exists)
            {
                case false:
                    if (this.LowestEmptyBlockOffset > y && y > 0)
                        this.LowestEmptyBlockOffset = (byte) (y - 1);
                    break;
                case true:
                    if (y > this.HighestSolidBlockOffset && y < MaxHeightIndexInBlocks)
                        this.HighestSolidBlockOffset = (byte) (y + 1);
                    break;
            }

            BlockStorage.FastSetBlockAt(this.WorldPosition.X + x, y, this.WorldPosition.Z + z, block);
            this.ChunkState = ChunkState.AwaitingRelighting;
        }

        /// <summary>
        /// Returns a string that represents chunks relative position and state.
        /// </summary>      
        /// <returns><see cref="String"/></returns>
        /// <remarks>Used by the Visual Studio debugger.</remarks>
        public override string ToString()
        {
            return string.Format("{0} {1}", RelativePosition, ChunkState);
        }

        #region ingame debugger

        /// <summary>
        /// Draws ingame chunk debugger.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="camera"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="spriteFont"></param>
        public void DrawInGameDebugVisual(GraphicsDevice graphicsDevice, ICamera camera, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            var position = RelativePosition + " " + this.ChunkState;
            var positionSize = spriteFont.MeasureString(position);

            var projected = graphicsDevice.Viewport.Project(Vector3.Zero, camera.Projection, camera.View,
                                                            Matrix.CreateTranslation(new Vector3(WorldPosition.X + WidthInBlocks/2, HighestSolidBlockOffset - 1, WorldPosition.Z + LenghtInBlocks/2)));

            spriteBatch.DrawString(spriteFont, position, new Vector2(projected.X - positionSize.X/2, projected.Y - positionSize.Y/2), Color.Yellow);

            BoundingBoxRenderer.Render(this.BoundingBox, graphicsDevice, camera.View, camera.Projection, Color.DarkRed);
        }

        #endregion

        #region de-ctor

        // IDisposable pattern: http://msdn.microsoft.com/en-us/library/fs2xkftw(v=VS.100).aspx

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
                // Take object out the finalization queue to prevent finalization code for it from executing a second time.
        }

        private void Dispose(bool disposing)
        {
            if (this.Disposed) return; // if already disposed, just return

            if (disposing) // only dispose managed resources if we're called from directly or in-directly from user code.
            {
                this.IndexList.Clear();
                this.IndexList = null;
                this.VertexList.Clear();
                this.VertexList = null;

                if (this.VertexBuffer != null)
                    this.VertexBuffer.Dispose();

                if (this.IndexBuffer != null)
                    this.IndexBuffer.Dispose();
            }

            Disposed = true;
        }

        ~Chunk() { Dispose(false); } // finalizer called by the runtime. we should only dispose unmanaged objects and should NOT reference managed ones. 

        #endregion
    }
}