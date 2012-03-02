using System;
using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqGame.Generators.Biomes;
using VolumetricStudios.VoxeliqGame.Generators.Terrain;
using VolumetricStudios.VoxeliqGame.Processors;
using VolumetricStudios.VoxeliqGame.Universe;

namespace VolumetricStudios.VoxeliqGame.Chunks.Processors
{
    public class SingleThreadProcessor:ChunkProcessor
    {
        /// <summary>
        /// Gets or sets if a dirty chunk exist.
        /// </summary>
        private bool Dirty { get; set; }

        public SingleThreadProcessor(Game game, World world) 
            : base(game, world)
        {
            this.Generator = new MountainousTerrain(new RainForest()); 
        }

        protected override void Run()
        {
            this.Dirty = true;

            while (this.Active)
            {
                this.Process();
            }
        }

        protected override void Process()
        {
            if (!this.Dirty) // if there is no dirty chunk (which awaits generation, lighting or building),
                return;  // just return.

            this.Dirty = false; // set dirty to false;

            foreach (var chunk in this.World.Chunks.Values)
            {
                switch(chunk.ChunkState)
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

        private void Generate(Chunk chunk)
        {
            chunk.ChunkState = ChunkState.Generating;
            Generator.Generate(chunk);
            chunk.ChunkState = ChunkState.AwaitingLighting;
        }

        private void Lighten(Chunk chunk)
        {
            chunk.ChunkState = ChunkState.Lighting;
            Lightning.Process(chunk);
            chunk.ChunkState=ChunkState.AwaitingBuild;
        }

        private void Build(Chunk chunk)
        {
            chunk.ChunkState = ChunkState.Building;
            this.VertexBuilder.Build(chunk);
            chunk.ChunkState = ChunkState.Ready;
        }
    }
}
