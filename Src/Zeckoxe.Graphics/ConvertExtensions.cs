// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	D3D12Convert.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;
using Vortice.DXGI;
using Vortice.DirectX.Direct3D;
using Vortice.Direct3D12;
using Vortice.Dxc;

namespace Zeckoxe.Graphics
{
    public static class ConvertExtensions
    {

        public static DxcShaderModel ToDxcShaderModel(ShaderModel shaderStage)
        {
            switch (shaderStage)
            {
                case ShaderModel.Model6_0:
                    return new DxcShaderModel(6, 0);

                case ShaderModel.Model6_1:
                    return new DxcShaderModel(6, 1);

                case ShaderModel.Model6_2:
                    return new DxcShaderModel(6, 2);

                case ShaderModel.Model6_3:
                    return new DxcShaderModel(6, 3);

                case ShaderModel.Model6_4:
                    return new DxcShaderModel(6, 4);

                case ShaderModel.Model6_5:
                    return new DxcShaderModel(6, 5);

                default:
                    throw new ArgumentOutOfRangeException(nameof(shaderStage));
            }
        }

        public static DxcShaderStage ToDxcShaderStage(ShaderStage shaderStage)
        {
            switch (shaderStage)
            {
                case ShaderStage.VertexShader:
                    return DxcShaderStage.VertexShader;

                case ShaderStage.PixelShader:
                    return DxcShaderStage.PixelShader;

                case ShaderStage.GeometryShader:
                    return DxcShaderStage.GeometryShader;

                case ShaderStage.HullShader:
                    return DxcShaderStage.HullShader;

                case ShaderStage.DomainShader:
                    return DxcShaderStage.DomainShader;

                case ShaderStage.ComputeShader:
                    return DxcShaderStage.ComputeShader;

                //case ShaderStage.Library:
                //    return DxcShaderStage.VertexShader;

                //case ShaderStage.Count:
                //    return DxcShaderStage.VertexShader;

                default:
                    throw new ArgumentOutOfRangeException(nameof(shaderStage));
            }
        }



        public static PrimitiveTopology ToPrimitiveType(PrimitiveType primitiveType)
        {
            switch (primitiveType)
            {
                case PrimitiveType.PointList:
                    return PrimitiveTopology.PointList;

                case PrimitiveType.LineList:
                    return PrimitiveTopology.LineList;

                case PrimitiveType.LineStrip:
                    return PrimitiveTopology.LineStrip;

                case PrimitiveType.TriangleList:
                    return PrimitiveTopology.TriangleList;

                case PrimitiveType.TriangleStrip:
                    return PrimitiveTopology.TriangleStrip;

                case PrimitiveType.LineListAdjacency:
                    return PrimitiveTopology.LineListAdjacency;

                case PrimitiveType.LineStripAdjacency:
                    return PrimitiveTopology.LineStripAdjacency;

                case PrimitiveType.TriangleListAdjacency:
                    return PrimitiveTopology.TriangleListAdjacency;

                case PrimitiveType.TriangleStripAdjacency:
                    return PrimitiveTopology.TriangleStripAdjacency;

                default:
                    throw new ArgumentOutOfRangeException(nameof(primitiveType));
            }
        }


        public static Vortice.Direct3D12.FillMode ToFillMode(FillMode fillMode)
        {
            switch (fillMode)
            {
                case FillMode.Solid:
                    return Vortice.Direct3D12.FillMode.Solid;

                case FillMode.Wireframe:
                    return Vortice.Direct3D12.FillMode.Wireframe;

                default:
                    throw new ArgumentOutOfRangeException(nameof(fillMode));
            }
        }


        public static Vortice.Direct3D12.CullMode ToCullMode(CullMode cullMode)
        {
            switch (cullMode)
            {
                case CullMode.Back:
                    return Vortice.Direct3D12.CullMode.Back;

                case CullMode.Front:
                    return Vortice.Direct3D12.CullMode.Front;

                case CullMode.None:
                    return Vortice.Direct3D12.CullMode.None;

                default:
                    throw new ArgumentOutOfRangeException(nameof(cullMode));
            }
        }


        public static bool IsUAVCompatibleFormat(PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.R32G32B32A32_Float:
                case PixelFormat.R32G32B32A32_UInt:
                case PixelFormat.R32G32B32A32_SInt:
                case PixelFormat.R16G16B16A16_Float:
                case PixelFormat.R16G16B16A16_UInt:
                case PixelFormat.R16G16B16A16_SInt:
                case PixelFormat.R8G8B8A8_UNorm:
                case PixelFormat.R8G8B8A8_UInt:
                case PixelFormat.R8G8B8A8_SInt:
                case PixelFormat.R32_Float:
                case PixelFormat.R32_UInt:
                case PixelFormat.R32_SInt:
                case PixelFormat.R16_Float:
                case PixelFormat.R16_UInt:
                case PixelFormat.R16_SInt:
                case PixelFormat.R8_UNorm:
                case PixelFormat.R8_UInt:
                case PixelFormat.R8_SInt:
                    return true;

                default:
                    return false;
            }
        }


