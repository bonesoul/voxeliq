/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VolumetricStudios.VoxeliqGame.States
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
        }

        public override void LoadContent()
        {
            //_background = Game.Content.Load<Texture2D>("Textures\\voxel");
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
