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
using VolumetricStudios.VoxlrEngine.Universe;

namespace VolumetricStudios.VoxlrClient.Interface
{
    public class UserInterface:DrawableGameComponent
    {
        private Texture2D _crosshairNormalTexture;
        private Texture2D _crosshairShovelTexture;
        private SpriteBatch _spriteBatch;
        private IPlayer _player;

        public UserInterface(Game game) : base(game) { }

        public override void Initialize()
        {
            this._player = (IPlayer)this.Game.Services.GetService(typeof(IPlayer));
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            this.LoadContent();
        }

        protected override void LoadContent()
        {
            _crosshairNormalTexture = Game.Content.Load<Texture2D>("Textures\\Crosshairs\\Normal");
            _crosshairShovelTexture = Game.Content.Load<Texture2D>("Textures\\Crosshairs\\Shovel");
        }

        public override void Draw(GameTime gameTime)
        {
            // draw cross-hair.            
            var crosshairTexture = this._player.AimedSolidBlock.HasValue ? _crosshairShovelTexture : _crosshairNormalTexture;

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            _spriteBatch.Draw(crosshairTexture, new Vector2((Game.GraphicsDevice.Viewport.Width / 2) - 10, (Game.GraphicsDevice.Viewport.Height / 2) - 10), Color.White);
            _spriteBatch.End();
        }
    }
}
