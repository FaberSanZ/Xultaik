using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.WIC;
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





        public static ShaderResourceView LoadFromFile(GraphicsDevice device, string fileName)
        {
            var factory = new ImagingFactory();

            BitmapDecoder bitmapDecoder = new BitmapDecoder(factory, fileName, DecodeOptions.CacheOnDemand);

            FormatConverter FC = new FormatConverter(factory);

            FC.Initialize(bitmapDecoder.GetFrame(0), PixelFormat.Format32bppPRGBA, BitmapDitherType.None, null, 0.0, BitmapPaletteType.Custom);

            Texture2DDescription desc = new Texture2DDescription();
            desc.Width = FC.Size.Width;
            desc.Height = FC.Size.Height;
            desc.ArraySize = 1;
            desc.BindFlags = BindFlags.ShaderResource;
            desc.Usage = ResourceUsage.Default;
            desc.CpuAccessFlags = CpuAccessFlags.None;
            desc.Format = Format.R8G8B8A8_UNorm;
            desc.MipLevels = 1;
            desc.OptionFlags = ResourceOptionFlags.None;
            desc.SampleDescription.Count = 1;
            desc.SampleDescription.Quality = 0;

            DataStream DS = new DataStream(FC.Size.Height * FC.Size.Width * 4, true, true);

            FC.CopyPixels(FC.Size.Width * 4, DS);

            DataRectangle rect = new DataRectangle(DS.DataPointer, FC.Size.Width * 4);

            Texture2D t2D = new Texture2D(device.NativeDevice, desc, rect);

            ShaderResourceViewDescription srvDesc = new ShaderResourceViewDescription();
            srvDesc.Format = t2D.Description.Format;
            srvDesc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
            srvDesc.Texture2D.MostDetailedMip = 0;
            srvDesc.Texture2D.MipLevels = -1;

            ShaderResourceView TextureResource = new ShaderResourceView(device.NativeDevice, t2D, srvDesc);

            device.NativeDeviceContext.GenerateMips(TextureResource);


            return TextureResource;

        }
    }
}
