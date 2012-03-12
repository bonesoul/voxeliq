/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using Microsoft.Xna.Framework.Graphics;

namespace VolumetricStudios.VoxeliqGame.Graphics
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
        public Rasterizer()
        {
            this.Wireframed = false;
        }

        /// <summary>
        /// Toggle's rasterizer's mode.
        /// </summary>
        public void ToggleRasterMode()
        {
            this.Wireframed = !this.Wireframed;
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
    }
}