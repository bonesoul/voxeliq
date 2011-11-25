/*
 * Copyright (C) 2011 voxlr project 
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using VolumetricStudios.VoxlrEngine.Common.Logging;

namespace VolumetricStudios.VoxlrClient
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

            Logger.Info("voxlr v{0} warming-up..", Assembly.GetExecutingAssembly().GetName().Version);

            using (var game = new VoxlrClient()) // startup voxlr client.
            {
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
            Console.WriteLine(@" ____    ____  ______   ___   ___  __      .______      ");  
            Console.WriteLine(@" \   \  /   / /  __  \  \  \ /  / |  |     |   _  \     ");
            Console.WriteLine(@"  \   \/   / |  |  |  |  \  V  /  |  |     |  |_)  |    ");
            Console.WriteLine(@"   \      /  |  |  |  |   >   <   |  |     |      /     ");
            Console.WriteLine(@"    \    /   |  `--'  |  /  .  \  |  `----.|  |\  \----.");
            Console.WriteLine(@"     \__/     \______/  /__/ \__\ |_______|| _| `._____|");
            Console.WriteLine();                                       
        }

        private static void PrintLicense()
        {
            Console.WriteLine("Copyright (C) 2011 voxlr project");
            Console.WriteLine("voxlr comes with ABSOLUTELY NO WARRANTY.");
            Console.WriteLine("This is free software, and you are welcome to redistribute it under certain conditions; see the LICENSE file for details.");
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

