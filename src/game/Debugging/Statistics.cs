/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxeliqGame.Chunks;
using VolumetricStudios.VoxeliqGame.Common.Logging;
using VolumetricStudios.VoxeliqGame.Environment;
using VolumetricStudios.VoxeliqGame.Universe;

namespace VolumetricStudios.VoxeliqGame.Debugging
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
    }

    public sealed class Statistics : DrawableGameComponent, IStatistics
    {
        // fps stuff.
        public int FPS { get; private set; } // the current FPS.
        private int _frameCounter = 0; // the frame count.
        private TimeSpan _elapsedTime = TimeSpan.Zero;

        // drawn-text stuff.
        private string _drawnBlocks;
        private string _totalBlocks;

        // resources.
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
                
        // required services.       
        private IWorld _world;
        private IWorldStatisticsService _worldStatistics;
        private IPlayer _player;
        private IFogService _fogService;
        private IChunkStorage _chunkStorage;

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
            this._player = (IPlayer)this.Game.Services.GetService(typeof(IPlayer));
            this._fogService = (IFogService)this.Game.Services.GetService(typeof(IFogService));
            this._world = (IWorld) this.Game.Services.GetService(typeof (IWorld));
            this._worldStatistics = (IWorldStatisticsService)this.Game.Services.GetService(typeof(IWorldStatisticsService));
            this._chunkStorage = (IChunkStorage) this.Game.Services.GetService(typeof (IChunkStorage));

            base.Initialize();
        }

        /// <summary>
        /// Loads required assets & content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Game.Content.Load<SpriteFont>("Fonts//calibri");
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
        public override void Draw(GameTime gameTime) // Attention: DO NOT use string.format as it's slower than string concat: https://www.assembla.com/wiki/show/voxlr/StringFormat_vs_StringConcat
        {            
            _frameCounter++;

            if (this._worldStatistics.ChunksDrawn >= 31) _drawnBlocks = (this._worldStatistics.ChunksDrawn / 31f).ToString("F2") + "M";
            else if (this._worldStatistics.ChunksDrawn > 1) _drawnBlocks = (this._worldStatistics.ChunksDrawn / 0.03f).ToString("F2") + "K";
            else _drawnBlocks = "0";

            if (this._chunkStorage.Count > 31) _totalBlocks = (this._chunkStorage.Count / 31f).ToString("F2") + "M";
            else if (this._chunkStorage.Count > 1) _totalBlocks = (this._chunkStorage.Count / 0.03f).ToString("F2") + "K";
            else _totalBlocks = Chunk.Volume.ToString(CultureInfo.InvariantCulture);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);            
            _spriteBatch.DrawString(_spriteFont, "fps: " + this.FPS, new Vector2(5, 5), Color.White);
            _spriteBatch.DrawString(_spriteFont, "mem: " +  this.GetMemSize(GC.GetTotalMemory(false)), new Vector2(75, 5), Color.White);
            _spriteBatch.DrawString(_spriteFont, "pos: " + this._player.Position, new Vector2(190, 5), Color.White);
            _spriteBatch.DrawString(_spriteFont, "chunks: " + this._worldStatistics.ChunksDrawn + "/" + this._chunkStorage.Count, new Vector2(5, 20), Color.White);
            _spriteBatch.DrawString(_spriteFont, "blocks: " + _drawnBlocks + "/" + _totalBlocks, new Vector2(130, 20), Color.White);
            _spriteBatch.DrawString(_spriteFont, "gen/buildQ: " + this._worldStatistics.GenerationQueueCount + "/" + this._worldStatistics.BuildingQueueCount, new Vector2(320, 20), Color.White);
            _spriteBatch.DrawString(_spriteFont, "inf: " + (this._world.IsInfinitive ? "On" : "Off"), new Vector2(5, 35), Color.White);            
            _spriteBatch.DrawString(_spriteFont, "fly: " + (this._player.FlyingEnabled?"On":"Off"), new Vector2(60, 35), Color.White);
            _spriteBatch.DrawString(_spriteFont, "fog: " + this._fogService.State, new Vector2(120, 35), Color.White);            
            _spriteBatch.End();
        }

        /// <summary>
        /// Returns pretty memory size text.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private string GetMemSize(long size)
        {
            int i;
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            double dblSByte = 0;
            for (i = 0; (int)(size / 1024) > 0; i++, size /= 1024) dblSByte = size / 1024.0;
            return dblSByte.ToString("0.00") + suffixes[i];
        }
    }
}
