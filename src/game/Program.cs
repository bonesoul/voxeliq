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
using VoxeliqEngine.Common.Logging;
using VoxeliqEngine.Common.Versions;
using VoxeliqEngine.Universe;

namespace VoxeliqGame
{
    public static class Program
    {
        private static readonly Logger Logger = LogManager.CreateLogger(); // logger instance.

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            #if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler; // Watch for any unhandled exceptions.
            #endif

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture; // Use invariant culture - we have to set it explicitly for every thread we create to prevent any mpq-reading problems (mostly because of number formats).
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintBanner();
            PrintLicense();
            PrintDebugKeys();
            Console.ResetColor();

            InitLoggers(); // init logging facility.

            // print version information.
            var frameworkVersion = Assembly.GetAssembly(typeof(Microsoft.Xna.Framework.Game)).GetName().Version;

            Logger.Info("voxeliq v{0} warming-up..", Assembly.GetAssembly(typeof (Player)).GetName().Version);
            Logger.Info(string.Format("Using framework {0} over {1}.", VersionInfo.GameFramework, VersionInfo.GraphicsApi));            

            using (var game = new Game()) // startup the game.
            {
                Logger.Trace("Starting game loop..");
                game.Run();
            }
        }

        #region logging facility

        private static void InitLoggers()
        {
            LogManager.Enabled = true; // enable logger by default.

            foreach (var targetConfig in LogConfig.Instance.Targets)
            {
                if (!targetConfig.Enabled)
                    continue;

                LogTarget target = null;
                switch (targetConfig.Target.ToLower())
                {
                    case "console":
                        target = new ConsoleTarget(targetConfig.MinimumLevel, targetConfig.MaximumLevel,
                                                   targetConfig.IncludeTimeStamps);
                        break;
                    case "file":
                        target = new FileTarget(targetConfig.FileName, targetConfig.MinimumLevel,
                                                targetConfig.MaximumLevel, targetConfig.IncludeTimeStamps,
                                                targetConfig.ResetOnStartup);
                        break;
                }

                if (target != null)
                    LogManager.AttachLogTarget(target);
            }
        }

        #endregion

        #region unhandled exception emitter

        /// <summary>
        /// Unhandled exception emitter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;

            if (e.IsTerminating)
                Logger.FatalException(ex, "Voxeliq Engine is terminating because of unhandled exception.");
            else
                Logger.ErrorException(ex, "Caught unhandled exception.");

            Console.ReadLine();
        }

        #endregion

        #region console banners

        /// <summary>
        /// Prints an info banner.
        /// </summary>
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

        /// <summary>
        /// Prints a copyright banner.
        /// </summary>
        private static void PrintLicense()
        {
            Console.WriteLine("Copyright (C) 2011 - 2013, Voxeliq Engine");
            Console.WriteLine("mooege comes with ABSOLUTELY NO WARRANTY.");
            Console.WriteLine("This is free software, and you are welcome to redistribute it under certain conditions; see the LICENSE file for details.");
            Console.WriteLine();
        }

        private static void PrintDebugKeys()
        {
            Console.WriteLine("Debug keys:");
            Console.WriteLine("-----------------------------");
            Console.WriteLine("F4: Dynamic Clouds: On/Off.");
            Console.WriteLine("F5: Capture Mouse: On/Off.");
            Console.WriteLine("F6: Bloom: On/Off.");
            Console.WriteLine("F7: Bloom State: Default/Soft/Desaturated/Saturated/Blurry/Subtle");
            Console.WriteLine("F10: In-game Debugger: On/Off.");
            Console.WriteLine("-----------------------------");
        }

        #endregion
    }
}