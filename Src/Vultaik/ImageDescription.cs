// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using Vortice.Vulkan;

namespace Vultaik
{

    public class ImageDescription
    {
        public VkImageType ImageType { get; set; }

        public byte[]? Data { get; set; }


        public int Size { get; set; }

        public bool IsCubeMap { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Depth { get; set; }

        public int ArraySize { get; set; }

        public int MipLevels { get; set; } = 1; //TODO: MipLevels

        public VkFormat Format { get; set; }

        public ResourceUsage Usage { get; set; }

        public ImageFlags Flags { get; set; }

    }
}