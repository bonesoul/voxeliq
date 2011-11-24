/*    
 * Copyright (C) 2011, Hüseyin Uslu
 *  
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace VolumetricStudios.VoxlrEngine.Universe.Graphics
{
    [Serializable]
    public struct BlockVertex : IVertexType
    {
        private Vector3 _position;
        private HalfVector2 _blockTextureCoordinate;
        private float _sunLight;
        //private Vector3 _localLight;

        public BlockVertex(Vector3 position, HalfVector2 blockTextureCoordinate, float sunLight) //, Vector3 localLight)
        {
            _position = position;
            _blockTextureCoordinate = blockTextureCoordinate;
            _sunLight = sunLight;
            //_localLight = localLight;
        }

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }

        private static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new[]
        {
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position,0),
            new VertexElement(sizeof (float)*3, VertexElementFormat.HalfVector2,VertexElementUsage.TextureCoordinate,0),
            new VertexElement(sizeof (float)*4, VertexElementFormat.Single, VertexElementUsage .Color, 0),
            //new VertexElement(sizeof (float)*5, VertexElementFormat.Vector3, VertexElementUsage.Color, 1)
        });
            
        public Vector3 Position { get { return _position; } set { _position = value; } }
        public HalfVector2 BlockTextureCoordinate { get { return _blockTextureCoordinate; } set { _blockTextureCoordinate = value; } }
        //public Vector3 LocalLight { get { return _localLight; } set { _localLight = value; } }
        public float SunLight { get { return _sunLight; } set { _sunLight = value; } }
        public static int SizeInBytes { get { return sizeof(float) * 5; } }
    }
}
