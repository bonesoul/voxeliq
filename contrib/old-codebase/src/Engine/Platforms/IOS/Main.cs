/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

#if IOS
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace VoxeliqEngine.Platforms.IOS
{
    [Register("AppDelegate")]
    class Program : UIApplicationDelegate
    {
        public override void FinishedLaunching(UIApplication app)
        {
            PlatformManager.Startup(PlatformManager.Game);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
#endif