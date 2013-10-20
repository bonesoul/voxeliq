/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using Engine.Platforms.Config;
using Microsoft.Xna.Framework;

namespace Engine.Platforms.WP7
{
    public class WindowsPhone7Platform : PlatformHandler
    {
        public WindowsPhone7Platform()
        {
            this.Config = new PlatformConfig
            {
                Screen =
                {
                    IsFullScreen = true,
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
                    ExtendedEffects = false,
                },
            };
        }

        public override void PlatformEntrance()
        {
            using (var game = PlatformManager.Game)
            {
                game.Run();
            }
        }

        public override void Initialize()
        {
            // Frame rate is 30 fps by default for Windows Phone.
            this.Game.TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            this.Game.InactiveSleepTime = TimeSpan.FromSeconds(1);
        }
    }
}
