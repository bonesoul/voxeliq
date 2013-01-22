/*
 * Copyright (C) 2011 - 2013 Int6 Studios - http://www.int6.org,
 * Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxeliqEngine.Common.Versions
{
    public static class VersionInfo
    {
        public enum GameFrameworks
        {
            XNA,
            MonoGame
        }

        public enum GraphicsAPI
        {
            DirectX9,
            DirectX11,
            OpenGL
        }

        public enum Platforms
        {
            Windows,
            Linux,
            MacOS,
            Android,
            IOS,
            PSP,
            Ouya,
        }

        public static Platforms Platform { get; private set; }

        public static string DotNetFramework { get; private set; }

        public static Version DotNetFrameworkVersion { get; private set; }

        public static GameFrameworks GameFramework { get; private set; }

        public static Version GameFrameworkVersion { get; private set; }

        public static GraphicsAPI GraphicsApi { get; private set; }

        static VersionInfo()
        {
            Platform = Platforms.Windows;
            DotNetFramework = IsRunningOnMono() ? "Mono" : ".Net";
            DotNetFrameworkVersion = Environment.Version;
            GameFrameworkVersion = System.Reflection.Assembly.GetAssembly(typeof(Microsoft.Xna.Framework.Game)).GetName().Version;

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

        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        /// <summary>
        /// Main assembly version info.
        /// </summary>
        public static class Assembly
        {
            /// <summary>
            /// Main assemblies version.
            /// </summary>
            public const string Version = "0.2.0.*";
        }
    }
}
