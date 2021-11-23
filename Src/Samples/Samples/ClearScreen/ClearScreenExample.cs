using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Vultaik.Desktop;
using Vultaik;
using Samples.Common;
using Vultaik.Toolkit;

namespace Samples.ClearScreen
{
    public class ClearScreenExample : ExampleBase, IDisposable
    {
        private AdapterConfig AdapterConfig;
        private Adapter Adapter;
        private Device Device;
        private Framebuffer Framebuffer;
        private SwapChain SwapChain;
        private CommandBuffer CommandBuffer;

        private float r, g, b, mod_r, mod_g, mod_b;

        public ClearScreenExample() : base()
        {
            AdapterConfig = new()
            {
                SwapChain = true,
                Debug = true,
                Fullscreen = false,
                Bindless = false,
                ConditionalRendering = false,
                ConservativeRasterization = false,
                RayTracing = false,
                ShadingRate = false,
            };
        }






        public override void Initialize()
        {

            Adapter = new(AdapterConfig);
            Device = new(Adapter);
            CommandBuffer = new(Device, CommandBufferType.AsyncGraphics);

            SwapChain = new(Device, new()
            {
                Source = GetSwapchainSource(Adapter),
                ColorSrgb = false,
                Height = Window.Height,
                Width = Window.Width,
                VSync = false,
                DepthFormat = Adapter.DepthFormat is Vortice.Vulkan.VkFormat.Undefined ? null : Adapter.DepthFormat
            });
            Framebuffer = new(SwapChain);
        }


        public override void Resize(int width, int height)
        {
            Console.WriteLine($"Height: {height}");
            Console.WriteLine($"Width: {width}");
            Console.WriteLine("=======");

            Device.WaitIdle();
            SwapChain.Resize(width, height);
            Framebuffer.Resize();
        }



        public override void Draw(ApplicationTime time)
        {

            Device.WaitIdle();

            CommandBuffer.Begin();
            CommandBuffer.BeginFramebuffer(Framebuffer, r, g, b, 1);

            CommandBuffer.Close();
            Device.Submit(CommandBuffer);

            SwapChain.Present();
        }







        public override void Update(ApplicationTime time)
        {
            if (mod_r == 0) mod_r = mod_g = mod_b += 20;
            //Update the colors of our scene
            r += mod_r * 0.00005f;
            g += mod_g * 0.00002f;
            b += mod_b * 0.00001f;

            if (r >= 1.0f || r <= 0.0f)
                mod_r *= -1;

            if (g >= 1.0f || g <= 0.0f)
                mod_g *= -1;

            if (b >= 1.0f || b <= 0.0f)
                mod_b *= -1;
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
