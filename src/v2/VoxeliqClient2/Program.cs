/* 
 * Voxeliq Project - http://www.voxeliq.org
 */

using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using VolumetricStudios.VoxeliqClient.Utils.Helpers;
using VolumetricStudios.VoxeliqEngine.Logging;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;

namespace VolumetricStudios.VoxeliqClient
{
    static class Program
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        static void Main()
        {
            // Watch for unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

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

            var renderForm = new GameForm();

            // the swap-chain descriptor - http://msdn.microsoft.com/en-us/library/bb173075.aspx, http://slimdx.org/tutorials/DeviceCreation.php
            var swapChainDescription = new SwapChainDescription()
            {
                BufferCount = 1, // The number of buffers to create, including the front buffer. For a windowed device, the desktop is an implicit front buffer and doesn’t need to be taken into account. At least one back buffer should be created in order to enable flipping, as opposed to copying the video memory each frame. 
                Usage = Usage.RenderTargetOutput, // Since we will be rendering to this, we will specify Usage.RenderTargetOutput.
                OutputHandle = renderForm.Handle, // hwnd of the output window.
                IsWindowed = true,
                ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm), // Allows you to specify the desired display mode of the swap chain. 
                SampleDescription = new SampleDescription(1, 0), // The Count parameter lets you pick the number of samples per pixel, and the Quality parameter picks an antialiasing quality level. 
                Flags = SwapChainFlags.AllowModeSwitch, //  AllowModeSwitch flag, which tells DXGI that the swap chain is allowed to adjust the display mode to a more optimal one if necessary. 
                SwapEffect = SwapEffect.Discard // Discard lets DXGI dump all back buffer data after it has been used, which allows it to do things in the most efficient manner.
            };

            Device device; // the graphics-device.
            SwapChain swapChain; // the swap-chain.
    
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, swapChainDescription, out device, out swapChain); // create the device & swap-chain.

            var context = device.ImmediateContext; // immediate-context doesn't care about multithreaded rendering.

            // create a view of our render target, which is the backbuffer of the swap chain we just created
            RenderTargetView renderTarget;
            using (var resource = Resource.FromSwapChain<Texture2D>(swapChain, 0))
            {
                renderTarget = new RenderTargetView(device, resource);
            }

            //  The Rasterizer Stage will use the viewport to transform vertices that are in homogenous clip space (-1 to 1 for X and Y and 0 to 1 for the Z range) into pixel coordinates. Typically, you set the viewport to be the size of the window to which you’ll be rendering.
            var viewport = new Viewport(0.0f, 0.0f, renderForm.ClientSize.Width, renderForm.ClientSize.Height);

            context.OutputMerger.SetTargets(renderTarget);
            context.Rasterizer.SetViewports(viewport);

            // prevent DXGI handling of alt+enter, which doesn't work properly with Winforms
            using (var factory = swapChain.GetParent<Factory>())
                factory.SetWindowAssociation(renderForm.Handle, WindowAssociationFlags.IgnoreAltEnter);

            // handle alt+enter on our own.
            renderForm.KeyDown += (o, e) =>
            {
                if (!e.Alt || e.KeyCode != Keys.Enter) return;
                swapChain.IsFullScreen = !swapChain.IsFullScreen;
            };

            // Uses interop to directly call into Win32 methods to bypass any allocations on the managed side.
            MessagePump.Run(renderForm, () =>
            {
                context.ClearRenderTargetView(renderTarget, new Color4(0.5f, 0.5f, 1.0f)); // clear the render target to a soothing blue
                swapChain.Present(0, PresentFlags.None); // flip back-buffer into front buffer memory.
            });

            // clean up all resources
            // anything we missed will show up in the debug output
            renderTarget.Dispose();
            swapChain.Dispose();
            device.Dispose();
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
            Console.WriteLine(@"Copyright (C) 2011-2012 voxeliq project");
            Console.WriteLine();
        }

        private static void InitLoggers()
        {
            LogManager.Enabled = true; // enable logging facility.
            LogManager.AttachLogTarget(new ConsoleTarget(Logger.Level.Trace, Logger.Level.Fatal, false)); // attach a console target.
            LogManager.AttachLogTarget(new FileTarget("client.log", Logger.Level.Trace, Logger.Level.Fatal, true, false)); // attach a file-target.
        }

        /// <summary>
        /// Unhandled exception handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">UnhandledExceptionEventArgs</param>
        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
                Logger.FatalException((e.ExceptionObject as Exception), "voxeliq-client terminating because of unhandled exception.");
            else
                Logger.ErrorException((e.ExceptionObject as Exception), "Caught unhandled exception.");
            Console.ReadLine();
        }
    }
}
