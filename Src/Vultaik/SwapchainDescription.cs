// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


using System;
using Vortice.Vulkan;

namespace Vultaik
{
    public struct SwapchainDescription : IEquatable<SwapchainDescription>
    {

        public SwapchainDescription(SwapchainSource source, int width, int height, VkFormat? depthFormat, bool vSync)
        {
            Source = source;
            Width = width;
            Height = height;
            DepthFormat = depthFormat;
            VSync = vSync;
            ColorSrgb = false;
        }


        public SwapchainDescription(SwapchainSource source, int width, int height, VkFormat? depthFormat, bool vSync, bool colorSrgb)
        {
            Source = source;
            Width = width;
            Height = height;
            DepthFormat = depthFormat;
            VSync = vSync;
            ColorSrgb = colorSrgb;
        }


        public SwapchainSource Source { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public VkFormat? DepthFormat { get; set; }

        public bool VSync { get; set; }

        public bool ColorSrgb { get; set; }


        public bool Equals(SwapchainDescription other)
        {
            return Source.Equals(other.Source)
                && Width.Equals(other.Width)
                && Height.Equals(other.Height)
                && DepthFormat == other.DepthFormat
                && VSync.Equals(other.VSync)
                && ColorSrgb.Equals(other.ColorSrgb);
        }


    }
}
