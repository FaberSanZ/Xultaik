// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	GraphicsDevice.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;
using Vortice.Direct3D12;
using Vortice.DirectX.Direct3D;
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




        internal ID3D12Device NativeDevice;

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
            InitializePlatformDevice(FeatureLevel.Level_11_1);
        }


        private void InitializePlatformDevice(FeatureLevel graphicsProfiles)
        {

            foreach (var Adapters in NativeAdapter.Adapters)
                if (D3D12CreateDevice(Adapters, graphicsProfiles, out ID3D12Device device).Success)
                    NativeDevice = device;



            // Create the NativeCommandQueue
            NativeDirectCommandQueue = new CommandQueue(this, CommandListType.Direct); //CreateCommandQueueDirect();

            CreateDescriptorAllocators();

        }

        public void CreateDescriptorAllocators()
        {
            RenderTargetViewAllocator = new DescriptorAllocator(this, DescriptorHeapType.RenderTargetView);

            DepthStencilViewAllocator = new DescriptorAllocator(this, DescriptorHeapType.DepthStencilView);
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
