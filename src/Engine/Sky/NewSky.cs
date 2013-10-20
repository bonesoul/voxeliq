/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Collections.Generic;
using Engine.Assets;
using Engine.Blocks;
using Engine.Common.Logging;
using Engine.Graphics;
using Engine.Graphics.Texture;
using Engine.Universe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Engine.Sky
{
    public interface INewSky
    {
    }

    public class NewSky : DrawableGameComponent, INewSky
    {
        private const int size = 150;

        private bool[,] Clouds = new bool[size, size];

        /// <summary>
        /// The vertex list.
        /// </summary>
        public List<BlockVertex> VertexList;

        /// <summary>
        /// The index list.
        /// </summary>
        public List<short> IndexList;

        /// <summary>
        /// Vertex buffer for chunk's blocks.
        /// </summary>
        public VertexBuffer VertexBuffer { get; set; }

        /// <summary>
        /// Index buffer for chunk's blocks.
        /// </summary>
        public IndexBuffer IndexBuffer { get; set; }

        public short Index;

        private bool _meshBuilt = false;

        private Effect _blockEffect; // block effect.
        private Texture2D _blockTextureAtlas; // block texture atlas

        private ICamera _camera;
        private IAssetManager _assetManager;
        private IFogger _fogger;

        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility.

        public NewSky(Game game)
            : base(game)
        {
            this.VertexList = new List<BlockVertex>();
            this.IndexList = new List<short>();
            this.Index = 0;

            this.Game.Services.AddService(typeof(INewSky), this); // export service.
        }

        private Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            this._camera = (ICamera)this.Game.Services.GetService(typeof(ICamera));
            this._fogger = (IFogger)this.Game.Services.GetService(typeof(IFogger));
            this._assetManager = (IAssetManager)this.Game.Services.GetService(typeof(IAssetManager));

            if (this._assetManager == null)
                throw new NullReferenceException("Can not find asset manager component.");

            var colors = TextureTo2DArray(_assetManager.CloudTexture);

            for (int x = 0; x < size; x++)
            {
                for (int z = 0; z < size; z++)
                {
                    this.Clouds[x, z] = colors[x, z] == Color.White;

                    //float cloudiness = SimplexNoise.noise(x*0.009f, 0, z*0.009f)*1f;
                    //this.Clouds[x, z] = cloudiness > 0.5f;
                }
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            this._blockEffect = this._assetManager.BlockEffect;
            this._blockTextureAtlas = this._assetManager.BlockTextureAtlas;
        }

        public override void Update(GameTime gameTime)
        {
            this.BuildMesh();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game.GraphicsDevice.BlendState = BlendState.Opaque;

            // general parameters
            _blockEffect.Parameters["World"].SetValue(Matrix.Identity);
            _blockEffect.Parameters["View"].SetValue(this._camera.View);
            _blockEffect.Parameters["Projection"].SetValue(this._camera.Projection);
            _blockEffect.Parameters["CameraPosition"].SetValue(this._camera.Position);

            // texture parameters
            _blockEffect.Parameters["BlockTextureAtlas"].SetValue(_blockTextureAtlas);

            // atmospheric settings
            _blockEffect.Parameters["SunColor"].SetValue(World.SunColor);
            _blockEffect.Parameters["NightColor"].SetValue(World.NightColor);
            _blockEffect.Parameters["HorizonColor"].SetValue(World.HorizonColor);
            _blockEffect.Parameters["MorningTint"].SetValue(World.MorningTint);
            _blockEffect.Parameters["EveningTint"].SetValue(World.EveningTint);

            // time of day parameters
            _blockEffect.Parameters["TimeOfDay"].SetValue(Time.GetGameTimeOfDay());

            // fog parameters
            _blockEffect.Parameters["FogNear"].SetValue(this._fogger.FogVector.X);
            _blockEffect.Parameters["FogFar"].SetValue(this._fogger.FogVector.Y);

            foreach (var pass in this._blockEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                if (IndexBuffer == null || VertexBuffer == null)
                    continue;

                if (VertexBuffer.VertexCount == 0)
                    continue;

                if (IndexBuffer.IndexCount == 0)
                    continue;

                Game.GraphicsDevice.SetVertexBuffer(VertexBuffer);
                Game.GraphicsDevice.Indices = IndexBuffer;
                Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VertexBuffer.VertexCount, 0, IndexBuffer.IndexCount / 3);
            }
        }

        private void BuildMesh()
        {
            if (this._meshBuilt)
                return;

            for (int x = 0; x < size; x++)
            {
                for (int z = 0; z < size; z++)
                {
                    if (this.Clouds[x, z] == false)
                        continue;

                        this.BuildBlockVertices(x, z);
                }
            }

            var vertices = VertexList.ToArray();
            var indices = IndexList.ToArray();

            if (vertices.Length == 0 || indices.Length == 0) 
                return;

            VertexBuffer = new VertexBuffer(this.Game.GraphicsDevice, typeof(BlockVertex), vertices.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData(vertices);

            IndexBuffer = new IndexBuffer(this.Game.GraphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
            IndexBuffer.SetData(indices);

            this._meshBuilt = true;
        }

        private void BuildBlockVertices(int x, int z)
        {
            var north = z != size-1 && this.Clouds[x, z + 1];
            var south = z != 0 && this.Clouds[x, z - 1];
            var east = x != size-1 && this.Clouds[x + 1, z];
            var west = x != 0 && this.Clouds[x - 1, z];
            
            if (!west) // -xface (if block on west doesn't exist.)
            {
                BuildFaceVertices(x, z, BlockFaceDirection.XDecreasing);
            }
            if (!east) // +xface (if block on east doesn't exist.)
            {
                BuildFaceVertices(x, z, BlockFaceDirection.XIncreasing);
            }
            
            // -yface (as clouds are one block in height, nothing exists on bottom of them)
            BuildFaceVertices(x, z, BlockFaceDirection.YDecreasing);

            // +yface (as clouds are on block in height, nothing exists on top of them).
            BuildFaceVertices(x, z, BlockFaceDirection.YIncreasing);

            if (!south) // -zface (if block on south doesn't exist.)
            {
                BuildFaceVertices(x, z, BlockFaceDirection.ZDecreasing);
            }
            if (!north) // +zface (if block on north doesn't exist.)
            {
                BuildFaceVertices(x, z, BlockFaceDirection.ZIncreasing);
            }
        }

        private void BuildFaceVertices(int x, int z, BlockFaceDirection faceDir)
        {
            BlockTexture texture = Block.GetTexture(BlockType.Snow, faceDir);
            int faceIndex = 0;

            var textureUVMappings = TextureHelper.BlockTextureMappings[(int)texture * 6 + faceIndex];


            switch (faceDir)
            {
                case BlockFaceDirection.XIncreasing:
                    {
                        //TR,TL,BR,BR,TL,BL
                        AddVertex(x,z, new Vector3(1, 1, 1), textureUVMappings[0]);
                        AddVertex(x, z, new Vector3(1, 1, 0), textureUVMappings[1]);
                        AddVertex(x, z, new Vector3(1, 0, 1), textureUVMappings[2]);
                        AddVertex(x, z, new Vector3(1, 0, 0), textureUVMappings[5]);
                        AddIndex( 0, 1, 2, 2, 1, 3);
                    }
                    break;

                case BlockFaceDirection.XDecreasing:
                    {
                        //TR,TL,BL,TR,BL,BR
                        AddVertex(x, z, new Vector3(0, 1, 0), textureUVMappings[0]);
                        AddVertex(x, z, new Vector3(0, 1, 1), textureUVMappings[1]);
                        AddVertex(x, z, new Vector3(0, 0, 0), textureUVMappings[5]);
                        AddVertex(x, z, new Vector3(0, 0, 1), textureUVMappings[2]);
                        AddIndex( 0, 1, 3, 0, 3, 2);
                    }
                    break;

                case BlockFaceDirection.YIncreasing:
                    {
                        //BL,BR,TR,BL,TR,TL
                        AddVertex(x, z, new Vector3(1, 1, 1), textureUVMappings[0]);
                        AddVertex(x, z, new Vector3(0, 1, 1), textureUVMappings[2]);
                        AddVertex(x, z, new Vector3(1, 1, 0), textureUVMappings[4]);
                        AddVertex(x, z, new Vector3(0, 1, 0), textureUVMappings[5]);
                        AddIndex( 3, 2, 0, 3, 0, 1);
                    }
                    break;

                case BlockFaceDirection.YDecreasing:
                    {
                        //TR,BR,TL,TL,BR,BL
                        AddVertex(x, z, new Vector3(1, 0, 1), textureUVMappings[0]);
                        AddVertex(x, z, new Vector3(0, 0, 1), textureUVMappings[2]);
                        AddVertex(x, z, new Vector3(1, 0, 0), textureUVMappings[4]);
                        AddVertex(x, z, new Vector3(0, 0, 0), textureUVMappings[5]);
                        AddIndex( 0, 2, 1, 1, 2, 3);
                    }
                    break;

                case BlockFaceDirection.ZIncreasing:
                    {
                        //TR,TL,BL,TR,BL,BR
                        AddVertex(x, z, new Vector3(0, 1, 1), textureUVMappings[0]);
                        AddVertex(x, z, new Vector3(1, 1, 1), textureUVMappings[1]);
                        AddVertex(x, z, new Vector3(0, 0, 1), textureUVMappings[5]);
                        AddVertex(x, z, new Vector3(1, 0, 1), textureUVMappings[2]);
                        AddIndex( 0, 1, 3, 0, 3, 2);
                    }
                    break;

                case BlockFaceDirection.ZDecreasing:
                    {
                        //TR,TL,BR,BR,TL,BL
                        AddVertex(x, z, new Vector3(1, 1, 0), textureUVMappings[0]);
                        AddVertex(x, z, new Vector3(0, 1, 0), textureUVMappings[1]);
                        AddVertex(x, z, new Vector3(1, 0, 0), textureUVMappings[2]);
                        AddVertex(x, z, new Vector3(0, 0, 0), textureUVMappings[5]);
                        AddIndex( 0, 1, 2, 2, 1, 3);
                    }
                    break;
            }
        }

        private void AddVertex(int x, int z, Vector3 addition, HalfVector2 textureCoordinate)
        {
            VertexList.Add(new BlockVertex(new Vector3(x, 128, z) + addition, textureCoordinate, 1f));
        }

        private void AddIndex( short i1, short i2, short i3, short i4, short i5, short i6)
        {
            IndexList.Add((short)(Index + i1));
            IndexList.Add((short)(Index + i2));
            IndexList.Add((short)(Index + i3));
            IndexList.Add((short)(Index + i4));
            IndexList.Add((short)(Index + i5));
            IndexList.Add((short)(Index + i6));
            Index += 4;
        }
    }
}
