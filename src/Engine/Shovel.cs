﻿/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxeliqEngine.Blocks;
using VoxeliqEngine.Chunks;
using VoxeliqEngine.Graphics;
using VoxeliqEngine.Universe;
using VoxeliqEngine.Utils.Vector;

namespace VoxeliqEngine
{
    public class Shovel : Weapon
    {
        // required services.
        private IPlayer _player;
        private IWorld _world;
        private IChunkCache _chunkCache;

        public Shovel(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            this._player = (IPlayer) this.Game.Services.GetService(typeof (IPlayer));
            this._world = (IWorld) this.Game.Services.GetService(typeof (IWorld));
            this._chunkCache = (IChunkCache) this.Game.Services.GetService(typeof (IChunkCache));
        }

        public override void Use()
        {
            if (!_player.AimedSolidBlock.HasValue) 
                return;

            this._chunkCache.SetBlockAt(_player.AimedSolidBlock.Value.Position, Block.Empty);
        }

        public override void SecondaryUse()
        {
            // Test for not pushing the player into walls.. this really should be handled differently /fasbat
            if (!_player.AimedEmptyBlock.HasValue || _player.AimedEmptyBlock.Value.Position == new Vector3Int(_player.Position + new Vector3(0f, -0.5f, 0f)))
                return;

            this._chunkCache.SetBlockAt(_player.AimedEmptyBlock.Value.Position, new Block(BlockType.Iron));
        }

        public override void DrawInGameDebugVisual(GraphicsDevice graphicsDevice, ICamera camera, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            if (!_player.AimedSolidBlock.HasValue) return;
            var text = _player.AimedSolidBlock.Value.Position + " Sun: " + _player.AimedSolidBlock.Value.Block.Sun;
            var textSize = spriteFont.MeasureString(text);

            Vector3 projected = graphicsDevice.Viewport.Project(Vector3.Zero, camera.Projection, camera.View,
                                                                Matrix.CreateTranslation(
                                                                    new Vector3(_player.AimedSolidBlock.Value.Position.X + 0.5f, _player.AimedSolidBlock.Value.Position.Y + 0.5f, _player.AimedSolidBlock.Value.Position.Z + 0.5f)));

            spriteBatch.DrawString(spriteFont, text, new Vector2(projected.X - textSize.X/2, projected.Y - textSize.Y/2), Color.Yellow);
        }
    }
}