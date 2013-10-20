/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Engine.Platforms.Config;

namespace Engine.Platforms.IOS
{
    public class IOSPlatform : PlatformHandler
    {
        public IOSPlatform()
        {
            this.Config = new PlatformConfig
            {
                Screen =
                {
                    Width = 480,
                    Height = 800,
                    IsFullScreen = true,
                    #if IOS
                    SupportedOrientations = DisplayOrientation.FaceDown | DisplayOrientation.FaceUp,
                    #endif
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
    }
}