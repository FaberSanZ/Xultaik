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
using System.Runtime.CompilerServices;

namespace Samples.PushConstant
{
    public class PushConstantExample : ExampleBase, IDisposable
    {

        private VertexPositionColor[] vertices = new VertexPositionColor[]
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

        private int[] indices = new[]
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



        private AdapterConfig AdapterConfig;
        private Adapter Adapter;
        private Device Device;
        private Framebuffer Framebuffer;
        private SwapChain SwapChain;
        private GraphicsContext Context;
        private GraphicsPipeline PipelineState;
        public DescriptorSet DescriptorSet;
        private Buffer VertexBuffer;
        private Buffer IndexBuffer;
        private Buffer ConstBuffer;

        private ViewUniform view_uniform;
        private const uint OBJECT_INSTANCES = 125;
        private Vector3[] rotationSpeeds = new Vector3[OBJECT_INSTANCES]; // Store random per-object rotations
        private bool cubes_random = false;

        public PushConstantExample() : base()
        {

        }


        public override void Initialize()
        {

            AdapterConfig = new()
            {
                SwapChain = true,
                Debug = false,
                Fullscreen = false,
                //ForceExclusiveTransferQueue = true
            };


            Camera.SetPosition(0, 0, -20.5f);
            Camera.Update();

            Adapter = new Adapter(AdapterConfig);
            Device = new Device(Adapter);
            SwapChain = new SwapChain(Device, new()
            {
                Source = GetSwapchainSource(Adapter),
                ColorSrgb = false,
                Height = Window.Height,
                Width = Window.Width,
                VSync = false,
                DepthFormat = Adapter.DepthFormat is VkFormat.Undefined ? null : Adapter.DepthFormat
            });

            Framebuffer = new Framebuffer(SwapChain);
            Context = new GraphicsContext(Device);

            view_uniform = new(Camera.Projection, Camera.View);



            CreateBuffers();

            CreatePipelineState();

        }




        public void CreateBuffers()
        {

            // Prepare per-object matrices with random rotations
            Random rndGen = new Random();
            Func<Random, float> rndDist = rand => (float)(rand.NextDouble() * 2 - 1.0);

            for (uint i = 0; i < OBJECT_INSTANCES; i++)
            {
                rotationSpeeds[i] = new Vector3(rndDist(rndGen), rndDist(rndGen), rndDist(rndGen));
            }


            VertexBuffer = new(Device, new()
            {
                BufferFlags = BufferFlags.VertexBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = Interop.SizeOf<VertexPositionColor>(vertices),
            });

            VertexBuffer.SetData(vertices);


            IndexBuffer = new(Device, new()
            {
                BufferFlags = BufferFlags.IndexBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = Interop.SizeOf<int>(indices),
            });
            IndexBuffer.SetData(indices);


            ConstBuffer = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = Interop.SizeOf<ViewUniform>(),
            });
        }


        public void CreatePipelineState()
        {

            var file = Constants.ShadersFile;

            ShaderBytecode Fragment = ShaderBytecode.LoadFromFile(file + "PushConstant/Fragment.hlsl", ShaderStage.Fragment);
            ShaderBytecode Vertex = ShaderBytecode.LoadFromFile(file + "PushConstant/Vertex.hlsl", ShaderStage.Vertex);



            GraphicsPipelineDescription Pipelinedescription0 = new();
            Pipelinedescription0.SetFramebuffer(Framebuffer);
            Pipelinedescription0.SetShader(Fragment);
            Pipelinedescription0.SetShader(Vertex);
            Pipelinedescription0.SetVertexBinding(VertexInputRate.Vertex, VertexPositionColor.Size);
            Pipelinedescription0.AddVertexAttribute<VertexPositionColor>();
            PipelineState = new(Pipelinedescription0);

            DescriptorData descriptorData_0 = new();
            descriptorData_0.SetUniformBuffer(0, ConstBuffer);
            DescriptorSet = new(PipelineState, descriptorData_0);


        }





        public override void Update(ApplicationTime time)
        {
            Camera.Update();


            view_uniform.Update(Camera);
            ConstBuffer.SetData(ref view_uniform);


            if (Window!.Input.Keyboards[0].IsKeyPressed(Key.R))
                cubes_random = true;

            if (Window.Input.Keyboards[0].IsKeyPressed(Key.N))
                cubes_random = false;




        }



        public void AddCube(CommandBuffer cmd, Vector3 position, Vector3 rotation, bool r)
        {

            Matrix4x4 model = Matrix4x4.Identity;
            model = Matrix4x4.CreateTranslation(position);

            // Update matrices
            if (r)
            {
                model *= Matrix4x4.CreateRotationX(MathUtil.DegreesToRadians(rotation.X));
                model *= Matrix4x4.CreateRotationY(MathUtil.DegreesToRadians(-rotation.Y));
                model *= Matrix4x4.CreateRotationZ(MathUtil.DegreesToRadians(rotation.Z));
            }
            else
            {
                model = Matrix4x4.CreateRotationX(MathUtil.DegreesToRadians(rotation.X)) * model;
                model = Matrix4x4.CreateRotationY(MathUtil.DegreesToRadians(rotation.Y)) * model;
                model = Matrix4x4.CreateRotationZ(MathUtil.DegreesToRadians(rotation.Z)) * model;
            }

            cmd.PushConstant(PipelineState, ShaderStage.Vertex, model); // Vertex.hlsl Line 28: [[vk::push_constant]]
            cmd.DrawIndexed(indices.Length, 1, 0, 0, 0);

        }

        public void GenerateCubes(CommandBuffer cmd, bool r)
        {
            float rotation = Time.TotalMilliseconds / 6;

            uint dim = (uint)(Math.Pow(OBJECT_INSTANCES, (1.0f / 3.0f)));
            Vector3 offset = new Vector3(5.0f);

            for (uint x = 0; x < dim; x++)
            {
                for (uint y = 0; y < dim; y++)
                {
                    for (uint z = 0; z < dim; z++)
                    {
                        uint index = x * dim * dim + y * dim + z;

                        Vector3 rotations = rotation * rotationSpeeds[index];
                        Vector3 pos = new Vector3(-((dim * offset.X) / 2.0f) + offset.X / 2.0f + x * offset.X, -((dim * offset.Y) / 2.0f) + offset.Y / 2.0f + y * offset.Y, -((dim * offset.Z) / 2.0f) + offset.Z / 2.0f + z * offset.Z);

                        AddCube(cmd, pos, rotations, r);
                    }
                }
            }
        }

        public override void Draw(ApplicationTime time)
        {

            Device.WaitIdle();
            CommandBuffer cmd = Context.CommandBuffer;

            cmd.Begin();
            cmd.BeginFramebuffer(Framebuffer, .0f, .2f, .4f);
            cmd.SetViewport(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);
            cmd.SetScissor(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);

            cmd.SetVertexBuffers(new[] { VertexBuffer });
            cmd.SetIndexBuffer(IndexBuffer);
            cmd.SetGraphicPipeline(PipelineState);
            cmd.BindDescriptorSets(DescriptorSet);

            GenerateCubes(cmd, cubes_random);


            cmd.Close();
            Device.Submit(cmd);
            SwapChain.Present();
        }

        public override void Resize(int width, int height)
        {
            Device.WaitIdle();
            SwapChain.Resize(width, height);
            Framebuffer.Resize();

            Camera.AspectRatio = (float)width / height;
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

}
