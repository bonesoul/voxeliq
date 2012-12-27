using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxeliqEngine.Graphics;
using VoxeliqEngine.Graphics.Drawing;

namespace VoxeliqEngine.Debugging.Graphs
{
    public class DebugGraph
    {
        // stuff required for drawing.
        protected PrimitiveBatch PrimitiveBatch;
        protected SpriteBatch SpriteBatch;
        protected SpriteFont SpriteFont;
        protected Matrix LocalProjection;
        protected Matrix LocalView;

        // graph values
        protected int MaxValue;
        protected int AverageValue;
        protected int MinimumValue;
        protected int CurrentValue;

        protected int AdaptiveMinimum;
        protected int AdaptiveMaximum = 1000;

        // settings 

        protected bool AdaptiveLimits { get; private set; }
        protected int ValuesToGraph {get; private set;}

        protected readonly List<float> GraphValues = new List<float>();
        protected readonly Vector2[] Background = new Vector2[4];
        public Rectangle Bounds { get; private set; }

        public bool Attached { get; private set; }
        protected Game Game { get; private set; }

        public DebugGraph(Game game, Rectangle bounds)            
        {
            this.Attached = false;
            this.Game = game;

            this.Bounds = bounds;

            this.AdaptiveLimits = true;
            this.ValuesToGraph = 2500;
        }

        public void AttachGraphics(PrimitiveBatch primitiveBatch, SpriteBatch spriteBatch, SpriteFont spriteFont, Matrix localProjection, Matrix localView)
        {
            if (this.Attached)
                return;

            this.PrimitiveBatch = primitiveBatch;
            this.SpriteBatch = spriteBatch;
            this.SpriteFont = spriteFont;
            this.LocalProjection = localProjection;
            this.LocalView = localView;

            this.Initialize();
            this.LoadContent();

            this.Attached = true;
        }

        protected virtual void Initialize()
        { }

        public void LoadContent()
        {
            Background[0] = new Vector2(Bounds.X, Bounds.Y);
            Background[1] = new Vector2(Bounds.X, Bounds.Y + Bounds.Height);
            Background[2] = new Vector2(Bounds.X + Bounds.Width, Bounds.Y + Bounds.Height);
            Background[3] = new Vector2(Bounds.X + Bounds.Width, Bounds.Y);
        }

        public virtual void Update(GameTime gameTime)
        { }

        public virtual void DrawStrings(GameTime gameTime)
        { }

        public virtual void DrawGraph(GameTime gameTime)
        { }
    }
}
