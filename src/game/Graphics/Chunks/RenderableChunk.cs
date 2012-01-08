/*
 * Copyright (C) 2011 voxeliq project 
 *
 */

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxeliqEngine.Chunks;
using VolumetricStudios.VoxeliqEngine.Universe;
using VolumetricStudios.VoxeliqEngine.Utils.Vector;

namespace VolumetricStudios.VoxeliqClient.Graphics.Chunks
{
    /// <summary>
    /// A renderable chunk.
    /// </summary>
    public class RenderableChunk : Chunk
    {
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

        public RenderableChunk(World world, Vector2Int relativePosition) 
            : base(world, relativePosition)
        {
            this.VertexList = new List<BlockVertex>();
            this.IndexList = new List<short>();
        }
    }
}
