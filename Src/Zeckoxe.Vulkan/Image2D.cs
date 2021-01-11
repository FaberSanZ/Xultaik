// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	Texture2D.cs
=============================================================================*/

using Vortice.Vulkan;
using Zeckoxe.Vulkan.Toolkit;

namespace Zeckoxe.Vulkan
{
    public unsafe class Image2D : Image
    {
        public Image2D(Device device, ImageDescription description) : base(device, description)
        {
        }

        public static Image2D LoadFromData(Device device, ImageData tex2D)
        {
            Image2D text2d = new Image2D(device, new ImageDescription
            {
                Flags = TextureFlags.ShaderResource,
                Usage = GraphicsResourceUsage.Staging,
                Width = tex2D.Width,
                Height = tex2D.Height,
                Size = tex2D.Size,
                Data = tex2D.Data,
                format = (VkFormat)tex2D.Format,
                Format = tex2D.Format,
                Dimension = ImageDimension.Image2D,
            });

            text2d.LoadTexture2D();

            return text2d;
        }


        public static Image2D LoadFromData(Device device, byte[] data)
        {
            var tex2D = IMGLoader.LoadFromData(data);

            Image2D text2d = new Image2D(device, new ImageDescription
            {
                Flags = TextureFlags.ShaderResource,
                Usage = GraphicsResourceUsage.Staging,
                Width = tex2D.Width,
                Height = tex2D.Height,
                Size = tex2D.Size,
                Data = tex2D.Data,
                format = (VkFormat)tex2D.Format,
                Format = tex2D.Format,
                Dimension = ImageDimension.Image2D,
            });

            text2d.LoadTexture2D();

            return text2d;
        }

        public static Image2D LoadFromFile(Device device, string path)
        {
            ImageData tex2D = new ImageData();

            if (path.EndsWith(".ktx") )
                tex2D = KTXLoader.LoadFromFile(path);

            else if (path.EndsWith(".png") || path.EndsWith(".bmp") || path.EndsWith(".jpg"))
                tex2D = IMGLoader.LoadFromFile(path);


            Image2D text2d = new Image2D(device, new ImageDescription
            {
                Flags = TextureFlags.ShaderResource,
                Usage = GraphicsResourceUsage.Staging,
                Width = tex2D.Width,
                Height = tex2D.Height,
                Size = tex2D.Size,
                Data = tex2D.Data,
                format = (VkFormat)tex2D.Format,
                Format = tex2D.Format,
                Dimension = ImageDimension.Image2D,
            });

            text2d.LoadTexture2D();

            return text2d;
        }
    }
}

