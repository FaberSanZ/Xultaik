// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	CommandList.cs
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
            commandAllocator = GraphicsDevice.NativeDevice.CreateCommandAllocator( Vortice.Direct3D12.CommandListType.Direct);

            fenceValue = 0;
            fence = new Fence(GraphicsDevice, fenceValue);

            nativeCommandList = GraphicsDevice.NativeDevice.CreateCommandList(Vortice.Direct3D12.CommandListType.Direct, commandAllocator, null);
            // We close it as it starts in open state
            nativeCommandList.Close();

        }



        public void SetPipelineState(PipelineState pipelineState)
        {

            nativeCommandList.SetPipelineState(pipelineState.oldPipelineState);

            nativeCommandList.SetGraphicsRootSignature(pipelineState.RootSignature);

        }
        public void Wait()
        {
            fence.Wait(fenceValue);
        }


        public void Reset()
        {
            commandAllocator.Reset();
            nativeCommandList.Reset(commandAllocator, null);
        }


        public void Close()
        {
            nativeCommandList.Close();
        }


        public void FinishFrame()
        {
            fenceValue++;

            fence.Signal(GraphicsDevice.NativeDirectCommandQueue, fenceValue);
        }


        public void SetViewport(int x, int y, int w, int h)
        {
            var viewport = new Vortice.Mathematics.Viewport
            {
                Width = w,
                Height = h,
                X = x,
                Y = y,
                MaxDepth = 1.0f,
                MinDepth = 0.0f
            };
            nativeCommandList.RSSetViewport(viewport);
        }


        public void SetScissor(int x, int y, int w, int h)
        {
            Vortice.Mathematics.Rect rect = new Vortice.Mathematics.Rect
            {
                
            };


            nativeCommandList.RSSetScissorRect(rect);
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
            nativeCommandList.ClearRenderTargetView(texture.NativeRenderTargetView, new Vortice.Mathematics.Color4(r, g, b, a));
            nativeCommandList.OMSetRenderTargets(texture.NativeRenderTargetView);
        }

        public void ClearDepth(CpuDescriptorHandle handle, float depth)
        {
            nativeCommandList.ClearDepthStencilView(handle, ClearFlags.Depth, depth, 0);
        }


        public void ClearDepth(Texture texture, float depth)
        {
            nativeCommandList.ClearDepthStencilView(texture.NativeDepthStencilView, ClearFlags.Depth, depth, 0);
        }

        public void ResourceTransition(ID3D12Resource resource, ResourceStates before, ResourceStates after)
        {
            ResourceBarrier barrier = new ResourceBarrier(new ResourceTransitionBarrier(resource, (ResourceStates)before, (ResourceStates)after));
            var barriers = stackalloc ResourceBarrier[1];
            barriers[0] = new ResourceBarrier(new ResourceTransitionBarrier(resource, (ResourceStates)before, (ResourceStates)after));

            nativeCommandList.ResourceBarrier(*barriers);
            //NativeCommandList.ResourceBarrierTransition(resource, (ResourceStates)before, (ResourceStates)after);
        }


        public void ResourceTransition(Texture resource, ResourceStates before, ResourceStates after)
        {
            //ResourceBarrier barrier = new ResourceBarrier(new ResourceTransitionBarrier(resource, (ResourceStates)before, (ResourceStates)after));
            var barriers = stackalloc ResourceBarrier[1];
            barriers[0] = new ResourceBarrier(new ResourceTransitionBarrier(resource.Resource, (ResourceStates)before, (ResourceStates)after));

            nativeCommandList.ResourceBarrier(*barriers);


            //NativeCommandList.ResourceBarrierTransition(resource.Resource, (ResourceStates)before, (ResourceStates)after);
        }
        public void SetVertexBuffer(Buffer buffer)
        {
            nativeCommandList.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);

            VertexBufferView vertexBufferView = new VertexBufferView()
            {
                BufferLocation = buffer.GPUVirtualAddress,
                SizeInBytes = buffer.SizeInBytes,
                StrideInBytes = buffer.StructureByteStride
            };
            nativeCommandList.IASetVertexBuffers(vertexBufferView);
        }




        public void SetIndexBuffer(Buffer buffer)
        {

            IndexBufferView indexBufferView = new IndexBufferView()
            {
                BufferLocation = buffer.GPUVirtualAddress,
                SizeInBytes = buffer.SizeInBytes,
                //Format = Vortice.DXGI.Format.R16_UInt
            };
            nativeCommandList.IASetIndexBuffer(indexBufferView);
        }



        public void Draw(int count)
        {
            nativeCommandList.DrawInstanced(count, 1, 0, 0);
        }

        public void DrawIndexed(int idxCnt)
        {

            nativeCommandList.DrawIndexedInstanced(idxCnt, 1, 0, 0, 0);
        }


        public void BeginDraw()
        {
            Reset();
        }

        public void EndDraw()
        {
            nativeCommandList.Close();

            GraphicsDevice.NativeDirectCommandQueue.ExecuteCommandList(this);
        }

    }
}
