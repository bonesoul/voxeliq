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

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxlrClient.GameEngine;
using VolumetricStudios.VoxlrEngine.Screen;
using VolumetricStudios.VoxlrEngine.Universe;

namespace VolumetricStudios.VoxlrClient.Interface.Debug
{
    public sealed class Statistics : DrawableGameComponent
    {
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        
        private int _fps = 0;
        private int _frameCounter = 0;
        private TimeSpan _elapsedTime = TimeSpan.Zero;

        private IWorldStatisticsService _worldStatistics;
        private ICameraService _camera;
        private IPlayer _player;

        public Statistics(Game game) : base(game) { }

        protected override void LoadContent()
        {
            this._worldStatistics = (IWorldStatisticsService)this.Game.Services.GetService(typeof(IWorldStatisticsService));
            this._camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));
            this._player = (IPlayer) this.Game.Services.GetService(typeof (IPlayer));
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Game.Content.Load<SpriteFont>("Fonts//calibri");
        }

        public override void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime;
            if (_elapsedTime < TimeSpan.FromSeconds(1)) return;

            _elapsedTime -= TimeSpan.FromSeconds(1);
            _fps = _frameCounter;
            _frameCounter = 0;
        }

        public override void Draw(GameTime gameTime)
        {
            _frameCounter++;

            _spriteBatch.Begin();
            // Attention: DO NOT use string.format as it's slower than string concat: https://www.assembla.com/wiki/show/voxlr/StringFormat_vs_StringConcat
            _spriteBatch.DrawString(_spriteFont, "fps: " + _fps, new Vector2(5, 5), Color.White);
            _spriteBatch.DrawString(_spriteFont, "mem: " +  this.GetSize(GC.GetTotalMemory(false)), new Vector2(75, 5), Color.White);
            _spriteBatch.DrawString(_spriteFont, "pos: " + this._player.Position, new Vector2(190, 5), Color.White);
            _spriteBatch.DrawString(_spriteFont, "chunks: " + this._worldStatistics.ChunksDrawn + "/" + this._worldStatistics.TotalChunks, new Vector2(5, 20), Color.White);
            _spriteBatch.DrawString(_spriteFont, "blocks: " + this._worldStatistics.ChunksDrawn * Chunk.Volume + "/" + this._worldStatistics.TotalChunks * Chunk.Volume, new Vector2(130, 20), Color.White);
            _spriteBatch.DrawString(_spriteFont, "gen/buildQ: " + this._worldStatistics.GenerationQueueCount + "/" + this._worldStatistics.BuildingQueueCount, new Vector2(320, 20), Color.White);
            _spriteBatch.DrawString(_spriteFont, "Inf: " + (this._worldStatistics.IsInfinitive ? "On" : "Off"), new Vector2(5, 35), Color.White);            
            _spriteBatch.DrawString(_spriteFont, "Fly: " + (this._player.FlyingEnabled?"On":"Off"), new Vector2(60, 35), Color.White);
            _spriteBatch.DrawString(_spriteFont, "Fog: " + this._worldStatistics.FogState, new Vector2(120, 35), Color.White);            
            _spriteBatch.End();
        }

        private string GetSize(long size)
        {
            int i;
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            double dblSByte = 0;
            for (i = 0; (int)(size / 1024) > 0; i++, size /= 1024) dblSByte = size / 1024.0;
            return dblSByte.ToString("0.00") + suffixes[i];
        }
    }
}
