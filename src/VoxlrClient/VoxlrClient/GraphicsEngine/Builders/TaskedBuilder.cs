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

using System.Threading.Tasks;
using VolumetricStudios.VoxlrClient.GameEngine;
using VolumetricStudios.VoxlrEngine.Universe;

namespace VolumetricStudios.VoxlrClient.GraphicsEngine.Builders
{
    // pattern used: http://stackoverflow.com/questions/3700724/what-is-the-blockingcollection-takefromany-method-useful-for

    public class TaskedBuilder : ChunkBuilder
    {
        public TaskedBuilder(IPlayer player, GameWorld world) : base(player, world) { }

        protected override void QueueChunks()
        {
            foreach (Chunk chunk in _world.Chunks.Values)
            {
                if (!chunk.Generated && !chunk.QueuedForGeneration)
                {
                    chunk.QueuedForGeneration = true;
                    this._generationQueue.Add(chunk);
                    Task.Factory.StartNew(Process);
                }

                if (chunk.Dirty && !chunk.QueuedForBuilding)
                {
                    chunk.QueuedForBuilding = true;
                    this._buildingQueue.Add(chunk);
                    Task.Factory.StartNew(Process);
                }
            }
        }
    }
}
