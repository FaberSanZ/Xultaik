// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)





using System;
using System.Collections.Generic;
using System.Text;
using Vortice.Direct3D12;
using Vortice.Direct3D12.Debug;
using Vortice.Direct3D;
using Vortice.DXGI;
using static Vortice.Direct3D12.D3D12;
using static Vortice.DXGI.DXGI;

namespace Xultaik.Graphics
{
    public sealed unsafe class GraphicsDevice : IDisposable
    {
        public CommandQueue NativeDirectCommandQueue { get; private set; }

        public GraphicsAdapter NativeAdapter { get; }

        public RenderDescriptor NativeParameters { get; internal set; }

        //public SwapChain NativeSwapChain { get; }

        public DescriptorAllocator DepthStencilViewAllocator { get; private set; }

        public DescriptorAllocator RenderTargetViewAllocator { get; private set; }
        public DescriptorAllocator ShaderResourceViewAllocator { get; internal set; }

        internal ID3D12Device5 NativeDevice;

        public GraphicsDevice()
        {
            NativeAdapter = new GraphicsAdapter();
            InitializeFromImpl();
        }

        public GraphicsDevice(GraphicsAdapter graphicsAdapter, RenderDescriptor presentation)
        {
            NativeAdapter = graphicsAdapter;
            NativeParameters = presentation;
            InitializeFromImpl();
        }


        public void InitializeFromImpl()
        {
            InitializePlatformDevice();
        }




        private void InitializePlatformDevice()
        {

            NativeDevice = CreateDevice(NativeAdapter);


            SupportedFeature();

            



            // Create the NativeCommandQueue
            NativeDirectCommandQueue = CreateCommandQueueDirect();

            CreateDescriptorAllocators();

        }

        public bool SupportedFeature()
        {
            FeatureDataD3D12Options1 Options1 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options1>(Vortice.Direct3D12.Feature.Options1);
            FeatureDataD3D12Options2 Options2 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options2>(Vortice.Direct3D12.Feature.Options2);
            FeatureDataD3D12Options3 Options3 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options3>(Vortice.Direct3D12.Feature.Options3);
            FeatureDataD3D12Options4 Options4 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options4>(Vortice.Direct3D12.Feature.Options4);
            FeatureDataD3D12Options5 Options5 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options5>(Vortice.Direct3D12.Feature.Options5);
            FeatureDataD3D12Options6 Options6 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options6>(Vortice.Direct3D12.Feature.Options6);
            FeatureDataD3D12Options7 Options7 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options7>(Vortice.Direct3D12.Feature.Options7);
            FeatureDataD3D12Options8 Options8 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options8>(Vortice.Direct3D12.Feature.Options8);
            FeatureDataD3D12Options9 Options9 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options9>(Vortice.Direct3D12.Feature.Options9);
            FeatureDataD3D12Options10 Options10 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options10>(Vortice.Direct3D12.Feature.Options10);
            FeatureDataD3D12Options11 Options11 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options11>(Vortice.Direct3D12.Feature.Options11);
            FeatureDataD3D12Options12 Options12 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options12>(Vortice.Direct3D12.Feature.Options12);
            FeatureDataD3D12Options13 Options13 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options13>(Vortice.Direct3D12.Feature.Options13);
            FeatureDataD3D12Options14 Options14 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options14>(Vortice.Direct3D12.Feature.Options14);
            FeatureDataD3D12Options15 Options15 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options15>(Vortice.Direct3D12.Feature.Options15);
            FeatureDataD3D12Options16 Options16 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options16>(Vortice.Direct3D12.Feature.Options16);
            FeatureDataD3D12Options17 Options17 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options17>(Vortice.Direct3D12.Feature.Options17);
            FeatureDataD3D12Options18 Options18 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options18>(Vortice.Direct3D12.Feature.Options18);
            FeatureDataD3D12Options19 Options19 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options19>(Vortice.Direct3D12.Feature.Options19);
            FeatureDataD3D12Options20 Options20 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options20>(Vortice.Direct3D12.Feature.Options20);
            FeatureDataD3D12Options21 Options21 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options21>(Vortice.Direct3D12.Feature.Options21);


            Console.WriteLine($"Int64ShaderOps {Options1.Int64ShaderOps}");
            Console.WriteLine($"ProgrammableSamplePositionsTier {Options2.ProgrammableSamplePositionsTier}");
            Console.WriteLine($"DepthBoundsTestSupported {Options2.DepthBoundsTestSupported}");
            Console.WriteLine($"SamplerFeedbackTier {Options7.SamplerFeedbackTier}");
            Console.WriteLine($"MeshShaderTier {Options7.MeshShaderTier}");
            Console.WriteLine($"UnalignedBlockTexturesSupported {Options8.UnalignedBlockTexturesSupported}");
            Console.WriteLine($"MeshShaderPipelineStatsSupported {Options9.MeshShaderPipelineStatsSupported}");
            Console.WriteLine($"ViewInstancingTier {Options3.ViewInstancingTier}");
            Console.WriteLine($"VariableShadingRateTier {Options6.VariableShadingRateTier}");
            Console.WriteLine($"MSAA64KBAlignedTextureSupported {Options4.MSAA64KBAlignedTextureSupported}");
            Console.WriteLine($"Native16BitShaderOpsSupported {Options4.Native16BitShaderOpsSupported}");
            Console.WriteLine($"Ray RaytracingTier {Options5.RaytracingTier}");
            Console.WriteLine($"SRVOnlyTiledResourceTier3 {Options5.SRVOnlyTiledResourceTier3}");
            Console.WriteLine($"VariableRateShadingSumCombinerSupported {Options10.VariableRateShadingSumCombinerSupported}");
            Console.WriteLine($"MeshShaderPerPrimitiveShadingRateSupported {Options10.MeshShaderPerPrimitiveShadingRateSupported}");
            Console.WriteLine($"AtomicInt64OnDescriptorHeapResourceSupported {Options11.AtomicInt64OnDescriptorHeapResourceSupported}");
            Console.WriteLine($"MSPrimitivesPipelineStatisticIncludesCulledPrimitives {Options12.MSPrimitivesPipelineStatisticIncludesCulledPrimitives}");
            Console.WriteLine($"EnhancedBarriersSupported {Options12.EnhancedBarriersSupported}");
            Console.WriteLine($"UnrestrictedBufferTextureCopyPitchSupported {Options13.UnrestrictedBufferTextureCopyPitchSupported}");
            Console.WriteLine($"InvertedViewportDepthFlipsZSupported {Options13.InvertedViewportDepthFlipsZSupported}");
            Console.WriteLine($"AlphaBlendFactorSupported {Options13.AlphaBlendFactorSupported}");
            Console.WriteLine($"WriteableMSAATexturesSupported {Options14.WriteableMSAATexturesSupported}");
            Console.WriteLine($"AdvancedTextureOpsSupported {Options14.AdvancedTextureOpsSupported}");
            Console.WriteLine($"IndependentFrontAndBackStencilRefMaskSupported {Options14.IndependentFrontAndBackStencilRefMaskSupported}");
            Console.WriteLine($"TriangleFanSupported {Options15.TriangleFanSupported}");
            Console.WriteLine($"GPUUploadHeapSupported {Options16.GPUUploadHeapSupported}");
            Console.WriteLine($"DynamicDepthBiasSupported {Options16.DynamicDepthBiasSupported}");
            Console.WriteLine($"ManualWriteTrackingResourceSupported {Options17.ManualWriteTrackingResourceSupported}");
            Console.WriteLine($"RenderPassesValid {Options18.RenderPassesValid}");
            Console.WriteLine($"ComputeOnlyCustomHeapSupported {Options19.ComputeOnlyCustomHeapSupported}");
            Console.WriteLine($"NarrowQuadrilateralLinesSupported {Options19.NarrowQuadrilateralLinesSupported}");
            Console.WriteLine($"RasterizerDesc2Supported {Options19.RasterizerDesc2Supported}");
            Console.WriteLine($"SupportedSampleCountsWithNoOutputs {Options19.SupportedSampleCountsWithNoOutputs}");
            Console.WriteLine($"ComputeOnlyWriteWatchSupported {Options20.ComputeOnlyWriteWatchSupported}");
            Console.WriteLine($"RecreateAtTier {Options20.RecreateAtTier}");
            Console.WriteLine($"ExecuteIndirectTier {Options21.ExecuteIndirectTier}");
            Console.WriteLine($"ExtendedCommandInfoSupported {Options21.ExtendedCommandInfoSupported}");
            Console.WriteLine($"SampleCmpGradientAndBiasSupported {Options21.SampleCmpGradientAndBiasSupported}");
            Console.WriteLine($"WorkGraphsTier {Options21.WorkGraphsTier}");


            return true;


        }

