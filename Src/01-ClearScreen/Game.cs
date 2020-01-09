// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Game.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using Zeckoxe.Desktop;
using Zeckoxe.Graphics;
//using Zeckoxe.Mathematics;

namespace _01_ClearScreen
{
    public class Game : IDisposable
    {
        public Window Window { get; set; }

        public PresentationParameters Parameters { get; set; }

        public GraphicsInstance Instance { get; set; }

        public GraphicsAdapter Adapter { get; set; }

        public GraphicsDevice Device { get; set; }

        public Texture Texture { get; set; }

        public Framebuffer Framebuffer { get; set; }

        public GraphicsContext Context { get; set; }




        public Game()
        {
            Window = new Window("Zeckoxe Engine - (Clear Screen)", 1000, 720)
            {
                StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen,
            };


            Parameters = new PresentationParameters()
            {
                BackBufferWidth = Window.Width,
                BackBufferHeight = Window.Height,
                DeviceHandle = Window.Handle,
                Settings = new Settings()
                {
                    Validation = true,
                    Fullscreen = true,
                    VSync = false,
                },
            };
        }

        public void Initialize()
        {
            Instance = new GraphicsInstance(Parameters);

            Adapter = new GraphicsAdapter(Instance);

            Device = new GraphicsDevice(Adapter);

            Texture = new Texture(Device);

            Framebuffer = new Framebuffer(Device);

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
            foreach (var Description in Device.NativeAdapter.Description)
                Console.WriteLine(Description);


        }

        public void Update()
        {

        }

        public void Draw()
        {
            CommandList CommandList = Context.CommandList;

            Device.WaitIdle();

            CommandList.Begin();
            CommandList.BeginFramebuffer(Framebuffer);
            CommandList.Clear(0.0f, 0.2f, 0.4f, 1.0f);

            CommandList.SetViewport(Window.Width, Window.Height, 0, 0);
            CommandList.SetScissor(Window.Width, Window.Height, 0, 0);

            CommandList.EndFramebuffer();
            CommandList.End();
            CommandList.Submit();

            Device.NativeSwapChain.Present();
        }

        public void Dispose()
        {
            //Device.Dispose();
        }

    }
}
