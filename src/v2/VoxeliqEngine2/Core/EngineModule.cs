using Ninject.Modules;
using VolumetricStudios.VoxeliqEngine.Input;

namespace VolumetricStudios.VoxeliqEngine.Core
{
    // http://dotnetslackers.com/articles/csharp/Get-Started-with-Ninject-2-0-in-C-sharp-Programming.aspx
    public class EngineModule : NinjectModule
    {
        public static readonly EngineModule Engine = new EngineModule(); 

        internal EngineModule()
        { }

        public override void Load()
        {
            Bind<IGameService>().To<Game>().InSingletonScope();
            Bind<IInputService>().To<InputService>().InSingletonScope();
        }
    }
}
