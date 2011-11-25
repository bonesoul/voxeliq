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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxlrClient.GameEngine;

namespace VolumetricStudios.VoxlrClient.States
{
    public sealed class LoadingState : GameState
    {
        private Texture2D _background;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private string _message;
        private Vector2 _messageSize;
        private Vector2 _messagePosition;

        public LoadingState(Game game) : base(game) { }

        public override void Initialize()
        {
            this._spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            Game.Components.Add(new Universe(this.Game) {UpdateOrder = 2});
        }

        public override void LoadContent()
        {
            _background = Game.Content.Load<Texture2D>("Textures\\voxel");
            _spriteFont = Game.Content.Load<SpriteFont>("Fonts\\calibri");
            this.UpdateMessage("Building world..");
        }
        
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(this._background, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);
            _spriteBatch.DrawString(_spriteFont, _message, _messagePosition, Color.White);
            _spriteBatch.End();
        }

        private void UpdateMessage(string message)
        {
            this._message = message;
            _messageSize = _spriteFont.MeasureString(this._message);
            _messagePosition = new Vector2(Game.GraphicsDevice.Viewport.Width / 2f - _messageSize.X / 2, Game.GraphicsDevice.Viewport.Height - _messageSize.Y);
        }
    }
}
