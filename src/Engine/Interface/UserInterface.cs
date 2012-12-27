/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxeliqEngine.Assets;
using VoxeliqEngine.Logging;

namespace VoxeliqEngine.Interface
{
    public class UserInterface : DrawableGameComponent
    {
        private Texture2D _crosshairNormalTexture;
        private Texture2D _crosshairShovelTexture;
        private SpriteBatch _spriteBatch;
        private IPlayer _player;

        /// <summary>
        /// Logging facility.
        /// </summary>
        private static readonly Logger Logger = LogManager.CreateLogger();

        public UserInterface(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            this._player = (IPlayer) this.Game.Services.GetService(typeof (IPlayer));
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            this.LoadContent();
        }

        protected override void LoadContent()
        {
            this._crosshairNormalTexture = AssetManager.Instance.CrossHairNormalTexture;
            this._crosshairShovelTexture = AssetManager.Instance.CrossHairShovelTexture;
        }

        public override void Draw(GameTime gameTime)
        {
            // draw cross-hair.            
            var crosshairTexture = this._player.AimedSolidBlock.HasValue
                                       ? _crosshairShovelTexture
                                       : _crosshairNormalTexture;

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            _spriteBatch.Draw(crosshairTexture,
                              new Vector2((Game.GraphicsDevice.Viewport.Width/2) - 10,
                                          (Game.GraphicsDevice.Viewport.Height/2) - 10), Color.White);
            _spriteBatch.End();
        }
    }
}