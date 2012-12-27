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
        protected Rectangle Bounds = new Rectangle(GraphicsConfig.Instance.Width - 280, 10, 270, 35);

        public bool Attached { get; private set; }
        protected Game Game { get; private set; }

        public DebugGraph(Game game)            
        {
            this.Attached = false;
            this.Game = game;

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

        protected void DrawSolidPolygon(Vector2[] vertices, int count, float red, float green, float blue)
        {
            DrawSolidPolygon(vertices, count, new Color(red, green, blue), true);
        }

        protected void DrawSolidPolygon(Vector2[] vertices, int count, Color color)
        {
            DrawSolidPolygon(vertices, count, color, true);
        }

        protected void DrawSolidPolygon(Vector2[] vertices, int count, Color color, bool outline)
        {
            if (!PrimitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            if (count == 2)
            {
                DrawPolygon(vertices, count, color);
                return;
            }

            Color colorFill = color * (outline ? 0.5f : 1.0f);

            for (int i = 1; i < count - 1; i++)
            {
                PrimitiveBatch.AddVertex(vertices[0], colorFill, PrimitiveType.TriangleList);
                PrimitiveBatch.AddVertex(vertices[i], colorFill, PrimitiveType.TriangleList);
                PrimitiveBatch.AddVertex(vertices[i + 1], colorFill, PrimitiveType.TriangleList);
            }

            if (outline)
            {
                DrawPolygon(vertices, count, color);
            }
        }

        protected void DrawPolygon(Vector2[] vertices, int count, float red, float green, float blue)
        {
            DrawPolygon(vertices, count, new Color(red, green, blue));
        }

        protected void DrawPolygon(Vector2[] vertices, int count, Color color)
        {
            if (!PrimitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            for (int i = 0; i < count - 1; i++)
            {
                PrimitiveBatch.AddVertex(vertices[i], color, PrimitiveType.LineList);
                PrimitiveBatch.AddVertex(vertices[i + 1], color, PrimitiveType.LineList);
            }

            PrimitiveBatch.AddVertex(vertices[count - 1], color, PrimitiveType.LineList);
            PrimitiveBatch.AddVertex(vertices[0], color, PrimitiveType.LineList);
        }

        protected void DrawSegment(Vector2 start, Vector2 end, float red, float green, float blue)
        {
            DrawSegment(start, end, new Color(red, green, blue));
        }

        protected void DrawSegment(Vector2 start, Vector2 end, Color color)
        {
            if (!PrimitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            PrimitiveBatch.AddVertex(start, color, PrimitiveType.LineList);
            PrimitiveBatch.AddVertex(end, color, PrimitiveType.LineList);
        }
    }
}
