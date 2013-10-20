/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System.Collections.Generic;
using Engine.Graphics.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Debugging.Graphs
{
    /// <summary>
    /// DebugGraph is a special graph module that can draw graphs for debugging purposes.
    /// <remarks>DebugGraph's is not a DrawableGameComponent itself but the GraphManager call's its update() and draw functions.</remarks>
    /// </summary>
    public class DebugGraph
    {
        /// <summary>
        /// The attached game.
        /// </summary>
        protected Game Game { get; private set; }

        /// <summary>
        /// Graph bounds.
        /// </summary>
        protected Rectangle Bounds { get; private set; }

        // settings 

        /// <summary>
        /// Is the graph adaptive to handled values maximum and mininums?
        /// </summary>
        public bool AdaptiveLimits { get; set; }

        /// <summary>
        /// Number of values to graph in a given moment.
        /// </summary>
        public int ValuesToGraph { get; private set; }

        /// <summary>
        /// Holds values of current session.
        /// </summary>
        protected readonly List<float> GraphValues = new List<float>();

        /// <summary>
        /// Used for drawing the background shape.
        /// </summary>
        protected readonly Vector2[] BackgroundPolygon = new Vector2[4];

        /// <summary>
        /// Current maximum value.
        /// </summary>
        protected int MaxValue;

        /// <summary>
        /// Current average value.
        /// </summary>
        protected int AverageValue;

        /// <summary>
        /// Current minimum value.
        /// </summary>
        protected int MinimumValue;

        /// <summary>
        /// Current value.
        /// </summary>
        protected int CurrentValue;

        /// <summary>
        /// Adaptive minimum value.
        /// </summary>
        protected int AdaptiveMinimum;

        /// <summary>
        /// Adaptive maximum value. When our current value is higher than AdaptiveMaximum, it will rise it AdaptiveMaximum.
        /// </summary>
        protected int AdaptiveMaximum = 1000;

        // stuff required for drawing.
        protected PrimitiveBatch PrimitiveBatch;
        protected SpriteBatch SpriteBatch;
        protected SpriteFont SpriteFont;
        protected Matrix LocalProjection;
        protected Matrix LocalView;

        /// <summary>
        /// Creates a new debug graph.
        /// </summary>
        /// <param name="game">The attached game.</param>
        /// <param name="bounds">Graphs bounds.</param>
        public DebugGraph(Game game, Rectangle bounds)            
        {
            this.Game = game;
            this.Bounds = bounds;
            this.AdaptiveLimits = true;
            this.ValuesToGraph = 2500;
        }

        /// <summary>
        /// Attachs the required drawing objects to graph.
        /// </summary>
        /// <param name="primitiveBatch"><see cref="PrimitiveBatch"/></param>
        /// <param name="spriteBatch"><see cref="SpriteBatch"/></param>
        /// <param name="spriteFont"><see cref="SpriteFont"/></param>
        /// <param name="localProjection"><see cref="Matrix"/></param>
        /// <param name="localView"><see cref="Matrix"/></param>
        public void AttachGraphics(PrimitiveBatch primitiveBatch, SpriteBatch spriteBatch, SpriteFont spriteFont, Matrix localProjection, Matrix localView)
        {
            this.PrimitiveBatch = primitiveBatch;
            this.SpriteBatch = spriteBatch;
            this.SpriteFont = spriteFont;
            this.LocalProjection = localProjection;
            this.LocalView = localView;

            this.Initialize();
            this.LoadContent();
        }

        /// <summary>
        /// Should be overloaded by the actual graph object and initialize itself.
        /// </summary>
        protected virtual void Initialize()
        { }

        /// <summary>
        /// Loads the required contents.
        /// </summary>
        public void LoadContent()
        {
            // calculate the coordinates for drawing the background.
            BackgroundPolygon[0] = new Vector2(Bounds.X - 2, Bounds.Y - 2); // top left
            BackgroundPolygon[3] = new Vector2(Bounds.X + 2 + Bounds.Width, Bounds.Y - 2); // top right
            BackgroundPolygon[1] = new Vector2(Bounds.X - 2, Bounds.Y + Bounds.Height + 14); // bottom left
            BackgroundPolygon[2] = new Vector2(Bounds.X + 2 + Bounds.Width, Bounds.Y + Bounds.Height + 14); // bottom right
        }

        /// <summary>
        /// Should be overloaded by the actual graph object and update itself.
        /// </summary>
        public virtual void Update(GameTime gameTime)
        { }

        /// <summary>
        /// Should be overloaded by the actual graph object and should only draw any strings.
        /// <remarks>SpriteBatch gets handled (begin and end functions) by the calling GraphManager itself.</remarks>
        /// </summary>
        public virtual void DrawStrings(GameTime gameTime)
        { }

        /// <summary>
        /// Should be overloaded by the actual graph object and should only draw primitive shapes.
        /// <remarks>PrimitiveBatch gets handled (begin and end functions) by the calling GraphManager itself.</remarks>
        /// </summary>
        public virtual void DrawGraph(GameTime gameTime)
        { }
    }
}
