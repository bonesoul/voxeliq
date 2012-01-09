/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqGame.Universe;

namespace VolumetricStudios.VoxeliqGame.Chunks.Builders
{
    // pattern used: http://stackoverflow.com/questions/3700724/what-is-the-blockingcollection-takefromany-method-useful-for

    public class TaskedBuilder : ChunkBuilder
    {
        public TaskedBuilder(Game game, IPlayer player, World world) 
            : base(game, player, world) { }

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
