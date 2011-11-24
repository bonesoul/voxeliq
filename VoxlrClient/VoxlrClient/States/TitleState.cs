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

namespace VolumetricStudios.VoxlrClient.States
{
    public sealed class TitleState : GameState
    {
        private SpriteBatch _spriteBatch;
        private const string Title = "Voxlr";
        private SpriteFont _spriteFont;
        private Vector2 _titlePosition;

        public TitleState(Game game) : base(game) { }

        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public override void LoadContent()
        {
            _spriteFont = Game.Content.Load<SpriteFont>("Fonts\\console");
            var titleSize = _spriteFont.MeasureString(Title);
            _titlePosition = new Vector2(Game.GraphicsDevice.Viewport.Width / 2f - titleSize.X / 2, Game.GraphicsDevice.Viewport.Height / 2f - titleSize.Y / 2);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Cornsilk);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_spriteFont, Title, _titlePosition, Color.Black);
            _spriteBatch.End();
        }
    }
}
