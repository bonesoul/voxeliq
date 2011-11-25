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
using VolumetricStudios.VoxlrClient.Configuration;

namespace VolumetricStudios.VoxlrClient.Screen
{
    public interface IScreenService
    {
        void ToggleFPSLimiting();
    }

    public sealed class ScreenManager : IScreenService
    {
        private bool _fpsLimited=false;
        private readonly Game _game;
        private readonly GraphicsDeviceManager _graphics;

        public ScreenManager(GraphicsDeviceManager graphics, Game game)
        {
            game.Services.AddService(typeof(IScreenService), this);

            this._game=game;
            this._graphics=graphics;

            graphics.IsFullScreen = Settings.IsFullScreen;
            graphics.PreferredBackBufferWidth = Settings.Width;
            graphics.PreferredBackBufferHeight = Settings.Height;
            game.IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();
        }

        public void ToggleFPSLimiting()
        {
            _fpsLimited = !_fpsLimited;
            this._game.IsFixedTimeStep = _fpsLimited;
            this._graphics.SynchronizeWithVerticalRetrace = _fpsLimited;
            this._graphics.ApplyChanges();
        }
    }
}
