using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Zeckoxe.Core;
using Zeckoxe.Desktop;
using Zeckoxe.Vulkan;

namespace Samples.Samples
{
    public class ClearScreen : IDisposable
    {
        public ClearScreen()
        {
            Window = new("Zeckoxe Engine", 1200, 800)
            {
            };


            Parameters = new()
            {
                BackBufferWidth = Window.Width,
                BackBufferHeight = Window.Height,
                Settings = new()
                {
                    Validation = ValidationType.Console | ValidationType.Debug,
                    Fullscreen = false,
                    VSync = false,
                    OptionalDeviceExtensions = OptionalDeviceExtensions.BindMemory2 | OptionalDeviceExtensions.CopyCommands2,
                },
            };
        }


        public Window Window { get; set; }
        public PresentationParameters Parameters { get; set; }
        public Adapter Adapter { get; set; }
        public Device Device { get; set; }
        public Framebuffer Framebuffer { get; set; }
        public SwapChain SwapChain { get; set; }

        public GraphicsContext Context { get; set; }



        public void Initialize()
        {

            Adapter = new(Parameters);

            Device = new(Adapter);

            SwapChain = new(Device, GetSwapchainSource());

            Framebuffer = new(SwapChain);

            Context = new(Device);
        }

        public SwapchainSource GetSwapchainSource()
        {
            if (Adapter.SupportsSurface)
            {
                if (Adapter.SupportsWin32Surface)
                    return Window.SwapchainWin32;

                if (Adapter.SupportsX11Surface)
                    return Window.SwapchainX11;

                if (Adapter.SupportsWaylandSurface)
                    return Window.SwapchainWayland;

                if (Adapter.SupportsMacOSSurface)
                    return Window.SwapchainNS;
            }

            throw new PlatformNotSupportedException("Cannot create a SwapchainSource.");
        }


        public void Run()
        {
            Initialize();

            BeginRun();

            Window?.Show();

            Tick();
        }

        public void Tick()
        {

            Window.RenderLoop(() =>
            {
                Update();
                Draw();
            });
        }



        public void BeginRun()
        {

        }

        public void Update()
        {

        }

        public void Draw()
        {
            CommandBuffer commandBuffer = Context.CommandBuffer;

            Device.WaitIdle();

            commandBuffer.Begin();
            commandBuffer.BeginFramebuffer(Framebuffer);

            commandBuffer.Close();
            commandBuffer.Submit();

            SwapChain.Present();
        }

        public void Dispose()
        {
            //Device.Dispose();
        }
    }
}
