/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

namespace Engine.Platforms.Config
{
    public class DebuggerConfig
    {
        /// <summary>
        /// Gets or sets if debug-bar is enabled.
        /// </summary>
        public bool BarEnabled { get; set; }

        /// <summary>
        /// Gets or sets if debug-graphs are enabled.
        /// </summary>
        public bool GraphsEnabled { get; set; }

        /// <summary>
        /// Creates a new instance of debugger-config.
        /// </summary>
        public DebuggerConfig()
        {
#if DEBUG
            this.BarEnabled = true;
            this.GraphsEnabled = true;
#else
                this.BarEnabled = false;
                this.GraphsEnabled = false;
#endif
        }

        public void ToggleBar()
        {
            this.BarEnabled = !this.BarEnabled;
        }

        public void ToggleGraphs()
        {
            this.GraphsEnabled = !this.GraphsEnabled;
        }
    }
}