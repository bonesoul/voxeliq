/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using VolumetricStudios.VoxeliqEngine.Chunks;
using VolumetricStudios.VoxeliqEngine.Universe;

namespace VolumetricStudios.VoxeliqEngine.Builders
{
    // pattern used: http://stackoverflow.com/questions/3700724/what-is-the-blockingcollection-takefromany-method-useful-for

    public class QueuedBuilder: ChunkBuilder
    {
        public QueuedBuilder(IPlayer player, World world) : base(player, world) { }

        protected override void QueueChunks()
        {
            foreach (Chunk chunk in World.Chunks.Values)
            {
                if (!chunk.Generated && !chunk.QueuedForGeneration)
                {
                    chunk.QueuedForGeneration = true;
                    this.GenerationQueue.Add(chunk);
                }

                if (chunk.Dirty && !chunk.QueuedForBuilding)
                {
                    chunk.QueuedForBuilding = true;
                    this.BuildingQueue.Add(chunk);
                }
            }

            int count = GenerationQueue.Count + BuildingQueue.Count;
            for (int i = 0; i < count ; i++)
            {
                Process();
            }
        }
    }
}