        public static Format ToPixelFormat(PixelFormat format)
        {

            switch (format)
            {
                case PixelFormat.Unknown:
                    return Format.Unknown;

                case PixelFormat.R32G32B32A32_Typeless:
                    return Format.R32G32B32A32_Typeless;

                case PixelFormat.R32G32B32A32_Float:
                    return Format.R32G32B32A32_Float;

                case PixelFormat.R32G32B32A32_UInt:
                    return Format.R32G32B32A32_UInt;

                case PixelFormat.R32G32B32A32_SInt:
                    return Format.R32G32B32A32_SInt;

                case PixelFormat.R32G32B32_Typeless:
                    return Format.R32G32B32_Typeless;

                case PixelFormat.R32G32B32_Float:
                    return Format.R32G32B32_Float;

                case PixelFormat.R32G32B32_UInt:
                    return Format.R32G32B32_UInt;

                case PixelFormat.R32G32B32_SInt:
                    return Format.R32G32B32_SInt;

                case PixelFormat.R16G16B16A16_Typeless:
                    return Format.R16G16B16A16_Typeless;

                case PixelFormat.R16G16B16A16_Float:
                    return Format.R16G16B16A16_Float;

                case PixelFormat.R16G16B16A16_UNorm:
                    return Format.R16G16B16A16_UNorm;

                case PixelFormat.R16G16B16A16_UInt:
                    return Format.R16G16B16A16_UInt;

                case PixelFormat.R16G16B16A16_SNorm:
                    return Format.R16G16B16A16_SNorm;

                case PixelFormat.R16G16B16A16_SInt:
                    return Format.R16G16B16A16_SInt;

                case PixelFormat.R32G32_Typeless:
                    return Format.R32G32_Typeless;

                case PixelFormat.R32G32_Float:
                    return Format.R32G32_Float;

                case PixelFormat.R32G32_UInt:
                    return Format.R32G32_UInt;

                case PixelFormat.R32G32_SInt:
                    return Format.R32G32_SInt;

                case PixelFormat.R32G8X24_Typeless:
                    return Format.R32G8X24_Typeless;

                case PixelFormat.D32_Float_S8X24_UInt:
                    return Format.Unknown;

                case PixelFormat.R32_Float_X8X24_Typeless:
                    return Format.Unknown;

                case PixelFormat.X32_Typeless_G8X24_UInt:
                    return Format.Unknown;

                case PixelFormat.R10G10B10A2_Typeless:
                    return Format.Unknown;

                case PixelFormat.R10G10B10A2_UNorm:
                    return Format.Unknown;

                case PixelFormat.R10G10B10A2_UInt:
                    return Format.Unknown;

                case PixelFormat.R11G11B10_Float:
                    return Format.Unknown;

                case PixelFormat.R8G8B8A8_Typeless:
                    return Format.Unknown;

                case PixelFormat.R8G8B8A8_UNorm:
                    return Format.R8G8B8A8_UNorm;

                case PixelFormat.R8G8B8A8_UNorm_SRgb:
                    return Format.Unknown;

                case PixelFormat.R8G8B8A8_UInt:
                    return Format.Unknown;

                case PixelFormat.R8G8B8A8_SNorm:
                    return Format.Unknown;

                case PixelFormat.R8G8B8A8_SInt:
                    return Format.Unknown;

                case PixelFormat.R16G16_Typeless:
                    return Format.Unknown;

                case PixelFormat.R16G16_Float:
                    return Format.Unknown;

                case PixelFormat.R16G16_UNorm:
                    return Format.Unknown;

                case PixelFormat.R16G16_UInt:
                    return Format.Unknown;

                case PixelFormat.R16G16_SNorm:
                    return Format.Unknown;

                case PixelFormat.R16G16_SInt:
                    return Format.Unknown;

                case PixelFormat.R32_Typeless:
                    return Format.Unknown;

                case PixelFormat.D32_Float:
                    return Format.Unknown;

                case PixelFormat.R32_Float:
                    return Format.Unknown;

                case PixelFormat.R32_UInt:
                    return Format.Unknown;

                case PixelFormat.R32_SInt:
                    return Format.Unknown;

                case PixelFormat.R24G8_Typeless:
                    return Format.Unknown;

                case PixelFormat.D24_UNorm_S8_UInt:
                    return Format.Unknown;

                case PixelFormat.R24_UNorm_X8_Typeless:
                    return Format.Unknown;

                case PixelFormat.X24_Typeless_G8_UInt:
                    return Format.Unknown;

                case PixelFormat.R8G8_Typeless:
                    return Format.Unknown;

                case PixelFormat.R8G8_UNorm:
                    return Format.Unknown;

                case PixelFormat.R8G8_UInt:
                    return Format.Unknown;

                case PixelFormat.R8G8_SNorm:
                    return Format.Unknown;

                case PixelFormat.R8G8_SInt:
                    return Format.Unknown;

                case PixelFormat.R16_Typeless:
                    return Format.Unknown;

                case PixelFormat.R16_Float:
                    return Format.Unknown;

                case PixelFormat.D16_UNorm:
                    return Format.Unknown;

                case PixelFormat.R16_UNorm:
                    return Format.Unknown;

                case PixelFormat.R16_UInt:
                    return Format.Unknown;

                case PixelFormat.R16_SNorm:
                    return Format.Unknown;

                case PixelFormat.R16_SInt:
                    return Format.Unknown;

                case PixelFormat.R8_Typeless:
                    return Format.Unknown;

                case PixelFormat.R8_UNorm:
                    return Format.Unknown;

                case PixelFormat.R8_UInt:
                    return Format.Unknown;

                case PixelFormat.R8_SNorm:
                    return Format.Unknown;

                case PixelFormat.R8_SInt:
                    return Format.Unknown;

                case PixelFormat.A8_UNorm:
                    return Format.Unknown;

                case PixelFormat.R1_UNorm:
                    return Format.Unknown;

                case PixelFormat.R9G9B9E5_SharedExp:
                    return Format.Unknown;

                case PixelFormat.R8G8_B8G8_UNorm:
                    return Format.Unknown;

                case PixelFormat.G8R8_G8B8_UNorm:
                    return Format.Unknown;

                case PixelFormat.BC1_Typeless:
                    return Format.Unknown;

                case PixelFormat.BC1_UNorm:
                    return Format.Unknown;

                case PixelFormat.BC1_UNorm_SRgb:
                    return Format.Unknown;

                case PixelFormat.BC2_Typeless:
                    return Format.Unknown;

                case PixelFormat.BC2_UNorm:
                    return Format.Unknown;

                case PixelFormat.BC2_UNorm_SRgb:
                    return Format.Unknown;

                case PixelFormat.BC3_Typeless:
                    return Format.Unknown;

                case PixelFormat.BC3_UNorm:
                    return Format.Unknown;

                case PixelFormat.BC3_UNorm_SRgb:
                    return Format.Unknown;

                case PixelFormat.BC4_Typeless:
                    return Format.Unknown;

                case PixelFormat.BC4_UNorm:
                    return Format.Unknown;

                case PixelFormat.BC4_SNorm:
                    return Format.Unknown;

                case PixelFormat.BC5_Typeless:
                    return Format.Unknown;

                case PixelFormat.BC5_UNorm:
                    return Format.Unknown;

                case PixelFormat.BC5_SNorm:
                    return Format.Unknown;

                case PixelFormat.B5G6R5_UNorm:
                    return Format.Unknown;

                case PixelFormat.B5G5R5A1_UNorm:
                    return Format.Unknown;

                case PixelFormat.B8G8R8A8_UNorm:
                    return Format.Unknown;

                case PixelFormat.B8G8R8X8_UNorm:
                    return Format.Unknown;

                case PixelFormat.R10G10B10_Xr_Bias_A2_UNorm:
                    return Format.Unknown;

                case PixelFormat.B8G8R8A8_Typeless:
                    return Format.Unknown;

                case PixelFormat.B8G8R8A8_UNorm_SRgb:
                    return Format.Unknown;

                case PixelFormat.B8G8R8X8_Typeless:
                    return Format.Unknown;

                case PixelFormat.B8G8R8X8_UNorm_SRgb:
                    return Format.Unknown;

                case PixelFormat.BC6H_Typeless:
                    return Format.Unknown;

                case PixelFormat.BC6H_Uf16:
                    return Format.Unknown;

                case PixelFormat.BC6H_Sf16:
                    return Format.Unknown;

                case PixelFormat.BC7_Typeless:
                    return Format.Unknown;

                case PixelFormat.BC7_UNorm:
                    return Format.Unknown;

                case PixelFormat.BC7_UNorm_SRgb:
                    return Format.Unknown;

                case PixelFormat.AYUV:
                    return Format.Unknown;

                case PixelFormat.Y410:
                    return Format.Unknown;

                case PixelFormat.Y416:
                    return Format.Unknown;

                case PixelFormat.NV12:
                    return Format.Unknown;

                case PixelFormat.P010:
                    return Format.Unknown;

                case PixelFormat.P016:
                    return Format.Unknown;

                case PixelFormat.Opaque420:
                    return Format.Unknown;

                case PixelFormat.YUY2:
                    return Format.Unknown;

                case PixelFormat.Y210:
                    return Format.Unknown;

                case PixelFormat.Y216:
                    return Format.Unknown;

                case PixelFormat.NV11:
                    return Format.Unknown;

                case PixelFormat.AI44:
                    return Format.Unknown;

                case PixelFormat.IA44:
                    return Format.Unknown;

                case PixelFormat.P8:
                    return Format.Unknown;

                case PixelFormat.A8P8:
                    return Format.Unknown;

                case PixelFormat.B4G4R4A4_UNorm:
                    return Format.B4G4R4A4_UNorm;

                case PixelFormat.P208:
                    return Format.P208;

                case PixelFormat.V208:
                    return Format.V208;

                case PixelFormat.V408:
                    return Format.V408;

                default:
                    throw new ArgumentOutOfRangeException(nameof(format));
            }



        }
    }
}
