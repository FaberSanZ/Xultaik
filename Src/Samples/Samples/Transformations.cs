using System;
using System.Numerics;
using Zeckoxe.Core;
using Zeckoxe.Desktop;
using Zeckoxe.Graphics;
using Zeckoxe.Graphics.Toolkit;
using Zeckoxe.ShaderCompiler;
using Buffer = Zeckoxe.Graphics.Buffer;


namespace Samples.Samples
{
    public class Transformations : IDisposable
    {
        public struct Vertex
        {
            public Vertex(Vector3 position, Vector3 color)
            {
                Position = position;
                Color = color;
            }

            public Vector3 Position;
            public Vector3 Color;
        }


        Vertex[] vertices = new Vertex[]
            {
                // front face
                new Vertex { Position = new Vector3(-0.5f,  0.5f, -0.5f), Color = new Vector3(1.0f, 0.0f, 0.0f) },
                new Vertex { Position = new Vector3(0.5f, -0.5f, -0.5f), Color = new Vector3(1.0f, 0.0f, 1.0f) },
                new Vertex { Position = new Vector3(-0.5f, -0.5f, -0.5f), Color = new Vector3(0.0f, 0.0f, 1.0f) },
                new Vertex { Position = new Vector3(0.5f,  0.5f, -0.5f), Color = new Vector3(0.0f, 1.0f, 0.0f) },

                // right side face
                new Vertex { Position = new Vector3(0.5f, -0.5f, -0.5f), Color = new Vector3(1.0f, 0.0f, 0.0f) },
                new Vertex { Position = new Vector3(0.5f,  0.5f,  0.5f), Color = new Vector3(1.0f, 0.0f, 1.0f) },
                    new Vertex { Position = new Vector3(0.5f, -0.5f,  0.5f), Color = new Vector3(0.0f, 0.0f, 1.0f) },
                    new Vertex { Position = new Vector3(0.5f,  0.5f, -0.5f), Color = new Vector3(0.0f, 1.0f, 0.0f) },


                    // left side face
                    new Vertex { Position = new Vector3(-0.5f,  0.5f,  0.5f), Color = new Vector3(1.0f, 0.0f, 0.0f) },
                    new Vertex { Position = new Vector3(-0.5f, -0.5f, -0.5f), Color = new Vector3(1.0f, 0.0f, 1.0f) },
                    new Vertex { Position = new Vector3(-0.5f, -0.5f,  0.5f), Color = new Vector3(0.0f, 0.0f, 1.0f) },
                    new Vertex { Position = new Vector3(-0.5f,  0.5f, -0.5f), Color = new Vector3(0.0f, 1.0f, 0.0f) },


                    // back face
                    new Vertex { Position = new Vector3(0.5f,  0.5f,  0.5f), Color = new Vector3(1.0f, 0.0f, 0.0f) },
                    new Vertex { Position = new Vector3(-0.5f, -0.5f,  0.5f), Color = new Vector3(1.0f, 0.0f, 1.0f) },
                    new Vertex { Position = new Vector3( 0.5f, -0.5f,  0.5f), Color = new Vector3(0.0f, 0.0f, 1.0f) },
                    new Vertex { Position = new Vector3(-0.5f,  0.5f,  0.5f), Color = new Vector3(0.0f, 1.0f, 0.0f) },



                    // top face
                    new Vertex { Position = new Vector3(-0.5f,  0.5f, -0.5f), Color = new Vector3(1.0f, 0.0f, 0.0f) },
                    new Vertex { Position = new Vector3(0.5f,  0.5f,  0.5f), Color = new Vector3(1.0f, 0.0f, 1.0f) },
                    new Vertex { Position = new Vector3( 0.5f,  0.5f, -0.5f), Color = new Vector3(0.0f, 0.0f, 1.0f) },
                    new Vertex { Position = new Vector3(-0.5f,  0.5f,  0.5f), Color = new Vector3(0.0f, 1.0f, 0.0f) },


                    // bottom face
                    new Vertex { Position = new Vector3( 0.5f, -0.5f,  0.5f), Color = new Vector3(1.0f, 0.0f, 0.0f) },
                    new Vertex { Position = new Vector3(-0.5f, -0.5f, -0.5f), Color = new Vector3(1.0f, 0.0f, 1.0f) },
                    new Vertex { Position = new Vector3( 0.5f, -0.5f, -0.5f), Color = new Vector3(0.0f, 0.0f, 1.0f) },
                    new Vertex { Position = new Vector3(-0.5f, -0.5f,  0.5f), Color = new Vector3(0.0f, 1.0f, 0.0f) },

            };




