using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VolumetricStudios.VoxeliqEngine.Core
{
    /// <summary>
    /// An updateable component that only engine itself can use for critical services.
    /// </summary>
    public interface IService
    {
        void Update();
    }
}
