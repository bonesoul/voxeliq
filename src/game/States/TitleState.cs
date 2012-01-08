/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VolumetricStudios.VoxeliqGame.States
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
