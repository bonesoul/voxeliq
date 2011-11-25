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
using System.IO;

namespace VolumetricStudios.VoxlrEngine.Utils.Log
{
    public static class LogManager
    {
        private static readonly FileStream FileStream;
        private static readonly StreamWriter LogStream;
        private const string LogFile = "debug.log";

        static LogManager()
        {
            FileStream = new FileStream(LogFile, FileMode.Append, FileAccess.Write);
            LogStream = new StreamWriter(FileStream);

            Write(MessageTypes.Info, ".::::::::::::: Logger started :::::::::::::.");
        }

        public static void Write(MessageTypes type, string str)
        {
            LogStream.WriteLine(string.Format("[{0}][{1}]: {2}", DateTime.Now.ToString("HH:mm:ss"), type.ToString().PadLeft(5), str));
            LogStream.AutoFlush = true;
        }
    }

    public enum MessageTypes
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal,
    }
}
