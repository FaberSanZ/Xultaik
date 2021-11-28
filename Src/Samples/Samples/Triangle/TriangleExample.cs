using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Vultaik;
using Vultaik.Desktop;
using Vultaik.Physics;
using Buffer = Vultaik.Buffer;
using Vortice.Vulkan;
using Interop = Vultaik.Interop;
using Samples.Common;
using Vultaik.Toolkit;

namespace Samples.Triangle
{
    public class TriangleExample : ExampleBase, IDisposable
    {

        private AdapterConfig AdapterConfig;
        private Adapter Adapter;
        private Device Device;
        private Framebuffer Framebuffer;
        private SwapChain SwapChain;
        private GraphicsContext Context;
        private GraphicsPipeline PipelineState;
        private DescriptorSet DescriptorSet;
        private Buffer VertexBuffer;
        private Buffer IndexBuffer;
        private Buffer ConstBuffer;

        private TransformUniform Uniform;

        public TriangleExample() : base()
        {

        }

        public override void Initialize()
        {

            AdapterConfig = new()
            {
                SwapChain = true,
                Debug = true,
            };


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

            Context = new GraphicsContext(Device);
            Framebuffer = new Framebuffer(SwapChain);

            Camera = new(45f, 1f, 0.1f, 64f);
            Camera.SetPosition(0, 0, -3.0f);
            Camera.AspectRatio = (float)Window.Width / Window.Height;
            Camera.Update();


            // Reset Model
            Model = Matrix4x4.Identity;


            Uniform = new(Camera.Projection, Model, Camera.View);

            CreateBuffers();

            CreatePipelineState();


        }


        public void CreateBuffers()
        {


            VertexPositionColor[] vertices = new[]
            {
                new VertexPositionColor(new Vector3(0.0f, -0.65f, -0.5f), new Vector3(1.6f, 0.0f, 0.0f)),
                new VertexPositionColor(new Vector3(0.65f, 0.65f, -0.5f), new Vector3(0.0f, 1.6f, 0.0f)),
                new VertexPositionColor(new Vector3(-0.65f, 0.65f, -0.5f), new Vector3(0.0f, 0.0f, 1.6f)),
            };


            int[] indices = new[]
            {
                0, 1, 2
            };


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


        }


        public void CreatePipelineState()
        {

            GraphicsPipelineDescription Pipelinedescription = new()
            {
                Shaders =
                {
                    new(Constants.ShadersFile + "Triangle/Fragment.hlsl", ShaderStage.Fragment),
                    new(Constants.ShadersFile + "Triangle/Vertex.hlsl", ShaderStage.Vertex),
                },

                Framebuffer = Framebuffer,

                PipelineVertexInput = 
                {
                    VertexBindingDescriptions =
                    {
                        new(0, VertexPositionColor.Size, VertexInputRate.Vertex),
                    },
                },
                InputAssemblyState =
                {
                    PrimitiveType = PrimitiveType.TriangleList
                },

            };
            Pipelinedescription.AddVertexAttribute<VertexPositionColor>();

            PipelineState = new(Pipelinedescription);


            DescriptorData descriptorData = new();
            descriptorData.SetUniformBuffer(0, ConstBuffer);

            DescriptorSet = new(PipelineState, descriptorData);
        }

        public override void Update(ApplicationTime time)
        {

            Camera.Update();

            Uniform.Update(Camera, Model);

            ConstBuffer.SetData(ref Uniform);



        }


        public override void Draw(ApplicationTime time)
        {
            Device.WaitIdle();
            CommandBuffer commandBuffer = Context.CommandBuffer;
            commandBuffer.Begin();


            commandBuffer.BeginFramebuffer(Framebuffer);
            commandBuffer.SetViewport(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);
            commandBuffer.SetScissor(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);

            commandBuffer.SetGraphicPipeline(PipelineState);
            commandBuffer.BindDescriptorSets(DescriptorSet);
            commandBuffer.SetVertexBuffer(VertexBuffer);
            commandBuffer.SetIndexBuffer(IndexBuffer);
            commandBuffer.DrawIndexed(3, 1, 0, 0, 0);

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
            ConstBuffer.Dispose();
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
            PipelineState.Dispose();
            DescriptorSet.Dispose();
            Framebuffer.Dispose();
            Device.GraphicsCommandBuffer.Dispose();
            SwapChain.Dispose();
            Device.Dispose();
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
