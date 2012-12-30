/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Voxeliq.MonsterEditor.Rendering.GraphicsDevice;

namespace WinFormsGraphicsDevice
{
    /// <summary>
    /// Example control inherits from GraphicsDeviceControl, which allows it to
    /// render using a GraphicsDevice. This control shows how to draw animating
    /// 3D graphics inside a WinForms application. It hooks the Application.Idle
    /// event, using this to invalidate the control, which will cause the animation
    /// to constantly redraw.
    /// </summary>
    public class XNAHost : GraphicsDeviceControl
    {
        private ContentManager content;
        private Texture2D monsterTexture;
        private Texture2D grassTexture;

        public Matrix Projection; // the camera lens.
        public Matrix World;  // the world.
        public Matrix View; // the camera position.

        BasicEffect quadEffect;
        Quad quad;
        VertexDeclaration quadVertexDecl;

        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {
            content = new ContentManager(Services, "Content");

            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            this.monsterTexture = content.Load<Texture2D>(@"mineplayer");
            this.grassTexture = content.Load<Texture2D>(@"grass");

            this.SetupViewport();

            quad = new Quad(Vector3.Zero, Vector3.Backward, Vector3.Up, 2, 2);
            quadEffect = new BasicEffect(this.GraphicsDevice);
            quadEffect.EnableDefaultLighting();

            quadEffect.World = this.World;
            quadEffect.View = this.View;
            quadEffect.Projection = this.Projection;
            quadEffect.TextureEnabled = true;
            quadEffect.Texture = grassTexture;
        }

        private void SetupViewport()
        {
            this.World = Matrix.Identity;
            this.View = Matrix.CreateLookAt(new Vector3(0, 0, 2), Vector3.Zero, Vector3.Up);
            this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 4.0f/3.0f, 1, 500);
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue); // clear back the background.
            
            foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList, quad.Vertices, 0, 4, quad.Indices, 0, 2);

            }
        }
    }
}
