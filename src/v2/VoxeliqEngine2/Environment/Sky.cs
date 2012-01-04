using VolumetricStudios.VoxeliqEngine.Core;
using VolumetricStudios.VoxeliqEngine.Utility.Logging;

namespace VolumetricStudios.VoxeliqEngine.Environment
{
    public class Sky : GameComponent
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        public Sky(Game game)
            :base(game, true)
        {
            Logger.Trace("init()");
        }

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
