/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using Engine.Assets;
using Engine.Chunks;
using Engine.Common.Logging;
using Engine.Graphics;
using Engine.Universe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Debugging.Ingame
{
    public interface IInGameDebuggerService
    {
        void ToggleInGameDebugger();
    }

    /// <summary>
    /// Allows drawing ingame debugger visuals.
    /// </summary>
    public interface IInGameDebuggable
    {
        /// <summary>
        /// Draws an ingame debug visual for game component.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device to draw.</param>
        /// <param name="camera">The camera.</param>
        /// <param name="spriteBatch">Sprite batch for fonts.</param>
        /// <param name="spriteFont">Font used for drawing strings.</param>
        void DrawInGameDebugVisual(GraphicsDevice graphicsDevice, ICamera camera, SpriteBatch spriteBatch,
                                   SpriteFont spriteFont);
    }

    public sealed class InGameDebugger : DrawableGameComponent, IInGameDebuggerService
    {
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private bool _active = false;

        // required services.
        private ICamera _camera;
        private IWorld _world;
        private IPlayer _player;
        private IChunkStorage _chunkStorage;
        private IAssetManager _assetManager;

        /// <summary>
        /// Logging facility.
        /// </summary>
        private static readonly Logger Logger = LogManager.CreateLogger();

        public InGameDebugger(Game game)
            : base(game)
        {
            game.Services.AddService(typeof (IInGameDebuggerService), this); // export service.
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            // import required service.
            this._camera = (ICamera) this.Game.Services.GetService(typeof (ICamera));
            this._world = (IWorld) this.Game.Services.GetService(typeof (IWorld));
            this._player = (IPlayer) this.Game.Services.GetService(typeof (IPlayer));
            this._chunkStorage = (IChunkStorage) this.Game.Services.GetService(typeof (IChunkStorage));
            this._assetManager = (IAssetManager)this.Game.Services.GetService(typeof(IAssetManager));
            
            if (this._assetManager == null)
                throw new NullReferenceException("Can not find asset manager component.");

            _spriteFont = this._assetManager.Verdana;
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!_active) return;
            var viewFrustrum = new BoundingFrustum(this._camera.View*this._camera.Projection);

            _spriteBatch.Begin();
            
            //foreach (Chunk chunk in this._chunkStorage.Values)
            //{
            //    if (chunk != this._player.CurrentChunk)
            //        continue;

            //    if (!chunk.BoundingBox.Intersects(viewFrustrum)) 
            //        continue;

            //    chunk.DrawInGameDebugVisual(Game.GraphicsDevice, _camera, _spriteBatch, _spriteFont);
            //}

            _player.Weapon.DrawInGameDebugVisual(Game.GraphicsDevice, _camera, _spriteBatch, _spriteFont);

            _spriteBatch.End();
        }

        public void ToggleInGameDebugger()
        {
            _active = !_active;
        }
    }
}