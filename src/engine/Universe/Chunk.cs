/*
 * Copyright (C) 2011 voxeliq project 
 *
 */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxeliqEngine.Blocks;
using VolumetricStudios.VoxeliqEngine.Screen;
using VolumetricStudios.VoxeliqEngine.Utils.Vector;

namespace VolumetricStudios.VoxeliqEngine.Universe
{
    /// <summary>
    /// Unit drawable chunk of blocks.
    /// </summary>
    public sealed class Chunk : IInGameDebuggable
    {
        public Chunk North { get { return World.Chunks[this.RelativePosition.X, this.RelativePosition.Z + 1]; } }
        public Chunk South { get { return World.Chunks[this.RelativePosition.X, this.RelativePosition.Z - 1]; } }
        public Chunk West { get { return World.Chunks[this.RelativePosition.X - 1, this.RelativePosition.Z]; } }
        public Chunk East { get { return World.Chunks[this.RelativePosition.X + 1, this.RelativePosition.Z]; } }
        public Chunk NorthWest { get { return World.Chunks[this.RelativePosition.X - 1, this.RelativePosition.Z + 1]; } }
        public Chunk NorthEast { get { return World.Chunks[this.RelativePosition.X + 1, this.RelativePosition.Z + 1]; } }
        public Chunk SouthWest { get { return World.Chunks[this.RelativePosition.X - 1, this.RelativePosition.Z - 1]; } }
        public Chunk SouthEast { get { return World.Chunks[this.RelativePosition.X + 1, this.RelativePosition.Z - 1]; } }

        public static byte MaxSunValue = 16;

        /// <summary>
        /// Chunk width in blocks.
        /// </summary>
        public static byte WidthInBlocks = 16;
        public static byte MaxWidthInBlocks = 15;

        /// <summary>
        /// Chunk lenght in blocks
        /// </summary>
        public static byte LenghtInBlocks = 16;
        public static byte MaxLenghtInBlocks = 15;

        /// <summary>
        /// Chunk height in blocks.
        /// </summary>
        public static byte HeightInBlocks = 128;
        public static byte MaxHeightInBlocks = 127;

        /// <summary>
        /// Chunk volume in blocks.
        /// </summary>
        public static readonly int Volume = WidthInBlocks * HeightInBlocks * LenghtInBlocks;

        /// <summary>
        /// Contained blocks as a flattened array.
        /// </summary>
        public Block[] Blocks { get; private set; }

        /// <summary>
        /// Used when accessing flatten blocks array.
        /// </summary>
        public static readonly int FlattenOffset = LenghtInBlocks * HeightInBlocks;

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
        public BoundingBox BoundingBox { get; private set; }

        /// <summary>
        /// The attached world object.
        /// </summary>
        public readonly World World;

        /// <summary>
        /// Does chunk need a re-build of vertices?
        /// </summary>
        public bool Dirty { get; set; }

        /// <summary>
        /// Is chunk terrain generated?
        /// </summary>
        public bool Generated { get; set; }

