/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

namespace Engine.Platforms.Config
{
    public class PlatformConfig
    {
        /// <summary>
        /// Screen config.
        /// </summary>
        public ScreenConfig Screen { get; private set; }

        /// <summary>
        /// Graphics config.
        /// </summary>
        public GraphicsConfig Graphics { get; private set; }

        /// <summary>
        /// Input config.
        /// </summary>
        public InputConfig Input { get; private set; }

        /// <summary>
        /// Debugger config.
        /// </summary>
        public DebuggerConfig Debugger { get; private set; }

        /// <summary>
        /// Creates a new instance of platform-config.
        /// </summary>
        public PlatformConfig()
        {
            // init. sub-configs.
            this.Screen = new ScreenConfig();
            this.Graphics = new GraphicsConfig();
            this.Input = new InputConfig();
            this.Debugger = new DebuggerConfig();
        }
    }
}