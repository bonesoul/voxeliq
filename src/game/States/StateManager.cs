/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using Microsoft.Xna.Framework;

namespace VolumetricStudios.VoxeliqGame.States
{
    public interface IStateManager
    {
        GameState ActiveState { get; set; }
    }

    // Implement using Microsoft XNA Unleashed.

    /// <summary>
    /// Game state manager.
    /// </summary>
    public class StateManager : DrawableGameComponent, IStateManager
    {
        private GameState _activeState = null;

        public StateManager(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IStateManager), this);
        }

        public GameState ActiveState
        {
            get { return this._activeState; }
            set
            {
                this._activeState = value;
                this.ActiveState.Initialize();
                this.ActiveState.LoadContent();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (ActiveState == null) return;
            ActiveState.Update(gameTime);
        }
    }
}
