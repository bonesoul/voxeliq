/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
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