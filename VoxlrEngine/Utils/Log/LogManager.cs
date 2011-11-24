/*    
 * Copyright (C) 2011, Hüseyin Uslu
 *  
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
