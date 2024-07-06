// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


using System;
using System.Collections.Generic;
using System.Text;
using Vortice.Direct3D12;
using Vortice.Direct3D;
using static Vortice.Direct3D12.D3D12;
using static Vortice.DXGI.DXGI;

namespace Xultaik.Graphics
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
                Buffer = new BufferRenderTargetView()
                {
                    FirstElement = 0,
                    NumElements = 0
                },  
                //Texture1D,
                Format = Vortice.DXGI.Format.R8G8B8A8_UNorm,
                //Texture1DArray
                Texture2D = new Texture2DRenderTargetView()
                {
                    MipSlice = 1,
                    PlaneSlice = 0
                },
                //Texture2DArray
                Texture2DMS = new Texture2DMultisampledRenderTargetView()
                {
                   // UnusedFieldNothingToDefine = 0x001,
                },
                //Texture2DMSArray
                //Texture3D
                ViewDimension = RenderTargetViewDimension.Texture2D
                
            };

            CpuDescriptorHandle descriptorHandle = GraphicsDevice.RenderTargetViewAllocator.Allocate(1);

            GraphicsDevice.NativeDevice.CreateRenderTargetView(Resource,/*null*/ RTVDescription, descriptorHandle);

            return descriptorHandle;
        }

    }
}
