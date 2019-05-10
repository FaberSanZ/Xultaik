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

        private GraphicsDevice GraphicsDevice { get; }

        private GraphicsSwapChain SwapChain { get; }



        public Texture(GraphicsDevice device, GraphicsSwapChain swapChain )
        {
            GraphicsDevice = device;
            SwapChain = swapChain;
            NativeRenderTargetView = GetRenderTargetView();
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
    }
}
