/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Globalization;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxeliqEngine.Assets;
using VoxeliqEngine.Chunks;
using VoxeliqEngine.Engine;
using VoxeliqEngine.Logging;
using VoxeliqEngine.Universe;
using VoxeliqEngine.Utils.Extensions;

namespace VoxeliqEngine.Debugging
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
    }

    public sealed class Statistics : DrawableGameComponent, IStatistics
    {
        // fps stuff.
        public int FPS { get; private set; } // the current FPS.
        private int _frameCounter = 0; // the frame count.
        private TimeSpan _elapsedTime = TimeSpan.Zero;

        // memory stuff
        /// <summary>
        /// Returns the memory size.
        /// </summary>
        /// <returns></returns>
        public long MemoryUsed
        {
            get { return GC.GetTotalMemory(false); }
        }

        // drawn-text stuff.
        private string _drawnBlocks;
        private string _totalBlocks;

        // resources.
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;

        // for grabbing internal string, we should init string builder capacity and max capacity ctor so that, grabbed internal string is always valid.
        // http://www.gavpugh.com/2010/03/23/xnac-stringbuilder-to-string-with-no-garbage/
        private readonly StringBuilder _stringBuilder = new StringBuilder(512, 512);

        // required services.       
        private IWorld _world;
        private IPlayer _player;
        private IFogger _fogger;
        private IChunkStorage _chunkStorage;
        private IChunkCache _chunkCache;

        public int GenerateQueue { get; private set; }
        public int LightenQueue { get; private set; }
        public int BuildQueue { get; private set; }
        public int ReadyQueue { get; private set; }

        // misc.
        private static readonly Logger Logger = LogManager.CreateLogger(); // loging-facility

        /// <summary>
        /// Creates a new statistics service instance.
        /// </summary>
        /// <param name="game"></param>
        public Statistics(Game game)
            : base(game)
        {
            this.Game.Services.AddService(typeof (IStatistics), this); // export the service.
        }

        /// <summary>
        /// Initializes the statistics service.
        /// </summary>
        public override void Initialize()
        {
            Logger.Trace("init()");

            // import required services.            
            this._player = (IPlayer) this.Game.Services.GetService(typeof (IPlayer));
            this._fogger = (IFogger) this.Game.Services.GetService(typeof (IFogger));
            this._world = (IWorld) this.Game.Services.GetService(typeof (IWorld));
            this._chunkStorage = (IChunkStorage) this.Game.Services.GetService(typeof (IChunkStorage));
            this._chunkCache = (IChunkCache) this.Game.Services.GetService(typeof (IChunkCache));

            base.Initialize();
        }

        /// <summary>
        /// Loads required assets & content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = AssetManager.Instance.Verdana;
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
            // Attention: DO NOT use string.format as it's slower than string concat.

            _frameCounter++;

            if (this._chunkCache.ChunksDrawn >= 31)
                _drawnBlocks = (this._chunkCache.ChunksDrawn / 31f).ToString("F2") + "M";
            else if (this._chunkCache.ChunksDrawn > 1)
                _drawnBlocks = (this._chunkCache.ChunksDrawn / 0.03f).ToString("F2") + "K";
            else _drawnBlocks = "0";

            if (this._chunkStorage.Count > 31) _totalBlocks = (this._chunkStorage.Count / 31f).ToString("F2") + "M";
            else if (this._chunkStorage.Count > 1) _totalBlocks = (this._chunkStorage.Count / 0.03f).ToString("F2") + "K";
            else _totalBlocks = Chunk.Volume.ToString(CultureInfo.InvariantCulture);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            // FPS
            _stringBuilder.Length = 0;
            _stringBuilder.Append("fps:");
            _stringBuilder.Append(this.FPS);
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(5, 5), Color.White);

            // mem used
            _stringBuilder.Length = 0;
            _stringBuilder.Append("mem:");
            _stringBuilder.Append(this.MemoryUsed.GetKiloString());
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(75, 5), Color.White);

            // player position
            _stringBuilder.Length = 0;
            _stringBuilder.Append("pos:");
            _stringBuilder.Append(this._player.Position);
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(190, 5), Color.White);

            // chunks
            _stringBuilder.Length = 0;
            _stringBuilder.Append("chunks:");
            _stringBuilder.AppendNumber(this._chunkCache.ChunksDrawn);
            _stringBuilder.Append('/');
            _stringBuilder.AppendNumber(this._chunkStorage.Count);
            _spriteBatch.DrawString(_spriteFont, _stringBuilder,new Vector2(5, 20), Color.White);

            // blocks
            _stringBuilder.Length = 0;
            _stringBuilder.Append("blocks:");
            _stringBuilder.Append(_drawnBlocks);
            _stringBuilder.Append('/');
            _stringBuilder.Append(_totalBlocks);
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(130, 20), Color.White);

            // infinitive world
            _stringBuilder.Length = 0;
            _stringBuilder.Append("inf:");
            _stringBuilder.Append(Settings.World.IsInfinitive ? "On" : "Off");
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(5, 35), Color.White);

            // fly
            _stringBuilder.Length = 0;
            _stringBuilder.Append("fly:");
            _stringBuilder.Append(this._player.FlyingEnabled ? "On" : "Off");
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(60, 35), Color.White);

            // fog
            _stringBuilder.Length = 0;
            _stringBuilder.Append("fog:");
            _stringBuilder.Append(this._fogger.State);
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(120, 35), Color.White);

            // process queues.
            this.GenerateQueue = this._chunkCache.StateStatistics[ChunkState.AwaitingGenerate] + this._chunkCache.StateStatistics[ChunkState.Generating];
            this.LightenQueue = this._chunkCache.StateStatistics[ChunkState.AwaitingLighting] + this._chunkCache.StateStatistics[ChunkState.Lighting] + this._chunkCache.StateStatistics[ChunkState.AwaitingRelighting];
            this.BuildQueue = this._chunkCache.StateStatistics[ChunkState.AwaitingBuild] + this._chunkCache.StateStatistics[ChunkState.Building] + this._chunkCache.StateStatistics[ChunkState.AwaitingRebuild];
            this.ReadyQueue = this._chunkCache.StateStatistics[ChunkState.Ready];

            // generation
            _stringBuilder.Length = 0;
            _stringBuilder.Append("GenerateQ:");
            _stringBuilder.AppendNumber(this.GenerateQueue);
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(5, 65), Color.White);

            // lighten
            _stringBuilder.Length = 0;
            _stringBuilder.Append("LightenQ:");
            _stringBuilder.AppendNumber(this.LightenQueue);
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(5, 80), Color.White);

            // build
            _stringBuilder.Length = 0;
            _stringBuilder.Append("BuildQ:");
            _stringBuilder.AppendNumber(this.BuildQueue);
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(5, 95), Color.White);

            // ready
            _stringBuilder.Length = 0;
            _stringBuilder.Append("Ready:");
            _stringBuilder.AppendNumber(this.ReadyQueue);
            _spriteBatch.DrawString(_spriteFont, _stringBuilder, new Vector2(5, 110), Color.White);

            _spriteBatch.End();
        }
    }
}