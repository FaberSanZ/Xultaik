// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Vortice.Direct3D12;


namespace Xultaik.Graphics
{

    public class DDSLoader
    {

        internal enum HeaderFlags
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


        internal enum FlagsDX10
        {
            AlphaModeUnknown = 0x0,

            AlphaModeStraight = 0x1,

            AlphaModePremultiplied = 0x2,

            AlphaModeOpaque = 0x3,

            AlphaModeCustom = 0x4,
        }


        internal enum DDSPixelFormats
        {
            AlphaPixels = 0x1,

            Alpha = 0x2,

            Fourcc = 0x4,

            RGB = 0x40,

            YUV = 0x200,

            Luminance = 0x20000,
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



        [StructLayout(LayoutKind.Sequential)]
        internal struct DdsHeader
        {
            const int DDSMagic = 0x20534444;

            public readonly static int StructSize = Interop.SizeOf<DdsHeader>();

            public int Size;

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


            public static bool GetInfo(byte[] data, out DdsHeader header, out HeaderDXT10? header10, out int offset)
            {
                // Validate DDS file in memory
                header = new DdsHeader();
                header10 = null;
                offset = 0;

                if (data.Length < (sizeof(uint) + DdsHeader.StructSize))
                    return false;
                

                // First is magic number
                int dwMagicNumber = BitConverter.ToInt32(data, 0x0);
                if (dwMagicNumber != DdsHeader.DDSMagic)
                    return false;
                

                header = Interop.ToStructure<DdsHeader>(data, 4, DdsHeader.StructSize);

                // Verify header to validate DDS file
                if (header.Size != DdsHeader.StructSize || header.PixelFormat.Size != DDSPixelFormat.StructSize)
                    return false;
                

                // Check for DX10 extension
                if (header.PixelFormat.IsDX10())
                {
                    int h10Offset = 4 + DdsHeader.StructSize + HeaderDXT10.StructSize;

                    // Must be long enough for both headers and magic value
                    if (data.Length < h10Offset)
                        return false;
                    

                    header10 = Interop.ToStructure<HeaderDXT10>(data, 4, HeaderDXT10.StructSize);

                    offset = h10Offset;
                }
                else
                    offset = 0x4 + DdsHeader.StructSize;
                

                return true;
            }

            public static bool GetInfo(string filename, out DdsHeader header, out HeaderDXT10? header10, out int offset, out byte[] buffer)
            {
                buffer = File.ReadAllBytes(filename);
                return GetInfo(buffer, out header, out header10, out offset);
            }


            public static bool ValidateTexture(DdsHeader header, HeaderDXT10? header10, out int depth, out PixelFormat format, out ResourceDimension resDim, out int arraySize, out bool isCubeMap)
            {
                if (header10.HasValue)
                    return ValidateTexture(header10.Value, header.Flags, out depth, out format, out resDim, out arraySize, out isCubeMap);
                
                else
                    return ValidateTexture(header, out depth, out format, out resDim, out arraySize, out isCubeMap);
                
            }

            private static bool ValidateTexture(DdsHeader header, out int depth, out PixelFormat format, out ResourceDimension resDim, out int arraySize, out bool isCubeMap)
            {
                depth = 0x0;
                format = Xultaik.Graphics.PixelFormat.Unknown;
                resDim = ResourceDimension.Unknown;
                arraySize = 1;
                isCubeMap = false;

                format = header.PixelFormat.GetDXGIFormat();
                if (format == Xultaik.Graphics.PixelFormat.Unknown)
                    return false;
                

                if (header.Flags.HasFlag(HeaderFlags.Depth))
                    resDim = ResourceDimension.Texture3D;
                
                else
                {
                    if (header.SurfaceFlags.HasFlag(SurfaceFlags.Cubemap))
                    {
                        // We require all six faces to be defined
                        if ((header.CubemapFlags & CubemapFlags.AllFaces) != CubemapFlags.AllFaces)
                            return false;
                        

                        arraySize = 6;
                        isCubeMap = true;
                    }

                    depth = 1;
                    resDim = ResourceDimension.Texture2D;
                }

                return true;
            }

