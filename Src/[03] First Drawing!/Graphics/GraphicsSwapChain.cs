
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DXGI;
using PresentParameters = Graphics.PresentationParameters;
namespace Graphics
{
    public class GraphicsSwapChain
    {

        public GraphicsDevice Device { get; }

        public Texture RenderTarget { get; }

        public PresentParameters PresentParameters { get; }

        public SwapChain SwapChain { get; set; }



        public GraphicsSwapChain(PresentParameters Parameters, GraphicsDevice device)
        {
            Device = device;
            PresentParameters = Parameters;


            SwapChain = CreateSwapChain();
            


            RenderTarget = new Texture(device, this);

        }


        private SwapChain CreateSwapChain()
        {
            using (Factory factory = Device.Adapter.Adapter.GetParent<Factory>())
            {
                //set swapchain desc
                ModeDescription BackBufferDesc = new ModeDescription();
                BackBufferDesc.Width = PresentParameters.Width;
                BackBufferDesc.Height = PresentParameters.Height;
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
                swapChainDesc.OutputHandle = PresentParameters.Handle;
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



                return new SwapChain(factory, Device.NativeDevice, swapChainDesc);
            }
        }


            public void Present(bool sync)
        {
            SwapChain.Present(sync ? 1 : 0, PresentFlags.None);
        }
    }
}
