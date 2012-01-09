/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxeliqGame.Chunks;
using VolumetricStudios.VoxeliqGame.Common.Logging;
using VolumetricStudios.VoxeliqGame.Graphics;
using VolumetricStudios.VoxeliqGame.Universe;

namespace VolumetricStudios.VoxeliqGame.Debugging
{
    public interface IInGameDebuggerService
    {
        void ToggleInGameDebugger();
    }

    public interface IInGameDebuggable
    {
        void PrintDebugInfo(GraphicsDevice graphicsDevice, ICamera camera, SpriteBatch spriteBatch, SpriteFont spriteFont);
    }

    public sealed class InGameDebugger:DrawableGameComponent, IInGameDebuggerService
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
            game.Services.AddService(typeof(IInGameDebuggerService), this); // export service.
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _spriteFont = Game.Content.Load<SpriteFont>("Fonts//CalibriDebug");

            // import service.
            this._camera = (ICamera)this.Game.Services.GetService(typeof(ICamera));
            this._world = (IWorld)this.Game.Services.GetService(typeof(IWorld));
            this._player = (IPlayer)this.Game.Services.GetService(typeof(IPlayer));
            this._chunkStorage = (IChunkStorage)this.Game.Services.GetService(typeof(IChunkStorage));
        }

        public override void Draw(GameTime gameTime)
        {
            if (!_active) return;
            var viewFrustrum = new BoundingFrustum(this._camera.View * this._camera.Projection);

            _spriteBatch.Begin();

            foreach (Chunk chunk in this._chunkStorage.Values)
            {
                if (!chunk.BoundingBox.Intersects(viewFrustrum)) continue;
                chunk.PrintDebugInfo(Game.GraphicsDevice, _camera, _spriteBatch, _spriteFont);
            }

            _player.Weapon.PrintDebugInfo(Game.GraphicsDevice, _camera, _spriteBatch, _spriteFont);

            _spriteBatch.End();

        }

        public void ToggleInGameDebugger()
        {
            _active = !_active;
        }
    }
}