            private static bool ValidateTexture(HeaderDXT10 header, HeaderFlags flags, out int depth, out PixelFormat format, out ResourceDimension resDim, out int arraySize, out bool isCubeMap)
            {
                depth = 0;
                format = Xultaik.Graphics.PixelFormat.Unknown;
                resDim = ResourceDimension.Unknown;
                arraySize = 0x1;
                isCubeMap = false;

                arraySize = header.ArraySize;
                if (arraySize == 0)
                    return false;
                

                if (header.MiscFlag2 != FlagsDX10.AlphaModeUnknown)
                    return false;
                

                if (DDSPixelFormat.BitsPerPixel(header.DXGIFormat) == 0)
                    return false;
                

                format = header.DXGIFormat;

                switch (header.Dimension)
                {
                    case ResourceDimension.Texture1D:
                        depth = 1;
                        break;

                    case ResourceDimension.Texture2D:
                        if (header.MiscFlag.HasFlag(ResourceOptionFlags.TextureCube))
                        {
                            arraySize *= 6;
                            isCubeMap = true;
                        }
                        depth = 1;
                        break;

                    case ResourceDimension.Texture3D:
                        if (!flags.HasFlag(HeaderFlags.Depth))
                            return false;
                        

                        if (arraySize > 1)
                            return false;
                        
                        break;

                    default:
                        return false;
                }

                resDim = header.Dimension;

                return true;
            }

        }






        internal struct DDSPixelFormat
        {

            public readonly static int StructSize = Interop.SizeOf<DDSPixelFormat>();

            public int Size;

            public DDSPixelFormats Flags;

            public int FourCC;

            public int RGBBitCount;

            public uint RBitMask;

            public uint GBitMask;

            public uint BBitMask;

            public uint ABitMask;

            public static int MakeFourCC(string text)
            {
                byte ch0 = (byte)text[0];
                byte ch1 = (byte)text[1];
                byte ch2 = (byte)text[2];
                byte ch3 = (byte)text[3];

                return (ch0 | (ch1 << 8) | (ch2 << 16) | (ch3 << 24));
            }

            public static void GetSurfaceInfo(int width, int height, PixelFormat fmt, out int outNumBytes, out int outRowBytes, out int outNumRows)
            {
                int numBytes = 0;
                int rowBytes = 0;
                int numRows = 0;

                bool bc = false;
                bool packed = false;
                int bcnumBytesPerBlock = 0;
                switch (fmt)
                {
                    case PixelFormat.BC1_Typeless:
                    case PixelFormat.BC1_UNorm:
                    case PixelFormat.BC1_UNorm_SRgb:
                    case PixelFormat.BC4_Typeless:
                    case PixelFormat.BC4_UNorm:
                    case PixelFormat.BC4_SNorm:
                        bc = true;
                        bcnumBytesPerBlock = 8;
                        break;

                    case PixelFormat.BC2_Typeless:
                    case PixelFormat.BC2_UNorm:
                    case PixelFormat.BC2_UNorm_SRgb:
                    case PixelFormat.BC3_Typeless:
                    case PixelFormat.BC3_UNorm:
                    case PixelFormat.BC3_UNorm_SRgb:
                    case PixelFormat.BC5_Typeless:
                    case PixelFormat.BC5_UNorm:
                    case PixelFormat.BC5_SNorm:
                    case PixelFormat.BC6H_Typeless:
                    case PixelFormat.BC6H_Uf16:
                    case PixelFormat.BC6H_Sf16:
                    case PixelFormat.BC7_Typeless:
                    case PixelFormat.BC7_UNorm:
                    case PixelFormat.BC7_UNorm_SRgb:
                        bc = true;
                        bcnumBytesPerBlock = 16;
                        break;

                    case PixelFormat.R8G8_B8G8_UNorm:
                    case PixelFormat.G8R8_G8B8_UNorm:
                        packed = true;
                        break;
                }

                if (bc)
                {
                    int numBlocksWide = 0;
                    if (width > 0)
                        numBlocksWide = Math.Max(1, (width + 3) / 4);
                    
                    int numBlocksHigh = 0;
                    if (height > 0)
                        numBlocksHigh = Math.Max(1, (height + 3) / 4);
                    
                    rowBytes = numBlocksWide * bcnumBytesPerBlock;
                    numRows = numBlocksHigh;
                }
                else if (packed)
                {
                    rowBytes = ((width + 1) >> 1) * 4;
                    numRows = height;
                }
                else
                {
                    int bpp = BitsPerPixel(fmt);
                    rowBytes = (width * bpp + 7) / 8; // round up to nearest byte
                    numRows = height;
                }

                numBytes = rowBytes * numRows;

                outNumBytes = numBytes;
                outRowBytes = rowBytes;
                outNumRows = numRows;
            }

