/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxeliqEngine.Assets;
using VoxeliqEngine.Chunks;
using VoxeliqEngine.Graphics;
using VoxeliqEngine.Logging;
using VoxeliqEngine.Universe;

namespace VoxeliqEngine.Debugging
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

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _spriteFont = AssetManager.Instance.Verdana;

            // import service.
            this._camera = (ICamera) this.Game.Services.GetService(typeof (ICamera));
            this._world = (IWorld) this.Game.Services.GetService(typeof (IWorld));
            this._player = (IPlayer) this.Game.Services.GetService(typeof (IPlayer));
            this._chunkStorage = (IChunkStorage) this.Game.Services.GetService(typeof (IChunkStorage));
        }

        public override void Draw(GameTime gameTime)
        {
            if (!_active) return;
            var viewFrustrum = new BoundingFrustum(this._camera.View*this._camera.Projection);

            _spriteBatch.Begin();

            foreach (Chunk chunk in this._chunkStorage.Values)
            {
                if (!chunk.BoundingBox.Intersects(viewFrustrum)) continue;
                chunk.DrawInGameDebugVisual(Game.GraphicsDevice, _camera, _spriteBatch, _spriteFont);
            }

            _player.Weapon.DrawInGameDebugVisual(Game.GraphicsDevice, _camera, _spriteBatch, _spriteFont);

            _spriteBatch.End();
        }

        public void ToggleInGameDebugger()
        {
            _active = !_active;
        }
    }
}