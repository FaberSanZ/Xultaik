using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Zeckoxe.Core;
using Zeckoxe.Desktop;
using Zeckoxe.Games;
using Zeckoxe.Graphics;
using Zeckoxe.Physics;
using Buffer = Zeckoxe.Graphics.Buffer;


namespace Samples.Samples
{
    public class Transformations : IDisposable
    {

        [StructLayout(LayoutKind.Sequential)]
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


        [StructLayout(LayoutKind.Sequential)]
        public struct TransformUniform
        {
            public TransformUniform(Matrix4x4 p, Matrix4x4 m, Matrix4x4 v)
            {
                P = p;
                M = m;
                V = v;
            }

            public Matrix4x4 P;

            public Matrix4x4 M;

            public Matrix4x4 V;

            public void Update(Camera camera, Matrix4x4 m)
            {
                P = camera.Projection;
                M = m;
                V = camera.View;
            }
        }

        public Vertex[] vertices = new[]
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
            // front face
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




        public Transformations()
        {
            Window = new Window(string.Empty, 1200, 800);


            Parameters = new PresentationParameters()
            {
                BackBufferWidth = Window.Width,
                BackBufferHeight = Window.Height,
                Win32Handle = Window.Win32Handle,
                Settings = new Settings()
                {
                    Validation = false,
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
        public GraphicsContext Context { get; set; }
        public PipelineState PipelineState { get; set; }
        public Buffer VertexBuffer { get; set; }
        public Buffer IndexBuffer { get; set; }
        public Buffer ConstBuffer { get; set; }
        public Buffer ConstBuffer2 { get; set; }
        public Camera Camera { get; set; }
        public GameTime GameTime { get; set; }
        public DescriptorSet Descriptor1 { get; set; }
        public DescriptorSet Descriptor2 { get; set; }

        // TransformUniform 
        public TransformUniform uniform;
        public Matrix4x4 Model;
        public float yaw;
        public float pitch;
        public float roll;


        public void Initialize()
        {
            Adapter = new GraphicsAdapter(Parameters);

            Device = new GraphicsDevice(Adapter);

            Framebuffer = new Framebuffer(Device);

            Context = new GraphicsContext(Device);

            GameTime = new GameTime();


            Camera = new Camera()
            {
                Mode = CameraType.Free,
                Position = new Vector3(0, 0, -3.5f),
            };

            Camera.SetLens(Window.Width, Window.Height);


            // Reset Model
            Model = Matrix4x4.Identity;

            uniform = new TransformUniform(Camera.Projection, Model, Camera.View);



            CreateBuffers();

            CreatePipelineState();


            // Binding 0: Uniform buffer (Vertex shader)
            Descriptor1 = new DescriptorSet(PipelineState);
            Descriptor1.SetUniformBuffer(0, ConstBuffer);

            // Binding 0: Uniform buffer (Vertex shader)
            Descriptor2 = new DescriptorSet(PipelineState);
            Descriptor2.SetUniformBuffer(0, ConstBuffer2);

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




            ConstBuffer = new Buffer(Device, new BufferDescription()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            });

            ConstBuffer2 = new Buffer(Device, new BufferDescription()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            });

            uniform = new TransformUniform();
        }


        public void CreatePipelineState()
        {
            PipelineStateDescription Pipelinedescription = new PipelineStateDescription()
            {
                Framebuffer = Framebuffer,

                Layouts =
                {
                    // Binding 0: Uniform buffer (Vertex shader)
                    new DescriptorSetLayout()
                    {
                        Stage = ShaderStage.Vertex,
                        Type = DescriptorType.UniformBuffer,
                        Binding = 0,
                    }
                },

                InputAssemblyState = new InputAssemblyState()
                {
                    PrimitiveType = PrimitiveType.TriangleList,
                },
                RasterizationState = new RasterizationState()
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.None,
                    FrontFace = FrontFace.Clockwise,
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

            Camera.Update(GameTime);
        }
        public void Update()
        {

            Model = Matrix4x4.CreateFromYawPitchRoll(-yaw, -pitch, -roll) * Matrix4x4.CreateTranslation(-0.45f, 0.0f, 0.0f);
            uniform.Update(Camera, Model);
            ConstBuffer.SetData(ref uniform);


            Model = Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix4x4.CreateTranslation(0.45f, 0.0f, 0.0f);
            uniform.Update(Camera, Model);
            ConstBuffer2.SetData(ref uniform);


            yaw += 0.0006f * MathF.PI;
            pitch += 0.0006f * MathF.PI;
            roll += 0.0006f * MathF.PI;
        }


        public void Draw()
        {
            CommandBuffer commandBuffer = Context.CommandBuffer;

            Device.WaitIdle();

            commandBuffer.Begin();
            commandBuffer.BeginFramebuffer(Framebuffer, .0f, .2f, .4f);
            commandBuffer.SetViewport(Window.Width, Window.Height, 0, 0);
            commandBuffer.SetScissor(Window.Width, Window.Height, 0, 0);

            commandBuffer.SetGraphicPipeline(PipelineState);
            commandBuffer.SetVertexBuffers(new Buffer[] { VertexBuffer });
            commandBuffer.SetIndexBuffer(IndexBuffer);


            // Draw Cube 1
            commandBuffer.BindDescriptorSets(Descriptor1);
            commandBuffer.DrawIndexed(indices.Length, 1, 0, 0, 0);

            // Draw Cube 2
            commandBuffer.BindDescriptorSets(Descriptor2);
            commandBuffer.DrawIndexed(indices.Length, 1, 0, 0, 0);


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
