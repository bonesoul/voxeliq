using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VolumetricStudios.VoxeliqEngine.Core
{
    public class GameComponent
    {
        protected Game Game { get; private set; }
        public bool Drawable { get; private set; }

        public GameComponent(Game game, bool drawable = false)
        {
            this.Game = game;
            this.Drawable = drawable;
        }

        public virtual void Initialize()
        { }

        public virtual void Update(GameTime gameTime)
        { }

        public virtual void Draw(GameTime gameTime)
        { }
    }
}
