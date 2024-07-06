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
    public unsafe class CommandList : GraphicsResource
    {
        internal ID3D12GraphicsCommandList nativeCommandList;
        internal ID3D12PipelineState pipelineState;
        internal ID3D12CommandAllocator commandAllocator;
        internal Fence fence;
        internal long fenceValue;


        public CommandList(GraphicsDevice device) : base(device)
        {
            Initialize();
        }



        private void Initialize()
        {
            commandAllocator = GraphicsDevice.NativeDevice.CreateCommandAllocator<ID3D12CommandAllocator>( Vortice.Direct3D12.CommandListType.Direct);

            fenceValue = 0;
            fence = new Fence(GraphicsDevice, fenceValue);

            nativeCommandList = GraphicsDevice.NativeDevice.CreateCommandList<ID3D12GraphicsCommandList>(0,Vortice.Direct3D12.CommandListType.Direct, commandAllocator, null);
            // We close it as it starts in open state
            nativeCommandList.Close();

        }



        public void SetPipelineState(PipelineState pipelineState)
        {

            nativeCommandList.SetPipelineState(pipelineState.oldPipelineState);

            nativeCommandList.SetGraphicsRootSignature(pipelineState.RootSignature);

        }
        


        public void Reset()
        {
            commandAllocator.Reset();
            nativeCommandList.Reset(commandAllocator, null);
        }


        public void SetViewport(int x, int y, int w, int h)
        {
            var viewport = new Vortice.Mathematics.Viewport
            {
                //Width = w,
                //Height = h,
                //X = x,
                //Y = y,
                //MaxDepth = 1.0f,
                //MinDepth = 0.0f
            };
            nativeCommandList.RSSetViewport(viewport);
        }


        public void SetScissor(int x, int y, int w, int h)
        {



            nativeCommandList.RSSetScissorRect(x,y);
        }





        public void SetRenderTargets(Texture depthStencilBuffer, Texture[] renderTargets)
        {
            CpuDescriptorHandle[] cpuDescriptorHandles = new CpuDescriptorHandle[renderTargets.Length];

            for (int i = 0; i < cpuDescriptorHandles.Length; i++)
                cpuDescriptorHandles[i] = renderTargets[i].NativeRenderTargetView;
            

            nativeCommandList.OMSetRenderTargets(cpuDescriptorHandles, depthStencilBuffer?.NativeDepthStencilView);
        }


        public void ClearTargetColor(Texture texture, float r, float g, float b, float a)
        {
            ResourceTransition(texture, ResourceStates.RenderTarget, ResourceStates.Present);

            nativeCommandList.ClearRenderTargetView(texture.NativeRenderTargetView, new Vortice.Mathematics.Color4(r, g, b, a));
            nativeCommandList.OMSetRenderTargets(texture.NativeRenderTargetView);

        }



        public void ClearDepth(Texture texture, float depth)
        {
            ResourceTransition(texture, ResourceStates.DepthRead, ResourceStates.Present);
            nativeCommandList.ClearDepthStencilView(texture.NativeDepthStencilView, ClearFlags.Depth, depth, 0);
        }





        public void ResourceTransition(Texture resource, ResourceStates before, ResourceStates after)
        {
            //ResourceBarrier barrier = new ResourceBarrier(new ResourceTransitionBarrier(resource, (ResourceStates)before, (ResourceStates)after));
            var barriers = stackalloc ResourceBarrier[1];
            barriers[0] = new ResourceBarrier(new ResourceTransitionBarrier(resource.Resource, (ResourceStates)before, (ResourceStates)after));

            nativeCommandList.ResourceBarrier(*barriers);


            //NativeCommandList.ResourceBarrierTransition(resource.Resource, (ResourceStates)before, (ResourceStates)after);
        }



        public void SetTopology(PrimitiveType primitiveType) => nativeCommandList.IASetPrimitiveTopology(ConvertExtensions.ToPrimitiveType(primitiveType));
        

        public void SetVertexBuffer(Buffer buffer)
        {

            VertexBufferView vertexBufferView = new VertexBufferView()
            {
                BufferLocation = (ulong)buffer.GPUVirtualAddress,
                SizeInBytes = buffer.SizeInBytes,
                StrideInBytes = buffer.StructureByteStride
            };
            nativeCommandList.IASetVertexBuffers(0,vertexBufferView);
        }


        public void SetIndexBuffer(Buffer buffer, IndexType type)
        {
            IndexBufferView indexBufferView = new IndexBufferView()
            {
                BufferLocation = (ulong)buffer.GPUVirtualAddress,
                SizeInBytes = buffer.SizeInBytes,
                Format = ConvertExtensions.ToIndexType(type),
            };

            nativeCommandList.IASetIndexBuffer(indexBufferView);
        }



        public void DrawInstanced(int count) => nativeCommandList.DrawInstanced(count, 1, 0, 0);
        

        public void DrawIndexedInstanced(int indexCountPerInstance, int instanceCount, int startIndexLocation, int baseVertexLocation, int startInstanceLocation) =>
            nativeCommandList.DrawIndexedInstanced(indexCountPerInstance, instanceCount, startIndexLocation, baseVertexLocation, startInstanceLocation);



        public void ExecuteCommandList()
        {
            nativeCommandList.Close();
            GraphicsDevice.NativeDirectCommandQueue.ExecuteCommandList(this);

            fenceValue++;
            fence.Signal(GraphicsDevice.NativeDirectCommandQueue, fenceValue);

            fence.Wait(fenceValue);

            //commandAllocator.Reset();
            //nativeCommandList.Reset(commandAllocator, null);
        }


    }
}
