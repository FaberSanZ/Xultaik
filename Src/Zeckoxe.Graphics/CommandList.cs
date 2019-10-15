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
        private ID3D12GraphicsCommandList nativeCommandList;
        internal ID3D12GraphicsCommandList NativeCommandList => nativeCommandList;

        private ID3D12PipelineState pipelineState;
        private ID3D12CommandAllocator commandAllocator;
        private ID3D12Fence fence;
        private long fenceValue;


        protected CommandList(GraphicsDevice device) : base(device)
        {

        }



        private void Initialize()
        {
            commandAllocator = GraphicsDevice.NativeDevice.CreateCommandAllocator( Vortice.Direct3D12.CommandListType.Direct);

            fenceValue = 0;
            fence = GraphicsDevice.NativeDevice.CreateFence(0, FenceFlags.None);

            nativeCommandList = GraphicsDevice.NativeDevice.CreateCommandList(Vortice.Direct3D12.CommandListType.Direct, commandAllocator, null);
            // We close it as it starts in open state
            NativeCommandList.Close();

        }


        public void Wait()
        {
            while (fence.CompletedValue < fenceValue)
            {
                System.Threading.Thread.Sleep(1);
            }
        }


        public void Reset()
        {
            commandAllocator.Reset();
            NativeCommandList.Reset(commandAllocator, null);
        }


        public void Close()
        {
            NativeCommandList.Close();
        }


        public void FinishFrame()
        {
            fenceValue++;
            GraphicsDevice.NativeCommandQueue.Queue.Signal(fence, fenceValue);
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
            NativeCommandList.RSSetViewport(viewport);
        }


        public void SetScissor(int x, int y, int w, int h)
        {
            //NativeCommandList.SetScissorRectangles();
        }


        public void SetColorTarget(CpuDescriptorHandle view)
        {
            //NativeCommandList.OMSetRenderTargets(1, view, null);
        }


        public void SetRenderTargets(CpuDescriptorHandle render, CpuDescriptorHandle depth)
        {
            //NativeCommandList.OMSetRenderTargets(1, render, depth);
        }


        public void SetRenderTargets(CpuDescriptorHandle render, Texture depth)
        {
            //NativeCommandList.SetRenderTargets(1, render, depth?.NativeDepthStencilView);
        }


        public void SetRenderTargets(Texture depth, Texture[] render)
        {
            //NativeCommandList.OMSetRenderTargets();
            var ss = render[0].NativeRenderTargetView;
            NativeCommandList.OMSetRenderTargets(ss, depth?.NativeDepthStencilView);
        }


        public void ClearTargetColor(CpuDescriptorHandle handle, float r, float g, float b, float a)
        {
            NativeCommandList.ClearRenderTargetView(handle, new Vortice.Mathematics.Color4(r, g, b, a));
        }


        public void ClearTargetColor(Texture texture, float r, float g, float b, float a)
        {
            NativeCommandList.ClearRenderTargetView(texture.NativeRenderTargetView, new Vortice.Mathematics.Color4(r, g, b, a));
        }

        public void ClearDepth(CpuDescriptorHandle handle, float depth)
        {
            NativeCommandList.ClearDepthStencilView(handle, ClearFlags.Depth, depth, 0);
        }


        public void ClearDepth(Texture texture, float depth)
        {
            NativeCommandList.ClearDepthStencilView(texture.NativeDepthStencilView, ClearFlags.Depth, depth, 0);
        }

        public void ResourceTransition(ID3D12Resource resource, ResourceStates before, ResourceStates after)
        {
            ResourceBarrier barrier = new ResourceBarrier(new ResourceTransitionBarrier(resource, (ResourceStates)before, (ResourceStates)after));
            var barriers = stackalloc ResourceBarrier[1];
            barriers[0] = new ResourceBarrier(new ResourceTransitionBarrier(resource, (ResourceStates)before, (ResourceStates)after));

            NativeCommandList.ResourceBarrier(*barriers);
            //NativeCommandList.ResourceBarrierTransition(resource, (ResourceStates)before, (ResourceStates)after);
        }


        public void ResourceTransition(Texture resource, ResourceStates before, ResourceStates after)
        {
            //ResourceBarrier barrier = new ResourceBarrier(new ResourceTransitionBarrier(resource, (ResourceStates)before, (ResourceStates)after));
            var barriers = stackalloc ResourceBarrier[1];
            barriers[0] = new ResourceBarrier(new ResourceTransitionBarrier(resource.Resource, (ResourceStates)before, (ResourceStates)after));

            NativeCommandList.ResourceBarrier(*barriers);


            //NativeCommandList.ResourceBarrierTransition(resource.Resource, (ResourceStates)before, (ResourceStates)after);
        }



        public void Draw(int count, bool indexed)
        {
            if (indexed)
            {
                DrawIndexed(count);
            }
            else
            {
                Draw(count);
            }
        }

        public void Draw(int count)
        {
            NativeCommandList.DrawInstanced(count, 1, 0, 0);
        }

        public void DrawIndexed(int idxCnt)
        {

            NativeCommandList.DrawIndexedInstanced(idxCnt, 1, 0, 0, 0);
        }


        public void BeginDraw()
        {
            Reset();
        }

        public void EndDraw()
        {
            NativeCommandList.Close();
            GraphicsDevice.NativeCommandQueue.Queue.ExecuteCommandList(NativeCommandList);
        }

    }
}
