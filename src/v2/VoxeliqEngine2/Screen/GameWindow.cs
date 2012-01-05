using System;
using System.Drawing;
using System.Windows.Forms;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D10;
using SlimDX.DXGI;
using SlimDX.Windows;
using VolumetricStudios.VoxeliqEngine.Core;
using VolumetricStudios.VoxeliqEngine.Utility.Logging;
using Buffer = SlimDX.Direct3D10.Buffer;
using Device = SlimDX.Direct3D10.Device;
using Font = SlimDX.Direct3D10.Font;
using Resource = SlimDX.Direct3D10.Resource;

namespace VolumetricStudios.VoxeliqEngine.Screen
{
    public interface IGameWindow
    {
        Device Device { get; } // the graphics-device.
        SwapChain SwapChain { get; } // the swap-chain.
        RenderTargetView RenderTarget { get; }
        float AspectRatio { get; }
    }

    public class RenderWindow : RenderForm, IGameWindow
    {
        public Device Device { get { return this._device; } }
        public SwapChain SwapChain { get { return this._swapChain; } }
        public RenderTargetView RenderTarget { get { return this._renderTarget; } }

        private readonly Game _game;
        private Device _device; // the graphics-device.
        private SwapChain _swapChain; // the swap-chain.
        private RenderTargetView _renderTarget;        
        private Viewport _viewport;

        private Effect effect;
        private EffectTechnique technique;
        private EffectPass pass;
        private InputLayout inputLayout;
        private Buffer vertexBuffer;

        // private DeviceContext _context; - DX-11 feature

        private static readonly Logger Logger = LogManager.CreateLogger();

        public RenderWindow(Game game)
        {
            Logger.Trace("init");
            
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

        public void BeingRender(GameTime gameTime)
        {
            //this.Device.ClearRenderTargetView(_renderTarget, new Color4(0.3f, 0.3f, 0.3f)); // clear the render target to a soothing blue
        }

        public void GameWindowRender()
        {
            this.Device.InputAssembler.SetInputLayout(inputLayout);
            this.Device.InputAssembler.SetPrimitiveTopology(PrimitiveTopology.TriangleList);
            this.Device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, 32, 0));

            pass.Apply();
            this.Device.Draw(3, 0);
        }

        public void EndRender(GameTime gameTime)
        {
            this._swapChain.Present(0, PresentFlags.None); // flip back-buffer into front buffer memory.
        }

        private void InitSwapChain()
        {
            var factory = new Factory();

            // TODO: introduce a graphicsDevice & graphicsManager.
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

            Device.CreateWithSwapChain(factory.GetAdapter(0), DriverType.Hardware, DeviceCreationFlags.None, swapChainDescription, out _device, out _swapChain); // create the device & swap-chain.

            // prevent DXGI handling of alt+enter, which doesn't work properly with Winforms
            using (factory = _swapChain.GetParent<Factory>())
                factory.SetWindowAssociation(this.Handle, WindowAssociationFlags.IgnoreAltEnter);

            // DX11: _context = _device.ImmediateContext; // immediate-context doesn't care about multithreaded rendering.

            //  The Rasterizer Stage will use the viewport to transform vertices that are in homogenous clip space (-1 to 1 for X and Y and 0 to 1 for the Z range) into pixel coordinates. Typically, you set the viewport to be the size of the window to which you’ll be rendering.
            this._viewport = new Viewport(0, 0, this.ClientSize.Width, this.ClientSize.Height);            

            // create a view of our render target, which is the backbuffer of the swap chain we just created
            using (var resource = Resource.FromSwapChain<Texture2D>(_swapChain, 0))
            {
                _renderTarget = new RenderTargetView(_device, resource);
            }

            this.Device.OutputMerger.SetTargets(_renderTarget);
            this.Device.Rasterizer.SetViewports(this._viewport);

            this.effect = Effect.FromFile(this.Device, "Effects\\triangle.fx", "fx_4_0");
            this.technique = effect.GetTechniqueByIndex(0);
            this.pass = technique.GetPassByIndex(0);

            var signature = pass.Description.Signature;
            this.inputLayout = new InputLayout(this.Device, signature, new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0) 
            });

            this.vertexBuffer = new Buffer(
                    this.Device, 3 * 32, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None
            );

            DataStream stream = vertexBuffer.Map(MapMode.WriteDiscard, SlimDX.Direct3D10.MapFlags.None);
            stream.WriteRange(new[] {
                new Vector4(0.0f, 0.5f, 0.5f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                new Vector4(-0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
            });
            vertexBuffer.Unmap();            
        }

        public float AspectRatio
        {
            get
            {
                if ((this._viewport.Height != 0f) && (this._viewport.Width != 0f))
                    return this._viewport.Width / this._viewport.Height;
                else
                    return 0;
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
