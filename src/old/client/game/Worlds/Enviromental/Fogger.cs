using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqEngine.Common.Logging;

namespace VolumetricStudios.VoxeliqClient.Worlds.Enviromental
{
    public class Fogger : GameComponent, IFogService
    {
        /// <summary>
        /// Fog state.
        /// </summary>
        public FogState State { get; private set; }

        /// <summary>
        /// Fog vector value for current fog-state.
        /// </summary>
        public Vector2 FogVector
        {
            get { return this._fogVectors[(byte)this.State]; }
        }

        /// <summary>
        /// Fog vectors.
        /// </summary>
        private readonly Vector2[] _fogVectors = new[]
        {
            new Vector2(0, 0),  // none
            new Vector2(175, 250),  // near
            new Vector2(250, 400) // far
        };


        /// <summary>
        /// Logging facility.
        /// </summary>
        private static readonly Logger Logger = LogManager.CreateLogger();

        public Fogger(Game game)
            :base(game)
        {
            Logger.Trace("init()");
            this.State = FogState.None;
            this.Game.Services.AddService(typeof(IFogService), this);
        }

        /// <summary>
        /// Toggles fog to near, far and none.
        /// </summary>
        public void ToggleFog()
        {
            switch (this.State)
            {
                case FogState.None:
                    this.State = FogState.Near;
                    break;
                case FogState.Near:
                    this.State = FogState.Far;
                    break;
                case FogState.Far:
                    this.State = FogState.None;
                    break;
            }
        }
    }

    /// <summary>
    /// Fog state enum.
    /// </summary>
    public enum FogState : byte
    {
        None,
        Near,
        Far
    }
}
