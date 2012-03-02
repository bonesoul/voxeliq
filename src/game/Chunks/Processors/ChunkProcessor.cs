using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
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

        protected virtual void Run()
        {            
            throw new NotSupportedException();
        }

        protected virtual void Process()
        {
            throw new NotSupportedException();
        }
    }
}
