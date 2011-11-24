/*
 * Copyright (C) 2011 - Hüseyin Uslu shalafiraistlin@gmail.com
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxlrEngine.Screen;
using VolumetricStudios.VoxlrEngine.Universe;

namespace VolumetricStudios.VoxlrEngine
{
    public interface InGameDebuggerService
    {
        void ToggleInGameDebugger();
    }

    public sealed class InGameDebugger:DrawableGameComponent, InGameDebuggerService
    {
        private ICameraService _camera;
        private IWorldService _world;
        private IPlayer _player;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private bool _active = false;

        public InGameDebugger(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(InGameDebuggerService), this);
        }

        public void ToggleInGameDebugger()
        {
            _active = !_active;
        }

        public override void Initialize()
        {
            this._camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));
            this._world = (IWorldService)this.Game.Services.GetService(typeof(IWorldService));
            this._player = (IPlayer)this.Game.Services.GetService(typeof(IPlayer));
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _spriteFont = Game.Content.Load<SpriteFont>("Fonts//CalibriDebug");
        }

        public override void Draw(GameTime gameTime)
        {
            if (!_active) return;
            var viewFrustrum = new BoundingFrustum(this._camera.View * this._camera.Projection);

            _spriteBatch.Begin();

            foreach (Chunk chunk in _world.Chunks.Values)
            {
                if (!chunk.BoundingBox.Intersects(viewFrustrum)) continue;
                chunk.PrintDebugInfo(Game.GraphicsDevice, _camera, _spriteBatch, _spriteFont);
            }

            _player.Weapon.PrintDebugInfo(Game.GraphicsDevice, _camera, _spriteBatch, _spriteFont);

            _spriteBatch.End();

        }
    }

    public interface IInGameDebuggable
    {
        void PrintDebugInfo(GraphicsDevice graphicsDevice, ICameraService camera, SpriteBatch spriteBatch, SpriteFont spriteFont);
    }
}
