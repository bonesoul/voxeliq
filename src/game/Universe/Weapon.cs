/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VolumetricStudios.VoxeliqGame.Universe
{
    // TODO: should extend from Item.
    public class Weapon:DrawableGameComponent,IInGameDebuggable
    {
        public Weapon(Game game) : base(game) { }

        public virtual void Use() { }
        public virtual void SecondaryUse() { }

        public virtual void PrintDebugInfo(GraphicsDevice graphicsDevice, ICameraService camera, SpriteBatch spriteBatch, SpriteFont spriteFont) { }
    }
}
