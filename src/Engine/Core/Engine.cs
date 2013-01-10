/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxeliqEngine.Assets;
using VoxeliqEngine.Audio;
using VoxeliqEngine.Chunks;
using VoxeliqEngine.Chunks.Processors;
using VoxeliqEngine.Debugging;
using VoxeliqEngine.Debugging.Console;
using VoxeliqEngine.Debugging.Console.Commands;
using VoxeliqEngine.Debugging.Graphs;
using VoxeliqEngine.Debugging.Ingame;
using VoxeliqEngine.Graphics;
using VoxeliqEngine.Input;
using VoxeliqEngine.Interface;
using VoxeliqEngine.Universe;

namespace VoxeliqEngine.Core
{
    public class Engine
    {
        /// <summary>
        /// The engine configuration.
        /// </summary>
        public EngineConfiguration Configuration { get; private set; }

        /// <summary>
        /// Attached game.
        /// </summary>
        public Game Game { get; private set; }

        public delegate void EngineStartHandler(object sender, EventArgs e);
        public event EngineStartHandler EngineStart;

        private static Engine _instance; // the memory instance.

        public GameConsole Console { get; private set; }

        public Engine(Game game, EngineConfiguration config)
        {
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
            this.Game.Components.Add(new InputManager(this.Game));

            this.Game.Components.Add(new AssetManager(this.Game));

#if XNA
            this.Game.Components.Add(new Sky(this.Game));
#endif

            this.Game.Components.Add(new Fogger(this.Game));

            var chunkStorage = new ChunkStorage(this.Game);
            this.Game.Components.Add(chunkStorage);

            var vertexBuilder = new VertexBuilder(this.Game);
            this.Game.Components.Add(vertexBuilder);

            var chunkCache = new ChunkCache(this.Game);
            this.Game.Components.Add(chunkCache);

            var world = new World(this.Game, chunkStorage, chunkCache);
            this.Game.Components.Add(world);

            this.Game.Components.Add(new Player(this.Game, world));

            this.Game.Components.Add(new Camera(this.Game));
            this.Game.Components.Add(new UserInterface(this.Game));

            this.Game.Components.Add(new InGameDebugger(this.Game));
            this.Game.Components.Add(new Statistics(this.Game));
            this.Game.Components.Add(new GraphManager(this.Game));

#if XNA
            this.Game.Components.Add(new AudioManager(this.Game));
#endif

            var spriteBatch = new SpriteBatch(this.Game.GraphicsDevice);
            var commands = new IConsoleCommand[]
                               {
                                   new RenderCommand(),
                                   new RasterizerCommand(), 
                               };
            Console = new GameConsole(this.Game, spriteBatch, commands, new GameConsoleOptions
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

        /// <summary>
        /// Returns the memory instance of AssetManager.
        /// </summary>
        public static Engine Instance
        {
            get { return _instance; }
        }
    }
}
