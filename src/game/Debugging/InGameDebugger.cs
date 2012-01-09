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

    public sealed class InGameDebugger:DrawableGameComponent, IInGameDebuggerService
    {
        private ICameraService _camera;
        private IWorldService _world;
        private IPlayer _player;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private bool _active = false;

        /// <summary>
        /// Logging facility.
        /// </summary>
        private static readonly Logger Logger = LogManager.CreateLogger();

        public InGameDebugger(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IInGameDebuggerService), this);
        }

        public void ToggleInGameDebugger()
        {
            _active = !_active;
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            this._camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));
            this._world = (IWorldService)this.Game.Services.GetService(typeof(IWorldService));
            this._player = (IPlayer)this.Game.Services.GetService(typeof(IPlayer));
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _spriteFont = Game.Content.Load<SpriteFont>("Fonts//CalibriDebug");
        }

        public override void Draw(GameTime gameTime)
        {
            if (!_active) return;
            var viewFrustrum = new BoundingFrustum(this._camera.View * this._camera.Projection);

            _spriteBatch.Begin();

            foreach (Chunk chunk in _world.Chunks.Values)
            {
                if (!chunk.BoundingBox.Intersects(viewFrustrum)) continue;
                chunk.PrintDebugInfo(Game.GraphicsDevice, _camera, _spriteBatch, _spriteFont);
            }

            _player.Weapon.PrintDebugInfo(Game.GraphicsDevice, _camera, _spriteBatch, _spriteFont);

            _spriteBatch.End();

        }
    }

    public interface IInGameDebuggable
    {
        void PrintDebugInfo(GraphicsDevice graphicsDevice, ICameraService camera, SpriteBatch spriteBatch, SpriteFont spriteFont);
    }
}
