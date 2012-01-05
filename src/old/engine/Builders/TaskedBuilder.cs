/*
 * Copyright (C) 2011 voxeliq project 
 *
 */

using System.Threading.Tasks;
using VolumetricStudios.VoxeliqEngine.Chunks;
using VolumetricStudios.VoxeliqEngine.Universe;

namespace VolumetricStudios.VoxeliqEngine.Builders
{
    // pattern used: http://stackoverflow.com/questions/3700724/what-is-the-blockingcollection-takefromany-method-useful-for

    public class TaskedBuilder : ChunkBuilder
    {
        public TaskedBuilder(IPlayer player, World world) : base(player, world) { }

        protected override void QueueChunks()
        {
            foreach (Chunk chunk in World.Chunks.Values)
            {
                if (!chunk.Generated && !chunk.QueuedForGeneration)
                {
                    chunk.QueuedForGeneration = true;
                    this.GenerationQueue.Add(chunk);
                    Task.Factory.StartNew(Process);
                }

                if (chunk.Dirty && !chunk.QueuedForBuilding)
                {
                    chunk.QueuedForBuilding = true;
                    this.BuildingQueue.Add(chunk);
                    Task.Factory.StartNew(Process);
                }
            }
        }
    }
}
