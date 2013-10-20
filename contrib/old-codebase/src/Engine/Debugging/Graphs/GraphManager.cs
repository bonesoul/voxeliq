/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Collections.Generic;
using Engine.Assets;
using Engine.Debugging.Graphs.Implementations;
using Engine.Debugging.Graphs.Implementations.ChunkGraphs;
using Engine.Graphics.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Debugging.Graphs
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

        private IAssetManager _assetManager;

        private readonly List<DebugGraph> _graphs=new List<DebugGraph>(); // the current graphs list.

        public GraphManager(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IGraphManager), this); // export the service.
        }

        public override void Initialize()
        {
            // create the graphs modules.
            this._graphs.Add(new FPSGraph(this.Game, new Rectangle(Core.Engine.Instance.Configuration.Graphics.Width - 280, 50, 270, 35)));
            this._graphs.Add(new MemGraph(this.Game, new Rectangle(Core.Engine.Instance.Configuration.Graphics.Width - 280, 105, 270, 35)));
            this._graphs.Add(new GenerateQ(this.Game, new Rectangle(Core.Engine.Instance.Configuration.Graphics.Width - 280, 160, 270, 35)));
            this._graphs.Add(new LightenQ(this.Game, new Rectangle(Core.Engine.Instance.Configuration.Graphics.Width - 280, 215, 270, 35)));
            this._graphs.Add(new BuildQ(this.Game, new Rectangle(Core.Engine.Instance.Configuration.Graphics.Width - 280, 270, 270, 35)));
            this._graphs.Add(new ReadyQ(this.Game, new Rectangle(Core.Engine.Instance.Configuration.Graphics.Width - 280, 325, 270, 35)));
            this._graphs.Add(new RemoveQ(this.Game, new Rectangle(Core.Engine.Instance.Configuration.Graphics.Width - 280, 380, 270, 35)));

            // import required services.
            this._assetManager = (IAssetManager)this.Game.Services.GetService(typeof(IAssetManager));
            if (this._assetManager == null)
                throw new NullReferenceException("Can not find asset manager component.");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // init the drawing related objects.
            _primitiveBatch = new PrimitiveBatch(this.GraphicsDevice, 1000);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = this._assetManager.Verdana;
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
            if (!Core.Engine.Instance.Configuration.Debugging.GraphsEnabled) // check if graphs are enabled.
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
            if (!Core.Engine.Instance.Configuration.Debugging.GraphsEnabled) // check if graphs are enabled.
                return;

            foreach (var graph in this._graphs)
            {
                graph.Update(gameTime); // let the graphs update themself.
            }

            base.Update(gameTime);
        }
    }
}
