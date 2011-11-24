/*    
 * Copyright (C) 2011, Hüseyin Uslu
 *  
 */

using System.Threading;
using Microsoft.Xna.Framework;
using VolumetricStudios.VoxlrClient.Input;
using VolumetricStudios.VoxlrClient.Interface;
using VolumetricStudios.VoxlrClient.Interface.Debug;
using VolumetricStudios.VoxlrClient.Screen;
using VolumetricStudios.VoxlrEngine;
using VolumetricStudios.VoxlrEngine.Universe;

namespace VolumetricStudios.VoxlrClient
{
    public class Universe : GameComponent
    {
        private World _world;
        private bool WaitForDebugger=false;

        public Universe(Game game):base(game) { }

        public override void Initialize()
        {
            if (WaitForDebugger) Thread.Sleep(5000);

            Game.Components.Add(new InputManager(this.Game) { UpdateOrder = 1 });
            Game.Components.Add(new Sky(this.Game) {UpdateOrder = 2, DrawOrder = 0});
            this._world = new World(this.Game) { UpdateOrder = 3, DrawOrder = 1 };
            Game.Components.Add(this._world);
            Game.Components.Add(new Camera(this.Game) { UpdateOrder = 5 });
            Game.Components.Add(new Player(this.Game, this._world) { UpdateOrder = 4,DrawOrder = 2 });
            Game.Components.Add(new UserInterface(this.Game) { UpdateOrder = 6, DrawOrder = 3 });
            Game.Components.Add(new InGameDebugger(this.Game) { UpdateOrder = 7, DrawOrder = 4 });
            Game.Components.Add(new Statistics(this.Game) { UpdateOrder = 8, DrawOrder = 5 });
        }
    }
}
