/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using VoxeliqEngine.Logging;

namespace VoxeliqStudios.Voxeliq
{
    public static class Program
    {
        private static readonly Logger Logger = LogManager.CreateLogger(); // the logger.

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            // Watch for unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            // Use invariant culture - we have to set it explicitly for every thread we create.
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintBanner();
            PrintLicense();
            Console.ResetColor();

            InitLoggers(); // init logging facility.

            Logger.Info("voxeliq v{0} warming-up..", Assembly.GetExecutingAssembly().GetName().Version);
            PrintKeys();

            var frameworkVersion = System.Reflection.Assembly.GetAssembly(typeof(Microsoft.Xna.Framework.Game)).GetName().Version;

            #if XNA
            Logger.Trace(string.Format("Using XNA (v{0}) as the framework.", frameworkVersion));
            #elif MONOGAME
            Logger.Trace(String.Format("Using MonoGame (v{0}) as the framework.", frameworkVersion));
            #else
            Logger.Trace("Can not determine underlying framework.");
            #endif  

            using (var game = new VoxeliqGame()) // startup the game.
            {
                Logger.Trace("Starting game loop..");
                game.Run();
            }
        }

        private static void InitLoggers()
        {
            LogManager.Enabled = true; // enable logging facility.
            LogManager.AttachLogTarget(new ConsoleTarget(Logger.Level.Trace, Logger.Level.Fatal, false)); // attach a console target.
        }

        private static void PrintKeys()
        {
            Console.WriteLine("Debug keys:");
            Console.WriteLine("-----------------------------");
            Console.WriteLine("F1: Infinitive-world: On/Off.");
            Console.WriteLine("F2: Fly-mode: On/Off.");
            Console.WriteLine("F3: Fog-mode: None/Near/Far.");
            Console.WriteLine("F4: Dynamic Clouds: On/Off.");
            Console.WriteLine("F5: Capture Mouse: On/Off.");
            Console.WriteLine("F6: Bloom: On/Off.");
            Console.WriteLine("F7: Bloom State: Default/Soft/Desaturated/Saturated/Blurry/Subtle");
            Console.WriteLine("F9: Debug Graphs: On/Off.");
            Console.WriteLine("F10: In-game Debugger: On/Off.");
            Console.WriteLine("F11: Frame-limiter: On/Off.");
            Console.WriteLine("F12: Wireframe mode: On/Off.");
            Console.WriteLine("-----------------------------");
        }

        private static void PrintBanner()
        {
            Console.WriteLine(@"                          .__  .__        ");
            Console.WriteLine(@" ___  _________  ___ ____ |  | |__| ______");
            Console.WriteLine(@" \  \/ /  _ \  \/  // __ \|  | |  |/ ____/");
            Console.WriteLine(@"  \   (  <_> >    <\  ___/|  |_|  < <_|  |");
            Console.WriteLine(@"   \_/ \____/__/\_ \\___  >____/__|\__   |");
            Console.WriteLine(@"                  \/    \/            |__|");

            Console.WriteLine();
        }

        private static void PrintLicense()
        {
            Console.WriteLine("Copyright (C) 2011 - 2012 voxeliq project");
            Console.WriteLine();
        }

        /// <summary>
        /// Unhandled exception handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">UnhandledExceptionEventArgs</param>
        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
                Logger.FatalException((e.ExceptionObject as Exception),"Voxeliq terminating because of unhandled exception.");
            else
                Logger.ErrorException((e.ExceptionObject as Exception), "Caught unhandled exception.");
            Console.ReadLine();
        }
    }
}