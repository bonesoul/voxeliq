/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Engine.Platforms.Config;
using Microsoft.Xna.Framework;

namespace Engine.Platforms.WP8
{
    public class WindowsPhone8Platform : PlatformHandler
    {
        public WindowsPhone8Platform()
        {
            this.Config = new PlatformConfig
            {
                Screen =
                {                    
                    Width = 1280,
                    Height = 720,
                    IsFullScreen = false,
                    SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight,
                },
                Input =
                {
                    IsMouseVisible = false,
                },
                Graphics =
                {

                    IsFixedTimeStep = false,
                    IsVsyncEnabled = false,
                    PostprocessEnabled = true,
                    ExtendedEffects = true,
                },
            };
        }
    }
}
