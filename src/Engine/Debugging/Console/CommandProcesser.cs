/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

/* Code based on: http://code.google.com/p/xnagameconsole/ */

using System.Linq;

namespace Engine.Debugging.Console
{
    class CommandProcesser
    {
        public string Process(string buffer)
        {
            var output = CommandManager.Parse(buffer);
            return output;
        }

        static string GetCommandName(string buffer)
        {
            var firstSpace = buffer.IndexOf(' ');
            return buffer.Substring(0, firstSpace < 0 ? buffer.Length : firstSpace);
        }

        static string[] GetArguments(string buffer)
        {
            var firstSpace = buffer.IndexOf(' ');
            if (firstSpace < 0)
            {
                return new string[0];
            }
            
            var args = buffer.Substring(firstSpace, buffer.Length - firstSpace).Split(' ');
            return args.Where(a => a != "").ToArray();
        }
    }
}
