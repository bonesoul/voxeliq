/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Microsoft.Xna.Framework.Graphics;

namespace VoxeliqEngine.Graphics
{
    /// <summary>
    /// Rasterizer helper.
    /// </summary>
    public class Rasterizer
    {
        /// <summary>
        /// Returns true when rasterizer is in wire-framed mode.
        /// </summary>
        public bool Wireframed { get; private set; }

        /// <summary>
        /// Returns the current mode of the rasterizer.
        /// </summary>
        public RasterizerState State
        {
            get { return !this.Wireframed ? NormalRaster : WireframedRaster; }
        }

        /// <summary>
        /// Creates a new rasterizer.
        /// </summary>
        private Rasterizer()
        {
            this.Wireframed = false;
        }

        public void ActivateWireframedMode()
        {
            this.Wireframed = true;
        }

        public void ActivateNormalMode()
        {
            this.Wireframed = false;
        }

        /// <summary>
        /// Wire-framed rasterizer.
        /// </summary>
        private static readonly RasterizerState WireframedRaster = new RasterizerState()
                                                                       {
                                                                           CullMode = CullMode.CullCounterClockwiseFace,
                                                                           FillMode = FillMode.WireFrame
                                                                       };

        /// <summary>
        /// Normal rasterizer.
        /// </summary>
        private static readonly RasterizerState NormalRaster = new RasterizerState()
                                                                   {
                                                                       CullMode = CullMode.CullCounterClockwiseFace,
                                                                       FillMode = FillMode.Solid
                                                                   };

        /// <summary>
        /// The memory instance of ScreenConfig.
        /// </summary>
        private static readonly Rasterizer _instance = new Rasterizer();

        /// <summary>
        /// The memory instance of ScreenConfig.
        /// </summary>
        public static Rasterizer Instance
        {
            get { return _instance; }
        }
    }
}