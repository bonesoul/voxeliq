using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxeliqEngine.Graphics.Drawing;

namespace VoxeliqEngine.Debugging.Graphs
{
    public interface IStatsGraphs
    {
        void AddGraph(string name, ref int valueToObserve);
    }

    public class StatsGraphs : DrawableGameComponent, IStatsGraphs
    {
        // required services
        private IStatistics _statistics;

        // stuff needed for drawing.
        private PrimitiveBatch _primitiveBatch;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private Matrix _localProjection;
        private Matrix _localView;

        private List<DebugGraph> _graphs=new List<DebugGraph>();  

        public StatsGraphs(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IStatsGraphs), this); // export the service.
        }

        public override void Initialize()
        {
            // import required services.
            this._statistics = (IStatistics)this.Game.Services.GetService(typeof(IStatistics));

            this._graphs.Add(new FPSGraph(this.Game));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _primitiveBatch = new PrimitiveBatch(this.GraphicsDevice, 1000);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Game.Content.Load<SpriteFont>("Fonts//calibri");
            _localProjection = Matrix.CreateOrthographicOffCenter(0f, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
            _localView = Matrix.Identity;           
            
            foreach (var graph in this._graphs)
            {
                graph.AttachGraphics(this._primitiveBatch, this._spriteBatch, this._spriteFont, this._localProjection, this._localView);
            }

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            // backup state.
            var previousRasterizerState = this.Game.GraphicsDevice.RasterizerState;
            var previousDepthStencilState = this.Game.GraphicsDevice.DepthStencilState;

            // set new state
            this.Game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            this.Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            _primitiveBatch.Begin(this._localProjection, this._localView);

            foreach (var graph in this._graphs)
            {
                graph.DrawGraph(gameTime);
            }

            _primitiveBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            foreach (var graph in this._graphs)
            {
                graph.DrawStrings(gameTime);
            }

            _spriteBatch.End();

            // restore old state.
            this.Game.GraphicsDevice.RasterizerState = previousRasterizerState;
            this.Game.GraphicsDevice.DepthStencilState = previousDepthStencilState;

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var graph in this._graphs)
            {
                graph.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public void AddGraph(string name, ref int valueToObserve)
        {
            throw new NotImplementedException();
        }
    }
}
