/*
 * Copyright (C) 2011 - Hüseyin Uslu shalafiraistlin@gmail.com
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

namespace VolumetricStudios.VoxlrClient.States
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