            public static int BitsPerPixel(PixelFormat fmt)
            {
                switch (fmt)
                {
                    case PixelFormat.R32G32B32A32_Typeless:
                    case PixelFormat.R32G32B32A32_Float:
                    case PixelFormat.R32G32B32A32_UInt:
                    case PixelFormat.R32G32B32A32_SInt:
                        return 128;

                    case PixelFormat.R32G32B32_Typeless:
                    case PixelFormat.R32G32B32_Float:
                    case PixelFormat.R32G32B32_UInt:
                    case PixelFormat.R32G32B32_SInt:
                        return 96;

                    case PixelFormat.R16G16B16A16_Typeless:
                    case PixelFormat.R16G16B16A16_Float:
                    case PixelFormat.R16G16B16A16_UNorm:
                    case PixelFormat.R16G16B16A16_UInt:
                    case PixelFormat.R16G16B16A16_SNorm:
                    case PixelFormat.R16G16B16A16_SInt:
                    case PixelFormat.R32G32_Typeless:
                    case PixelFormat.R32G32_Float:
                    case PixelFormat.R32G32_UInt:
                    case PixelFormat.R32G32_SInt:
                    case PixelFormat.R32G8X24_Typeless:
                    case PixelFormat.D32_Float_S8X24_UInt:
                    case PixelFormat.R32_Float_X8X24_Typeless:
                    case PixelFormat.X32_Typeless_G8X24_UInt:
                        return 64;

                    case PixelFormat.R10G10B10A2_Typeless:
                    case PixelFormat.R10G10B10A2_UNorm:
                    case PixelFormat.R10G10B10A2_UInt:
                    case PixelFormat.R11G11B10_Float:
                    case PixelFormat.R8G8B8A8_Typeless:
                    case PixelFormat.R8G8B8A8_UNorm:
                    case PixelFormat.R8G8B8A8_UNorm_SRgb:
                    case PixelFormat.R8G8B8A8_UInt:
                    case PixelFormat.R8G8B8A8_SNorm:
                    case PixelFormat.R8G8B8A8_SInt:
                    case PixelFormat.R16G16_Typeless:
                    case PixelFormat.R16G16_Float:
                    case PixelFormat.R16G16_UNorm:
                    case PixelFormat.R16G16_UInt:
                    case PixelFormat.R16G16_SNorm:
                    case PixelFormat.R16G16_SInt:
                    case PixelFormat.R32_Typeless:
                    case PixelFormat.D32_Float:
                    case PixelFormat.R32_Float:
                    case PixelFormat.R32_UInt:
                    case PixelFormat.R32_SInt:
                    case PixelFormat.R24G8_Typeless:
                    case PixelFormat.D24_UNorm_S8_UInt:
                    case PixelFormat.R24_UNorm_X8_Typeless:
                    case PixelFormat.X24_Typeless_G8_UInt:
                    case PixelFormat.R9G9B9E5_Sharedexp:
                    case PixelFormat.R8G8_B8G8_UNorm:
                    case PixelFormat.G8R8_G8B8_UNorm:
                    case PixelFormat.B8G8R8A8_UNorm:
                    case PixelFormat.B8G8R8X8_UNorm:
                    case PixelFormat.R10G10B10_Xr_Bias_A2_UNorm:
                    case PixelFormat.B8G8R8A8_Typeless:
                    case PixelFormat.B8G8R8A8_UNorm_SRgb:
                    case PixelFormat.B8G8R8X8_Typeless:
                    case PixelFormat.B8G8R8X8_UNorm_SRgb:
                        return 32;

                    case PixelFormat.R8G8_Typeless:
                    case PixelFormat.R8G8_UNorm:
                    case PixelFormat.R8G8_UInt:
                    case PixelFormat.R8G8_SNorm:
                    case PixelFormat.R8G8_SInt:
                    case PixelFormat.R16_Typeless:
                    case PixelFormat.R16_Float:
                    case PixelFormat.D16_UNorm:
                    case PixelFormat.R16_UNorm:
                    case PixelFormat.R16_UInt:
                    case PixelFormat.R16_SNorm:
                    case PixelFormat.R16_SInt:
                    case PixelFormat.B5G6R5_UNorm:
                    case PixelFormat.B5G5R5A1_UNorm:
                    case PixelFormat.B4G4R4A4_UNorm:
                        return 16;

                    case PixelFormat.R8_Typeless:
                    case PixelFormat.R8_UNorm:
                    case PixelFormat.R8_UInt:
                    case PixelFormat.R8_SNorm:
                    case PixelFormat.R8_SInt:
                    case PixelFormat.A8_UNorm:
                        return 8;

                    case PixelFormat.R1_UNorm:
                        return 1;

                    case PixelFormat.BC1_Typeless:
                    case PixelFormat.BC1_UNorm:
                    case PixelFormat.BC1_UNorm_SRgb:
                    case PixelFormat.BC4_Typeless:
                    case PixelFormat.BC4_UNorm:
                    case PixelFormat.BC4_SNorm:
                        return 4;

                    case PixelFormat.BC2_Typeless:
                    case PixelFormat.BC2_UNorm:
                    case PixelFormat.BC2_UNorm_SRgb:
                    case PixelFormat.BC3_Typeless:
                    case PixelFormat.BC3_UNorm:
                    case PixelFormat.BC3_UNorm_SRgb:
                    case PixelFormat.BC5_Typeless:
                    case PixelFormat.BC5_UNorm:
                    case PixelFormat.BC5_SNorm:
                    case PixelFormat.BC6H_Typeless:
                    case PixelFormat.BC6H_Uf16:
                    case PixelFormat.BC6H_Sf16:
                    case PixelFormat.BC7_Typeless:
                    case PixelFormat.BC7_UNorm:
                    case PixelFormat.BC7_UNorm_SRgb:
                        return 8;

                    default:
                        return 0;
                }
            }




