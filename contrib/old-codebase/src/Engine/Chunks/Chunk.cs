/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Collections.Generic;
using Engine.Blocks;
using Engine.Common.Vector;
using Engine.Debugging.Ingame;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Chunks
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
        public static byte WidthInBlocks = Core.Engine.Instance.Configuration.Chunk.WidthInBlocks;

        /// <summary>
        /// Maximum width index in blocks for chunk.
        /// </summary>
        public static byte MaxWidthIndexInBlocks = Core.Engine.Instance.Configuration.Chunk.MaxWidthInBlocks;

        /// <summary>
        /// Chunk length in blocks
        /// </summary>
        public static byte LengthInBlocks = Core.Engine.Instance.Configuration.Chunk.LengthInBlocks;

        /// <summary>
        /// Maximum length index in blocks for chunk.
        /// </summary>
        public static byte MaxLenghtIndexInBlocks = Core.Engine.Instance.Configuration.Chunk.MaxLengthInBlocks;

        /// <summary>
        /// Chunk height in blocks.
        /// </summary>
        public static byte HeightInBlocks = Core.Engine.Instance.Configuration.Chunk.HeightInBlocks;

        /// <summary>
        /// Maximum height index in blocks for chunk.
        /// </summary>
        public static byte MaxHeightIndexInBlocks = Core.Engine.Instance.Configuration.Chunk.MaxHeightInBlocks;

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
        /// Creates a new chunk instance.
        /// </summary>
        /// <param name="relativePosition">The relative position of the chunk.</param>
        public Chunk(Vector2Int relativePosition)
        {
            this.ChunkState = ChunkState.AwaitingGenerate; // set initial state to awaiting generation.
            this.RelativePosition = relativePosition; // set the relative position.

            // calculate the real world position.
            this.WorldPosition = new Vector2Int(this.RelativePosition.X*WidthInBlocks,
                                                this.RelativePosition.Z * LengthInBlocks);

            // calculate bounding-box.
            this.BoundingBox = new BoundingBox(new Vector3(WorldPosition.X, 0, WorldPosition.Z),
                                               new Vector3(this.WorldPosition.X + WidthInBlocks, HeightInBlocks,
                                                           this.WorldPosition.Z + LengthInBlocks));

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

        public void CalculateHeightIndexes()
        {
            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                var worldPositionX = this.WorldPosition.X + x;

                for (byte z = 0; z < Chunk.LengthInBlocks; z++)
                {
                    int worldPositionZ = this.WorldPosition.Z + z;

                    var offset = BlockStorage.BlockIndexByWorldPosition(worldPositionX, worldPositionZ);
                    for (int y = Chunk.MaxHeightIndexInBlocks; y >= 0; y--)
                    {
                        if ((y > this.HighestSolidBlockOffset) && (BlockStorage.Blocks[offset + y].Exists))
                        {
                            this.HighestSolidBlockOffset = (byte)y;
                        }
                        else if ((this.LowestEmptyBlockOffset > y) && (!BlockStorage.Blocks[offset + y].Exists))
                        {
                            this.LowestEmptyBlockOffset = (byte)y;
                        }
                    }
                }
            }

            this.LowestEmptyBlockOffset--;
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
                                                            Matrix.CreateTranslation(new Vector3(WorldPosition.X + WidthInBlocks / 2, HighestSolidBlockOffset - 1, WorldPosition.Z + LengthInBlocks / 2)));

            spriteBatch.DrawString(spriteFont, position, new Vector2(projected.X - positionSize.X/2, projected.Y - positionSize.Y/2), Color.Yellow);

            BoundingBoxRenderer.Render(this.BoundingBox, graphicsDevice, camera.View, camera.Projection, Color.DarkRed);
        }

        #endregion

        #region de-ctor

        // IDisposable pattern: http://msdn.microsoft.com/en-us/library/fs2xkftw(v=VS.100).aspx

        /// <summary>
        /// Is the region disposed already?
        /// </summary>
        public bool Disposed = false;

        private void Dispose(bool disposing)
        {
            if (this.Disposed) 
                return; // if already disposed, just return

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

        public void Dispose()
        {
            Dispose(true); // Object being disposed by the code itself, dispose both managed and unmanaged objects.
            GC.SuppressFinalize(this); // Take object out the finalization queue to prevent finalization code for it from executing a second time.
        }

        ~Chunk() { Dispose(false); } // finalizer called by the runtime. we should only dispose unmanaged objects and should NOT reference managed ones. 

        #endregion

        public enum Edges
        {
            /// <summary>
            /// Left edge
            /// </summary>
            XDecreasing = 0,
            /// <summary>
            /// Right edge
            /// </summary>
            XIncreasing = 1,
            /// <summary>
            /// Front edge
            /// </summary>
            ZDecreasing = 2,
            /// <summary>
            /// Backwards edge
            /// </summary>
            ZIncreasing = 3
        }
    }
}