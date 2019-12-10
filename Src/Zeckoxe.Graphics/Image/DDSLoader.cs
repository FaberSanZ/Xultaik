// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	DDSLoader.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Vortice.Direct3D12;
using Zeckoxe.Graphics;

namespace Zeckoxe.Image
{
    public class DDSLoader
    {
        public enum Caps
        {
            Complex = 0x8,

            Mipmap = 0x400000,

            Texture = 0x1000,
        }


        [Flags]
        public enum Caps2
        {
            Cubemap = 0x200,

            CubemapPositiveX = 0x400,
 
            CubemapNegativeX = 0x800,
 
            CubemapPositiveY = 0x1000,

            CubemapNegativeY = 0x2000,

            CubemapPositiveZ = 0x4000,

            CubemapNegativeZ = 0x8000,

            Volume = 0x200000,

            AllFaces = 0x200 | CubemapPositiveX | CubemapNegativeX | CubemapPositiveY | CubemapNegativeY | CubemapPositiveZ | CubemapNegativeZ,
        }

        public enum FlagTypes
        {
            Caps = 0x1,

            Height = 0x2,

            Width = 0x4,

            Pitch = 0x8,

            PixelFormat = 0x1000,

            MipmapCount = 0x20000,

            LinearSize = 0x80000,

            Depth = 0x800000,
        }


        public enum FlagsDX10
        {
            AlphaModeUnknown = 0x0,

            AlphaModeStraight = 0x1,

            AlphaModePremultiplied = 0x2,

            AlphaModeOpaque = 0x3,

            AlphaModeCustom = 0x4,
        }


        public enum DDSPixelFormat
        {
            AlphaPixels = 0x1,

            Alpha = 0x2,

            Fourcc = 0x4,

            RGB = 0x40,

            YUV = 0x200,

            Luminance = 0x20000,
        }



        [Flags]
        public enum HeaderFlags
        {
            Texture = 0x00001007, 

            Mipmap = 0x00020000, 

            Volume = 0x00800000, 

            Pitch = 0x00000008, 

            LinearSize = 0x00080000, 

            Height = 0x00000002, 

            Width = 0x00000004, 
        }



        [Flags]
        public enum SurfaceFlags
        {
            Texture = 0x00001000, 

            Mipmap = 0x00400008,  

            Cubemap = 0x00000008, 
        }



        [Flags]
        public enum CubemapFlags
        {
            CubeMap = 0x00000200, 

            Volume = 0x00200000, 

            PositiveX = 0x00000600, 

            NegativeX = 0x00000a00, 

            PositiveY = 0x00001200, 

            NegativeY = 0x00002200, 

            PositiveZ = 0x00004200, 

            NegativeZ = 0x00008200, 

            AllFaces = PositiveX | NegativeX | PositiveY | NegativeY | PositiveZ | NegativeZ,
        }


        public enum ResourceDimension : int
        {
            Unknown = unchecked(0),

            Buffer = unchecked(1),

            Texture1D = unchecked(2),

            Texture2D = unchecked(3),

            Texture3D = unchecked(4),
        }


        [Flags]
        public enum ResourceOptionFlags : int
        {
            None = unchecked(0),

            GenerateMipMaps = unchecked(1),

            Shared = unchecked(2),

            TextureCube = unchecked(4),

            DrawindirectArgs = unchecked(16),

            BufferAllowRawViews = unchecked(32),

            BufferStructured = unchecked(64),

            ResourceClamp = unchecked(128),

            SharedKeyedmutex = unchecked(256),

            GdiCompatible = unchecked(512),
        }




        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public int StructSize;

            public HeaderFlags Flags;

            public int Height;

            public int Width;

            public int PitchOrLinearSize;

            public int Depth; 

            public int MipMapCount;

            private readonly uint unused1;
            private readonly uint unused2;
            private readonly uint unused3;
            private readonly uint unused4;
            private readonly uint unused5;
            private readonly uint unused6;
            private readonly uint unused7;
            private readonly uint unused8;
            private readonly uint unused9;
            private readonly uint unused10;
            private readonly uint unused11;

            public DDSPixelFormat PixelFormat;

            public SurfaceFlags SurfaceFlags;

            public CubemapFlags CubemapFlags;

            private readonly uint unused12;
            private readonly uint unused13;
            private readonly uint unused14;
            
        }

    }
}
