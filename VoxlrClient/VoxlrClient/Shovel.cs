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
