using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace VolumetricStudios.VoxeliqEngine.Utils.Helpers
{
    public class ConsoleHelper
    {
        // Win32 API constants.
        private const int StdOutputHandle = -11;
        private const int CodePage = 437;

        public static void InitConsole() // binds a new console window to a windowed application.
        {
            NativeMethods.AllocConsole(); // allocate a new console window.
            var stdHandle = NativeMethods.GetStdHandle(StdOutputHandle); // the stdout handle.
            var safeFileHandle = new SafeFileHandle(stdHandle, true);
            var fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            var encoding = Encoding.GetEncoding(CodePage);
            var standardOutput = new StreamWriter(fileStream, encoding) { AutoFlush = true };
            Console.SetOut(standardOutput); // set console's output stream to stdout.
        }

        internal static class NativeMethods
        {
            /* Win32 API entries; GetStdHandle() and AllocConsole() allows a windowed application to bind a console window */
            [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            internal static extern IntPtr GetStdHandle(int nStdHandle);

            [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            internal static extern int AllocConsole();
        }
    }
}
