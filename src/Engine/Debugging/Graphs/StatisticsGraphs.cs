/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxeliqEngine.Chunks;
using VoxeliqEngine.Graphics;
using VoxeliqEngine.Graphics.Drawing;
using VoxeliqEngine.Logging;

namespace VoxeliqEngine.Debugging.Graphs
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

        // fps graph values
        private int _fpsMax;
        private int _fpsAverage;
        private int _fpsMinimum;
        private int _fpsCurrent;

        public int FPSGraphAdaptiveMinimum;
        public int FPSGraphAdaptiveMaximum = 1000;

        // ready chunks graph values
        private int _readyChunksMax;
        private int _readyChunksAverage;
        private int _readyChunksMinimum;
        private int _readyChunksCurrent;

        public int ReadyChunksGraphAdaptiveMinimum;
        public int ReadyChunksGraphAdaptiveMaximum = 1000;

        public bool AdaptiveLimits = true;
        public int ValuesToGraph = 2500;

        private readonly List<float> _fpsGraphValues = new List<float>();
        private readonly Vector2[] _fpsGraphBackground = new Vector2[4];
        private Rectangle _fpsGraphBounds = new Rectangle(GraphicsConfig.Instance.Width - 280, 10, 270, 35);

        private readonly List<float> _readyChunksGraphValues = new List<float>();
        private readonly Vector2[] _readyChunksBackground = new Vector2[4];
        private Rectangle _readyChunksBounds = new Rectangle(GraphicsConfig.Instance.Width - 280, 70, 270, 35);

        // required services
        private IStatistics _statistics;
        private IChunkCache _chunkCache;

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
            this._chunkCache = (IChunkCache)this.Game.Services.GetService(typeof(IChunkCache));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _primitiveBatch = new PrimitiveBatch(this.GraphicsDevice, 1000);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Game.Content.Load<SpriteFont>("Fonts//calibri");
            _localProjection = Matrix.CreateOrthographicOffCenter(0f, this.GraphicsDevice.Viewport.Width,this.GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
            _localView = Matrix.Identity;

            this.InitGraphs();
        }

        private void InitGraphs()
        {
            _fpsGraphBackground[0] = new Vector2(_fpsGraphBounds.X, _fpsGraphBounds.Y);
            _fpsGraphBackground[1] = new Vector2(_fpsGraphBounds.X, _fpsGraphBounds.Y + _fpsGraphBounds.Height);
            _fpsGraphBackground[2] = new Vector2(_fpsGraphBounds.X + _fpsGraphBounds.Width, _fpsGraphBounds.Y + _fpsGraphBounds.Height);
            _fpsGraphBackground[3] = new Vector2(_fpsGraphBounds.X + _fpsGraphBounds.Width, _fpsGraphBounds.Y);

            _readyChunksBackground[0] = new Vector2(_readyChunksBounds.X, _readyChunksBounds.Y);
            _readyChunksBackground[1] = new Vector2(_readyChunksBounds.X, _readyChunksBounds.Y + _readyChunksBounds.Height);
            _readyChunksBackground[2] = new Vector2(_readyChunksBounds.X + _readyChunksBounds.Width, _readyChunksBounds.Y + _readyChunksBounds.Height);
            _readyChunksBackground[3] = new Vector2(_readyChunksBounds.X + _readyChunksBounds.Width, _readyChunksBounds.Y);
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
            this.DrawReadyChunksGraph(gameTime);

            _primitiveBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            _spriteBatch.DrawString(_spriteFont, "fps",
                                    new Vector2(_fpsGraphBounds.Left, _fpsGraphBounds.Bottom), Color.White);
            _spriteBatch.DrawString(_spriteFont, "curr:" + _fpsCurrent,
                                    new Vector2(_fpsGraphBounds.Left + 25, _fpsGraphBounds.Bottom),
                                    Color.White);
            _spriteBatch.DrawString(_spriteFont, "max:" + _fpsMax,
                                    new Vector2(_fpsGraphBounds.Left + 90, _fpsGraphBounds.Bottom),
                                    Color.White);
            _spriteBatch.DrawString(_spriteFont, "avg:" + _fpsAverage,
                                    new Vector2(_fpsGraphBounds.Left + 150, _fpsGraphBounds.Bottom),
                                    Color.White);
            _spriteBatch.DrawString(_spriteFont, "min:" + _fpsMinimum,
                                    new Vector2(_fpsGraphBounds.Left + 210, _fpsGraphBounds.Bottom),
                                    Color.White);

            _spriteBatch.DrawString(_spriteFont, "rdy",
                        new Vector2(_readyChunksBounds.Left, _readyChunksBounds.Bottom), Color.White);
            _spriteBatch.DrawString(_spriteFont, "curr:" + _readyChunksCurrent,
                                    new Vector2(_readyChunksBounds.Left + 25, _readyChunksBounds.Bottom),
                                    Color.White);
            _spriteBatch.DrawString(_spriteFont, "max:" + _readyChunksMax,
                                    new Vector2(_readyChunksBounds.Left + 90, _readyChunksBounds.Bottom),
                                    Color.White);
            _spriteBatch.DrawString(_spriteFont, "avg:" + _readyChunksAverage,
                                    new Vector2(_readyChunksBounds.Left + 150, _readyChunksBounds.Bottom),
                                    Color.White);
            _spriteBatch.DrawString(_spriteFont, "min:" + _readyChunksMinimum,
                                    new Vector2(_readyChunksBounds.Left + 210, _readyChunksBounds.Bottom),
                                    Color.White);
            _spriteBatch.End();

            // restore old state.
            this.GraphicsDevice.RasterizerState = previousRasterizerState;
            this.GraphicsDevice.DepthStencilState = previousDepthStencilState;
        }

        private void DrawFPSGraph(GameTime gameTime)
        {
            DrawSolidPolygon(_fpsGraphBackground, 4, Color.DarkBlue, true);

            _fpsGraphValues.Add(this._statistics.FPS);

            if (_fpsGraphValues.Count > ValuesToGraph + 1)
                _fpsGraphValues.RemoveAt(0);

            float x = _fpsGraphBounds.X;
            float deltaX = _fpsGraphBounds.Width/(float) ValuesToGraph;
            float yScale = _fpsGraphBounds.Bottom - (float) _fpsGraphBounds.Top;

            // we must have at least 2 values to start rendering
            if (_fpsGraphValues.Count > 2)
            {
                _fpsMax = (int) _fpsGraphValues.Max();
                _fpsAverage = (int) _fpsGraphValues.Average();
                _fpsMinimum = (int) _fpsGraphValues.Min();
                _fpsCurrent = this._statistics.FPS;

                if (AdaptiveLimits)
                {
                    FPSGraphAdaptiveMaximum = _fpsMax;
                    FPSGraphAdaptiveMinimum = 0;
                }

                // start at last value (newest value added)
                // continue until no values are left
                for (int i = _fpsGraphValues.Count - 1; i > 0; i--)
                {
                    float y1 = _fpsGraphBounds.Bottom -
                               ((_fpsGraphValues[i]/(FPSGraphAdaptiveMaximum - FPSGraphAdaptiveMinimum))*yScale);
                    float y2 = _fpsGraphBounds.Bottom -
                               ((_fpsGraphValues[i - 1]/(FPSGraphAdaptiveMaximum - FPSGraphAdaptiveMinimum))*yScale);

                    Vector2 x1 =
                        new Vector2(MathHelper.Clamp(x, _fpsGraphBounds.Left, _fpsGraphBounds.Right),
                                    MathHelper.Clamp(y1, _fpsGraphBounds.Top, _fpsGraphBounds.Bottom));

                    Vector2 x2 =
                        new Vector2(
                            MathHelper.Clamp(x + deltaX, _fpsGraphBounds.Left, _fpsGraphBounds.Right),
                            MathHelper.Clamp(y2, _fpsGraphBounds.Top, _fpsGraphBounds.Bottom));

                    DrawSegment(x1, x2, Color.DeepSkyBlue);

                    x += deltaX;
                }
            }
        }

        private void DrawReadyChunksGraph(GameTime gameTime)
        {
            DrawSolidPolygon(_readyChunksBackground, 4, Color.DarkBlue, true);

            _readyChunksGraphValues.Add(this._chunkCache.StateStatistics[ChunkState.Ready]);

            if (_readyChunksGraphValues.Count > ValuesToGraph + 1)
                _readyChunksGraphValues.RemoveAt(0);

            float x = _fpsGraphBounds.X;
            float deltaX = _readyChunksBounds.Width / (float)ValuesToGraph;
            float yScale = _readyChunksBounds.Bottom - (float)_readyChunksBounds.Top;

            // we must have at least 2 values to start rendering
            if (_readyChunksGraphValues.Count > 2)
            {
                _readyChunksMax = (int)_readyChunksGraphValues.Max();
                _readyChunksAverage = (int)_readyChunksGraphValues.Average();
                _readyChunksMinimum = (int)_readyChunksGraphValues.Min();
                _readyChunksCurrent = this._chunkCache.StateStatistics[ChunkState.Ready];

                if (AdaptiveLimits)
                {
                    ReadyChunksGraphAdaptiveMaximum = _readyChunksMax;
                    ReadyChunksGraphAdaptiveMinimum = 0;
                }

                // start at last value (newest value added)
                // continue until no values are left
                for (int i = _readyChunksGraphValues.Count - 1; i > 0; i--)
                {
                    float y1 = _readyChunksBounds.Bottom -
                               ((_readyChunksGraphValues[i] / (ReadyChunksGraphAdaptiveMaximum - ReadyChunksGraphAdaptiveMinimum)) * yScale);
                    float y2 = _readyChunksBounds.Bottom -
                               ((_readyChunksGraphValues[i - 1] / (ReadyChunksGraphAdaptiveMaximum - ReadyChunksGraphAdaptiveMinimum)) * yScale);

                    Vector2 x1 =
                        new Vector2(MathHelper.Clamp(x, _readyChunksBounds.Left, _readyChunksBounds.Right),
                                    MathHelper.Clamp(y1, _readyChunksBounds.Top, _readyChunksBounds.Bottom));

                    Vector2 x2 =
                        new Vector2(
                            MathHelper.Clamp(x + deltaX, _readyChunksBounds.Left, _readyChunksBounds.Right),
                            MathHelper.Clamp(y2, _readyChunksBounds.Top, _readyChunksBounds.Bottom));

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