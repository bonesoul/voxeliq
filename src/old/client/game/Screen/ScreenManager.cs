/*
 * Copyright (C) 2011 voxeliq project 
 *
 */

using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqEngine.Common.Logging;

namespace VolumetricStudios.VoxeliqClient.Screen
{
    /// <summary>
    /// Screen service for controlling screen.
    /// </summary>
    public interface IScreenService
    {
        void ToggleFPSLimiting();
    }

    /// <summary>
    /// The screen manager that controls various graphical aspects.
    /// </summary>
    public sealed class ScreenManager : IScreenService
    {
        /// <summary>
        /// Limit the game FPS to 60?
        /// </summary>
        public bool LimitFPS { get; private set; }

        /// <summary>
        /// The attached game.
        /// </summary>
        private readonly Game _game;

        /// <summary>
        /// Attached graphics device.
        /// </summary>
        private readonly GraphicsDeviceManager _graphics;

        /// <summary>
        /// Logging facility.
        /// </summary>
        private static readonly Logger Logger = LogManager.CreateLogger();

        public ScreenManager(GraphicsDeviceManager graphics, Game game)
        {
            Logger.Trace("ctor()");

            this.LimitFPS = false;
            this._game = game;
            this._graphics = graphics;

            this._game.Services.AddService(typeof(IScreenService), this); // add screen-manager as a service for the game.

            this._graphics.IsFullScreen = ScreenConfig.Instance.FullScreenEnabled;
            this._graphics.PreferredBackBufferWidth = ScreenConfig.Instance.ScreenWidth;
            this._graphics.PreferredBackBufferHeight = ScreenConfig.Instance.ScreenHeight;
            this._game.IsFixedTimeStep = false;
            this._graphics.SynchronizeWithVerticalRetrace = false;
            this._graphics.ApplyChanges();
        }

        /// <summary>
        /// Toggles FPS limiting.
        /// </summary>
        public void ToggleFPSLimiting()
        {
            LimitFPS = !LimitFPS;
            this._game.IsFixedTimeStep = LimitFPS;
            this._graphics.SynchronizeWithVerticalRetrace = LimitFPS;
            this._graphics.ApplyChanges();
        }
    }
}
