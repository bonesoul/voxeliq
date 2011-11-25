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

using System.Threading;
using Microsoft.Xna.Framework;
using VolumetricStudios.VoxlrClient.Input;
using VolumetricStudios.VoxlrClient.Interface;
using VolumetricStudios.VoxlrClient.Interface.Debug;
using VolumetricStudios.VoxlrClient.Screen;
using VolumetricStudios.VoxlrEngine;
using VolumetricStudios.VoxlrEngine.Universe;

namespace VolumetricStudios.VoxlrClient.GameEngine
{
    public class Universe : GameComponent
    {
        private World _world;
        private bool WaitForDebugger=false;

        public Universe(Microsoft.Xna.Framework.Game game):base(game) { }

        public override void Initialize()
        {
            if (WaitForDebugger) Thread.Sleep(5000);

            Game.Components.Add(new InputManager(this.Game) { UpdateOrder = 1 });
            Game.Components.Add(new Sky(this.Game) {UpdateOrder = 2, DrawOrder = 0});
            this._world = new GameWorld(this.Game) {UpdateOrder = 3, DrawOrder = 1};
            Game.Components.Add(this._world);
            Game.Components.Add(new Camera(this.Game) { UpdateOrder = 5 });
            Game.Components.Add(new Player(this.Game, this._world) { UpdateOrder = 4,DrawOrder = 2 });
            Game.Components.Add(new UserInterface(this.Game) { UpdateOrder = 6, DrawOrder = 3 });
            Game.Components.Add(new InGameDebugger(this.Game) { UpdateOrder = 7, DrawOrder = 4 });
            Game.Components.Add(new Statistics(this.Game) { UpdateOrder = 8, DrawOrder = 5 });
        }
    }
}
