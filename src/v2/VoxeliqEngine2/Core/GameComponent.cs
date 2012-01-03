using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VolumetricStudios.VoxeliqEngine.Core
{
    public class GameComponent
    {
        public bool Drawable { get; private set; }

        public GameComponent(bool drawable = false)
        {
            this.Drawable = drawable;
        }

        public virtual void Initialize()
        { }

        public virtual void Update()
        { }

        public virtual void Draw()
        { }
    }
}
