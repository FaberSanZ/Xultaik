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

namespace Samples.Transformations
{
    public class TransformationsExample : ExampleBase, IDisposable
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
        private GraphicsPipeline PipelineState_0;
        private GraphicsPipeline PipelineState_1;
        private DescriptorSet DescriptorSet_0;
        private DescriptorSet DescriptorSet_1;
        private Buffer VertexBuffer;
        private Buffer IndexBuffer;
        private Buffer ConstBuffer;
        private Buffer ConstBuffer2;
        private TransformUniform uniform;



        public TransformationsExample() : base()
        {

        }



        public override void Initialize()
        {

            AdapterConfig = new()
            {
                SwapChain = true,
                Debug = false,
                Fullscreen = false,
            };


            Camera.SetPosition(0, 0, -3.5f);
            Camera.Update();



            Adapter = new(AdapterConfig);
            Device = new(Adapter);
            SwapChain = new(Device, new()
            {
                Source = GetSwapchainSource(Adapter),
                ColorSrgb = false,
                Height = Window.Height,
                Width = Window.Width,
                VSync = false,
                DepthFormat = Adapter.DepthFormat is VkFormat.Undefined ? null : Adapter.DepthFormat
            });

            Framebuffer = new(SwapChain);
            Context = new(Device);


            uniform = new(Camera.Projection, Model, Camera.View);



            CreateBuffers();
            CreatePipelineState();

        }




        public void CreateBuffers()
        {
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
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            });

            ConstBuffer2 = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            });
        }


        public void CreatePipelineState()
        {
            var file = Constants.ShadersFile;

            ShaderBytecode Fragment = ShaderBytecode.LoadFromFile(file + "Transformations/Fragment.hlsl", ShaderStage.Fragment);
            ShaderBytecode Vertex = ShaderBytecode.LoadFromFile(file + "Transformations/Vertex.hlsl", ShaderStage.Vertex);



            GraphicsPipelineDescription Pipelinedescription0 = new();
            Pipelinedescription0.SetFramebuffer(Framebuffer);
            Pipelinedescription0.SetShader(Fragment);
            Pipelinedescription0.SetShader(Vertex);
            Pipelinedescription0.SetVertexBinding(VertexInputRate.Vertex, VertexPositionColor.Size);
            Pipelinedescription0.AddVertexAttribute<VertexPositionColor>();
            PipelineState_0 = new(Pipelinedescription0);

            DescriptorData descriptorData_0 = new();
            descriptorData_0.SetUniformBuffer(0, ConstBuffer);
            DescriptorSet_0 = new(PipelineState_0, descriptorData_0);




            GraphicsPipelineDescription Pipelinedescription1 = new();
            Pipelinedescription1.SetFramebuffer(Framebuffer);
            Pipelinedescription1.SetShader(Fragment);
            Pipelinedescription1.SetShader(Vertex);
            Pipelinedescription1.SetVertexBinding(VertexInputRate.Vertex, VertexPositionColor.Size);
            Pipelinedescription1.AddVertexAttribute<VertexPositionColor>();
            PipelineState_1 = new(Pipelinedescription1);


            DescriptorData descriptorData_1 = new();
            descriptorData_1.SetUniformBuffer(0, ConstBuffer2);
            DescriptorSet_1 = new(PipelineState_1, descriptorData_1);

        }




        public override void Update(ApplicationTime time)
        {
            Camera.Update();

            float rotation = time.TotalMilliseconds / 800;

            Model = Matrix4x4.CreateFromYawPitchRoll(-rotation, -rotation, -rotation) * Matrix4x4.CreateTranslation(-0.45f, 0.0f, 0.0f);
            uniform.Update(Camera, Model);
            ConstBuffer.SetData(ref uniform);


            Model = Matrix4x4.CreateFromYawPitchRoll(rotation, rotation, rotation) * Matrix4x4.CreateTranslation(0.45f, 0.0f, 0.0f);
            uniform.Update(Camera, Model);
            ConstBuffer2.SetData(ref uniform);

        }


        public override void Draw(ApplicationTime time)
        {

            Device.WaitIdle();
            CommandBuffer commandBuffer = Context.CommandBuffer;

            commandBuffer.Begin();
            commandBuffer.BeginFramebuffer(Framebuffer, .0f, .2f, .4f);
            commandBuffer.SetViewport(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);
            commandBuffer.SetScissor(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);

            commandBuffer.SetVertexBuffers(new Buffer[] { VertexBuffer });
            commandBuffer.SetIndexBuffer(IndexBuffer);


            // Draw Cube 1
            commandBuffer.SetGraphicPipeline(PipelineState_0);
            commandBuffer.BindDescriptorSets(DescriptorSet_0);
            commandBuffer.DrawIndexed(indices.Length, 1, 0, 0, 0);

            // Draw Cube 2
            commandBuffer.SetGraphicPipeline(PipelineState_1);
            commandBuffer.BindDescriptorSets(DescriptorSet_1);
            commandBuffer.DrawIndexed(indices.Length, 1, 0, 0, 0);


            commandBuffer.Close();
            Device.Submit(commandBuffer);
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
    public struct TransformUniform
    {
        public TransformUniform(Matrix4x4 m, Matrix4x4 v, Matrix4x4 p)
        {
            P = p;
            M = m;
            V = v;
        }

        public Matrix4x4 M;

        public Matrix4x4 V;

        public Matrix4x4 P;


        public void Update(Camera camera, Matrix4x4 m)
        {
            P = camera.Projection;
            M = m;
            V = camera.View;
        }
    }
}