            public bool IsDX10() => this.Flags.HasFlag(DDSPixelFormats.Fourcc) && (MakeFourCC("DX10") == this.FourCC);
            

            public PixelFormat GetDXGIFormat()
            {

                switch (this.Flags)
                {
                    case DDSPixelFormats.Alpha:
                        return GetFormatDDPFAlpha();

                    case DDSPixelFormats.Fourcc:
                        return GetFormatDDPFFOURCC();

                    case DDSPixelFormats.RGB:
                        return GetFormatDDPFRGB();

                    //case DDSPixelFormats.YUV:
                    //    break;

                    case DDSPixelFormats.Luminance:
                        return GetFormatDDPFLuminance();

                    default:
                        return PixelFormat.Unknown;
                }


            }

            private PixelFormat GetFormatDDPFRGB()
            {
                // Note that sRGB formats are written using the "DX10" extended header
                switch (this.RGBBitCount)
                {
                    case 32:
                        return GetFormatDDPFRGB32();

                    case 24:
                        return GetFormatDDPFRGB24();

                    case 16:
                        return GetFormatDDPFRGB16();
                }

                return PixelFormat.Unknown;
            }

            private PixelFormat GetFormatDDPFRGB32()
            {
                if (this.IsBitMask(0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000))
                    return PixelFormat.R8G8B8A8_UNorm;
                

                if (this.IsBitMask(0x00ff0000, 0x0000ff00, 0x000000ff, 0xff000000))
                    return PixelFormat.B8G8R8A8_UNorm;
                

                if (this.IsBitMask(0x00ff0000, 0x0000ff00, 0x000000ff, 0x00000000))
                    return PixelFormat.B8G8R8X8_UNorm;
                

                // No DXGI format maps to ISBITMASK(0x000000ff, 0x0000ff00, 0x00ff0000, 0x00000000) aka D3DFMT_X8B8G8R8

                // Note that many common DDS reader/writers (including D3DX) swap the
                // the RED/BLUE masks for 10:10:10:2 formats. We assumme
                // below that the 'backwards' header mask is being used since it is most
                // likely written by D3DX. The more robust solution is to use the 'DX10'
                // header extension and specify the DXGI_FORMAT_R10G10B10A2_UNORM format directly

                // For 'correct' writers, this should be 0x000003ff, 0x000ffc00, 0x3ff00000 for RGB data
                if (this.IsBitMask(0x3ff00000, 0x000ffc00, 0x000003ff, 0xc0000000))
                    return PixelFormat.R10G10B10A2_UNorm;
                

                // No DXGI format maps to ISBITMASK(0x000003ff, 0x000ffc00, 0x3ff00000, 0xc0000000) aka D3DFMT_A2R10G10B10

                if (this.IsBitMask(0x0000ffff, 0xffff0000, 0x00000000, 0x00000000))
                    return PixelFormat.R16G16_UNorm;
                

                if (this.IsBitMask(0xffffffff, 0x00000000, 0x00000000, 0x00000000))
                    // Only 32-bit color channel format in D3D9 was R32F
                    return PixelFormat.R32_Float; // D3DX writes this out as a FourCC of 114
                

                return PixelFormat.Unknown;
            }

