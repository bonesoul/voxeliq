using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqEngine.Logging;

namespace VolumetricStudios.Voxeliq.Graphics
{
    public interface IGraphicsManager
    {
        /// <summary>
        /// Returns true if game is set to fixed time steps.
        /// </summary>
        bool IsFixedTimeStep { get; }

        /// <summary>
        /// Returns true if vertical sync is enabled.
        /// </summary>
        bool VerticalSyncEnabled { get; }

        /// <summary>
        /// Returns true if full-screen is enabled.
        /// </summary>
        bool FullScreenEnabled { get; }

        void ToggleFixedTimeSteps();
        void ToggleVerticalSync();
    }

    public class GraphicsManager : IGraphicsManager
    {
        public bool IsFixedTimeStep { get; private set; } // Returns true if game is set to fixed time steps.
        public bool VerticalSyncEnabled { get; private set; } // Returns true if vertical sync is enabled.
        public bool FullScreenEnabled { get; private set; } // Returns true if full-screen is enabled.

        private readonly Game _game; // the attached game.
        private readonly GraphicsDeviceManager _graphicsDeviceManager; // attached graphics device manager.
        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility.   

        public GraphicsManager(Game game, GraphicsDeviceManager graphicsDeviceManager)
        {
            Logger.Trace("init()");

            this._game = game;
            this._graphicsDeviceManager = graphicsDeviceManager;
            this._game.Services.AddService(typeof(IGraphicsManager), this); // export service.

            this.FullScreenEnabled = this._graphicsDeviceManager.IsFullScreen = GraphicsConfig.Instance.FullScreenEnabled;
            this._graphicsDeviceManager.PreferredBackBufferWidth = GraphicsConfig.Instance.Width;
            this._graphicsDeviceManager.PreferredBackBufferHeight = GraphicsConfig.Instance.Height;
            this.IsFixedTimeStep = this._game.IsFixedTimeStep = false;
            this.VerticalSyncEnabled = this._graphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
            this._graphicsDeviceManager.ApplyChanges();
        }

        /// <summary>
        /// Toggles fixed time steps.
        /// </summary>
        public void ToggleFixedTimeSteps()
        {
            this.IsFixedTimeStep = !this.IsFixedTimeStep;
            this._game.IsFixedTimeStep = this.IsFixedTimeStep;
            this._graphicsDeviceManager.ApplyChanges();
        }

        /// <summary>
        /// Toggles vertical syncs.
        /// </summary>
        public void ToggleVerticalSync()
        {
            this.VerticalSyncEnabled = !this.VerticalSyncEnabled;
            this._graphicsDeviceManager.SynchronizeWithVerticalRetrace = this.VerticalSyncEnabled;
            this._graphicsDeviceManager.ApplyChanges();
        }

        /// <summary>
        /// Sets full screen on or off.
        /// </summary>
        /// <param name="enabled"></param>
        public void EnableFullScreen(bool enabled)
        {
            this.FullScreenEnabled = enabled;
            this._graphicsDeviceManager.IsFullScreen = this.FullScreenEnabled;
            this._graphicsDeviceManager.ApplyChanges();
        }
    }
}
