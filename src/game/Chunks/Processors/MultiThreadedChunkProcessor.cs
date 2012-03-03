using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqGame.Generators.Biomes;
using VolumetricStudios.VoxeliqGame.Generators.Terrain;
using VolumetricStudios.VoxeliqGame.Universe;

namespace VolumetricStudios.VoxeliqGame.Chunks.Processors
{
    public class MultiThreadedChunkProcessor:ChunkProcessor
    {
        public bool NeedGeneration { get; private set; }
        public bool NeedLightning { get; private set; }
        public bool NeedBuilding { get; private set; }

        /// <summary>
        /// Multi threaded chunk processor which generates, ligtens and builds in different threads.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="world"></param>
        public MultiThreadedChunkProcessor(Game game, World world) 
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
            this.NeedGeneration = true;
            this.NeedLightning = true;
            this.NeedBuilding = true;

            var generationThread = new Thread(ProcessGeneration) {IsBackground = true};
            generationThread.Start();

            var lightingThread = new Thread(ProcessLightning) { IsBackground = true };
            lightingThread.Start();

            var buildingThread = new Thread(ProcessBuilding) { IsBackground = true };
            buildingThread.Start();
        }

        private void ProcessGeneration()
        {
            while (Active)
            {
                if(!this.NeedGeneration)
                    continue;

                foreach (var chunk in this.World.Chunks.Values)
                {
                    switch (chunk.ChunkState) // switch on the chunk state.
                    {
                        case ChunkState.AwaitingGenerate:
                            this.Generate(chunk);
                            break;
                        default:
                            break;
                    }
                }

                this.NeedGeneration = false;
                this.NeedLightning = true;
            }
        }

        private void ProcessLightning()
        {
            while (Active)
            {
                if(!this.NeedLightning)
                    continue;

                foreach (var chunk in this.World.Chunks.Values)
                {
                    switch (chunk.ChunkState) // switch on the chunk state.
                    {
                        case ChunkState.AwaitingLighting:
                            this.Lighten(chunk);
                            break;
                        default:
                            break;
                    }
                }

                this.NeedLightning = false;
                this.NeedBuilding = true;
            }
        }

        private void ProcessBuilding()
        {
            while (Active)
            {
                if(!this.NeedBuilding)
                    continue;

                foreach (var chunk in this.World.Chunks.Values)
                {
                    switch (chunk.ChunkState) // switch on the chunk state.
                    {
                        case ChunkState.AwaitingBuild:
                            this.Build(chunk);
                            break;
                        default:
                            break;
                    }
                }

                this.NeedBuilding = false;
            }
        }
    }
}
