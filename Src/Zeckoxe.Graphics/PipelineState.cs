using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Vortice.Direct3D12;
using Vortice.Dxc;
using Vortice.DXGI;

namespace Zeckoxe.Graphics
{
    public static class ShaderCompiler
    {
        public static byte[] Compile(DxcShaderStage shaderStage, string source, string entryPoint, string sourceName = "")
        {
            return Compile(shaderStage, File.ReadAllText(source), entryPoint, sourceName, new DxcShaderModel(6, 1)/* DxcShaderModel.Model6_0*/);
        }

        public static byte[] Compile(DxcShaderStage shaderStage, string source, string entryPoint, string sourceName, DxcShaderModel shaderModel)
        {
            return Compile(shaderStage, source, entryPoint, sourceName, new DxcCompilerOptions
            {
                ShaderModel = shaderModel,
            });
        }

        public static byte[] Compile(DxcShaderStage shaderStage, string source, string entryPoint, string sourceName, DxcCompilerOptions options)
        {
            IDxcOperationResult result = DxcCompiler.Compile(shaderStage, source, entryPoint, sourceName, options);

            return Dxc.GetBytesFromBlob(result.GetResult());



        }
    }


    public class PipelineState : GraphicsResource
    {
        /* Pipeline */
        /* -- para este ejemplo no necesitamos pipeline ni root signature */
        //ComPtr<ID3DBlob> LoadShader(LPCWSTR Filename, LPCSTR EntryPoint, LPCSTR Target);
        internal ID3D12RootSignature RootSignature;
        internal ID3D12PipelineState oldPipelineState;


        public PipelineState(GraphicsDevice device) : base(device)
        {
            Recreate();
        }

        public void Recreate()
        {
            CreateRootSignature();
            CreatePipeline();
        }


        public void CreatePipeline()
        {
            var inputElementDescs = new[]
            {
                new InputElementDescription("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                new InputElementDescription("COLOR", 0, Format.R32G32B32A32_Float, 12, 0),

            };
            var VertexShaderBytecode = ShaderCompiler.Compile(DxcShaderStage.VertexShader, "shaders.hlsl", "VS");
            var PixelShaderBytecode = ShaderCompiler.Compile(DxcShaderStage.PixelShader, "shaders.hlsl", "PS");

            var psoDesc = new GraphicsPipelineStateDescription()
            {
                RootSignature = RootSignature,
                VertexShader = VertexShaderBytecode,
                PixelShader = PixelShaderBytecode,
                InputLayout = new InputLayoutDescription(inputElementDescs),
                SampleMask = uint.MaxValue,
                PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
                RasterizerState = RasterizerDescription.CullCounterClockwise,
                BlendState = BlendDescription.Opaque,
                DepthStencilState = DepthStencilDescription.None,
                RenderTargetFormats = new[] { Format.R8G8B8A8_UNorm },
                DepthStencilFormat = Format.Unknown,
                SampleDescription = new SampleDescription(1, 0),
                //IndexBufferStripCutValue = IndexBufferStripCutValue.Value0xFFFF
                StreamOutput = new StreamOutputDescription()
                {
                    //RasterizedStream =
                }
            };

            oldPipelineState = GraphicsDevice.NativeDevice.CreateGraphicsPipelineState(psoDesc);
        }


        private void CreateRootSignature()
        {

            //RootDescriptorTable table = new RootDescriptorTable()
            //{
            //Ranges = new DescriptorRange()
            //    {

            //   }
            //}

            //RootParameter rootParameter = new RootParameter(RootDescriptorTable)
            //{
            //    DescriptorTable = new RootDescriptorTable
            //}


            DescriptorRange Ranges = new DescriptorRange()
            {
                BaseShaderRegister = 0,
                NumDescriptors = 1,
                OffsetInDescriptorsFromTableStart = 0,
                RangeType = DescriptorRangeType.ShaderResourceView,
                RegisterSpace = 0,
            };

            RootParameter[] slotRootParameters = new RootParameter[]
            {
                //new RootParameter(new roo
                //new RootParameter(RootParameterType.ConstantBufferView, new RootDescriptor(0, 0), ShaderVisibility.All),
                //new RootParameter(RootParameterType.ConstantBufferView, new RootDescriptor(1, 0), ShaderVisibility.All),
                //new RootParameter(RootParameterType.ShaderResourceView, new RootDescriptor(1, 0), ShaderVisibility.All),
                //new RootParameter(new RootDescriptorTable(new DescriptorRange[]{ Ranges }), ShaderVisibility.All)
            };




            RootSignatureDescription SignatureDesc = new RootSignatureDescription()
            {
                Flags = RootSignatureFlags.AllowInputAssemblerInputLayout,
                //Parameters = slotRootParameters,
                //StaticSamplers = new StaticSamplerDescription[]
                //{
                //    new StaticSamplerDescription()
                //    {

                //        ShaderRegister = 0,
                //        RegisterSpace = 0,
                //        ShaderVisibility = ShaderVisibility.Pixel,

                //        Filter = Filter.MinMagMipPoint,
                //        AddressU = TextureAddressMode.Border,
                //        AddressV = TextureAddressMode.Border,
                //        AddressW = TextureAddressMode.Border,
                //        MipLODBias = 0,
                //        MaxAnisotropy = 0,
                //        ComparisonFunction = ComparisonFunction.Never,
                //        BorderColor = StaticBorderColor.TransparentBlack,
                //        MinLOD = 0.0f,
                //        MaxLOD =  int.MaxValue,
                //    }
                //},
            };


            RootSignature = GraphicsDevice.NativeDevice.CreateRootSignature(0, SignatureDesc, RootSignatureVersion.Version10);
            //RootSignature.
        }
    }
}
