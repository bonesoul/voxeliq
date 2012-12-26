/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxeliqEngine.Debugging;
using VoxeliqEngine.Graphics;

namespace VoxeliqEngine.Universe
{
    public class Weapon : DrawableGameComponent, IInGameDebuggable
    {
        public Weapon(Game game) : base(game)
        { }

        public virtual void Use()
        { }

        public virtual void SecondaryUse()
        { }

        public virtual void DrawInGameDebugVisual(GraphicsDevice graphicsDevice, ICamera camera, SpriteBatch spriteBatch, SpriteFont spriteFont)
        { }
    }
}