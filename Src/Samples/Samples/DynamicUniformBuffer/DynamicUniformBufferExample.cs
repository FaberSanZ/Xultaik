using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Vultaik.Desktop;
using Vultaik;
using Vultaik.Physics;
using Buffer = Vultaik.Buffer;
using Vortice.Vulkan;
using Interop = Vultaik.Interop;
using Samples.Common;
using Vultaik.Toolkit;

namespace Samples.DynamicUniformBuffer
{
    public class DynamicUniformBufferExample : IDisposable
    {

        public VertexPositionColor[] vertices = new VertexPositionColor[]
        {
            // front face
            new(new(-0.5f,  0.5f, -0.5f), new(1.0f, 0.0f, 0.0f)),
            new(new( 0.5f, -0.5f, -0.5f), new(1.0f, 0.0f, 1.0f)),
            new(new(-0.5f, -0.5f, -0.5f), new(0.0f, 0.0f, 1.0f)),
            new(new( 0.5f,  0.5f, -0.5f), new(0.0f, 1.0f, 0.0f)),


            // right side face
            new(new(0.5f, -0.5f, -0.5f), new(1.0f, 0.0f, 0.0f)),
            new(new(0.5f,  0.5f,  0.5f), new(1.0f, 0.0f, 1.0f)),
            new(new(0.5f, -0.5f,  0.5f), new(0.0f, 0.0f, 1.0f)),
            new(new(0.5f,  0.5f, -0.5f), new(0.0f, 1.0f, 0.0f)),


            // left side face
            new(new(-0.5f,  0.5f,  0.5f), new(1.0f, 0.0f, 0.0f)),
            new(new(-0.5f, -0.5f, -0.5f), new(1.0f, 0.0f, 1.0f)),
            new(new(-0.5f, -0.5f,  0.5f), new(0.0f, 0.0f, 1.0f)),
            new(new(-0.5f,  0.5f, -0.5f), new(0.0f, 1.0f, 0.0f)),


            // back face
            new(new( 0.5f,  0.5f,  0.5f), new(1.0f, 0.0f, 0.0f)),
            new(new(-0.5f, -0.5f,  0.5f), new(1.0f, 0.0f, 1.0f)),
            new(new( 0.5f, -0.5f,  0.5f), new(0.0f, 0.0f, 1.0f)),
            new(new(-0.5f,  0.5f,  0.5f), new(0.0f, 1.0f, 0.0f)),



            // top face
            new(new(-0.5f, 0.5f, -0.5f), new(1.0f, 0.0f, 0.0f)),
            new(new( 0.5f, 0.5f,  0.5f), new(1.0f, 0.0f, 1.0f)),
            new(new( 0.5f, 0.5f, -0.5f), new(0.0f, 0.0f, 1.0f)),
            new(new(-0.5f, 0.5f,  0.5f), new(0.0f, 1.0f, 0.0f)),


            // bottom face
            new(new( 0.5f, -0.5f,  0.5f), new(1.0f, 0.0f, 0.0f) ),
            new(new(-0.5f, -0.5f, -0.5f), new(1.0f, 0.0f, 1.0f) ),
            new(new( 0.5f, -0.5f, -0.5f), new(0.0f, 0.0f, 1.0f) ),
            new(new(-0.5f, -0.5f,  0.5f), new(0.0f, 1.0f, 0.0f) ),

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



        public Camera Camera { get; set; }
        public PresentationParameters Parameters { get; set; }
        public Adapter Adapter { get; set; }
        public Device Device { get; set; }
        public Framebuffer Framebuffer { get; set; }
        public SwapChain SwapChain { get; set; }
        public GraphicsContext Context { get; set; }
        public Matrix4x4 Model { get; set; }
        public Window? Window { get; set; }
        public ApplicationTime Timer;


        public GraphicsPipeline PipelineState { get; set; }

        public DescriptorSet DescriptorSet { get; set; }


        public Buffer VertexBuffer { get; set; }
        public Buffer IndexBuffer { get; set; }
        public Buffer ConstBuffer { get; set; }
        public Buffer ConstBuffer2 { get; set; }



        public DynamicUniformBufferExample()
        {

        }


        public ViewUniform view_uniform;
        public ModelUniform model_uniform;
        public float yaw;
        public float pitch;
        public float roll;
        private IntPtr dynamicAlignment;

        public void Initialize()
        {
            Window = new Window("Vultaik", 1200, 800);

            Parameters = new PresentationParameters()
            {
                BackBufferWidth = Window.Width,
                BackBufferHeight = Window.Height,
                //SwapchainSource = Window.GetSwapchainSource(Adapter),
                Settings = new Settings()
                {
                    Validation = ValidationType.None,
                    Fullscreen = false,
                    VSync = false,
                },
            };


            Camera = new(45f, 1f, 0.1f, 64f);
            Camera.SetPosition(0, 0, -20.5f);
            //Camera.SetRotation(0, 0.0f, .5f);
            Camera.AspectRatio = (float)Window.Width / Window.Height;
            Camera.Update();


            Timer = new();
            Timer.Start();


            Adapter = new Adapter(Parameters);
            Device = new Device(Adapter);
            SwapChain = new SwapChain(Device, new()
            {
                Source = GetSwapchainSource(),
                ColorSrgb = false,
                Height = Window.Height,
                Width = Window.Width,
                SyncToVerticalBlank = false,
                DepthFormat = Adapter.DepthFormat is VkFormat.Undefined ? null : Adapter.DepthFormat
            });

            Framebuffer = new Framebuffer(SwapChain);
            Context = new GraphicsContext(Device);

            // Reset Model
            Model = Matrix4x4.Identity;


            view_uniform = new(Camera.Projection, Camera.View);



            CreateBuffers();

            CreatePipelineState();

        }




        public unsafe void CreateBuffers()
        {
            // Calculate required alignment depending on device limits
            ulong uboAlignment = Adapter.MinUniformBufferOffsetAlignment;
            dynamicAlignment = (IntPtr)(((ulong)sizeof(Matrix4x4) / uboAlignment) * uboAlignment + (((ulong)sizeof(Matrix4x4) % uboAlignment) > 0 ? uboAlignment : 0));
            IntPtr bufferSize = (IntPtr)(OBJECT_INSTANCES * (ulong)dynamicAlignment);

            unsafe
            {
                model_uniform.Model = (Matrix4x4*)alignedAlloc(bufferSize, dynamicAlignment);
            }

            // Prepare per-object matrices with offsets and random rotations
            Random rndGen = new Random();
            Func<Random, float> rndDist = rand => (float)(rand.NextDouble() * 2 - 1.0);

            for (uint i = 0; i < OBJECT_INSTANCES; i++)
            {
                rotations[i] = new Vector3(rndDist(rndGen), rndDist(rndGen), rndDist(rndGen)) * 2.0f * (float)Math.PI;
                rotationSpeeds[i] = new Vector3(rndDist(rndGen), rndDist(rndGen), rndDist(rndGen));
            }


            VertexBuffer = new(Device, new()
            {
                BufferFlags = BufferFlags.VertexBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<VertexPositionColor>(vertices),
            });

            VertexBuffer.SetData(vertices);


            IndexBuffer = new(Device, new()
            {
                BufferFlags = BufferFlags.IndexBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<int>(indices),
            });
            IndexBuffer.SetData(indices);


            ConstBuffer = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<ViewUniform>(),
            });

