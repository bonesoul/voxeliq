/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxeliqGame.Debugging;
using VolumetricStudios.VoxeliqGame.Graphics;

namespace VolumetricStudios.VoxeliqGame.Universe
{
    public class Weapon : DrawableGameComponent, IInGameDebuggable
    {
        public Weapon(Game game) : base(game)
        {
        }

        public virtual void Use()
        {
        }

        public virtual void SecondaryUse()
        {
        }

        public virtual void DrawInGameDebugVisual(GraphicsDevice graphicsDevice, ICamera camera, SpriteBatch spriteBatch,
                                                  SpriteFont spriteFont)
        {
        }
    }
}