            private PixelFormat GetFormatDDPFRGB24() => PixelFormat.Unknown; // No 24bpp DXGI formats aka D3DFMT_R8G8B8


            private PixelFormat GetFormatDDPFRGB16()
            {
                if (this.IsBitMask(0x7c00, 0x03e0, 0x001f, 0x8000))
                    return PixelFormat.B5G5R5A1_UNorm;
                
                if (this.IsBitMask(0xf800, 0x07e0, 0x001f, 0x0000))
                    return PixelFormat.B5G6R5_UNorm;
                

                // No DXGI format maps to ISBITMASK(0x7c00, 0x03e0, 0x001f, 0x0000) aka D3DFMT_X1R5G5B5
                if (this.IsBitMask(0x0f00, 0x00f0, 0x000f, 0xf000))
                    return PixelFormat.B4G4R4A4_UNorm;
                

                // No DXGI format maps to ISBITMASK(0x0f00, 0x00f0, 0x000f, 0x0000) aka D3DFMT_X4R4G4B4

                // No 3:3:2, 3:3:2:8, or paletted DXGI formats aka D3DFMT_A8R3G3B2, D3DFMT_R3G3B2, D3DFMT_P8, D3DFMT_A8P8, etc.
                return PixelFormat.Unknown;
            }

            private PixelFormat GetFormatDDPFLuminance()
            {
                if (8 == this.RGBBitCount && this.IsBitMask(0x000000ff, 0x00000000, 0x00000000, 0x00000000))
                    return PixelFormat.R8_UNorm; // D3DX10/11 writes this out as DX10 extension
                
                // No DXGI format maps to ISBITMASK(0x0f, 0x00, 0x00, 0xf0) aka D3DFMT_A4L4

                if (16 == this.RGBBitCount)
                {
                    if (this.IsBitMask(0x0000ffff, 0x00000000, 0x00000000, 0x00000000))
                        return PixelFormat.R16_UNorm; // D3DX10/11 writes this out as DX10 extension
                    
                    if (this.IsBitMask(0x000000ff, 0x00000000, 0x00000000, 0x0000ff00))
                        return PixelFormat.R8G8_UNorm; // D3DX10/11 writes this out as DX10 extension
                    
                }

                return PixelFormat.Unknown;
            }

            private PixelFormat GetFormatDDPFAlpha() => 8 == this.RGBBitCount ? PixelFormat.A8_UNorm : PixelFormat.Unknown;


