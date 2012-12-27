using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxeliqEngine.Assets;
using VoxeliqEngine.Chunks;
using VoxeliqEngine.Debugging.Graphs.Implementations;
using VoxeliqEngine.Debugging.Graphs.Implementations.ChunkGraphs;
using VoxeliqEngine.Graphics;
using VoxeliqEngine.Graphics.Drawing;

namespace VoxeliqEngine.Debugging.Graphs
{
    /// <summary>
    /// GraphManager can render debug graphs.
    /// </summary>
    public interface IGraphManager
    { }

    /// <summary>
    /// GraphManager is DrawableGameComponent that can render debug graphs.
    /// </summary>
    public class GraphManager : DrawableGameComponent, IGraphManager
    {
        // stuff needed for drawing.
        private PrimitiveBatch _primitiveBatch;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private Matrix _localProjection;
        private Matrix _localView;

        private readonly List<DebugGraph> _graphs=new List<DebugGraph>(); // the current graphs list.

        public GraphManager(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IGraphManager), this); // export the service.
        }

        public override void Initialize()
        {
            // create the graphs modules.
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
            // init the drawing related objects.
            _primitiveBatch = new PrimitiveBatch(this.GraphicsDevice, 1000);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = AssetManager.Instance.Verdana;
            _localProjection = Matrix.CreateOrthographicOffCenter(0f, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
            _localView = Matrix.Identity;           
            
            // attach the drawing objects to the graph modules.
            foreach (var graph in this._graphs)
            {
                graph.AttachGraphics(this._primitiveBatch, this._spriteBatch, this._spriteFont, this._localProjection, this._localView);
            }

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Engine.Settings.Debugging.DebugGraphsEnabled) // check if graphs are enabled.
                return;

            // backup  the raster and depth-stencil states.
            var previousRasterizerState = this.Game.GraphicsDevice.RasterizerState;
            var previousDepthStencilState = this.Game.GraphicsDevice.DepthStencilState;

            // set new states for drawing primitive shapes.
            this.Game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            this.Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            _primitiveBatch.Begin(this._localProjection, this._localView); // initialize the primitive batch.

            foreach (var graph in this._graphs)
            {
                graph.DrawGraph(gameTime); // let the graphs draw their primitives.
            }

            _primitiveBatch.End(); // end the batch.

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend); // initialize the sprite batch.

            foreach (var graph in this._graphs)
            {
                graph.DrawStrings(gameTime); // let the graphs draw their sprites.
            }

            _spriteBatch.End(); // end the batch.

            // restore old states.
            this.Game.GraphicsDevice.RasterizerState = previousRasterizerState;
            this.Game.GraphicsDevice.DepthStencilState = previousDepthStencilState;

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Engine.Settings.Debugging.DebugGraphsEnabled) // check if graphs are enabled.
                return;

            foreach (var graph in this._graphs)
            {
                graph.Update(gameTime); // let the graphs update themself.
            }

            base.Update(gameTime);
        }
    }
}
