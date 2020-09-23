using System;
using System.Numerics;
using Zeckoxe.Core;
using Zeckoxe.Desktop;
using Zeckoxe.Games;
using Zeckoxe.GLTF;
using Zeckoxe.Graphics;
using Zeckoxe.Physics;
using Buffer = Zeckoxe.Graphics.Buffer;

namespace Samples.Samples
{

    public class LoadGLTF : IDisposable
    {
        public LoadGLTF()
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
        public PipelineState PipelineState1 { get; set; }
        public PipelineState PipelineState2 { get; set; }
        public Buffer ConstBuffer { get; set; }
        public Buffer ConstBuffer2 { get; set; }
        public Camera Camera { get; set; }
        public GameTime GameTime { get; set; }
        public DescriptorSet Descriptor1 { get; set; }
        public DescriptorSet Descriptor2 { get; set; }
        public GLTFLoader Model { get; set; }



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
                Position = new Vector3(1, -.3f, -3.5f),
            };

            Camera.SetLens(Window.Width, Window.Height);


            Model = new GLTFLoader("Models/model.glb", Device);

            CreateBuffers();
            CreatePipelineState();


            // Binding 0: Uniform buffer (Vertex shader)
            Descriptor1 = new DescriptorSet(PipelineState1);
            Descriptor1.SetUniformBuffer(0, ConstBuffer);

            Descriptor2 = new DescriptorSet(PipelineState2);
            Descriptor2.SetUniformBuffer(0, ConstBuffer2);

        }


        public void CreateBuffers()
        {
            ConstBuffer = new Buffer(Device, new BufferDescription()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<CameraUbo>(),
            });

            ConstBuffer2 = new Buffer(Device, new BufferDescription()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<CameraUbo>(),
            });


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
                    FillMode = FillMode.Wireframe,
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

            PipelineState1 = new PipelineState(Pipelinedescription);






            PipelineStateDescription Pipelinedescription2 = new PipelineStateDescription()
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

            PipelineState2 = new PipelineState(Pipelinedescription2);
        }


        public void Run()
        {
            Initialize();

            BeginRun();

            Window.Title += "Zeckoxe Engine - (LoadGLTF) ";

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
        float r = 0;
        public void Update()
        {
            Camera.Update(GameTime);
            Camera.ModelRotate(new Vector3(3.15f, 0, 0));
            Camera.Model = Camera.Model * Matrix4x4.CreateTranslation(new Vector3(1.0f, 0.0f, 0.0f));
            ConstBuffer.SetData(Camera.CameraUbo);

            Camera.Update(GameTime);
            Camera.ModelRotate(new Vector3(3.15f, r, 0));
            Camera.Model = Camera.Model * Matrix4x4.CreateTranslation(new Vector3(-1.0f, 0.0f, 0.0f));
            ConstBuffer2.SetData(Camera.CameraUbo);

            //r += 0.002f;
        }

        public void Draw()
        {
            CommandBuffer commandBuffer = Context.CommandBuffer;

            Device.WaitIdle();

            commandBuffer.Begin();
            commandBuffer.BeginFramebuffer(Framebuffer);
            commandBuffer.SetViewport(Window.Width, Window.Height, 0, 0);
            commandBuffer.SetScissor(Window.Width, Window.Height, 0, 0);


            commandBuffer.BindDescriptorSets(Descriptor1);
            commandBuffer.SetGraphicPipeline(PipelineState1);
            Model.Draw(commandBuffer);


            commandBuffer.BindDescriptorSets(Descriptor2);
            commandBuffer.SetGraphicPipeline(PipelineState2);
            Model.Draw(commandBuffer);


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