            ConstBuffer2 = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = GraphicsResourceUsage.DynamicUniform,
                SizeInBytes = (int)(ulong)bufferSize,
            });



        }


        public unsafe void CreatePipelineState()
        {

            var file = Constants.ShadersFile;

            ShaderBytecode Fragment = ShaderBytecode.LoadFromFile(file + "DynamicUniformBuffer/Fragment.hlsl", ShaderStage.Fragment);
            ShaderBytecode Vertex = ShaderBytecode.LoadFromFile(file + "DynamicUniformBuffer/Vertex.hlsl", ShaderStage.Vertex);



            GraphicsPipelineDescription Pipelinedescription0 = new();
            Pipelinedescription0.SetFramebuffer(Framebuffer);
            Pipelinedescription0.SetShader(Fragment);
            Pipelinedescription0.SetShader(Vertex);
            Pipelinedescription0.SetVertexBinding(VkVertexInputRate.Vertex, VertexPositionColor.Size);
            Pipelinedescription0.AddVertexAttribute<VertexPositionColor>();
            PipelineState = new(Pipelinedescription0);

            DescriptorData descriptorData_0 = new();
            descriptorData_0.SetUniformBuffer(0, ConstBuffer);
            descriptorData_0.SetUniformBufferDynamic(1, ConstBuffer2);
            DescriptorSet = new(PipelineState, descriptorData_0);


        }

        private const uint OBJECT_INSTANCES = 125;


        // Store random per-object rotations
        Vector3[] rotations = new Vector3[OBJECT_INSTANCES];
        Vector3[] rotationSpeeds = new Vector3[OBJECT_INSTANCES];

        private unsafe void* alignedAlloc(IntPtr size, IntPtr alignment)
        {
            return Marshal.AllocHGlobal(size).ToPointer();
        }

        public unsafe void Update()
        {
            Camera.Update();

            float rotation = Timer.TotalMilliseconds / 6;

            view_uniform.Update(Camera);
            ConstBuffer.SetData(ref view_uniform);



            uint dim = (uint)(Math.Pow(OBJECT_INSTANCES, (1.0f / 3.0f)));
            Vector3 offset = new Vector3(5.0f);

            for (uint x = 0; x < dim; x++)
            {
                for (uint y = 0; y < dim; y++)
                {
                    for (uint z = 0; z < dim; z++)
                    {
                        uint index = x * dim * dim + y * dim + z;

                        // Aligned offset
                        Matrix4x4* modelMat = (Matrix4x4*)(((ulong)model_uniform.Model + (index * (ulong)dynamicAlignment)));

                        // Update rotations
                        rotations[index] = rotation * rotationSpeeds[index];

                        // Update matrices
                        Vector3 pos = new Vector3(-((dim * offset.X) / 2.0f) + offset.X / 2.0f + x * offset.X, -((dim * offset.Y) / 2.0f) + offset.Y / 2.0f + y * offset.Y, -((dim * offset.Z) / 2.0f) + offset.Z / 2.0f + z * offset.Z);
                        *modelMat = Matrix4x4.CreateTranslation(pos);
                        *modelMat = Matrix4x4.CreateRotationX(MathUtil.DegreesToRadians(rotations[index].X)) * *modelMat;
                        *modelMat = Matrix4x4.CreateRotationY(MathUtil.DegreesToRadians(rotations[index].Y)) * *modelMat;
                        *modelMat = Matrix4x4.CreateRotationZ(MathUtil.DegreesToRadians(rotations[index].Z)) * *modelMat;

                    }
                }
            }


            var map = ConstBuffer2.Map();

            // Copy
            System.Runtime.CompilerServices.Unsafe.CopyBlock(map, model_uniform.Model, (uint)ConstBuffer2.SizeInBytes);
            Interop.MemoryHelper.Write(map, ref model_uniform);


            //ConstBuffer2.Unmap();

            ConstBuffer2.FlushMappedMemoryRanges();

            Timer.Update();
        }


        public void Draw()
        {

            Device.WaitIdle();
            CommandBuffer commandBuffer = Context.CommandBuffer;

            commandBuffer.Begin();
            commandBuffer.BeginFramebuffer(Framebuffer, .0f, .2f, .4f);
            commandBuffer.SetViewport(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);
            commandBuffer.SetScissor(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);

            commandBuffer.SetVertexBuffers(new Buffer[] { VertexBuffer });
            commandBuffer.SetIndexBuffer(IndexBuffer);


            commandBuffer.SetGraphicPipeline(PipelineState);



            for (uint j = 0; j < OBJECT_INSTANCES; j++)
            {
                // One dynamic offset per dynamic descriptor to offset into the ubo containing all model matrices
                uint dynamicOffset = j * (uint)(dynamicAlignment);

                // Bind the descriptor set for rendering a mesh using the dynamic offset
                commandBuffer.BindDescriptorSets(DescriptorSet, 1, dynamicOffset);

                commandBuffer.DrawIndexed(indices.Length, 1, 0, 0, 0);
            }




            commandBuffer.Close();
            Device.Submit(commandBuffer);
            SwapChain.Present();
        }

        private void Window_Resize((int Width, int Height) obj)
        {
            Console.WriteLine($"Height: {obj.Height}");
            Console.WriteLine($"Width: {obj.Width}");
            Console.WriteLine("=======");


            Device.WaitIdle();
            SwapChain.Resize(obj.Width, obj.Height);
            Framebuffer.Resize();

            Camera.AspectRatio = (float)obj.Width / obj.Height;
        }

        public void Run()
        {
            Initialize();

            Window?.Show();
            Window!.Resize += Window_Resize;
            Window.RenderLoop(() =>
            {
                Update();
                Draw();
            });
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



        public void Dispose()
        {
            Adapter.Dispose();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ViewUniform
    {
        public ViewUniform(Matrix4x4 v, Matrix4x4 p)
        {
            P = p;
            V = v;
        }

        public Matrix4x4 V;

        public Matrix4x4 P;


        public void Update(Camera camera)
        {
            P = camera.Projection;
            V = camera.View;
        }
    }

    public unsafe struct ModelUniform
    {
        public Matrix4x4* Model;

    }
}
