using VolumetricStudios.VoxeliqClient.Input;
using VolumetricStudios.VoxeliqClient.Screen;
using VolumetricStudios.VoxeliqEngine.Core;
using VolumetricStudios.VoxeliqEngine.Environment;

namespace VolumetricStudios.VoxeliqClient
{
    public class Client : Game
    {
        public Client()
        {
            this.AddComponent(new InputService(this));
            this.AddComponent(new Camera(this));
            this.AddComponent(new Sky(this));
        }
    }
}
