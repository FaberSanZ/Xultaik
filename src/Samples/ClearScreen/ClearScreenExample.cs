using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Zeckoxe.Core;
using Zeckoxe.Desktop;
using Zeckoxe.Vulkan;

namespace Samples.ClearScreen
{
    public class ClearScreenExample : IDisposable
    {
        public ClearScreenExample()
        {
            Window = new("Zeckoxe Engine", 1200, 800);

            Parameters = new()
            {
                BackBufferWidth = Window.Width,
                BackBufferHeight = Window.Height,
                Settings = new()
                {
                    Validation = ValidationType.Error,
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

        public CommandBuffer CommandBuffer { get; set; }



        public void Initialize()
        {

            Adapter = new(Parameters);

            Device = new(Adapter);


            SwapChain = new(Device, new()
            {
                Source = GetSwapchainSource(),
                ColorSrgb = false,
                Height = Window.Height,
                Width = Window.Width,
                SyncToVerticalBlank = false,
                DepthFormat = Adapter.DepthFormat is Vortice.Vulkan.VkFormat.Undefined ? null : Adapter.DepthFormat
            });

            Framebuffer = new(SwapChain);

            CommandBuffer = new(Device, CommandBufferType.AsyncGraphics);
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

            Device.WaitIdle();

            CommandBuffer.Begin();
            CommandBuffer.BeginFramebuffer(Framebuffer);

            CommandBuffer.Close();
            CommandBuffer.Submit();

            SwapChain.Present();
        }

        public void Dispose()
        {
            Framebuffer.Dispose();
            SwapChain.Dispose();
            Device.Dispose();
            Adapter.Dispose();
        }
    }
}