            private PixelFormat GetFormatDDPFFOURCC()
            {
                if (MakeFourCC("DXT1") == this.FourCC)
                    return PixelFormat.BC1_UNorm;
                
                if (MakeFourCC("DXT3") == this.FourCC)
                    return PixelFormat.BC2_UNorm;
                
                if (MakeFourCC("DXT5") == this.FourCC)
                    return PixelFormat.BC3_UNorm;
                

                // While pre-mulitplied alpha isn't directly supported by the DXGI formats,
                // they are basically the same as these BC formats so they can be mapped
                if (MakeFourCC("DXT2") == this.FourCC)
                    return PixelFormat.BC2_UNorm;
                
                if (MakeFourCC("DXT4") == this.FourCC)
                    return PixelFormat.BC3_UNorm;
                

                if (MakeFourCC("ATI1") == this.FourCC)
                    return PixelFormat.BC4_UNorm;
                
                if (MakeFourCC("BC4U") == this.FourCC)
                    return PixelFormat.BC4_UNorm;
                
                if (MakeFourCC("BC4S") == this.FourCC)
                    return PixelFormat.BC4_SNorm;
                

                if (MakeFourCC("ATI2") == this.FourCC)
                    return PixelFormat.BC5_UNorm;
                
                if (MakeFourCC("BC5U") == this.FourCC)
                    return PixelFormat.BC5_UNorm;
                
                if (MakeFourCC("BC5S") == this.FourCC)
                    return PixelFormat.BC5_SNorm;
                

                // BC6H and BC7 are written using the "DX10" extended header
                if (MakeFourCC("RGBG") == this.FourCC)
                    return PixelFormat.R8G8_B8G8_UNorm;
                
                if (MakeFourCC("GRGB") == this.FourCC)
                    return PixelFormat.G8R8_G8B8_UNorm;
                

                // Check for D3DFORMAT enums being set here
                switch (this.FourCC)
                {
                    case 36: // D3DFMT_A16B16G16R16
                        return PixelFormat.R16G16B16A16_UNorm;

                    case 110: // D3DFMT_Q16W16V16U16
                        return PixelFormat.R16G16B16A16_SNorm;

                    case 111: // D3DFMT_R16F
                        return PixelFormat.R16_Float;

                    case 112: // D3DFMT_G16R16F
                        return PixelFormat.R16G16_Float;

                    case 113: // D3DFMT_A16B16G16R16F
                        return PixelFormat.R16G16B16A16_Float;

                    case 114: // D3DFMT_R32F
                        return PixelFormat.R32_Float;

                    case 115: // D3DFMT_G32R32F
                        return PixelFormat.R32G32_Float;

                    case 116: // D3DFMT_A32B32G32R32F
                        return PixelFormat.R32G32B32A32_Float;
                }

                return PixelFormat.Unknown;
            }

            private bool IsBitMask(uint r, uint g, uint b, uint a) => RBitMask == r && GBitMask == g && BBitMask == b && ABitMask == a;
            
        }





        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct HeaderDXT10
        {
            public readonly static int StructSize = Interop.SizeOf<HeaderDXT10>();

            public PixelFormat DXGIFormat;

            public ResourceDimension Dimension;

            public ResourceOptionFlags MiscFlag;

            public int ArraySize;

            public FlagsDX10 MiscFlag2;
        }


        public TextureData TextureData { get; private set; }



        public DDSLoader(string filename)
        {

            if (DdsHeader.GetInfo(filename, out DdsHeader header, out HeaderDXT10? header10, out int offset, out byte[] buffer))
                TextureData = LoadTexture(header, header10, buffer, offset, 0);

            else
                TextureData = LoadTexture(header, header10, buffer, offset, 0);

        }




        public static TextureData LoadFromFile(string filename) => new DDSLoader(filename).TextureData;
        

        

        internal TextureData LoadTexture(DdsHeader header, HeaderDXT10? header10, byte[] bitData, int offset, int maxsize)
        {
            bool validFile = DdsHeader.ValidateTexture(
            header, header10,
            out int depth, out PixelFormat format, out ResourceDimension resDim, out int arraySize, out bool isCubeMap);

            TextureData textureData = new TextureData()
            {
                Width = header.Width,
                Height = header.Height,
                Depth = depth,
                Format = format,
                Size = arraySize,
                IsCubeMap = isCubeMap,
                MipMaps = header.MipMapCount is 0 ? 1 : header.MipMapCount,
            };



            byte[] bytes = new byte[bitData.Length - offset];

            Array.Copy(bitData, offset, bytes, 0, bytes.Length);

            textureData.Data = bytes;

            return textureData;
        }

    }
}
