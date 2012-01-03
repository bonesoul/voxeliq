using VolumetricStudios.VoxeliqClient.Input;
using VolumetricStudios.VoxeliqEngine.Core;

namespace VolumetricStudios.VoxeliqClient
{
    public class Client : Game
    {
        public Client()
        {
            var inputManager = new InputService(this);
            this.AddComponent(inputManager);
        }
    }
}
