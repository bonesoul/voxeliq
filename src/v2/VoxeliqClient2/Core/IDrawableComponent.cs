using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VolumetricStudios.VoxeliqClient.Core
{
    /// <summary>
    /// Drawable game component
    /// </summary>
    public interface IDrawableComponent : IComponent
    {
        void Draw();
    }
}
