/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Microsoft.Xna.Framework;

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

        private static Engine _instance; // the memory instance.

        public Engine(Game game, EngineConfiguration config)
        {
            _instance = this;
            this.Game = game;
            this.Configuration = config;

            config.Validate(); // validate the config.
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
