using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Windows.Forms;
using Device = SharpDX.Direct3D11.Device;

namespace _13__Framework
{
    public class D3D11
    {
        // Properties.
        private bool VerticalSyncEnabled { get; set; }
        public int VideoCardMemory { get; private set; }
        public string VideoCardDescription { get; private set; }
        private SwapChain SwapChain { get; set; }
        public Device Device { get; private set; }
        public DeviceContext DeviceContext { get; private set; }
        private RenderTargetView RenderTargetView { get; set; }
        private Texture2D DepthStencilBuffer { get; set; }
        public DepthStencilState DepthStencilState { get; set; }
        private DepthStencilView DepthStencilView { get; set; }
        private RasterizerState RasterState { get; set; }

        // Constructor
        public D3D11() { }

        // Methods
        public bool Initialize(Control configuration)
        {
            try
            {
                // Store the vsync setting.
                VerticalSyncEnabled = true;

                // Create a DirectX graphics interface factory.
                Factory1 factory = new Factory1();

                // Use the factory to create an adapter for the primary graphics interface (video card).
                Adapter1 adapter = factory.GetAdapter1(0);

                // Get the primary adapter output (monitor).
                Output monitor = adapter.GetOutput(0);

                // Get modes that fit the DXGI_FORMAT_R8G8B8A8_UNORM display format for the adapter output (monitor).
                ModeDescription[] modes = monitor.GetDisplayModeList(Format.R8G8B8A8_UNorm, DisplayModeEnumerationFlags.Interlaced);

                // Now go through all the display modes and find the one that matches the screen width and height.
                // When a match is found store the the refresh rate for that monitor, if vertical sync is enabled. 
                // Otherwise we use maximum refresh rate.
                Rational rational = new Rational(0, 1);
                if (VerticalSyncEnabled)
                {
                    foreach (ModeDescription mode in modes)
                    {
                        if (mode.Width == configuration.ClientSize.Width && mode.Height == configuration.ClientSize.Height)
                        {
                            rational = new Rational(mode.RefreshRate.Numerator, mode.RefreshRate.Denominator);
                            break;
                        }
                    }
                }

                // Get the adapter (video card) description.
                AdapterDescription adapterDescription = adapter.Description;

                // Store the dedicated video card memory in megabytes.
                VideoCardMemory = adapterDescription.DedicatedVideoMemory >> 10 >> 10;

                // Convert the name of the video card to a character array and store it.
                VideoCardDescription = adapterDescription.Description.Trim('\0');

                // Release the adapter output.
                monitor.Dispose();
                // Release the adapter.
                adapter.Dispose();
                // Release the factory.
                factory.Dispose();

                //ModeDescription description = new ModeDescription();
                //description.Format = Format.R8G8B8A8_UNorm;
                //description.Height = configuration.ClientSize.Height;
                //description.Width = configuration.ClientSize.Width;

                // Initialize the swap chain description.
                SwapChainDescription swapChainDesc = new SwapChainDescription();
                // Set to a single back buffer.
                swapChainDesc.BufferCount = 1;
                // Set the width and height of the back buffer.
                swapChainDesc.ModeDescription = new ModeDescription(configuration.ClientSize.Width, configuration.ClientSize.Height, rational, Format.R8G8B8A8_UNorm);
                // Set the usage of the back buffer.
                swapChainDesc.Usage = Usage.RenderTargetOutput;
                // Set the handle for the window to render to.
                swapChainDesc.OutputHandle = configuration.Handle;
                // Turn multisampling off.
                swapChainDesc.SampleDescription = new SampleDescription(1, 0);
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

                // Get the pointer to the back buffer.
                Texture2D backBuffer = Texture2D.FromSwapChain<Texture2D>(SwapChain, 0);

                // Create the render target view with the back buffer pointer.
                RenderTargetView = new RenderTargetView(device, backBuffer);

                // Release pointer to the back buffer as we no longer need it.
                backBuffer.Dispose();

                // Initialize and set up the description of the depth buffer.
                Texture2DDescription depthBufferDesc = new Texture2DDescription();
                depthBufferDesc.Width = configuration.ClientSize.Width;
                depthBufferDesc.Height = configuration.ClientSize.Height;
                depthBufferDesc.MipLevels = 1;
                depthBufferDesc.ArraySize = 1;
                depthBufferDesc.Format = Format.D24_UNorm_S8_UInt;
                depthBufferDesc.SampleDescription = new SampleDescription(1, 0);
                depthBufferDesc.Usage = ResourceUsage.Default;
                depthBufferDesc.BindFlags = BindFlags.DepthStencil;
                depthBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
                depthBufferDesc.OptionFlags = ResourceOptionFlags.None;

                // Create the texture for the depth buffer using the filled out description.
                DepthStencilBuffer = new Texture2D(device, depthBufferDesc);


                // Initialize and set up the description of the stencil state.
                DepthStencilStateDescription depthStencilDesc = new DepthStencilStateDescription();
                depthStencilDesc.IsDepthEnabled = true;
                depthStencilDesc.DepthWriteMask = DepthWriteMask.All;
                depthStencilDesc.DepthComparison = Comparison.Less;
                depthStencilDesc.IsStencilEnabled = true;
                depthStencilDesc.StencilReadMask = 0xFF;
                depthStencilDesc.StencilWriteMask = 0xFF;
                // Stencil operation if pixel front-facing.
                depthStencilDesc.FrontFace = new DepthStencilOperationDescription()
                {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Increment,
                    PassOperation = StencilOperation.Keep,
                    Comparison = Comparison.Always
                };
                // Stencil operation if pixel is back-facing.
                depthStencilDesc.BackFace = new DepthStencilOperationDescription()
                {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Decrement,
                    PassOperation = StencilOperation.Keep,
                    Comparison = Comparison.Always
                };

                // Create the depth stencil state.
                DepthStencilState = new DepthStencilState(Device, depthStencilDesc);

                // Set the depth stencil state.
                DeviceContext.OutputMerger.SetDepthStencilState(DepthStencilState, 1);



                // Initialize and set up the depth stencil view.
                DepthStencilViewDescription depthStencilViewDesc = new DepthStencilViewDescription();
                depthStencilViewDesc.Format = Format.D24_UNorm_S8_UInt;
                depthStencilViewDesc.Dimension = DepthStencilViewDimension.Texture2D;
                depthStencilViewDesc.Texture2D = new DepthStencilViewDescription.Texture2DResource()
                {
                    MipSlice = 0
                };

                // Create the depth stencil view.
                DepthStencilView = new DepthStencilView(Device, DepthStencilBuffer, depthStencilViewDesc);

                // Bind the render target view and depth stencil buffer to the output render pipeline.
                DeviceContext.OutputMerger.SetTargets(DepthStencilView, RenderTargetView);



                // Setup the raster description which will determine how and what polygon will be drawn.
                RasterizerStateDescription rasterDesc = new RasterizerStateDescription();
                rasterDesc.IsAntialiasedLineEnabled = false;
                rasterDesc.CullMode = CullMode.None;
                rasterDesc.DepthBias = 0;
                rasterDesc.DepthBiasClamp = 0.0f;
                rasterDesc.IsDepthClipEnabled = true;
                rasterDesc.FillMode = FillMode.Solid;
                rasterDesc.IsFrontCounterClockwise = false;
                rasterDesc.IsMultisampleEnabled = false;
                rasterDesc.IsScissorEnabled = false;
                rasterDesc.SlopeScaledDepthBias = 0.0f;

                // Create the rasterizer state from the description we just filled out.
                RasterState = new RasterizerState(Device, rasterDesc);

                // Now set the rasterizer state.
                DeviceContext.Rasterizer.State = RasterState;


                // Setup and create the viewport for rendering.
                Viewport viewport = new Viewport();
                viewport.Width = configuration.ClientSize.Width;
                viewport.Height = configuration.ClientSize.Height;
                viewport.X = 0;
                viewport.Y = 0;
                viewport.MaxDepth = 1;
                viewport.MinDepth = 0;

                DeviceContext.Rasterizer.SetViewport(viewport);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void BeginScene(float red, float green, float blue, float alpha = 0.0f)
        {
            // Clear the depth buffer.
            DeviceContext.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth, 1, 0);

            // Clear the back buffer.   Color.Transparent.ToColor4()
            DeviceContext.ClearRenderTargetView(RenderTargetView, new Color4(red, green, blue, alpha));
        }

        public void EndScene()
        {
            // Present the back buffer to the screen since rendering is complete.
            if (VerticalSyncEnabled)
                SwapChain.Present(1, 0); // Lock to screen refresh rate.
            else
                SwapChain.Present(0, 0); // Present as fast as possible.
        }

        public void ShutDown()
        {
            // Before shutting down set to windowed mode or when you release the swap chain it will throw an exception.
            if (SwapChain != null)
            {
                SwapChain?.SetFullscreenState(false, null);
                SwapChain.Dispose();
                SwapChain = null;
            }

            if (RasterState != null)
            {
                RasterState.Dispose();
                RasterState = null;
            }

            if (DepthStencilView != null)
            {
                DepthStencilView.Dispose();
                DepthStencilView = null;
            }

            if (DepthStencilState != null)
            {
                DepthStencilState.Dispose();
                DepthStencilState = null;
            }

            if (DepthStencilBuffer != null)
            {
                DepthStencilBuffer.Dispose();
                DepthStencilBuffer = null;
            }

            if (RenderTargetView != null)
            {
                RenderTargetView.Dispose();
                RenderTargetView = null;
            }

            if (DeviceContext != null)
            {
                DeviceContext.Dispose();
                DeviceContext = null;
            }

            if (Device != null)
            {
                Device.Dispose();
                Device = null;
            }

            if (SwapChain != null)
            {
                SwapChain.Dispose();
                SwapChain = null;
            }
        }
    }
}
