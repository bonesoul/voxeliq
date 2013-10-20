/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using Engine.Assets;
using Engine.Audio;
using Engine.Blocks;
using Engine.Chunks;
using Engine.Chunks.Processors;
using Engine.Core.Config;
using Engine.Debugging;
using Engine.Debugging.Console;
using Engine.Debugging.Graphs;
using Engine.Debugging.Ingame;
using Engine.Graphics;
using Engine.Input;
using Engine.Interface;
using Engine.Sky;
using Engine.Universe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Core
{
    public class Engine
    {
        /// <summary>
        /// The engine configuration.
        /// </summary>
        public EngineConfig Configuration { get; private set; }

        /// <summary>
        /// Attached game.
        /// </summary>
        public Game Game { get; private set; }

        public delegate void EngineStartHandler(object sender, EventArgs e);
        public event EngineStartHandler EngineStart;

        public GameConsole Console { get; private set; }

        public Rasterizer Rasterizer { get; private set; }

        public Engine(Game game, EngineConfig config)
        {
            if (_instance != null)
                throw new Exception("You can not instantiate the Engine more than once.");

            _instance = this;

            this.Game = game;
            this.Configuration = config;

            config.Validate(); // validate the config.
        }

        public void Run()
        {
            this.AddComponents();
            this.NotifyEngineStart(EventArgs.Empty);
        }

        private void NotifyEngineStart(EventArgs e)
        {
            var handler = EngineStart;
            if (handler != null) 
                handler(typeof(Engine), e);
        }

        /// <summary>
        /// Adds game-components.
        /// </summary>
        private void AddComponents()
        {
            this.Rasterizer = new Rasterizer();

            this.Game.Components.Add(new InputManager(this.Game));

            this.Game.Components.Add(new AssetManager(this.Game));

#if XNA
            //this.Game.Components.Add(new Sky(this.Game));
#endif

            this.Game.Components.Add(new NewSky(this.Game));

            this.Game.Components.Add(new Fogger(this.Game));

            var chunkStorage = new ChunkStorage(this.Game);
            this.Game.Components.Add(chunkStorage);            

            var vertexBuilder = new VertexBuilder(this.Game);
            this.Game.Components.Add(vertexBuilder);

            var chunkCache = new ChunkCache(this.Game);
            this.Game.Components.Add(chunkCache);

            var blockStorage = new BlockStorage(this.Game);
            this.Game.Components.Add(blockStorage);

            var world = new World(this.Game, chunkStorage, chunkCache);
            this.Game.Components.Add(world);

            this.Game.Components.Add(new Player(this.Game, world));

            this.Game.Components.Add(new Camera(this.Game));
            this.Game.Components.Add(new UserInterface(this.Game));

            this.Game.Components.Add(new InGameDebugger(this.Game));
            this.Game.Components.Add(new DebugBar(this.Game));
            this.Game.Components.Add(new GraphManager(this.Game));

#if XNA
            this.Game.Components.Add(new AudioManager(this.Game));
#endif

            var spriteBatch = new SpriteBatch(this.Game.GraphicsDevice);
            Console = new GameConsole(this.Game, spriteBatch,  new GameConsoleOptions
                                                             {
                                                                 Font = Game.Content.Load<SpriteFont>(@"Fonts/Verdana"),
                                                                 FontColor = Color.LawnGreen,
                                                                 Prompt = ">",
                                                                 PromptColor = Color.Crimson,
                                                                 CursorColor = Color.OrangeRed,
                                                                 BackgroundColor = Color.Black*0.8f,
                                                                 PastCommandOutputColor = Color.Aqua,
                                                                 BufferColor = Color.Gold
                                                             });
        }

        private static Engine _instance; // the memory instance.

        /// <summary>
        /// Returns the memory instance of Engine.
        /// </summary>
        public static Engine Instance
        {
            get { return _instance; }
        }

        #region de-ctor

        // IDisposable pattern: http://msdn.microsoft.com/en-us/library/fs2xkftw(v=VS.100).aspx

        /// <summary>
        /// Is the engine instance disposed already?
        /// </summary>
        public bool Disposed = false;

        private void Dispose(bool disposing)
        {
            if (this.Disposed) 
                return; // if already disposed, just return

            if (disposing) // only dispose managed resources if we're called from directly or in-directly from user code.
            {
                _instance = null;
            }

            Disposed = true;
        }

        public void Dispose()
        {
            Dispose(true); // Object being disposed by the code itself, dispose both managed and unmanaged objects.
            GC.SuppressFinalize(this); // Take object out the finalization queue to prevent finalization code for it from executing a second time.
        }

        ~Engine() { Dispose(false); } // finalizer called by the runtime. we should only dispose unmanaged objects and should NOT reference managed ones. 

        #endregion
    }
}
