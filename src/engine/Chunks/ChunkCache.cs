using Microsoft.Xna.Framework;

namespace VolumetricStudios.VoxeliqEngine.Chunks
{
    public interface IChunkCache
    {
        
    }

    /// <summary>
    /// The chunk cache & manager.
    /// </summary>
    public class ChunkCache: DrawableGameComponent, IChunkCache
    {
        // required services.
        private IChunkStorage _chunkStorage;

        public ChunkCache(Game game) 
            : base(game)
        {
            this.Game.Services.AddService(typeof(IChunkCache), this); // export service.
        }

        public override void Initialize()
        {
            // import required services.
            this._chunkStorage = (IChunkStorage) this.Game.Services.GetService(typeof (IChunkStorage));
        }
    }
}
