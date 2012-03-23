/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using VolumetricStudios.LibVolumetric.Logging;

namespace VolumetricStudios.VoxeliqGame
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
            //AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            // Use invariant culture - we have to set it explicitly for every thread we create.
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintBanner();
            PrintLicense();
            Console.ResetColor();

            InitLoggers(); // init logging facility.

            Logger.Info("voxeliq v{0} warming-up..", Assembly.GetExecutingAssembly().GetName().Version);

            using (var game = new VoxeliqGame()) // startup voxlr client.
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