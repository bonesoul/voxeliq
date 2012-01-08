using Microsoft.Xna.Framework;

namespace VolumetricStudios.VoxeliqGame.Chunks
{
    public interface IChunkCache
    {
        
    }

    /// <summary>
    /// The chunk cache & manager.
    /// </summary>
    class ChunkCache: DrawableGameComponent, IChunkCache
    {
        public ChunkCache(Game game) 
            : base(game)
        {
        }
    }
}
