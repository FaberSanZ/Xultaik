// Copyright(c) 2019-2020 Faber Leonardo.All Rights Reserved.

/*=============================================================================
	SwapChain.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Text;
using Vortice.DirectX;
using Vortice.DXGI;
using static Vortice.DXGI.DXGI;

using static Vortice.Direct3D12.D3D12;
using System.Runtime.InteropServices;
using Vortice.Direct3D12;

namespace Zeckoxe.Graphics
{
    public class SwapChain
    {
        internal IDXGISwapChain3 swapChain;

        internal GraphicsDevice GraphicsDevice;

        public PresentationParameters Description { get; private set; }

        public Texture BackBuffer { get; set; }

        public int BackBufferIndex { get; private set; } = 0;

        public SwapChain(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            Description = graphicsDevice.NativeParameters;

            swapChain = CreateSwapChain();

            BackBufferIndex = swapChain.GetCurrentBackBufferIndex();


            BackBuffer = new Texture(GraphicsDevice);
            BackBuffer.InitializeFromImpl(swapChain.GetBuffer<ID3D12Resource>(BackBufferIndex));
        }



        private IDXGISwapChain3 CreateSwapChain()
        {

            //switch (Description.Settings.Platform)
            //{
            //    case Platform.Win32:
            //        return CreateSwapChainForDesktop();

            //    case Platform.UWP:
            //    default:
            //        throw new ArgumentException();

            //}


            return CreateSwapChainForDesktop();

        }



        private IDXGISwapChain3 CreateSwapChainForDesktop()
        {

            ModeDescription BackBufferDesc = new ModeDescription();
            BackBufferDesc.Width = Description.BackBufferWidth;
            BackBufferDesc.Height = Description.BackBufferHeight;
            //BackBufferDesc.Format = ConvertExtensions.ToPixelFormat(Description.BackBufferFormat);
            BackBufferDesc.RefreshRate = new Rational(60, 1);
            BackBufferDesc.ScanlineOrdering = ModeScanlineOrder.Unspecified;
            BackBufferDesc.Scaling = ModeScaling.Unspecified;

            // Initialize the swap chain description.
            SwapChainDescription swapChainDesc = new SwapChainDescription();
            // Set to a single back buffer.
            swapChainDesc.BufferCount = 3;
            // Set the width and height of the back buffer.
            swapChainDesc.BufferDescription = BackBufferDesc;
            // Set the usage of the back buffer.
            swapChainDesc.Usage = Usage.RenderTargetOutput;
            // Set the handle for the window to render to.
            swapChainDesc.OutputWindow = Description.DeviceHandle;
            // Turn multisampling off.
            swapChainDesc.SampleDescription = new SampleDescription()
            {
                Count = 1,
                Quality = 0
            };
            // Set to full screen or windowed mode.
            swapChainDesc.IsWindowed = true;
            // Don't set the advanced flags.
            swapChainDesc.Flags = SwapChainFlags.None;
            // Discard the back buffer content after presenting.
            swapChainDesc.SwapEffect = SwapEffect.FlipDiscard;

            swapChainDesc.Flags = SwapChainFlags.AllowModeSwitch;

            swapChainDesc.SwapEffect = SwapEffect.FlipDiscard;



            IDXGIFactory4 Factory = GraphicsDevice.NativeAdapter.NativeFactory;

            IDXGISwapChain swapChain = Factory.CreateSwapChain(GraphicsDevice.NativeCommandQueue.Queue, swapChainDesc);


            if (Description.Settings.Fullscreen)
            {
                swapChain.ResizeTarget(BackBufferDesc);

                swapChain.SetFullscreenState(true, default);

                //swapChain.ResizeBuffers(3, Description.BackBufferWidth, Description.BackBufferHeight, ConvertExtensions.ToPixelFormat(Description.BackBufferFormat), SwapChainFlags.AllowModeSwitch);
            }


            return swapChain.QueryInterface<IDXGISwapChain3>();

        }
    }
}