        /// <summary>
        /// Is chunk currently queued for built?
        /// </summary>
        public bool QueuedForBuilding { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool QueuedForGeneration { get; set; }

        public short Index;

        /// <summary>
        /// Highest solid blocks offset.
        /// </summary>
        public byte HighestSolidBlockOffset = 0;

        /// <summary>
        /// Lowest empty block offset.
        /// </summary>
        public byte LowestEmptyBlockOffset =  (byte)HeightInBlocks;

        /// <summary>
        /// Is the region disposed already?
        /// </summary>
        public bool Disposed = false;

        /// <summary>
        /// Vertex buffer for chunk's blocks.
        /// </summary>
        public VertexBuffer VertexBuffer { get; set; }

        public IndexBuffer IndexBuffer { get; set; }

        /// <summary>
        /// The vertex list.
        /// </summary>
        public List<BlockVertex> VertexList;

        public List<short> IndexList;

        public Chunk(World world, Vector2Int relativePosition)
        {
            this.Generated = false;
            this.Dirty = true;
            this.QueuedForBuilding = false;
            this.QueuedForGeneration = false;

            this.World = world;
            this.RelativePosition = relativePosition;
            this.WorldPosition = new Vector2Int(this.RelativePosition.X * WidthInBlocks, this.RelativePosition.Z * LenghtInBlocks);
            this.Blocks = new Block[WidthInBlocks*LenghtInBlocks*HeightInBlocks];
            this.BoundingBox = new BoundingBox(new Vector3(WorldPosition.X, 0, WorldPosition.Z), new Vector3(this.WorldPosition.X + WidthInBlocks, HeightInBlocks, this.WorldPosition.Z + LenghtInBlocks));

            this.VertexList = new List<BlockVertex>();
            this.IndexList = new List<short>();

            InitBlocks();
        }

        private void InitBlocks()
        {
            if (this.Disposed) return;
            for (byte x = 0; x < WidthInBlocks; x++)
            {
                for (byte z = 0; z < LenghtInBlocks; z++)
                {
                    int offset = x*FlattenOffset + z*HeightInBlocks;
                    for (byte y = 0; y < HeightInBlocks; y++)
                    {
                        Blocks[offset + y] = Block.Empty;
                    }
                }
            }
        }

        public bool IsInBounds(float x, float z)
        {
            if (x < this.BoundingBox.Min.X || z < this.BoundingBox.Min.Z || x >= this.BoundingBox.Max.X || z >= this.BoundingBox.Max.Z) return false;
            return true;
        }

        public Block BlockAt(int x, int y, int z)
        {
            return this.Blocks[x*FlattenOffset + z*HeightInBlocks + y];
        }

        public void SetBlock(byte x, byte y, byte z, Block block)
        {
            switch (block.Exists)
            {
                case false:
                    if (this.LowestEmptyBlockOffset > y) this.LowestEmptyBlockOffset = (byte)(y - 1);
                    break;
                case true:
                    if (y > this.HighestSolidBlockOffset) this.HighestSolidBlockOffset = (byte) (y + 1);
                    break;
            }

            this.Blocks[x * Chunk.FlattenOffset + z * Chunk.HeightInBlocks + y] = block;
            this.Dirty = true;

            if (x == 0) this.West.Dirty = true;
            else if (x == MaxWidthInBlocks) this.East.Dirty = true;

            if (z == 0) this.South.Dirty = true;
            else if (z == MaxLenghtInBlocks) this.North.Dirty = true;

        }

        public void PrintDebugInfo(GraphicsDevice graphicsDevice, ICameraService camera, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            var position = RelativePosition + " " + LowestEmptyBlockOffset + "/" + HighestSolidBlockOffset;
            var positionSize = spriteFont.MeasureString(position);
            Vector3 projected = graphicsDevice.Viewport.Project(Vector3.Zero, camera.Projection, camera.View, Matrix.CreateTranslation(new Vector3(WorldPosition.X+WidthInBlocks/2, HighestSolidBlockOffset-1, WorldPosition.Z + LenghtInBlocks / 2)));
            spriteBatch.DrawString(spriteFont, position, new Vector2(projected.X - positionSize.X/2, projected.Y - positionSize.Y/2), Color.White);
        }

        #region de-ctor

        // IDisposable pattern: http://msdn.microsoft.com/en-us/library/fs2xkftw(v=VS.100).aspx

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Take object out the finalization queue to prevent finalization code for it from executing a second time.
        }

        private void Dispose(bool disposing)
        {
            if (this.Disposed) return; // if already disposed, just return

            if (disposing) // only dispose managed resources if we're called from directly or in-directly from user code.
            {
                //this.Blocks = null;
                //this.VertexList.Clear();
                //this.VertexList = null;
                //if(this.VertexBuffer!=null) this.VertexBuffer.Dispose();
            }

            Disposed = true;
        }

        ~Chunk() { Dispose(false); } // finalizer called by the runtime. we should only dispose unmanaged objects and should NOT reference managed ones.    

        #endregion
    }
}
