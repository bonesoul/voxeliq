using System;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using VolumetricStudios.VoxeliqEngine.Core;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;

namespace VolumetricStudios.VoxeliqEngine.Screen
{
    public interface IGameWindow
    {
        float AspectRatio { get; }
    }

    public class RenderWindow : RenderForm, IGameWindow
    {
        private Device _device; // the graphics-device.
        private SwapChain _swapChain; // the swap-chain.
        private RenderTargetView _renderTarget;
        private DeviceContext _context;
        private Viewport _viewport;
        private Game _game;

        public RenderWindow(Game game)
        {
            this._game = game;
            this._game.AddService(typeof(IGameWindow), this);
            this.InitSwapChain(); // init the swap-chain.         
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Text = "Voxeliq";
            this.Width = ScreenConfig.Instance.Width;
            this.Height = ScreenConfig.Instance.Height;            
        }

        public void RenderFrame()
        {
            this._context.ClearRenderTargetView(_renderTarget, new Color4(0.5f, 0.5f, 1.0f)); // clear the render target to a soothing blue
            this._swapChain.Present(0, PresentFlags.None); // flip back-buffer into front buffer memory.
        }

        private void InitSwapChain()
        {
            // the swap-chain descriptor - http://msdn.microsoft.com/en-us/library/bb173075.aspx, http://slimdx.org/tutorials/DeviceCreation.php
            var swapChainDescription = new SwapChainDescription()
            {
                BufferCount = 1, // The number of buffers to create, including the front buffer. For a windowed device, the desktop is an implicit front buffer and doesn’t need to be taken into account. At least one back buffer should be created in order to enable flipping, as opposed to copying the video memory each frame. 
                Usage = Usage.RenderTargetOutput, // Since we will be rendering to this, we will specify Usage.RenderTargetOutput.
                OutputHandle = this.Handle, // hwnd of the output window.
                IsWindowed = true,
                ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm), // Allows you to specify the desired display mode of the swap chain. 
                SampleDescription = new SampleDescription(1, 0), // The Count parameter lets you pick the number of samples per pixel, and the Quality parameter picks an antialiasing quality level. 
                Flags = SwapChainFlags.AllowModeSwitch, //  AllowModeSwitch flag, which tells DXGI that the swap chain is allowed to adjust the display mode to a more optimal one if necessary. 
                SwapEffect = SwapEffect.Discard // Discard lets DXGI dump all back buffer data after it has been used, which allows it to do things in the most efficient manner.
            };

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, swapChainDescription, out _device, out _swapChain); // create the device & swap-chain.
            _context = _device.ImmediateContext; // immediate-context doesn't care about multithreaded rendering.

            // create a view of our render target, which is the backbuffer of the swap chain we just created
            using (var resource = Resource.FromSwapChain<Texture2D>(_swapChain, 0))
            {
                _renderTarget = new RenderTargetView(_device, resource);
            }

            //  The Rasterizer Stage will use the viewport to transform vertices that are in homogenous clip space (-1 to 1 for X and Y and 0 to 1 for the Z range) into pixel coordinates. Typically, you set the viewport to be the size of the window to which you’ll be rendering.
            this._viewport = new Viewport(0.0f, 0.0f, this.ClientSize.Width, this.ClientSize.Height);

            _context.OutputMerger.SetTargets(_renderTarget);
            _context.Rasterizer.SetViewports(this._viewport);

            // prevent DXGI handling of alt+enter, which doesn't work properly with Winforms
            using (var factory = _swapChain.GetParent<Factory>())
                factory.SetWindowAssociation(this.Handle, WindowAssociationFlags.IgnoreAltEnter);

            // handle alt+enter on our own.
            this.KeyDown += (o, e) =>
            {
                if (!e.Alt || e.KeyCode != Keys.Enter) return;
                _swapChain.IsFullScreen = !_swapChain.IsFullScreen;
            };            
        }

        public float AspectRatio
        {
            get
            {
                return (this._viewport.Height != 0f) && (this._viewport.Width != 0f)
                           ? this._viewport.Width/this._viewport.Height
                           : 0f;
            }
        }


        protected override void Dispose(bool disposing)
        {            
            // clean up all resources - anything we missed will show up in the debug output.
            if(_renderTarget!=null)
                _renderTarget.Dispose();
            if(_swapChain!=null)
                _swapChain.Dispose();
            if(_device!=null)
                _device.Dispose();
        }
    }
}
