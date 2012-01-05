using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using VolumetricStudios.VoxeliqEngine.Logging;
using VolumetricStudios.VoxeliqEngine.Utility;

namespace VolumetricStudios.Voxeliq
{
    public static class Program
    {
        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility.

        [STAThread]
        static void Main(string[] args)
        {
            #if !DEBUG // Watch for unhandled exceptions
                AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            #endif

            // Use invariant culture - we have to set it explicitly for every thread we create.
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // TODO: check if console is enabled in configs.
            ConsoleHelper.InitConsole(); // init a log-console.
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintBanner();
            PrintLicense();
            Console.ResetColor();

            InitLoggers(); // init logging facility.
            Logger.Info("voxeliq v{0} warming-up..", Assembly.GetExecutingAssembly().GetName().Version);
            
            System.Windows.Forms.Application.EnableVisualStyles();
            using (var game = new VoxeliqGame())
            {
                Logger.Trace("Starting game loop..");
                game.Run();
            }
        }

        private static void InitLoggers()
        {
            // TODO: read log-targets from config file.
            LogManager.Enabled = true; // enable logging facility.
            LogManager.AttachLogTarget(new ConsoleTarget(Logger.Level.Trace, Logger.Level.Fatal, false)); // attach a console target.
            LogManager.AttachLogTarget(new FileTarget("client.log", Logger.Level.Trace, Logger.Level.Fatal, true, false)); // attach a file-target.
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
