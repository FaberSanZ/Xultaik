using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Resource = SharpDX.Direct3D11.Resource;

namespace Graphics
{
    public class Texture
    {
        public RenderTargetView NativeRenderTargetView { get; set; }

        public DepthStencilView NativeDepthStencilView { get; set; }

        private GraphicsDevice GraphicsDevice { get; }

        private GraphicsSwapChain SwapChain { get; }



        public Texture(GraphicsDevice device, GraphicsSwapChain swapChain )
        {
            GraphicsDevice = device;
            SwapChain = swapChain;

            NativeRenderTargetView = GetRenderTargetView();
            NativeDepthStencilView = GetDepthStencilView();
        }


        private RenderTargetView GetRenderTargetView()
        {
            RenderTargetViewDescription NativeRenderTargetViewDes = new RenderTargetViewDescription();
            NativeRenderTargetViewDes.Dimension = RenderTargetViewDimension.Texture2D;
            NativeRenderTargetViewDes.Format = Format.R8G8B8A8_UNorm;
            NativeRenderTargetViewDes.Texture2D = new RenderTargetViewDescription.Texture2DResource()
            {
                MipSlice = 0,
            };
            

            Texture2D Resource = SwapChain.SwapChain.GetBackBuffer<Texture2D>(0);


            return new RenderTargetView(GraphicsDevice.NativeDevice, Resource, NativeRenderTargetViewDes);
        }


        private DepthStencilView GetDepthStencilView()
        {
            // Initialize and set up the description of the depth buffer.
            Texture2DDescription DepthBufferDesc = new Texture2DDescription();
            DepthBufferDesc.Width = SwapChain.PresentParameters.Width;
            DepthBufferDesc.Height = SwapChain.PresentParameters.Width;
            DepthBufferDesc.MipLevels = 1;
            DepthBufferDesc.ArraySize = 1;
            DepthBufferDesc.Format = Format.D24_UNorm_S8_UInt;
            DepthBufferDesc.SampleDescription = new SampleDescription(1, 0);
            DepthBufferDesc.Usage = ResourceUsage.Default;
            DepthBufferDesc.BindFlags = BindFlags.DepthStencil;
            DepthBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            DepthBufferDesc.OptionFlags = ResourceOptionFlags.None;


            return new DepthStencilView(GraphicsDevice.NativeDevice, new Texture2D(GraphicsDevice.NativeDevice, DepthBufferDesc));
        }
    }
}
