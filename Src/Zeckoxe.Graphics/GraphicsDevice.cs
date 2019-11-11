// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	GraphicsDevice.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;
using Vortice.Direct3D12;
using Vortice.Direct3D12.Debug;
using Vortice.DirectX.Direct3D;
using Vortice.DXGI;
using static Vortice.Direct3D12.D3D12;
using static Vortice.DXGI.DXGI;

namespace Zeckoxe.Graphics
{
    public sealed unsafe class GraphicsDevice : IDisposable
    {
        public CommandQueue NativeDirectCommandQueue { get; private set; }

        public GraphicsAdapter NativeAdapter { get; }

        public PresentationParameters NativeParameters { get; internal set; }

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

        public GraphicsDevice(GraphicsAdapter graphicsAdapter, PresentationParameters presentation)
        {
            NativeAdapter = graphicsAdapter;
            NativeParameters = presentation;
            InitializeFromImpl();
        }


        public void InitializeFromImpl()
        {
            InitializePlatformDevice(FeatureLevel.Level_12_0);
        }


        private ID3D12Device5 CreateDevice(IDXGIFactory4 factory4)
        {
            var adapters = factory4.EnumAdapters1();
            for (int i = 0; i < adapters.Length; i++)
            {
                var desc = adapters[i].Description1;
                if (desc.Flags.HasFlag(AdapterFlags.Software))
                    continue;
                

                D3D12CreateDevice(adapters[i], FeatureLevel.Level_12_1, out var dev);
                FeatureDataD3D12Options5 opt5 = dev.CheckFeatureSupport<FeatureDataD3D12Options5>(Vortice.Direct3D12.Feature.Options5);
                if (opt5.RaytracingTier != RaytracingTier.Tier1_0)
                {
                    throw new NotSupportedException("Raytracing not supported");
                }
                return dev.QueryInterface<ID3D12Device5>();
            }
            return null;
        }




        private void InitializePlatformDevice(FeatureLevel graphicsProfiles)
        {

            foreach (var Adapters in NativeAdapter.Adapters)
                if (D3D12CreateDevice(Adapters, graphicsProfiles, out var device).Success)
                    NativeDevice = device.QueryInterface<ID3D12Device5>();


            FeatureDataD3D12Options5 Options5 = NativeDevice.CheckFeatureSupport<FeatureDataD3D12Options5>(Vortice.Direct3D12.Feature.Options5);

            if (Options5.RaytracingTier != RaytracingTier.Tier1_0)
                Console.WriteLine("Raytracing not supported");




            // Create the NativeCommandQueue
            NativeDirectCommandQueue = new CommandQueue(this, CommandListType.Direct); //CreateCommandQueueDirect();

            CreateDescriptorAllocators();

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
