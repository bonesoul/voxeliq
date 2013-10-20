/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Engine.Common.Helpers.IO
{
    public static class FileHelpers
    {
        public static string AssemblyRoot
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        }

        public static List<string> GetFilesByExtensionRecursive(string directory, string fileExtension)
        {
            var files = new List<string>(); // Store results in the file results list.
            var stack = new Stack<string>(); // Store a stack of our directories.

            stack.Push(directory); // Add initial directory.

            while (stack.Count > 0) // Continue while there are directories to process
            {
                var topDir = stack.Pop(); // Get top directory
                var dirInfo = new DirectoryInfo(topDir);

                files.AddRange((from fileInfo in dirInfo.GetFiles()
                                where string.Compare(fileInfo.Extension, fileExtension,System.StringComparison.OrdinalIgnoreCase) == 0
                                select topDir + "/" + fileInfo.Name).ToList());

                foreach (var dir in Directory.GetDirectories(topDir)) // Add all directories at this directory.
                {
                    stack.Push(dir);
                }
            }

            return files.Select(file => file.Replace("\\", "/")).ToList();
        }
    }
}