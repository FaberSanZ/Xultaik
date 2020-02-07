// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Game.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Zeckoxe.Desktop;
using Zeckoxe.Graphics;
using Zeckoxe.ShaderCompiler;
using Buffer = Zeckoxe.Graphics.Buffer;
using Zeckoxe.Mathematics;

namespace _02_Hello_Triangle
{
    public class Game : IDisposable
    {

        public Window Window { get; set; }

        public PresentationParameters Parameters { get; set; }

        public GraphicsInstance Instance { get; set; }

        public GraphicsAdapter Adapter { get; set; }

        public GraphicsDevice Device { get; set; }

        public DeviceLimits Limits { get; set; }

        public Framebuffer Framebuffer { get; set; }

        public GraphicsContext Context { get; set; }

        public PipelineState PipelineState { get; set; }


        public Game()
        {
            Window = new Window("Zeckoxe Engine - (Hello Triangle)", 1000, 720);


            Parameters = new PresentationParameters()
            {
                BackBufferWidth = Window.Width,
                BackBufferHeight = Window.Height,
                Win32Handle = Window.Win32Handle,
                Settings = new Settings()
                {
                    Validation = true,
                    Fullscreen = true,
                    VSync = false,
                },
            };

            //foreach (var item in Compiler.LoadFromFile("Shaders/pntriangles.tesc", Stage.TessControl, Language.GLSL))
            //    Console.WriteLine(item);
        }





        public void Initialize()
        {
            Instance = new GraphicsInstance(Parameters);

            Adapter = new GraphicsAdapter(Instance);

            Device = new GraphicsDevice(Adapter);

            Limits = new DeviceLimits(Device);

            Framebuffer = new Framebuffer(Device);

            Context = new GraphicsContext(Device);


            PipelineState = new PipelineState(new ShaderBytecode[] 
            {
                new ShaderBytecode(Compiler.LoadFromFile("Shaders/shader.vert",  Stage.Vertex, Language.GLSL)),
                new ShaderBytecode(Compiler.LoadFromFile("Shaders/PixelShader.hlsl", Stage.Pixel, Language.HLSL)), 
            }
            ,Framebuffer);
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
            Console.WriteLine($"DeviceName : { Device.NativeAdapter.DeviceName }");
            Console.WriteLine($"MultisampleCount : { Limits.MultisampleCount }");
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
            commandBuffer.Clear(0.0f, 0.2f, 0.4f, 1.0f);

            commandBuffer.SetViewport(Window.Width, Window.Height, 0, 0);
            commandBuffer.SetScissor(Window.Width, Window.Height, 0, 0);
            commandBuffer.SetGraphicPipeline(PipelineState);
            commandBuffer.Draw(3, 1, 0, 0);

            commandBuffer.Close();
            commandBuffer.Submit();

            Device.NativeSwapChain.Present();
        }

        public void Dispose()
        {
            //Device.Dispose();
        }

    }
}
