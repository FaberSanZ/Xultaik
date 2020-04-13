// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Game.cs
=============================================================================*/

using System;
using System.Numerics;
using Zeckoxe.Core;
using Zeckoxe.Desktop;
using Zeckoxe.Graphics;
using Zeckoxe.Graphics.Toolkit;
using Zeckoxe.ShaderCompiler;
using Buffer = Zeckoxe.Graphics.Buffer;


namespace _02_Hello_Triangle
{
    public struct Vertex
    {
        public Vector2 pos;
        public Vector3 color;
        public Vertex(Vector2 pos, Vector3 color)
        {
            this.pos = pos;
            this.color = color;
        }
    }

    public class Game : IDisposable
    {
        public Vertex[] vertices = new[]
        {
                new Vertex(new Vector2(0.0f, -0.5f), new Vector3(1.0f, 0.0f, 0.0f)),
                new Vertex(new Vector2(0.5f, 0.5f), new Vector3(0.0f, 1.0f, 0.0f)),
                new Vertex(new Vector2(-0.5f, 0.5f), new Vector3(0.0f, 0.0f, 1.0f)),
        };

        public Window Window { get; set; }
        public PresentationParameters Parameters { get; set; }
        public GraphicsInstance Instance { get; set; }
        public GraphicsAdapter Adapter { get; set; }
        public GraphicsDevice Device { get; set; }
        public Framebuffer Framebuffer { get; set; }
        public GraphicsContext Context { get; set; }
        public PipelineState PipelineState { get; set; }
        public Buffer VertexBuffer { get; set; }



        public Game()
        {
            Window = new Window(string.Empty, 1000, 720);


            Parameters = new PresentationParameters()
            {
                BackBufferWidth = Window.Width,
                BackBufferHeight = Window.Height,
                Win32Handle = Window.Win32Handle,
                Settings = new Settings()
                {
                    Validation = false,
                    Fullscreen = true,
                    VSync = false,
                },
            };
            //var s = DDSLoader.LoadFromFile("");
        }




        public void Initialize()
        {
            Instance = new GraphicsInstance(Parameters);

            Adapter = new GraphicsAdapter(Instance);

            Device = new GraphicsDevice(Adapter);



            Framebuffer = new Framebuffer(Device);

            Context = new GraphicsContext(Device);

            CreateBuffers();
            CreatePipelineState();
        }


        public void CreateBuffers()
        {


            VertexBuffer = new Buffer(Device, new BufferDescription()
            {
                BufferFlags = BufferFlags.VertexBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = vertices.Length * Interop.SizeOf<Vertex>(),
            });

        }


        public void CreatePipelineState()
        {
            PipelineStateDescription Pipelinedescription = new PipelineStateDescription()
            {
                Framebuffer = Framebuffer,

                InputAssemblyState = new InputAssemblyState()
                {
                    PrimitiveType = PrimitiveType.TriangleList,
                    //PrimitiveRestartEnable = false,
                },
                RasterizationState = new RasterizationState()
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.Back,
                    FrontFace = FrontFace.Clockwise
                },

                Vertex = ShaderLoader.LoadFromFile("Shaders/shader.vert", Stage.Vertex),
                Pixel = ShaderLoader.LoadFromFile("Shaders/shader.frag", Stage.Fragment),
            };

            PipelineState = new PipelineState(Pipelinedescription);
        }


        public void Run()
        {
            Initialize();

            BeginRun();

            Window.Title = ("Zeckoxe Engine - (Hello Triangle) " + Device.NativeAdapter.DeviceName);

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
            //vertices[0].color.Y += 0.001f;
            //vertices[1].color.X += 0.001f;
            //vertices[2].color.Z += 0.001f;
            VertexBuffer.SetData(vertices);

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
            commandBuffer.SetVertexBuffers(new Buffer[] { VertexBuffer });
            commandBuffer.Draw(3, 1, 0, 0);

            commandBuffer.Close();
            commandBuffer.Submit();

            Device.NativeSwapChain.Present();
        }

        public void Dispose()
        {
            Instance.Dispose();
        }

    }
}
