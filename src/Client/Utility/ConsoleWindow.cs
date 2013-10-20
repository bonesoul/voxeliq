/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;

namespace Client.Utility
{
    /// <summary>
    /// Utility class to handle console window stuff.
    /// </summary>
    public static class ConsoleWindow
    {
        /// <summary>
        /// Prints an info banner.
        /// </summary>
        public static void PrintBanner()
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
        public static void PrintLicense()
        {
            Console.WriteLine("Copyright (C) 2011 - 2013, Voxeliq");
            Console.WriteLine("Voxeliq comes with ABSOLUTELY NO WARRANTY.");
            Console.WriteLine("This is free software, and you are welcome to redistribute it under certain conditions; see the LICENSE file for details.");
            Console.WriteLine();
        }
    }
}
