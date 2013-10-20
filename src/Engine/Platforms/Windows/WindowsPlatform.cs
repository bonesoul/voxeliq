/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Engine.Platforms.Config;

namespace Engine.Platforms.Windows
{
    public class WindowsPlatform : PlatformHandler
    {
        public WindowsPlatform()
        {
            this.Config = new PlatformConfig
            {
                Screen =
                {
                    IsFullScreen = false,
                    Width = 1680,
                    Height = 1050,
                },
                Input =
                {
                    IsMouseVisible = true,
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

        public override void PlatformEntrance()
        {
            using (var game = PlatformManager.Game)
            {
                game.Run();
            }
        }
    }
}