using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Zeckoxe.Desktop;
using Zeckoxe.Graphics;

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
                SwapchainSource = Window.GetSwapchainSource(),
                Settings = new()
                {
                    Validation = ValidationType.Console | ValidationType.Debug,
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

        public GraphicsContext Context { get; set; }



        public void Initialize()
        {

            Adapter = new(Parameters);

            Device = new(Adapter);

            SwapChain = new(Device);

            Framebuffer = new(SwapChain);

            Context = new(Device);
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

            commandBuffer.Begin(SwapChain);
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
