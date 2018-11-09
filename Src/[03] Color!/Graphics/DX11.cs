using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX;
using Device = SharpDX.Direct3D11.Device;
using SharpDX.Mathematics.Interop;
using System.Drawing;

namespace _03__Color_
{
    public class DX11
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        protected DepthStencilView DepthStencilView { get; set; }

        public Device Device { get; set; }

        public DeviceContext DeviceContext { get; set; }

        public SwapChain SwapChain { get; set; }

        public RenderTargetView RenderTargetView { get; set; }
        private Viewport Viewport;

        public DX11() { }

        public void Initialize(Control Con)
        {
            ModeDescription BackBufferDesc = new ModeDescription();
            BackBufferDesc.Width = Con.ClientSize.Width;
            BackBufferDesc.Height = Con.ClientSize.Height;
            BackBufferDesc.Format = Format.R8G8B8A8_UNorm;
            BackBufferDesc.RefreshRate = new Rational(60, 1);
            BackBufferDesc.ScanlineOrdering = DisplayModeScanlineOrder.Unspecified;
            BackBufferDesc.Scaling = DisplayModeScaling.Unspecified;

            // Initialize the swap chain description.
            SwapChainDescription swapChainDesc = new SwapChainDescription();
            // Set to a single back buffer.
            swapChainDesc.BufferCount = 1;
            // Set the width and height of the back buffer.
            swapChainDesc.ModeDescription = BackBufferDesc;
            // Set the usage of the back buffer.
            swapChainDesc.Usage = Usage.RenderTargetOutput;
            // Set the handle for the window to render to.
            swapChainDesc.OutputHandle = Con.Handle;
            // Turn multisampling off.
            swapChainDesc.SampleDescription = new SampleDescription()
            {
                Count = 1,
                Quality = 0
            };
            // Set to full screen or windowed mode.
            swapChainDesc.IsWindowed = !false;
            // Don't set the advanced flags.
            swapChainDesc.Flags = SwapChainFlags.None;
            // Discard the back buffer content after presenting.
            swapChainDesc.SwapEffect = SwapEffect.Discard;

            // Create the swap chain, Direct3D device, and Direct3D device context.
            Device device;
            SwapChain swapChain;
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, swapChainDesc, out device, out swapChain);

            Device = device;
            SwapChain = swapChain;
            DeviceContext = device.ImmediateContext;

            // Create render target view for back buffer
            Texture2D backBuffer = SwapChain.GetBackBuffer<Texture2D>(0);
            RenderTargetView = new RenderTargetView(Device, backBuffer);

            CreateViewport(Con);
        }


        public void Clear(float red, float green, float blue, float alpha = 1.0f)
        {
            R = red;
            G = green;
            B = blue;
            A = alpha;

            // Set back buffer as current render target view
            DeviceContext.OutputMerger.SetRenderTargets(RenderTargetView);

            // Clear the screen
            DeviceContext.ClearRenderTargetView(RenderTargetView, new Color4(R, G, B, A));
        }

        public void CreateViewport(Control Con)
        {
            Viewport = new Viewport();
            Viewport.Height = Con.ClientSize.Height;
            Viewport.Width = Con.ClientSize.Width;
            Viewport.X = 0;
            Viewport.Y = 0;
            Viewport.MaxDepth = 1;
            Viewport.MinDepth = 0;

            // Setup and create the viewport for rendering.
            DeviceContext.Rasterizer.SetViewport(Viewport);
        }

        public void Clear()
        {
            // Set back buffer as current render target view
            DeviceContext.OutputMerger.SetRenderTargets(RenderTargetView);

            // Clear the screen
            DeviceContext.ClearRenderTargetView(RenderTargetView, new Color4(R, G, B, A));
        }


        public void Present()
        {
            SwapChain.Present(1, PresentFlags.None);
        }

        internal void Dispose()
        {
            RenderTargetView.Dispose();
            SwapChain.Dispose();
            Device.Dispose();
            DeviceContext.Dispose();
        }
    }
}
