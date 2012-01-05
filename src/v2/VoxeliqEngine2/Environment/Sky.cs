using SlimDX;
using VolumetricStudios.VoxeliqEngine.Core;
using VolumetricStudios.VoxeliqEngine.Screen;
using VolumetricStudios.VoxeliqEngine.Utility.Logging;

namespace VolumetricStudios.VoxeliqEngine.Environment
{
    public class Sky : GameComponent
    {
        private IGameWindow _gameWindow;
        private static readonly Logger Logger = LogManager.CreateLogger();

        public Sky(Game game)
            :base(game, true)
        {
            
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            // import required services.
            this._gameWindow = (IGameWindow)this.Game.GetService(typeof(IGameWindow));
        }

        public override void Draw(GameTime gameTime)
        {
            this._gameWindow.Device.ClearRenderTargetView(this._gameWindow.RenderTarget, new Color4(245, 245, 245,255));
            // TODO: distable the depth-buffer.
        }
    }
}
