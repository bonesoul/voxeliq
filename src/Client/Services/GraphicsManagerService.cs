/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Microsoft.Xna.Framework;

namespace Client.Services
{
    /// <summary>
    /// Graphics manager service interface that is responsible for screen & graphics managment.
    /// </summary>
    public interface IGraphicsManagerService
    {
        /// <summary>
        /// Returns true if game is set to fixed time steps.
        /// </summary>
        bool FixedTimeStepsEnabled { get; }

        /// <summary>
        /// Returns true if vertical sync is enabled.
        /// </summary>
        bool VerticalSyncEnabled { get; }

        /// <summary>
        /// Returns true if full-screen is enabled.
        /// </summary>
        bool FullScreenEnabled { get; }

        /// <summary>
        /// Toggles fixed time steps.
        /// </summary>
        void ToggleFixedTimeSteps();

        /// <summary>
        /// Sets vertical sync on or off.
        /// </summary>
        /// <param name="enabled">Vertical sync enabled?</param>
        void ToggleVerticalSync(bool enabled);

        /// <summary>
        /// Sets full screen on or off.
        /// </summary>
        /// <param name="enabled">Full screen enabled?</param>
        void ToggleFullScreen(bool enabled);

        /// <summary>
        /// Graphics device manager bound to the game.
        /// </summary>
        GraphicsDeviceManager GraphicsDeviceManager { get; }
    }

    /// <summary>
    /// Graphics manager service that is responsible for screen & graphics managment.
    /// </summary>
    public class GraphicsManagerService : GameComponent, IGraphicsManagerService
    {
        #region settings

        public bool FixedTimeStepsEnabled { get; private set; } // returns true if game is set to fixed time steps.
        public bool VerticalSyncEnabled { get; private set; } // returns true if vertical sync is enabled.
        public bool FullScreenEnabled { get; private set; } // returns true if full-screen is enabled.

        #endregion

        #region internal stuff

        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; } // graphics device manager for the game.

        #endregion

        public GraphicsManagerService(Game game)
            : base(game)
        {
            this.GraphicsDeviceManager = new GraphicsDeviceManager(this.Game); // init graphics device manager.
            this.Game.Services.AddService(typeof (IGraphicsManagerService), this); // register ourself as a service.

            // TODO read from configs.
            this.FullScreenEnabled = this.GraphicsDeviceManager.IsFullScreen = false;
            this.GraphicsDeviceManager.PreferredBackBufferWidth = 1280;
            this.GraphicsDeviceManager.PreferredBackBufferHeight = 724;
            this.FixedTimeStepsEnabled = this.Game.IsFixedTimeStep = true;
            this.VerticalSyncEnabled = this.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            this.GraphicsDeviceManager.ApplyChanges();
        }

        public void ToggleFixedTimeSteps()
        {
            this.FixedTimeStepsEnabled = !this.FixedTimeStepsEnabled;
            this.Game.IsFixedTimeStep = this.FixedTimeStepsEnabled;
            this.GraphicsDeviceManager.ApplyChanges();
        }

        public void ToggleVerticalSync(bool enabled)
        {
            this.VerticalSyncEnabled = enabled;
            this.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = this.VerticalSyncEnabled;
            this.GraphicsDeviceManager.ApplyChanges();
        }

        public void ToggleFullScreen(bool enabled)
        {
            this.FullScreenEnabled = enabled;
            this.GraphicsDeviceManager.IsFullScreen = this.FullScreenEnabled;
            this.GraphicsDeviceManager.ApplyChanges();
        }
    }
}
