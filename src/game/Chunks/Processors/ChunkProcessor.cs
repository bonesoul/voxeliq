/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqGame.Common.Logging;
using VolumetricStudios.VoxeliqGame.Generators.Terrain;
using VolumetricStudios.VoxeliqGame.Processors;
using VolumetricStudios.VoxeliqGame.Universe;

namespace VolumetricStudios.VoxeliqGame.Chunks.Processors
{
    /// <summary>
    /// Processes chunks.
    /// </summary>
    public class ChunkProcessor : GameComponent
    {
        /// <summary>
        /// Returns if processor is active.
        /// </summary>
        public bool Active { get; private set; }

        /// <summary>
        /// The bound world.
        /// </summary>
        protected World World;

        /// <summary>
        /// The terrain generator.
        /// </summary>
        protected TerrainGenerator Generator { get; set; }

        /// <summary>
        /// The chunk vertex builder.
        /// </summary>
        protected IVertexBuilder VertexBuilder { get; set; }

        /// <summary>
        /// Logging facility.
        /// </summary>
        protected static readonly Logger Logger = LogManager.CreateLogger();

        public ChunkProcessor(Game game, World world)
            : base(game)
        {
            this.Active = false;
            this.World = world;
        }

        public override void Initialize()
        {
            // import required services.
            this.VertexBuilder = (IVertexBuilder)this.Game.Services.GetService(typeof(IVertexBuilder));

            this.Active = true;
            Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Run the chunk processor.
        /// </summary>
        protected virtual void Run()
        {            
            throw new NotSupportedException(); // method should be overloadded.
        }

        /// <summary>
        /// Generates the chunk.
        /// </summary>
        /// <param name="chunk">Chunk to be generated.</param>
        protected void Generate(Chunk chunk)
        {
            chunk.ChunkState = ChunkState.Generating; // set chunk state to generating.
            Generator.Generate(chunk);
            chunk.ChunkState = ChunkState.AwaitingLighting; // chunk should be lighten now.
        }

        /// <summary>
        /// Ligtens the chunk (calculates the lighting amount on chunks blocks).
        /// </summary>
        /// <param name="chunk">Chunk to lighten.</param>
        protected void Lighten(Chunk chunk)
        {
            chunk.ChunkState = ChunkState.Lighting; // set chunk state to generating.
            Lightning.Process(chunk);
            chunk.ChunkState = ChunkState.AwaitingBuild; // chunk should be built now.
        }

        /// <summary>
        /// Builds the chunk (calculates vertexes and indices).
        /// </summary>
        /// <param name="chunk">Chunk to build</param>
        protected void Build(Chunk chunk)
        {
            chunk.ChunkState = ChunkState.Building; // set chunk state to building.
            this.VertexBuilder.Build(chunk);
            chunk.ChunkState = ChunkState.Ready; // chunk is al ready now.
        }
    }
}
