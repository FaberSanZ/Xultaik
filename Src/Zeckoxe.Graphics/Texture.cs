// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Texture.cs
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
    public class Texture : GraphicsResource
    {
        internal CpuDescriptorHandle NativeRenderTargetView;
        internal CpuDescriptorHandle NativeDepthStencilView;


        public int Width { get; private set; }


        public int Height { get; private set; }


        /// <summary>
        /// Constructs a texture.
        /// </summary>
        /// <param name="device"></param>
        protected internal Texture(GraphicsDevice device) : base(device)
        {

        }

        /// <summary>
        /// Gets the resource.
        /// </summary>
        public ID3D12Resource Resource;

        /// <summary>
        /// Gets the upload resource.
        /// </summary>
        public ID3D12Resource UploadResource;

        internal void InitializeFromImpl(ID3D12Resource resource)
        {
            Resource = resource;

            NativeRenderTargetView = GetRenderTargetView();
        }

        private CpuDescriptorHandle GetRenderTargetView()
        {
            var descriptorHandle = GraphicsDevice.RenderTargetViewAllocator.Allocate(1);
            GraphicsDevice.NativeDevice.CreateRenderTargetView(Resource, null, NativeRenderTargetView);
            return descriptorHandle;
        }

    }
}
