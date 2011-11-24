/*    
 * Copyright (C) 2011, Hüseyin Uslu
 *  
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxlrEngine.Screen;
using VolumetricStudios.VoxlrEngine.Universe;

namespace VolumetricStudios.VoxlrClient
{
    public class Shovel:Weapon
    {
        private IPlayer _player;
        private IWorldService _world;

        public Shovel(Game game) : base(game) { }

        public override void Initialize()
        {
            this._player = (IPlayer)this.Game.Services.GetService(typeof(IPlayer));
            this._world = (IWorldService)this.Game.Services.GetService(typeof(IWorldService));
        }

        public override void Use()
        {
            if (!_player.AimedSolidBlock.HasValue) return;
            _world.SetBlock(_player.AimedSolidBlock.Value.Position, Block.Empty);
        }

        public override void SecondaryUse()
        {
            if (!_player.AimedSolidBlock.HasValue) return;
            _world.SetBlock(_player.AimedEmptyBlock.Value.Position, new Block(BlockType.Iron));
        }

        public override void PrintDebugInfo(GraphicsDevice graphicsDevice, ICameraService camera, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            if (!_player.AimedSolidBlock.HasValue) return;
            var text = _player.AimedSolidBlock.Value.Position + " Sun: " + _player.AimedSolidBlock.Value.Block.Sun;
            var textSize = spriteFont.MeasureString(text);
            Vector3 projected = graphicsDevice.Viewport.Project(Vector3.Zero, camera.Projection, camera.View, Matrix.CreateTranslation(new Vector3(_player.AimedSolidBlock.Value.Position.X+0.5f, _player.AimedSolidBlock.Value.Position.Y+0.5f, _player.AimedSolidBlock.Value.Position.Z+0.5f)));
            spriteBatch.DrawString(spriteFont, text, new Vector2(projected.X - textSize.X / 2, projected.Y - textSize.Y / 2), Color.Yellow);
        }
    }
}
