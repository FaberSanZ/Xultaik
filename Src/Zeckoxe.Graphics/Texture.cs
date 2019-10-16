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
        public int Width { get; private set; }

        public int Height { get; private set; }



        internal CpuDescriptorHandle NativeRenderTargetView;
        internal CpuDescriptorHandle NativeDepthStencilView;
        internal ID3D12Resource Resource;
        internal ID3D12Resource UploadResource;

        public Texture(GraphicsDevice device) : base(device)
        {

        }



        internal void InitializeFromImpl(ID3D12Resource resource)
        {
            Resource = resource;

            NativeRenderTargetView = GetRenderTargetView();
        }

        private CpuDescriptorHandle GetRenderTargetView()
        {
            RenderTargetViewDescription RTVDescription = new RenderTargetViewDescription()
            {
                //Buffer,  
                //Texture1D,
                Format = Vortice.DXGI.Format.R8G8B8A8_UNorm,
                //Texture1DArray
                //Texture2D
                //Texture2DArray
                //Texture2DMS
                //Texture2DMSArray
                //Texture3D
                //ViewDimension = RenderTargetViewDimension.Unknown
                
            };

            CpuDescriptorHandle descriptorHandle = GraphicsDevice.RenderTargetViewAllocator.Allocate(1);

            GraphicsDevice.NativeDevice.CreateRenderTargetView(Resource,null /*RTVDescription*/, descriptorHandle);

            return descriptorHandle;
        }

    }
}
