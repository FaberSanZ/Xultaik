using System;
using System.IO;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.WIC;
using Device = SharpDX.Direct3D11.Device;
using PixelFormat = SharpDX.WIC.PixelFormat;

namespace _07__Texture
{
    public class BitmapLoader
    {
        // Propertues
        public ShaderResourceView TextureResource { get; private set; }

        public static ShaderResourceView LoadTextureFromFile(Device device, string fileName)
        {
            BitmapLoader loader = new BitmapLoader();
            loader.Initialize(device, fileName);
            return loader.TextureResource;
        }

        // Methods.
        private void Initialize(Device device, string fileName)
        {
            Texture2D texture = LoadFromFile(device, new ImagingFactory(), fileName);

            ShaderResourceViewDescription srvDesc = new ShaderResourceViewDescription();

            srvDesc.Format = texture.Description.Format;
            srvDesc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
            srvDesc.Texture2D.MostDetailedMip = 0;
            srvDesc.Texture2D.MipLevels = -1;

            TextureResource = new ShaderResourceView(device, texture, srvDesc);
            device.ImmediateContext.GenerateMips(TextureResource);
        }

        public void ShutDown()
        {
            TextureResource?.Dispose();
            TextureResource = null;
        }

        public Texture2D LoadFromFile(Device device, ImagingFactory factory, string fileName)
        {
            using (BitmapSource bs = LoadBitmap(factory, fileName))
                return CreateTexture2DFromBitmap(device, bs);
        }

        public BitmapSource LoadBitmap(ImagingFactory factory, string filename)
        {
            BitmapDecoder bitmapDecoder = new BitmapDecoder(factory, filename, DecodeOptions.CacheOnDemand);

            FormatConverter result = new FormatConverter(factory);

            result.Initialize(bitmapDecoder.GetFrame(0), PixelFormat.Format32bppPRGBA, 
                              BitmapDitherType.None, null, 0.0, BitmapPaletteType.Custom);

            return result;
        }

        public Texture2D CreateTexture2DFromBitmap(Device device, BitmapSource bitmapSource)
        {
            Texture2DDescription desc;
            desc.Width = bitmapSource.Size.Width;
            desc.Height = bitmapSource.Size.Height;
            desc.ArraySize = 1;
            desc.BindFlags = BindFlags.ShaderResource;
            desc.Usage = ResourceUsage.Default;
            desc.CpuAccessFlags = CpuAccessFlags.None;
            desc.Format = Format.R8G8B8A8_UNorm;
            desc.MipLevels = 1;
            desc.OptionFlags = ResourceOptionFlags.None;
            desc.SampleDescription.Count = 1;
            desc.SampleDescription.Quality = 0;

            DataStream DS = new DataStream(bitmapSource.Size.Height * bitmapSource.Size.Width * 4, true, true);
            bitmapSource.CopyPixels(bitmapSource.Size.Width * 4, DS);

            DataRectangle rect = new DataRectangle(DS.DataPointer, bitmapSource.Size.Width * 4);

            Texture2D t2D = new Texture2D(device, desc, rect);

            return t2D;
        }
        
    }
}
