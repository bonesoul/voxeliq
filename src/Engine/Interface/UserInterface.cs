/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using Engine.Assets;
using Engine.Common.Logging;
using Engine.Universe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Interface
{
    public class UserInterface : DrawableGameComponent
    {
        private Texture2D _crosshairNormalTexture;
        private Texture2D _crosshairShovelTexture;
        private SpriteBatch _spriteBatch;
        
        private IPlayer _player;
        private IAssetManager _assetManager;

        /// <summary>
        /// Logging facility.
        /// </summary>
        private static readonly Logger Logger = LogManager.CreateLogger();

        public UserInterface(Game game)
            : base(game)
        { }

        public override void Initialize()
        {
            Logger.Trace("init()");

            // import required services.
            this._player = (IPlayer) this.Game.Services.GetService(typeof (IPlayer));
            if (this._player == null)
                throw new NullReferenceException("Can not find player component.");

            this._assetManager = (IAssetManager)this.Game.Services.GetService(typeof(IAssetManager));
            if (this._assetManager == null)
                throw new NullReferenceException("Can not find asset manager component.");

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            this.LoadContent();
        }

        protected override void LoadContent()
        {
            this._crosshairNormalTexture = this._assetManager.CrossHairNormalTexture;
            this._crosshairShovelTexture = this._assetManager.CrossHairShovelTexture;
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