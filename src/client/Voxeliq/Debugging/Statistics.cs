using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.Voxeliq.Graphics.Utility;
using VolumetricStudios.VoxeliqEngine.Logging;

namespace VolumetricStudios.Voxeliq.Debugging
{
    public interface  IStatistics
    {
        /// <summary>
        /// Current FPS.
        /// </summary>
        int FPS { get; }
    }

    public class Statistics : DrawableGameComponent, IStatistics
    {
        // fps
        public int FPS { get; private set; }
        private int _frameCounter = 0;
        private TimeSpan _elapsedTime = TimeSpan.Zero;

        // resources
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;

        // required services
        private IPlayer _player;

        // misc.
        private static readonly Logger Logger = LogManager.CreateLogger(); //logging-facility.

        public Statistics(Game game) : base(game)
        {    
            this.Game.Services.AddService(typeof(IStatistics), this); // export service.
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            // import required services
            this._player = (IPlayer)this.Game.Services.GetService(typeof(IPlayer));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);            
            _spriteFont = Game.Content.Load<SpriteFont>("Fonts//calibri");
        }

        public override void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime;
            if (_elapsedTime < TimeSpan.FromSeconds(1)) return;

            _elapsedTime -= TimeSpan.FromSeconds(1);
            FPS = _frameCounter;
            _frameCounter = 0;
        }

        public override void Draw(GameTime gameTime) // Attention: DO NOT use string.format as it's slower than string concat: https://www.assembla.com/wiki/show/voxlr/StringFormat_vs_StringConcat
        {            
            _frameCounter++;

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);            
            _spriteBatch.DrawString(_spriteFont, "fps: " + FPS, new Vector2(5, 5), Color.White);
            _spriteBatch.DrawString(_spriteFont, "mem: " + this.GetMemSize(GC.GetTotalMemory(false)), new Vector2(75, 5), Color.White);
            _spriteBatch.DrawString(_spriteFont, "pos: " + this._player.Position, new Vector2(190, 5), Color.White);
            _spriteBatch.DrawString(_spriteFont, "chunks: n/a", new Vector2(5, 20), Color.White);
            _spriteBatch.DrawString(_spriteFont, "blocks: n/a", new Vector2(130, 20), Color.White);
            _spriteBatch.DrawString(_spriteFont, "gen/buildQ: n/a", new Vector2(320, 20), Color.White);
            _spriteBatch.DrawString(_spriteFont, "Inf: n/a", new Vector2(5, 35), Color.White);
            _spriteBatch.DrawString(_spriteFont, "Fly: n/a", new Vector2(60, 35), Color.White);
            _spriteBatch.DrawString(_spriteFont, "Fog: n/a", new Vector2(120, 35), Color.White);
            _spriteBatch.End();

        }

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
