/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Engine.Blocks
{
    /// Represents block vertex.
    [Serializable]
    public struct BlockVertex : IVertexType
    {
        private Vector3 _position;
        private HalfVector2 _blockTextureCoordinate;
        private float _sunLight;
        //private Vector3 _localLight;

        /// <summary>
        /// Creates a new instance of BlockVertex.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="blockTextureCoordinate"></param>
        /// <param name="sunLight"></param>
        public BlockVertex(Vector3 position, HalfVector2 blockTextureCoordinate, float sunLight) //, Vector3 localLight)
        {
            _position = position;
            _blockTextureCoordinate = blockTextureCoordinate;
            _sunLight = sunLight;
            //_localLight = localLight;
        }

        /// <summary>
        /// Returns the block vertex declaration.
        /// </summary>
        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }

        /// <summary>
        /// The actual block vertex declaration.
        /// </summary>
        private static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new[]
        {
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof (float)*3,VertexElementFormat.HalfVector2, VertexElementUsage.TextureCoordinate,0),
            new VertexElement(sizeof (float)*4,VertexElementFormat.Single,VertexElementUsage.Color, 0),
            //new VertexElement(sizeof (float)*5, VertexElementFormat.Vector3, VertexElementUsage.Color, 1)
        });

        /// <summary>
        /// The position.
        /// </summary>
        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// Texture coordinates.
        /// </summary>
        public HalfVector2 BlockTextureCoordinate
        {
            get { return _blockTextureCoordinate; }
            set { _blockTextureCoordinate = value; }
        }

        /// <summary>
        /// Sunlight.
        /// </summary>
        public float SunLight
        {
            get { return _sunLight; }
            set { _sunLight = value; }
        }

        /// <summary>
        /// The size of vertex declaration in bytes.
        /// </summary>
        public static int SizeInBytes
        {
            get { return sizeof (float)*5; }
        }

        //public Vector3 LocalLight { get { return _localLight; } set { _localLight = value; } }
    }
}