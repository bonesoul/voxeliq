/*
 * Copyright (C) 2011 voxeliq project 
 *
 */

using System.Threading.Tasks;
using VolumetricStudios.VoxeliqClient.Worlds;
using VolumetricStudios.VoxeliqEngine.Universe;

namespace VolumetricStudios.VoxeliqClient.Graphics.Builders
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
