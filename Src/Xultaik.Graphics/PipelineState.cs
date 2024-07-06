// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)




using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Vortice.Direct3D12;
using Vortice.Dxc;
using Vortice.DXGI;

namespace Xultaik.Graphics
{

    public class PipelineState : GraphicsResource
    {
        PipelineStateDescription PipelineStateDescription { get; set; }



        //Internal 
        internal ID3D12RootSignature RootSignature;
        internal ID3D12PipelineState oldPipelineState;


        public PipelineState(GraphicsDevice device, PipelineStateDescription description) : base(device)
        {
            PipelineStateDescription = description;
            Recreate();
        }

        public void Recreate()
        {
            CreateRootSignature();
            CreatePipeline();
        }


        public void CreatePipeline()
        {
            InputElementDescription[] inputElementDescs = new InputElementDescription[]  
            {
                new InputElementDescription("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                new InputElementDescription("COLOR", 0, Format.R32G32B32A32_Float, 12, 0),

            };

            var psoDesc = new GraphicsPipelineStateDescription()
            {
                RootSignature = RootSignature,

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
                },


                HullShader = PipelineStateDescription.HullShader == null ? null : PipelineStateDescription.HullShader.Data,

                GeometryShader = PipelineStateDescription.GeometryShader == null ? null : PipelineStateDescription.GeometryShader.Data,

                DomainShader = PipelineStateDescription.DomainShader == null ? null : PipelineStateDescription.DomainShader.Data,

                VertexShader = PipelineStateDescription.VertexShader == null ? null : PipelineStateDescription.VertexShader.Data,

                PixelShader = PipelineStateDescription.PixelShader == null ? null : PipelineStateDescription.PixelShader.Data,
            };

            oldPipelineState = GraphicsDevice.NativeDevice.CreateGraphicsPipelineState<ID3D12PipelineState>(psoDesc);
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


            RootSignature = GraphicsDevice.NativeDevice.CreateRootSignature<ID3D12RootSignature>(0, SignatureDesc, RootSignatureVersion.Version10);
            //RootSignature.
        }
    }
}
