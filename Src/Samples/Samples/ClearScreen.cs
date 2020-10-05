using System;
using System.Collections.Generic;
using System.Text;
using Zeckoxe.Desktop;
using Zeckoxe.Graphics;

namespace Samples.Samples
{
    public class ClearScreen : IDisposable
    {
        public ClearScreen()
        {
            Window = new Window(" - (Clear Screen)", 1200, 800)
            {
                //StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen,
            };


            Parameters = new PresentationParameters()
            {
                BackBufferWidth = Window.Width,
                BackBufferHeight = Window.Height,
                Win32Handle = Window.Win32Handle,
                Settings = new Settings()
                {
                    Validation = true,
                    Fullscreen = false,
                    VSync = false,
                },
            };
        }


        public Window Window { get; set; }
        public PresentationParameters Parameters { get; set; }
        public GraphicsAdapter Adapter { get; set; }
        public GraphicsDevice Device { get; set; }
        public Framebuffer Framebuffer { get; set; }
        public SwapChain SwapChain { get; set; }

        public GraphicsContext Context { get; set; }



        public void Initialize()
        {

            Adapter = new GraphicsAdapter(Parameters);

            Device = new GraphicsDevice(Adapter);

            SwapChain = new SwapChain(Device);

            Framebuffer = new Framebuffer(SwapChain);

            Context = new GraphicsContext(Device);
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
