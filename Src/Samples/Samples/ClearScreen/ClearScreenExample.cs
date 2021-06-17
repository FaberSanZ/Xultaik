using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Vultaik.Desktop;
using Vultaik;

namespace Samples.ClearScreen
{
    public class ClearScreenExample : IDisposable
    {
        public ClearScreenExample()
        {
            Window = new("Vultaik - (ClearScreen)", 1200, 800);

            Parameters = new()
            {
                BackBufferWidth = Window.Width,
                BackBufferHeight = Window.Height,
                Settings = new()
                {
                    Validation = ValidationType.None,
                    Fullscreen = false,
                    VSync = false,
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


        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        private float ModR = 20;
        private float ModG = 20;
        private float ModB = 20;



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


        private void Window_Resize((int Width, int Height) obj)
        {
            Console.WriteLine($"Height: {obj.Height}");
            Console.WriteLine($"Width: {obj.Width}");
            Console.WriteLine("=======");

            Device.WaitIdle();
            SwapChain.Resize(obj.Width, obj.Height);
            Framebuffer.Resize();
        }


        public void Run()
        {
            Initialize();

            BeginRun();

            Window?.Show();
            Window!.Resize += Window_Resize;
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
            //Update the colors of our scene
            R += ModR * 0.00005f;
            G += ModG * 0.00002f;
            B += ModB * 0.00001f;

            if (R >= 1.0f || R <= 0.0f)
                ModR *= -1;

            if (G >= 1.0f || G <= 0.0f)
                ModG *= -1;

            if (B >= 1.0f || B <= 0.0f)
                ModB *= -1;
        }

        public void Draw()
        {

            Device.WaitIdle();

            CommandBuffer.Begin();
            CommandBuffer.BeginFramebuffer(Framebuffer, R, G, B, A);

            CommandBuffer.Close();
            Device.Submit(CommandBuffer);

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
