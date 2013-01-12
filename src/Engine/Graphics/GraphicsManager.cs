/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Microsoft.Xna.Framework;
using VoxeliqEngine.Common.Logging;

namespace VoxeliqEngine.Graphics
{
    /// <summary>
    /// Screen service for controlling screen.
    /// </summary>
    public interface IGraphicsManager
    {
        /// <summary>
        /// Enables or disabled fixed time steps.
        /// </summary>
        bool FixedTimeStepsEnabled { get; set; }

        /// <summary>
        /// Enables or disables vertical sync.
        /// </summary>
        bool VerticalSyncEnabled { get; set; }

        /// <summary>
        /// Enables or disables the full-screen mode.
        /// </summary>
        bool FullScreenEnabled { get; set; }
    }

    /// <summary>
    /// The screen manager that controls various graphical aspects.
    /// </summary>
    public sealed class GraphicsManager : IGraphicsManager
    {
        /// <summary>
        /// Enables or disabled fixed time steps.
        /// </summary>
        public bool FixedTimeStepsEnabled
        {
            get { return this._graphicsDeviceManager.SynchronizeWithVerticalRetrace; }
            set
            {
                this._graphicsDeviceManager.SynchronizeWithVerticalRetrace = value;
                this._game.IsFixedTimeStep = value;
                this._graphicsDeviceManager.ApplyChanges();
            }
        }

        /// <summary>
        /// Enables or disables vertical sync.
        /// </summary>
        public bool VerticalSyncEnabled
        {
            get { return this._graphicsDeviceManager.SynchronizeWithVerticalRetrace; }
            set
            {
                this._graphicsDeviceManager.SynchronizeWithVerticalRetrace = value;
                this._game.IsFixedTimeStep = value;
                this._graphicsDeviceManager.ApplyChanges();
            }
        }

        /// <summary>
        /// Enables or disables the full-screen mode.
        /// </summary>
        public bool FullScreenEnabled
        {
            get { return this._graphicsDeviceManager.IsFullScreen; }
            set
            {
                this._graphicsDeviceManager.IsFullScreen = value;
                this._graphicsDeviceManager.ApplyChanges();
            }
        }

        // Returns true if full-screen is enabled.

        // principal stuff
        private readonly Game _game; // the attached game.
        private readonly GraphicsDeviceManager _graphicsDeviceManager; // attached graphics device manager.

        // misc
        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility.

        public GraphicsManager(GraphicsDeviceManager graphicsDeviceManager, Game game)
        {
            Logger.Trace("ctor()");

            this._game = game;
            this._graphicsDeviceManager = graphicsDeviceManager;
            this._game.Services.AddService(typeof (IGraphicsManager), this); // export service.

            this.FullScreenEnabled =  GraphicsConfig.Instance.FullScreenEnabled;
            this._graphicsDeviceManager.PreferredBackBufferWidth = GraphicsConfig.Instance.Width;
            this._graphicsDeviceManager.PreferredBackBufferHeight = GraphicsConfig.Instance.Height;
            this.FixedTimeStepsEnabled = this._game.IsFixedTimeStep = GraphicsConfig.Instance.FixedTimeStepsEnabled;
            this.VerticalSyncEnabled = this._graphicsDeviceManager.SynchronizeWithVerticalRetrace = GraphicsConfig.Instance.VerticalSyncEnabled;
            this._graphicsDeviceManager.ApplyChanges();
        }
    }
}