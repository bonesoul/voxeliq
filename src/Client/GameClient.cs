/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Microsoft.Xna.Framework;

namespace Client
{
    public class GameClient : Game
    {
        // Graphics device manager for the game.
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        /// <summary>
        /// Initializes a new GameClient instance.
        /// </summary>
        public GameClient()
        {
            this.Content.RootDirectory = "Content";
            this._graphicsDeviceManager = new GraphicsDeviceManager(this);
        }

        /// <summary>
        /// Game's update method.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// Game's draw method.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            var skyColor = new Color(128, 173, 254);
            this.GraphicsDevice.Clear(skyColor);
        }
    }
}
