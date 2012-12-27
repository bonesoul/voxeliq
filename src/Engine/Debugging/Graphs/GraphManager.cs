using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxeliqEngine.Chunks;
using VoxeliqEngine.Debugging.Graphs.Implementations;
using VoxeliqEngine.Debugging.Graphs.Implementations.ChunkGraphs;
using VoxeliqEngine.Graphics;
using VoxeliqEngine.Graphics.Drawing;

namespace VoxeliqEngine.Debugging.Graphs
{
    public interface IGraphManager
    {
    }

    public class GraphManager : DrawableGameComponent, IGraphManager
    {
        // required services
        private IStatistics _statistics;

        // stuff needed for drawing.
        private PrimitiveBatch _primitiveBatch;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private Matrix _localProjection;
        private Matrix _localView;

        private readonly List<DebugGraph> _graphs=new List<DebugGraph>();  

        public GraphManager(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IGraphManager), this); // export the service.
        }

        public override void Initialize()
        {
            // import required services.
            this._statistics = (IStatistics)this.Game.Services.GetService(typeof(IStatistics));

            this._graphs.Add(new FPSGraph(this.Game, new Rectangle(GraphicsConfig.Instance.Width - 280, 10, 270, 35)));
            this._graphs.Add(new MemGraph(this.Game, new Rectangle(GraphicsConfig.Instance.Width - 280, 65, 270, 35)));
            this._graphs.Add(new GenerateQ(this.Game, new Rectangle(GraphicsConfig.Instance.Width - 280, 120, 270, 35)));
            this._graphs.Add(new LightenQ(this.Game, new Rectangle(GraphicsConfig.Instance.Width - 280, 175, 270, 35)));
            this._graphs.Add(new BuildQ(this.Game, new Rectangle(GraphicsConfig.Instance.Width - 280, 230, 270, 35)));
            this._graphs.Add(new ReadyQ(this.Game, new Rectangle(GraphicsConfig.Instance.Width - 280, 285, 270, 35)));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _primitiveBatch = new PrimitiveBatch(this.GraphicsDevice, 1000);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Game.Content.Load<SpriteFont>("Fonts//Verdana");
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
            if (!Engine.Settings.Debugging.DebugGraphsEnabled)
                return;

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
            if (!Engine.Settings.Debugging.DebugGraphsEnabled)
                return;

            foreach (var graph in this._graphs)
            {
                graph.Update(gameTime);
            }

            base.Update(gameTime);
        }
    }
}
