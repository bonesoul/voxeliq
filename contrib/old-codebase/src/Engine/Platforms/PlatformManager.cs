/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using Microsoft.Xna.Framework;

namespace Engine.Platforms
{
    /// <summary>
    /// Platform Manager that identifies platforms & manages them.
    /// </summary>
    public static class PlatformManager
    {
        /// <summary>
        /// The current platform.
        /// </summary>
        public static Platforms Platform { get; private set; }

        /// <summary>
        /// Current .Net framework.
        /// </summary>
        public static NetFrameworks DotNetFramework { get; private set; }

        /// <summary>
        /// Current .Net framework's version.
        /// </summary>
        public static Version DotNetFrameworkVersion { get; private set; }

        /// <summary>
        /// Current game framework.
        /// </summary>
        public static GameFrameworks GameFramework { get; private set; }

        /// <summary>
        /// Current game framework's version.
        /// </summary>
        public static Version GameFrameworkVersion { get; private set; }

        /// <summary>
        /// Current graphics api.
        /// </summary>
        public static GraphicsAPI GraphicsApi { get; private set; }

        /// <summary>
        /// Handler for current platform.
        /// </summary>
        public static PlatformHandler Handler { get; private set; }

        /// <summary>
        /// Helper for current platform.
        /// </summary>
        public static PlatformHelper Helper { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public static Game Game { get; private set; }

        static PlatformManager()
        {
            IdentifyPlatform();
        }

        /// <summary>
        /// Should be called by platform-specific startup code.
        /// </summary>
        public static void Startup(Game game)
        {
            Game = game;
            Handler.PlatformEntrance(); // run the appropriate platform entrace code.
        }

        /// <summary>
        /// Should be called by actual game code's Initialize() method.
        /// </summary>
        /// <param name="game"><see cref="Game"/></param>
        /// <param name="graphicsDeviceManager">The <see cref="GraphicsDeviceManager"/>.</param>
        public static void Initialize(Game game, GraphicsDeviceManager graphicsDeviceManager)
        {
            Handler.Initialize(game, graphicsDeviceManager); // run the appropriate platform initialization code
        }

        /// <summary>
        /// Identifies the current platform and used frameworks.
        /// </summary>
        private static void IdentifyPlatform()
        {
            // find base platform.
            #if WINDOWS && DESKTOP
                Platform = Platforms.Windows;
                Handler = new Windows.WindowsPlatform();
                Helper = new Windows.WindowsHelper();
            #elif WINDOWS && METRO
                Platform = Platforms.WindowsMetro;
                Handler = new WindowsMetro.WindowsMetroPlatform();
                Helper = new WindowsMetro.WindowsMetroHelper();
            #elif LINUX && DESKTOP
                Platform = Platforms.Linux;
                Handler = new Linux.LinuxPlatform();
            #elif MACOS && DESKTOP
                Platform = Platforms.MacOS;
                Handler = new MacOS.MacOSPlatform();
            #elif WINPHONE7
                Platform = Platforms.WindowsPhone7;
                Handler = new WindowsPhone7.WindowsPhone7Platform();
                Helper = new WindowsPhone7.WindowsPhone7Helper();
            #elif WINPHONE8
                Platform = Platforms.WindowsPhone8;
                Handler = new WindowsPhone8.WindowsPhone8Platform();
                Helper = new WindowsPhone8.WindowsPhone8Helper();
            #elif ANDROID
                Platform = Platforms.Android;
                Handler = new Android.AndroidPlatform();
            #elif IOS
                Platform = Platforms.IOS;
                Handler = new IOS.IOSPlatform();
            #endif

            if (Handler == null)
                throw new Exception("Unsupported platform!");

            // find dot.net framework.
            DotNetFramework = IsRunningOnMono() ? NetFrameworks.Mono : NetFrameworks.DotNet;

            // find dot.net framework and game framework version.
            #if METRO
                DotNetFrameworkVersion = System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(Object)).Assembly.GetName().Version;
                GameFrameworkVersion = System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(Microsoft.Xna.Framework.Game)).Assembly.GetName().Version;
            #else
                DotNetFrameworkVersion = Environment.Version;
                #if WINPHONE7 || WINPHONE8
                    GameFrameworkVersion = new Version(typeof(Microsoft.Xna.Framework.Game).Assembly.FullName.Split(',')[1].Split('=')[1]);
                #else
                    GameFrameworkVersion = System.Reflection.Assembly.GetAssembly(typeof(Microsoft.Xna.Framework.Game)).GetName().Version;
                #endif
            #endif

            // find game framework & graphics-api.
            #if XNA
                GameFramework = GameFrameworks.XNA;
                GraphicsApi = GraphicsAPI.DirectX9;
            #elif MONOGAME
                GameFramework = GameFrameworks.MonoGame;
                #if DIRECTX11
                    GraphicsApi = GraphicsAPI.DirectX11;
                #elif OPENGL
                    GraphicsApi = GraphicsAPI.OpenGL;
                #endif
            #endif
        }

        /// <summary>
        /// Returns true if code runs over Mono framework.
        /// </summary>
        /// <returns>true if running over Mono, false otherwise.</returns>
        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }
    }
}