        public int[] indices = new[]
        {
                // ffront face
                0, 1, 2, // first triangle
                0, 3, 1, // second triangle

                // left face
                4, 5, 6, // first triangle
                4, 7, 5, // second triangle

                // right face
                8, 9, 10, // first triangle
                8, 11, 9, // second triangle

                // back face
                12, 13, 14, // first triangle
                12, 15, 13, // second triangle

                // top face
                16, 17, 18, // first triangle
                16, 19, 17, // second triangle

                // bottom face
                20, 21, 22, // first triangle
                20, 23, 21, // second triangle
        };


        public Window Window { get; set; }
        public PresentationParameters Parameters { get; set; }
        public GraphicsAdapter Adapter { get; set; }
        public GraphicsDevice Device { get; set; }
        public Framebuffer Framebuffer { get; set; }
        public GraphicsContext Context { get; set; }
        public PipelineState PipelineState { get; set; }
        public Fence Fence { get; set; } // Synchronization Primitives
        public Buffer VertexBuffer { get; set; }
        public Buffer IndexBuffer { get; set; }


        public Transformations()
        {
            Window = new Window(string.Empty, 1000, 720);


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




        public void Initialize()
        {
            Adapter = new GraphicsAdapter(Parameters);

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
                SizeInBytes = Interop.SizeOf<Vertex>(vertices),
            });


            IndexBuffer = new Buffer(Device, new BufferDescription()
            {
                BufferFlags = BufferFlags.IndexBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<int>(indices),
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
                },
                RasterizationState = new RasterizationState()
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.None,
                    FrontFace = FrontFace.Clockwise
                },
                PipelineVertexInput = new PipelineVertexInput
                {
                    VertexAttributeDescriptions =
                    {
                        new VertexInputAttribute
                        {
                            Binding = 0,
                            Location = 0,
                            Format = PixelFormat.R32G32B32SFloat,
                            Offset = 0,
                        },
                        new VertexInputAttribute
                        {
                            Binding = 0,
                            Location = 1,
                            Format = PixelFormat.R32G32B32SFloat,
                            Offset = 12,
                        }
                    },
                    VertexBindingDescriptions =
                    {
                        new VertexInputBinding
                        {
                            Binding = 0,
                            InputRate = VertexInputRate.Vertex,
                            Stride = Interop.SizeOf<Vertex>(),
                        }
                    },
                },
                Shaders =
                {
                    ShaderBytecode.LoadFromFile("Shaders/Transformations/shader.frag", ShaderStage.Fragment),
                    ShaderBytecode.LoadFromFile("Shaders/Transformations/shader.vert", ShaderStage.Vertex),
                },
            };

            PipelineState = new PipelineState(Pipelinedescription);
        }


        public void Run()
        {
            Initialize();

            BeginRun();

            Window.Title += "Zeckoxe Engine - (Transformations) ";

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
            VertexBuffer.SetData(vertices);
            IndexBuffer.SetData(indices);
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
            commandBuffer.SetVertexBuffers(new Buffer[] { VertexBuffer });
            commandBuffer.SetIndexBuffer(IndexBuffer);
            commandBuffer.DrawIndexed(3, 1, 0, 0, 0);


            commandBuffer.Close();
            commandBuffer.Submit(/*Fence*/);

            Device.NativeSwapChain.Present();
        }

        public void Dispose()
        {
            Adapter.Dispose();
        }
    }
}