        internal ID3D12Device5 CreateDevice(GraphicsAdapter factory4)
        {


            foreach (var Adapters in factory4.Adapters)
                if (D3D12CreateDevice<ID3D12Device5>(Adapters, FeatureLevel.Level_12_2, out var device).Success)
                    return device.QueryInterface<ID3D12Device5>();


            return null;
        }

        public void CreateDescriptorAllocators()
        {
            RenderTargetViewAllocator = new DescriptorAllocator(this, DescriptorHeapType.RenderTargetView);

            DepthStencilViewAllocator = new DescriptorAllocator(this, DescriptorHeapType.DepthStencilView);

            ShaderResourceViewAllocator = new DescriptorAllocator(this, DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);
        }

        public CommandQueue CreateCommandQueueDirect()
        {
            // Describe and create the command queue.
            CommandQueue Queue = new CommandQueue(this)
            {
                Type = CommandListType.Direct
            };

            return Queue;
        }


        public CommandQueue CreateCommandQueueCopy()
        {
            // Describe and create the command queue.
            CommandQueue Queue = new CommandQueue(this)
            {
                Type = CommandListType.Copy
            };

            return Queue;
        }


        public CommandQueue CreateCommandQueueBundle()
        {
            // Describe and create the command queue.
            CommandQueue Queue = new CommandQueue(this)
            {
                Type = CommandListType.Bundle
            };

            return Queue;
        }


        public CommandQueue CreateCommandQueueCompute()
        {
            // Describe and create the command queue.
            CommandQueue Queue = new CommandQueue(this)
            {
                Type = CommandListType.Compute
            };

            return Queue;
        }


        public CommandQueue CreateCommandQueueVideoDecode()
        {
            // Describe and create the command queue.
            CommandQueue Queue = new CommandQueue(this)
            {
                Type = CommandListType.VideoDecode
            };

            return Queue;
        }

        public CommandQueue CreateCommandQueueVideoEncode()
        {
            // Describe and create the command queue.
            CommandQueue Queue = new CommandQueue(this)
            {
                Type = CommandListType.VideoEncode
            };

            return Queue;
        }


        public CommandQueue CreateCommandQueueVideoProcess()
        {
            // Describe and create the command queue.
            CommandQueue Queue = new CommandQueue(this)
            {
                Type = CommandListType.VideoProcess
            };

            return Queue;
        }


        public void Dispose()
        {

        }
    }
}
