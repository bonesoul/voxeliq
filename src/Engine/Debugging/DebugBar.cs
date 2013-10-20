/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Globalization;
using System.Text;
using Engine.Assets;
using Engine.Chunks;
using Engine.Common.Extensions;
using Engine.Common.Logging;
using Engine.Graphics.Drawing;
using Engine.Universe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Debugging
{
    /// <summary>
    /// Allows interaction with the statistics service.
    /// </summary>
    public interface IStatistics
    {
        /// <summary>
        /// Returns current FPS.
        /// </summary>
        int FPS { get; }

        /// <summary>
        /// Returns the memory size.
        /// </summary>
        /// <returns></returns>
        long MemoryUsed { get; }

        int GenerateQueue { get; }
        int LightenQueue { get; }
        int BuildQueue { get; }
        int ReadyQueue { get; }
        int RemovalQueue { get; }
    }

    internal class DebugBar : DrawableGameComponent, IStatistics
    {
        // exported statistics.
        public int FPS { get; private set; }
        public long MemoryUsed { get { return GC.GetTotalMemory(false); } }
        public int GenerateQueue { get; private set; }
        public int LightenQueue { get; private set; }
        public int BuildQueue { get; private set; }
        public int ReadyQueue { get; private set; }
        public int RemovalQueue { get; private set; }

        // internal counters.
        private int _frameCounter = 0; // the frame count.
        private TimeSpan _elapsedTime = TimeSpan.Zero;

        // drawn-blocks stuff.
        private string _drawnBlocks;
        private string _totalBlocks;

        // resources.
        private PrimitiveBatch _primitiveBatch;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private Matrix _localProjection;
        private Matrix _localView;
        private Rectangle _bounds;
        private readonly Vector2[] _backgroundPolygon = new Vector2[4];

        // for grabbing internal string, we should init string builder capacity and max capacity ctor so that, grabbed internal string is always valid. - http://www.gavpugh.com/2010/03/23/xnac-stringbuilder-to-string-with-no-garbage/
        private readonly StringBuilder _stringBuilder = new StringBuilder(512, 512);

        // required services.       
        private IWorld _world;
        private IPlayer _player;
        private IFogger _fogger;
        private IChunkStorage _chunkStorage;
        private IChunkCache _chunkCache;
        private IAssetManager _assetManager;

        // misc.
        private static readonly Logger Logger = LogManager.CreateLogger(); // loging-facility

        public DebugBar(Game game)
            : base(game)
        {
            this.Game.Services.AddService(typeof(IStatistics), this); // export the service.
        }

        /// <summary>
        /// Initializes the debug-bar service.
        /// </summary>
        public override void Initialize()
        {
            Logger.Trace("init()");

            // import required services.            
            this._player = (IPlayer)this.Game.Services.GetService(typeof(IPlayer));
            this._fogger = (IFogger)this.Game.Services.GetService(typeof(IFogger));
            this._world = (IWorld)this.Game.Services.GetService(typeof(IWorld));
            this._chunkStorage = (IChunkStorage)this.Game.Services.GetService(typeof(IChunkStorage));
            this._chunkCache = (IChunkCache)this.Game.Services.GetService(typeof(IChunkCache));
            this._assetManager = (IAssetManager)this.Game.Services.GetService(typeof(IAssetManager));

            if (this._assetManager == null)
                throw new NullReferenceException("Can not find asset manager component.");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // load resources.
            this._primitiveBatch = new PrimitiveBatch(this.GraphicsDevice, 1000);
            this._localProjection = Matrix.CreateOrthographicOffCenter(0f, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
            this._localView = Matrix.Identity;
            this._spriteBatch = new SpriteBatch(GraphicsDevice);
            this._spriteFont = this._assetManager.Verdana;

            // init bounds.
            this._bounds = new Rectangle(10, 10, this.Game.GraphicsDevice.Viewport.Bounds.Width - 20, 20);
            this._backgroundPolygon[0] = new Vector2(_bounds.X - 2, _bounds.Y - 2); // top left
            this._backgroundPolygon[1] = new Vector2(_bounds.X - 2, _bounds.Y + _bounds.Height + 14); // bottom left
            this._backgroundPolygon[2] = new Vector2(_bounds.X + 2 + _bounds.Width, _bounds.Y + _bounds.Height + 14); // bottom right
            this._backgroundPolygon[3] = new Vector2(_bounds.X + 2 + _bounds.Width, _bounds.Y - 2); // top right
        }

        /// <summary>
        /// Calculates the FPS.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            this._elapsedTime += gameTime.ElapsedGameTime;
            if (this._elapsedTime < TimeSpan.FromSeconds(1))
                return;

            this._elapsedTime -= TimeSpan.FromSeconds(1);
            this.FPS = _frameCounter;
            this._frameCounter = 0;
        }

        /// <summary>
        /// Draws the statistics.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            _frameCounter++;

            // backup  the raster and depth-stencil states.
            var previousRasterizerState = this.Game.GraphicsDevice.RasterizerState;
            var previousDepthStencilState = this.Game.GraphicsDevice.DepthStencilState;

            // set new states for drawing primitive shapes.
            this.Game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            this.Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            _primitiveBatch.Begin(this._localProjection, this._localView); // initialize the primitive batch.

            BasicShapes.DrawSolidPolygon(this._primitiveBatch, this._backgroundPolygon, 4, Color.Black, true);

            _primitiveBatch.End(); // end the batch.

            // restore old states.
            this.Game.GraphicsDevice.RasterizerState = previousRasterizerState;
            this.Game.GraphicsDevice.DepthStencilState = previousDepthStencilState;

            // calculate drawn blocks
            if (this._chunkCache.ChunksDrawn >= 31)
                _drawnBlocks = (this._chunkCache.ChunksDrawn / 31f).ToString("F2") + "M";
            else if (this._chunkCache.ChunksDrawn > 1)
                _drawnBlocks = (this._chunkCache.ChunksDrawn / 0.03f).ToString("F2") + "K";
            else _drawnBlocks = "0";

            // calculate total blocks
            if (this._chunkStorage.Count > 31) 
                _totalBlocks = (this._chunkStorage.Count / 31f).ToString("F2") + "M";
            else if (this._chunkStorage.Count > 1) 
                _totalBlocks = (this._chunkStorage.Count / 0.03f).ToString("F2") + "K";
            else 
                _totalBlocks = Core.Engine.Instance.Configuration.Chunk.Volume.ToString(CultureInfo.InvariantCulture);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            // Attention: DO NOT use string.format as it's slower than string concat.

            // FPS
            _stringBuilder.Length = 0;
            _stringBuilder.Append("fps:");
            _stringBuilder.Append(this.FPS);
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(this._bounds.X + 5, this._bounds.Y + 5), Color.White);

            // mem used
            _stringBuilder.Length = 0;
            _stringBuilder.Append("mem:");
            _stringBuilder.Append(this.MemoryUsed.GetKiloString());
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(this._bounds.X + 105, this._bounds.Y + 5), Color.White);

            // chunks
            _stringBuilder.Length = 0;
            _stringBuilder.Append("chunks:");
            _stringBuilder.AppendNumber(this._chunkCache.ChunksDrawn);
            _stringBuilder.Append('/');
            _stringBuilder.AppendNumber(this._chunkStorage.Count);
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(this._bounds.X + 205, this._bounds.Y + 5), Color.White);

            // blocks
            _stringBuilder.Length = 0;
            _stringBuilder.Append("blocks:");
            _stringBuilder.Append(_drawnBlocks);
            _stringBuilder.Append('/');
            _stringBuilder.Append(_totalBlocks);
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(this._bounds.X + 305, this._bounds.Y + 5), Color.White);

            // process queues.
            this.GenerateQueue = this._chunkCache.StateStatistics[ChunkState.AwaitingGenerate] + this._chunkCache.StateStatistics[ChunkState.Generating];
            this.LightenQueue = this._chunkCache.StateStatistics[ChunkState.AwaitingLighting] + this._chunkCache.StateStatistics[ChunkState.Lighting] + this._chunkCache.StateStatistics[ChunkState.AwaitingRelighting];
            this.BuildQueue = this._chunkCache.StateStatistics[ChunkState.AwaitingBuild] + this._chunkCache.StateStatistics[ChunkState.Building] + this._chunkCache.StateStatistics[ChunkState.AwaitingRebuild];
            this.ReadyQueue = this._chunkCache.StateStatistics[ChunkState.Ready];
            this.RemovalQueue = this._chunkCache.StateStatistics[ChunkState.AwaitingRemoval];

            // chunk generation queue
            _stringBuilder.Length = 0;
            _stringBuilder.Append("GenerateQ:");
            _stringBuilder.AppendNumber(this.GenerateQueue);
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(this._bounds.X + 450, this._bounds.Y + 5), Color.White);

            // chunk lighting queue
            _stringBuilder.Length = 0;
            _stringBuilder.Append("LightenQ:");
            _stringBuilder.AppendNumber(this.LightenQueue);
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(this._bounds.X + 550, this._bounds.Y + 5), Color.White);

            // chunk build queue
            _stringBuilder.Length = 0;
            _stringBuilder.Append("BuildQ:");
            _stringBuilder.AppendNumber(this.BuildQueue);
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(this._bounds.X + 650, this._bounds.Y + 5), Color.White);

            // ready chunks queue
            _stringBuilder.Length = 0;
            _stringBuilder.Append("Ready:");
            _stringBuilder.AppendNumber(this.ReadyQueue);
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(this._bounds.X + 750, this._bounds.Y + 5), Color.White);

            // chunk removal queue
            _stringBuilder.Length = 0;
            _stringBuilder.Append("Removal:");
            _stringBuilder.AppendNumber(this.RemovalQueue);
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(this._bounds.X + 850, this._bounds.Y + 5), Color.White);

            // infinitive world
            _stringBuilder.Length = 0;
            _stringBuilder.Append("inf:");
            _stringBuilder.Append(Core.Engine.Instance.Configuration.World.IsInfinitive ? "On" : "Off");
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(this._bounds.X + 5, this._bounds.Y + 15), Color.White);

            // fly
            _stringBuilder.Length = 0;
            _stringBuilder.Append("fly:");
            _stringBuilder.Append(this._player.FlyingEnabled ? "On" : "Off");
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(this._bounds.X + 105, this._bounds.Y + 15), Color.White);

            // fog
            _stringBuilder.Length = 0;
            _stringBuilder.Append("fog:");
            _stringBuilder.Append(this._fogger.State);
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(this._bounds.X + 205, this._bounds.Y + 15), Color.White);

            // player position
            _stringBuilder.Length = 0;
            _stringBuilder.Append("pos:");
            _stringBuilder.Append(this._player.Position);
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(this._bounds.X + 305, this._bounds.Y + 15), Color.White);

            _spriteBatch.End();
        }
    }
}
