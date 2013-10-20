/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Engine.Debugging.Console;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics
{
    /// <summary>
    /// Rasterizer helper.
    /// </summary>
    public class Rasterizer
    {
        /// <summary>
        /// Returns true when rasterizer is in wire-framed mode.
        /// </summary>
        public bool Wireframed
        {
            get { return this.State == WireframedRaster; }
            set { this.State = value == true ? WireframedRaster : NormalRaster; }
        }

        /// <summary>
        /// Returns the current mode of the rasterizer.
        /// </summary>
        public RasterizerState State { get; private set; }

        /// <summary>
        /// Creates a new rasterizer.
        /// </summary>
        public Rasterizer()
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
    }

    [Command("rasterizer", "Sets rasterizer mode.\nusage: rasterizer [wireframed|normal]")]
    public class RasterizerCommand : Command
    {
        [DefaultCommand]
        public string Default(string[] @params)
        {
            return string.Format("Rasterizer is currently set to {0} mode.\nusage: rasterizer [wireframed|normal].",
                                 Core.Engine.Instance.Rasterizer.Wireframed
                                     ? "wireframed"
                                     : "normal");
        }
        
        [Subcommand("wireframed","Sets rasterizer mode to wireframed.")]
        public string Wireframed(string[] @params)
        {
            Core.Engine.Instance.Rasterizer.Wireframed = true;
            return "Rasterizer mode set to wireframed.";
        }

        [Subcommand("normal", "Sets rasterizer mode to normal.")]
        public string Normal(string[] @params)
        {
            Core.Engine.Instance.Rasterizer.Wireframed = false;
            return "Rasterizer mode set to normal mode.";
        }
    }
}