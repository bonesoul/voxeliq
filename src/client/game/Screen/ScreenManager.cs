/*
 * Copyright (C) 2011 voxeliq project 
 *
 */

using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqClient.Configuration;

namespace VolumetricStudios.VoxeliqClient.Screen
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
