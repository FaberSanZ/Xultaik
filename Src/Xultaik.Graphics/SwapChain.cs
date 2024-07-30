// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using System.Collections.Generic;
using System.Text;
using Vortice.Direct3D;
using Vortice.DXGI;
using static Vortice.DXGI.DXGI;

using static Vortice.Direct3D12.D3D12;
using System.Runtime.InteropServices;
using Vortice.Direct3D12;

namespace Xultaik.Graphics
{
    public class SwapChain
    {
        public RenderDescriptor Description { get; private set; }

        //public Texture BackBuffer { get; set; }
        public Texture[] BackBuffers { get; set; }
        public Texture BackBuffer => BackBuffers[BackBufferIndex];

        public int BackBufferIndex { get; private set; } = 0;




        internal IDXGISwapChain3 NativeSwapChain;
        internal GraphicsDevice GraphicsDevice;
        private int bufferCount = 2;

        public SwapChain(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            Description = graphicsDevice.NativeParameters;

            CreateSwapChain();

            //BackBufferIndex = NativeSwapChain.CurrentBackBufferIndex;



            BackBuffers = new Texture[bufferCount];

            for (int i = 0; i < bufferCount; i++)
            {
                BackBuffers[i] = new Texture(GraphicsDevice);
                BackBuffers[i].InitializeFromImpl(NativeSwapChain.GetBuffer<ID3D12Resource>(i));
            }


            //BackBuffer = new Texture(GraphicsDevice);
            //BackBuffer.InitializeFromImpl(NativeSwapChain.GetBuffer<ID3D12Resource>(BackBufferIndex));
        }



        private void CreateSwapChain()
        {

            //switch (Description.Settings.Platform)
            //{
            //    case Platform.Win32:
            //         CreateSwapChainForDesktop();

            //    case Platform.UWP:
            //    default:
            //        throw new ArgumentException();

            //}


            CreateSwapChainForDesktop();

        }

        public void Present()
        {

            if (!Description.Settings.AllowTearing && !Description.Settings.VSync)
            {
                NativeSwapChain.Present(0, PresentFlags.None); // TODO: Defaul Full Screen ?
            }


            if (Description.Settings.AllowTearing && !Description.Settings.VSync)
            {
                NativeSwapChain.Present(0, PresentFlags.AllowTearing);
            }


            if (Description.Settings.VSync && !Description.Settings.AllowTearing)
            {
                NativeSwapChain.Present(1, PresentFlags.None);
            }


            BackBufferIndex = NativeSwapChain.CurrentBackBufferIndex;

        }     



        private void CreateSwapChainForDesktop()
        {
            SwapChainFlags Flags =  SwapChainFlags.None; // TODO: Defaul Full Screen ?


            if (Description.Settings.AllowTearing)
            {
                Flags = SwapChainFlags.AllowTearing;

                Description.Settings.VSync = false;
            }



            if (Description.Settings.VSync)
            {
                Flags = SwapChainFlags.None;

                Description.Settings.AllowTearing = false;
            }


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

            


            SwapChainDescription swapChainDesc = new SwapChainDescription()   
            {
                BufferCount = bufferCount,                                      // Set to a single back buffer.
                BufferDescription = BackBufferDesc,                             // Set the width and height of the back buffer.
                BufferUsage = Usage.RenderTargetOutput,                         // Set the usage of the back buffer.
                OutputWindow = Description.DeviceHandle,                        // Set the handle for the window to render to.
                SampleDescription = sampleDescription,                          // Turn multisampling off.
                Windowed = true,                                                // Set to full screen or windowed mode.
                Flags = Flags,                                                  // Don't set the advanced flags.
                SwapEffect = SwapEffect.FlipDiscard,                            // Discard the back buffer content after presenting.

                
            };
            





            IDXGIFactory4 Factory = GraphicsDevice.NativeAdapter.NativeFactory;

            IDXGISwapChain swapChain = Factory.CreateSwapChain(GraphicsDevice.NativeDirectCommandQueue.Queue, swapChainDesc);

            if (Description.Settings.Fullscreen)
            {
                // Before fullscreen switch
                swapChain.ResizeTarget(BackBufferDesc);

                // Switch to full screen
                swapChain.SetFullscreenState(true, default);

                // This is really important to call ResizeBuffers AFTER switching to IsFullScreen 
                swapChain.ResizeBuffers(bufferCount, Description.BackBufferWidth, Description.BackBufferHeight, Format.R8G8B8A8_UNorm, Flags);
            }



            NativeSwapChain = swapChain.QueryInterface<IDXGISwapChain3>();
        }
    }
}
