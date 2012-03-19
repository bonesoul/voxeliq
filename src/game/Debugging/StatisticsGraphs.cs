/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxeliqGame.Common.Logging;
using VolumetricStudios.VoxeliqGame.Graphics;
using VolumetricStudios.VoxeliqGame.Graphics.Drawing;

namespace VolumetricStudios.VoxeliqGame.Debugging
{
    /// <summary>
    /// Draws pretty statistics graphs!
    /// </summary>
    public class StatisticsGraphs : DrawableGameComponent
    {
        // resources.
        private PrimitiveBatch _primitiveBatch;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private Matrix _localProjection;
        private Matrix _localView;

        // graph settings
        private int _max;
        private int _avg;
        private int _min;
        public bool AdaptiveLimits = true;
        public int ValuesToGraph = 2500;
        public int MinimumValue;
        public int MaximumValue = 1000;
        private List<float> _graphValues = new List<float>();
        public Rectangle PerformancePanelBounds = new Rectangle(GraphicsConfig.Instance.Width - 210, 10, 200, 35);
        private Vector2[] _background = new Vector2[4];

        // required services
        private IStatistics _statistics;

        // misc.
        private static readonly Logger Logger = LogManager.CreateLogger(); //logging-facility.

        public StatisticsGraphs(Game game)
            : base(game)
        { }

        public override void Initialize()
        {
            Logger.Trace("init()");

            // import required services.
            this._statistics = (IStatistics) this.Game.Services.GetService(typeof (IStatistics));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _primitiveBatch = new PrimitiveBatch(this.GraphicsDevice, 1000);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Game.Content.Load<SpriteFont>("Fonts//calibri");
            _localProjection = Matrix.CreateOrthographicOffCenter(0f, this.GraphicsDevice.Viewport.Width,
                                                                  this.GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
            _localView = Matrix.Identity;

            this.InitGraphs();
        }

        private void InitGraphs()
        {
            _background[0] = new Vector2(PerformancePanelBounds.X, PerformancePanelBounds.Y);
            _background[1] = new Vector2(PerformancePanelBounds.X,
                                         PerformancePanelBounds.Y + PerformancePanelBounds.Height);
            _background[2] = new Vector2(PerformancePanelBounds.X + PerformancePanelBounds.Width,
                                         PerformancePanelBounds.Y + PerformancePanelBounds.Height);
            _background[3] = new Vector2(PerformancePanelBounds.X + PerformancePanelBounds.Width,
                                         PerformancePanelBounds.Y);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Engine.Settings.Debugging.FPSGraphEnabled)
                return;

            // backup state.
            var previousRasterizerState = this.GraphicsDevice.RasterizerState;
            var previousDepthStencilState = this.GraphicsDevice.DepthStencilState;

            // set new state
            this.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            this.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            _primitiveBatch.Begin(this._localProjection, this._localView);

            this.DrawFPSGraph(gameTime);

            _primitiveBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            _spriteBatch.DrawString(_spriteFont, "fps",
                                    new Vector2(PerformancePanelBounds.Left, PerformancePanelBounds.Bottom), Color.White);
            _spriteBatch.DrawString(_spriteFont, "max:" + _max,
                                    new Vector2(PerformancePanelBounds.Left + 25, PerformancePanelBounds.Bottom),
                                    Color.White);
            _spriteBatch.DrawString(_spriteFont, "avg:" + _avg,
                                    new Vector2(PerformancePanelBounds.Left + 90, PerformancePanelBounds.Bottom),
                                    Color.White);
            _spriteBatch.DrawString(_spriteFont, "min:" + _min,
                                    new Vector2(PerformancePanelBounds.Left + 150, PerformancePanelBounds.Bottom),
                                    Color.White);
            _spriteBatch.End();

            // restore old state.
            this.GraphicsDevice.RasterizerState = previousRasterizerState;
            this.GraphicsDevice.DepthStencilState = previousDepthStencilState;
        }

