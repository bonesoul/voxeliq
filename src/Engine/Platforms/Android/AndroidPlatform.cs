/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
#endif

using Engine.Platforms.Config;
using Microsoft.Xna.Framework;

namespace Engine.Platforms.Android
{
    public class AndroidPlatform : PlatformHandler
    {
        public AndroidPlatform()
        {
            this.Config = new PlatformConfig
            {
                Screen =
                {
                    Width = 800,
                    Height = 480,
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
                    PostprocessEnabled = false,
                    ExtendedEffects = true,
                },
            };
        }
    }
}