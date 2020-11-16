// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	Texture2D.cs
=============================================================================*/

using System.IO;
using Vortice.Vulkan;
using Zeckoxe.Graphics.Toolkit;

namespace Zeckoxe.Graphics
{
    public static unsafe class Texture2D 
    {

        //public Texture2D(Device device) : base(device, new TextureDescription() 
        //{ 
        //    Flags = TextureFlags.ShaderResource,

        //})
        //{
        //    //Recreate();
        //}


        public static unsafe Texture LoadFromData(Device device, TextureData tex2D)
        {
            var text2d = new Texture(device, new TextureDescription
            {
                Flags = TextureFlags.ShaderResource,
                Usage = GraphicsResourceUsage.Staging,
                Width = tex2D.Width,
                Height = tex2D.Height,
                Size = tex2D.Size,
                Data = tex2D.Data,
                format = (VkFormat)tex2D.Format,
                Format = tex2D.Format,
                Dimension = ImageDimension.Texture2D,
            });

            text2d.LoadTexture2D();

            return text2d;
        }

        public static unsafe Texture LoadFromFile(Device device, string path)
        {
            TextureData tex2D = new TextureData();

            if (path.EndsWith(".ktx"))
            {
                tex2D = KTXLoader.LoadFromFile(path);
            }

            if (path.EndsWith(".png"))
            {
                tex2D = IMGLoader.LoadFromFile(path);
            }

            if (path.EndsWith(".bmp"))
            {
                tex2D = IMGLoader.LoadFromFile(path);
            }

            

            var text2d = new Texture(device, new TextureDescription
            {
                Flags = TextureFlags.ShaderResource,
                Usage = GraphicsResourceUsage.Staging,
                Width = tex2D.Width,
                Height = tex2D.Height,
                Size = tex2D.Size,
                Data = tex2D.Data,
                format = (VkFormat)tex2D.Format,
                Format = tex2D.Format,
                Dimension = ImageDimension.Texture2D,
            });

            text2d.LoadTexture2D();

            return text2d;
        }
    }
}

