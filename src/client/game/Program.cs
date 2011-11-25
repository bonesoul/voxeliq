/*
 * Copyright (C) 2011 voxeliq project 
 *
 */

using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using VolumetricStudios.VoxeliqEngine.Common.Logging;

namespace VolumetricStudios.VoxeliqClient
{
    public static class Program
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        static void Main(string[] args)
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

            using (var game = new Client()) // startup voxlr client.
            {
                Logger.Info("Starting game loop..");
                game.Run();
            }
        }

        private static void InitLoggers()
        {
            LogManager.Enabled = true;

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

                if (target != null) LogManager.AttachLogTarget(target);
            }
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
            Console.WriteLine("Copyright (C) 2011 voxeliq project");
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
                Logger.FatalException((e.ExceptionObject as Exception), "voxlr-client terminating because of unhandled exception.");
            else
                Logger.ErrorException((e.ExceptionObject as Exception), "Caught unhandled exception.");
            Console.ReadLine();
        }
    }
}

