/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics.Drawing
{
    /* From FarSeer.DebugView - http://farseerphysics.codeplex.com/ */

    public class PrimitiveBatch : IDisposable
    {
        private const int DefaultBufferSize = 500;

        private BasicEffect _basicEffect; // a basic effect, which contains the shaders that we will use to draw our primitives.        

        private GraphicsDevice _device; // the device that we will issue draw calls to.        

        private bool _hasBegun; // hasBegun is flipped to true once Begin is called, and is used to make sure users don't call End before Begin is called.

        private VertexPositionColor[] _lineVertices;
        private VertexPositionColor[] _triangleVertices;
        private int _lineVertsCount;
        private int _triangleVertsCount;

        private bool _isDisposed;

        /// <summary>
        /// the constructor creates a new PrimitiveBatch and sets up all of the internals
        /// that PrimitiveBatch will need.
        /// </summary>
        public PrimitiveBatch(GraphicsDevice graphicsDevice, int bufferSize)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException("graphicsDevice");

            _device = graphicsDevice;

            _triangleVertices = new VertexPositionColor[bufferSize - bufferSize%3];
            _lineVertices = new VertexPositionColor[bufferSize - bufferSize%2];

            // set up a new basic effect, and enable vertex colors.
            _basicEffect = new BasicEffect(graphicsDevice) {VertexColorEnabled = true};
        }

        public void SetProjection(ref Matrix projection)
        {
            _basicEffect.Projection = projection;
        }

        /// <summary>
        /// Begin is called to tell the PrimitiveBatch what kind of primitives will be
        /// drawn, and to prepare the graphics card to render those primitives.
        /// </summary>
        /// <param name="projection">The projection.</param>
        /// <param name="view">The view.</param>
        public void Begin(Matrix projection, Matrix view)
        {
            if (_hasBegun)
                throw new InvalidOperationException("End must be called before Begin can be called again.");

            //tell our basic effect to begin.
            _basicEffect.Projection = projection;
            _basicEffect.View = view;
            _basicEffect.CurrentTechnique.Passes[0].Apply();

            // flip the error checking boolean. It's now ok to call AddVertex, Flush and End.
            _hasBegun = true;
        }

        public bool IsReady()
        {
            return _hasBegun;
        }

        public void AddVertex(Vector2 vertex, Color color, PrimitiveType primitiveType)
        {
            if (!_hasBegun)
                throw new InvalidOperationException("Begin() must be called before AddVertex can be called.");

            if (primitiveType == PrimitiveType.LineStrip || primitiveType == PrimitiveType.TriangleStrip)
                throw new NotSupportedException("The specified primitiveType is not supported by PrimitiveBatch.");

            if (primitiveType == PrimitiveType.TriangleList)
            {
                if (_triangleVertsCount >= _triangleVertices.Length)
                    FlushTriangles();

                _triangleVertices[_triangleVertsCount].Position = new Vector3(vertex, -0.1f);
                _triangleVertices[_triangleVertsCount].Color = color;
                _triangleVertsCount++;
            }

            if (primitiveType == PrimitiveType.LineList)
            {
                if (_lineVertsCount >= _lineVertices.Length)
                    FlushLines();

                _lineVertices[_lineVertsCount].Position = new Vector3(vertex, 0f);
                _lineVertices[_lineVertsCount].Color = color;
                _lineVertsCount++;
            }
        }

        /// <summary>
        /// End is called once all the primitives have been drawn using AddVertex.
        /// it will call Flush to actually submit the draw call to the graphics card, and
        /// then tell the basic effect to end.
        /// </summary>
        public void End()
        {
            if (!_hasBegun)
                throw new InvalidOperationException("Begin must be called before End can be called.");

            // Draw whatever the user wanted us to draw
            FlushTriangles();
            FlushLines();

            _hasBegun = false;
        }

        private void FlushTriangles()
        {
            if (!_hasBegun)
                throw new InvalidOperationException("Begin must be called before Flush can be called.");

            if (_triangleVertsCount >= 3)
            {
                int primitiveCount = _triangleVertsCount/3;

                // submit the draw call to the graphics card
                _device.SamplerStates[0] = SamplerState.AnisotropicClamp;
                _device.DrawUserPrimitives(PrimitiveType.TriangleList, _triangleVertices, 0, primitiveCount);
                _triangleVertsCount -= primitiveCount*3;
            }
        }

        private void FlushLines()
        {
            if (!_hasBegun)
                throw new InvalidOperationException("Begin must be called before Flush can be called.");

            if (_lineVertsCount >= 2)
            {
                int primitiveCount = _lineVertsCount/2;

                // submit the draw call to the graphics card
                _device.SamplerStates[0] = SamplerState.AnisotropicClamp;
                _device.DrawUserPrimitives(PrimitiveType.LineList, _lineVertices, 0, primitiveCount);
                _lineVertsCount -= primitiveCount*2;
            }
        }

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {
                if (_basicEffect != null)
                    _basicEffect.Dispose();

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}