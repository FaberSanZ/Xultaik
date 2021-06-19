using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortice.Vulkan;

namespace Vultaik
{
    public class ImageFile : IDisposable
    {

        public static Image LoadFromData(Device device, ImageDescription description)
        {
            Image text2d = new Image(device, description);

            return text2d;
        }


        public static Image Load2DFromBytes(Device device, byte[] data)
        {
            var tex2D = IMGLoader.LoadFromData(data);

            Image text2d = new Image(device, new ImageDescription
            {
                Flags = ImageFlags.ShaderResource,
                Usage = ResourceUsage.GPU_Only,
                Width = tex2D.Width,
                Height = tex2D.Height,
                Size = tex2D.Size,
                Data = tex2D.Data,
                Format = tex2D.Format,
                ImageType = VkImageType.Image2D,
            });

            text2d.Image2D();

            return text2d;
        }

        public static Image Load2DFromFile(Device device, string path)
        {
            ImageDescription description = new();

            if (path.EndsWith(".ktx"))
                description = KTXDecoder.LoadFromFile(path);

            else
                description = IMGLoader.LoadFromFile(path);

            description.ImageType = VkImageType.Image2D;
            description.Flags = ImageFlags.ShaderResource;
            description.Usage = ResourceUsage.GPU_Only;


            Image text2d = new Image(device, description);

            text2d.Image2D();

            return text2d;
        }




        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
