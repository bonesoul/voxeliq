/*    
 * Copyright (C) 2011, Hüseyin Uslu
 *  
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VolumetricStudios.VoxlrClient.Screen;
using VolumetricStudios.VoxlrClient.States;

namespace VolumetricStudios.VoxlrClient
{
    /// <summary>
    /// Game client.
    /// </summary>
    public class VoxlrClient : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        
        /* rasterizer */
        private readonly RasterizerState _wireframedRaster = new RasterizerState() { CullMode = CullMode.CullCounterClockwiseFace, FillMode = FillMode.WireFrame };
        private readonly RasterizerState _normalRaster = new RasterizerState() { CullMode = CullMode.CullCounterClockwiseFace, FillMode = FillMode.Solid };
        private bool _wireframed = false;

        public VoxlrClient()
        {
            this._graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.Window.Title = "VoxlrGameClient";
            new ScreenManager(this._graphicsDeviceManager, this); // startup the screen manager.             

            this.Components.Add(new StateManager(this) { UpdateOrder = 0, ActiveState = new LoadingState(this) });

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) this.Exit(); // Allows the game to exit

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.RasterizerState = !this._wireframed ? this._normalRaster : this._wireframedRaster;

            base.Draw(gameTime);
        }

        public void ToggleRasterMode()
        {
            this._wireframed = !this._wireframed;
        }
    }
}
