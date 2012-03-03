/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqGame.Generators.Biomes;
using VolumetricStudios.VoxeliqGame.Generators.Terrain;
using VolumetricStudios.VoxeliqGame.Universe;

namespace VolumetricStudios.VoxeliqGame.Chunks.Processors
{
    /// <summary>
    /// Tasked chunk processor, which runs Process() as tasks.
    /// </summary>
    public class TaskedChunkProcessor : ChunkProcessor
    {
        /// <summary>
        /// Gets or sets if a dirty chunk exist.
        /// </summary>
        private bool Dirty { get; set; }

        public TaskedChunkProcessor(Game game, World world) 
            : base(game, world)
        {
            this.Generator = new MountainousTerrain(new RainForest()); 
        }

        public override void Initialize()
        {
            Logger.Trace("init()");
            base.Initialize();
        }

        protected override void Run()
        {
            this.Dirty = true;

            while (this.Active)
            {
                if (!this.Dirty) // if there is no dirty chunk (which awaits generation, lighting or building),
                    continue; // just return.

                this.Dirty = false; // set dirty to false;

                foreach (var chunk in this.World.Chunks.Values)
                {
                    Task.Factory.StartNew(Process, (object)chunk);

                    if (chunk.ChunkState != ChunkState.Ready) // if there exists a chunk which is dirty,
                        this.Dirty = true; // next process() should just run.
                }
            }
        }

        protected void Process(Object @object)
        {
            var chunk = (Chunk) @object;
            switch (chunk.ChunkState) // switch on the chunk state.
            {
                case ChunkState.AwaitingGenerate:
                    this.Generate(chunk);
                    break;
                case ChunkState.AwaitingLighting:
                    this.Lighten(chunk);
                    break;
                case ChunkState.AwaitingBuild:
                    this.Build(chunk);
                    break;
                case ChunkState.AwaitingRelighting:
                    this.Lighten(chunk);
                    break;
                case ChunkState.AwaitingRebuild:
                    this.Build(chunk);
                    break;
                case ChunkState.AwaitingRemoval:
                    break;
                case ChunkState.Generating:
                case ChunkState.Building:
                case ChunkState.Lighting:
                    break;
                case ChunkState.Ready:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
