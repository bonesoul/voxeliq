/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using System;
using System.Threading;
using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqGame.Generators.Biomes;
using VolumetricStudios.VoxeliqGame.Generators.Terrain;
using VolumetricStudios.VoxeliqGame.Universe;

namespace VolumetricStudios.VoxeliqGame.Chunks.Processors
{
    /// <summary>
    /// Single threaded chunk processor which processes (generation, lightning, building) in a single thread.
    /// </summary>
    public class SingleThreadedChunkProcessor:ChunkProcessor
    {
        /// <summary>
        /// Gets or sets if a dirty chunk exist.
        /// </summary>
        private bool Dirty { get; set; }

        /// <summary>
        /// Creates a new SingleThreadProcessor.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="world"></param>
        public SingleThreadedChunkProcessor(Game game, World world) 
            : base(game, world)
        {
            this.Generator = new BasicTerrain(new RainForest()); 
        }

        public override void Initialize()
        {
            Logger.Trace("init()");
            base.Initialize();
        }

        protected override void Run()
        {
            this.Dirty = true;

            var processorThread = new Thread(ProcessorThread) { IsBackground = true };
            processorThread.Start();
        }

        private void ProcessorThread()
        {
            while (this.Active)
            {
                if (!this.Dirty) // if there is no dirty chunk (which awaits generation, lighting or building),
                    continue;  // just return.

                this.Process();
            }            
        }

        protected void Process()
        {
            this.Dirty = false; // set dirty to false;

            foreach (var chunk in this.World.Chunks.Values)
            {
                switch(chunk.ChunkState) // switch on the chunk state.
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

                if (chunk.ChunkState != ChunkState.Ready) // if there exists a chunk which is dirty,
                    this.Dirty = true; // next process() should just run.
            }
        }
    }
}