        private void DrawFPSGraph(GameTime gameTime)
        {
            DrawSolidPolygon(_background, 4, Color.DarkBlue, true);

            _graphValues.Add(this._statistics.FPS);

            if (_graphValues.Count > ValuesToGraph + 1)
                _graphValues.RemoveAt(0);

            float x = PerformancePanelBounds.X;
            float deltaX = PerformancePanelBounds.Width/(float) ValuesToGraph;
            float yScale = PerformancePanelBounds.Bottom - (float) PerformancePanelBounds.Top;

            // we must have at least 2 values to start rendering
            if (_graphValues.Count > 2)
            {
                _max = (int) _graphValues.Max();
                _avg = (int) _graphValues.Average();
                _min = (int) _graphValues.Min();

                if (AdaptiveLimits)
                {
                    MaximumValue = _max;
                    MinimumValue = 0;
                }

                // start at last value (newest value added)
                // continue until no values are left
                for (int i = _graphValues.Count - 1; i > 0; i--)
                {
                    float y1 = PerformancePanelBounds.Bottom -
                               ((_graphValues[i]/(MaximumValue - MinimumValue))*yScale);
                    float y2 = PerformancePanelBounds.Bottom -
                               ((_graphValues[i - 1]/(MaximumValue - MinimumValue))*yScale);

                    Vector2 x1 =
                        new Vector2(MathHelper.Clamp(x, PerformancePanelBounds.Left, PerformancePanelBounds.Right),
                                    MathHelper.Clamp(y1, PerformancePanelBounds.Top, PerformancePanelBounds.Bottom));

                    Vector2 x2 =
                        new Vector2(
                            MathHelper.Clamp(x + deltaX, PerformancePanelBounds.Left, PerformancePanelBounds.Right),
                            MathHelper.Clamp(y2, PerformancePanelBounds.Top, PerformancePanelBounds.Bottom));

                    DrawSegment(x1, x2, Color.DeepSkyBlue);

                    x += deltaX;
                }
            }
        }

        public void DrawSolidPolygon(Vector2[] vertices, int count, float red, float green, float blue)
        {
            DrawSolidPolygon(vertices, count, new Color(red, green, blue), true);
        }

        public void DrawSolidPolygon(Vector2[] vertices, int count, Color color)
        {
            DrawSolidPolygon(vertices, count, color, true);
        }

        public void DrawSolidPolygon(Vector2[] vertices, int count, Color color, bool outline)
        {
            if (!_primitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            if (count == 2)
            {
                DrawPolygon(vertices, count, color);
                return;
            }

            Color colorFill = color*(outline ? 0.5f : 1.0f);

            for (int i = 1; i < count - 1; i++)
            {
                _primitiveBatch.AddVertex(vertices[0], colorFill, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(vertices[i], colorFill, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(vertices[i + 1], colorFill, PrimitiveType.TriangleList);
            }

            if (outline)
            {
                DrawPolygon(vertices, count, color);
            }
        }

        public void DrawPolygon(Vector2[] vertices, int count, float red, float green, float blue)
        {
            DrawPolygon(vertices, count, new Color(red, green, blue));
        }

        public void DrawPolygon(Vector2[] vertices, int count, Color color)
        {
            if (!_primitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            for (int i = 0; i < count - 1; i++)
            {
                _primitiveBatch.AddVertex(vertices[i], color, PrimitiveType.LineList);
                _primitiveBatch.AddVertex(vertices[i + 1], color, PrimitiveType.LineList);
            }

            _primitiveBatch.AddVertex(vertices[count - 1], color, PrimitiveType.LineList);
            _primitiveBatch.AddVertex(vertices[0], color, PrimitiveType.LineList);
        }

        public void DrawSegment(Vector2 start, Vector2 end, float red, float green, float blue)
        {
            DrawSegment(start, end, new Color(red, green, blue));
        }

        public void DrawSegment(Vector2 start, Vector2 end, Color color)
        {
            if (!_primitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            _primitiveBatch.AddVertex(start, color, PrimitiveType.LineList);
            _primitiveBatch.AddVertex(end, color, PrimitiveType.LineList);
        }
    }
}