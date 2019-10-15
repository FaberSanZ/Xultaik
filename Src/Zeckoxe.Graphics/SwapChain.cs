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
        internal IDXGISwapChain3 NativeSwapChain;

        internal GraphicsDevice GraphicsDevice;

        public PresentationParameters Description { get; private set; }

        public Texture BackBuffer { get; set; }

        public int BackBufferIndex { get; private set; } = 0;

        public SwapChain(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            Description = graphicsDevice.NativeParameters;

            CreateSwapChainForDesktop();

            BackBufferIndex = NativeSwapChain.GetCurrentBackBufferIndex();


            BackBuffer = new Texture(GraphicsDevice);
            BackBuffer.InitializeFromImpl(NativeSwapChain.GetBuffer<ID3D12Resource>(BackBufferIndex));
        }



        private void CreateSwapChain()
        {

            //switch (Description.Settings.Platform)
            //{
            //    case Platform.Win32:
            //        return CreateSwapChainForDesktop();

            //    case Platform.UWP:
            //    default:
            //        throw new ArgumentException();

            //}


            CreateSwapChainForDesktop();

        }

        public void Present()
        {
            NativeSwapChain.Present(1, PresentFlags.None);
            BackBufferIndex = NativeSwapChain.GetCurrentBackBufferIndex();

            BackBuffer.Resource.Dispose();
            BackBuffer.InitializeFromImpl(NativeSwapChain.GetBuffer<ID3D12Resource>(BackBufferIndex));

        }



        private void CreateSwapChainForDesktop()
        {

            ModeDescription BackBufferDesc = new ModeDescription()
            {
                Width = Description.BackBufferWidth,
                Height = Description.BackBufferHeight,
                Format = Format.R8G8B8A8_UNorm,
                RefreshRate = new Rational()
                {
                    Numerator = 60,
                    Denominator = 1
                },
                ScanlineOrdering = ModeScanlineOrder.Unspecified,
                Scaling = ModeScaling.Unspecified,
            };

            SampleDescription sampleDescription = new SampleDescription()
            {
                Count = 1,
                Quality = 0
            };

            SwapChainFlags Flags = Description.Settings.Fullscreen ? SwapChainFlags.AllowModeSwitch : SwapChainFlags.None;
            


            SwapChainDescription swapChainDesc = new SwapChainDescription() // Initialize the swap chain description.
            {
                BufferCount = 3,                                            // Set to a single back buffer.
                BufferDescription = BackBufferDesc,                         // Set the width and height of the back buffer.
                Usage = Usage.Backbuffer | Usage.RenderTargetOutput,        // Set the usage of the back buffer.
                OutputWindow = Description.DeviceHandle,                    // Set the handle for the window to render to.
                SampleDescription = sampleDescription,                      // Turn multisampling off.
                IsWindowed = true,                                          // Set to full screen or windowed mode.
                Flags = Flags,                                              // Don't set the advanced flags.
                SwapEffect = SwapEffect.FlipDiscard,                        // Discard the back buffer content after presenting.
            };
            





            IDXGIFactory4 Factory = GraphicsDevice.NativeAdapter.NativeFactory;

            IDXGISwapChain swapChain = Factory.CreateSwapChain(GraphicsDevice.NativeDirectCommandQueue.Queue, swapChainDesc);



            if (Description.Settings.Fullscreen)
            {
                swapChain.ResizeTarget(BackBufferDesc);
                swapChain.SetFullscreenState(true, default);
                swapChain.ResizeBuffers(3, Description.BackBufferWidth, Description.BackBufferHeight, Format.R8G8B8A8_UNorm, SwapChainFlags.AllowModeSwitch);
            }

            NativeSwapChain = swapChain.QueryInterface<IDXGISwapChain3>();


        }
    }
}
