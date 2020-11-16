// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	Texture2D.cs
=============================================================================*/

using Vortice.Vulkan;

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


        public static unsafe Texture LoadFromFile(Device ctx, TextureData tex2D)
        {
            var text2d = new Texture(ctx, new TextureDescription
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

