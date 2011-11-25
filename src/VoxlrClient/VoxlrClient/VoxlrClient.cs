/*
 * Copyright (C) 2011 voxlr project 
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VolumetricStudios.VoxlrClient.Screen;
using VolumetricStudios.VoxlrClient.States;

namespace VolumetricStudios.VoxlrClient
{
    /// <summary>
    /// The game client.
    /// </summary>
    public class VoxlrClient : Game
    {
        /// <summary>
        /// Graphics device manager.
        /// </summary>
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        
        /* rasterizer */

        /// <summary>
        /// Wire-framed rasterizer.
        /// </summary>
        private readonly RasterizerState _wireframedRaster = new RasterizerState() { CullMode = CullMode.CullCounterClockwiseFace, FillMode = FillMode.WireFrame };

        /// <summary>
        /// Normal rasterizer.
        /// </summary>
        private readonly RasterizerState _normalRaster = new RasterizerState() { CullMode = CullMode.CullCounterClockwiseFace, FillMode = FillMode.Solid };

        /// <summary>
        /// Sets if rendering mode is wire-framed.
        /// </summary>
        public bool Wireframed { get; private set; }

        /// <summary>
        /// Creates a new VoxlrClient.
        /// </summary>
        public VoxlrClient()
        {
            this.Wireframed = false;
            this._graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.Window.Title = "Voxlr " + Assembly.GetExecutingAssembly().GetName().Version;
            
            new ScreenManager(this._graphicsDeviceManager, this); // startup the screen manager.
            this.Components.Add(new StateManager(this) { UpdateOrder = 0, ActiveState = new LoadingState(this) });

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) 
                this.Exit(); // Allows the game to exit.

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.RasterizerState = !this.Wireframed ? this._normalRaster : this._wireframedRaster; // set the rasterizer state.

            base.Draw(gameTime);
        }

        public void ToggleRasterMode()
        {
            this.Wireframed = !this.Wireframed;
        }
    }
}
