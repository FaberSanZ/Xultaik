using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Zeckoxe.Core;
using Zeckoxe.Desktop;
using Zeckoxe.Engine;
using Zeckoxe.Vulkan;
using Zeckoxe.Physics;
using Buffer = Zeckoxe.Vulkan.Buffer;
using Vortice.Vulkan;
using Interop = Zeckoxe.Core.Interop;
using Samples.Common;

namespace Samples.Triangle
{
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

    public class TriangleExample : Application, IDisposable
    {

        public TriangleExample() : base()
        {

        }

        public Camera Camera { get; set; }

        public GraphicsPipeline PipelineState { get; set; }
        public DescriptorSet DescriptorSet { get; set; }

        public Buffer VertexBuffer { get; set; }
        public Buffer IndexBuffer { get; set; }
        public Buffer ConstBuffer { get; set; }

        public TransformUniform Uniform;



        public override void InitializeSettings()
        {
            base.InitializeSettings();
            Parameters.Settings.Validation = ValidationType.None;
            Window.Title += " - (Triangle) ";
        }

        public override void Initialize()
        {
            base.Initialize();

            Camera = new(45f, 1f, 0.1f, 64f);
            Camera.SetPosition(0, 0, -3.0f);
            Camera.AspectRatio = (float)Window.Width / Window.Height;
            Camera.Update(); 



            // Reset Model
            Model = Matrix4x4.Identity;
            Models.Add(Model);


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
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            });


        }


        public void CreatePipelineState()
        {

            var file = Constants.ShadersFile;

            PipelineStateDescription Pipelinedescription = new();
            Pipelinedescription.SetFramebuffer(Framebuffer);
            Pipelinedescription.SetShader(new ShaderBytecode(file + "PositionColor/Fragment.hlsl", ShaderStage.Fragment, ShaderBackend.Hlsl));
            Pipelinedescription.SetShader(new ShaderBytecode(file + "PositionColor/Vertex.hlsl", ShaderStage.Vertex, ShaderBackend.Hlsl));
            Pipelinedescription.SetVertexBinding(VkVertexInputRate.Vertex, VertexPositionColor.Size);
            Pipelinedescription.SetVertexAttribute(VertexType.Position);
            Pipelinedescription.SetVertexAttribute(VertexType.Color);
            Pipelinedescription.SetFillMode(VkPolygonMode.Fill);
            Pipelinedescription.SetCullMode(VkCullModeFlags.None);
            Pipelinedescription.SetPrimitiveType(VkPrimitiveTopology.TriangleList);

            PipelineState = new(Pipelinedescription);


            DescriptorData descriptorData = new();
            descriptorData.SetUniformBuffer(0, ConstBuffer);

            DescriptorSet = new(PipelineState, descriptorData);
        }

        public override void Update(ApplicationTime game)
        {
            Camera.Update();


            Uniform.Update(Camera, Model);

            ConstBuffer.SetData(ref Uniform);
        }


        public override void BeginDraw()
        {
            base.BeginDraw();

            CommandBuffer commandBuffer = Context.CommandBuffer;


            commandBuffer.BeginFramebuffer(Framebuffer);

            commandBuffer.SetViewport(Window.Width, Window.Height, 0, 0);
            commandBuffer.SetScissor(Window.Width, Window.Height, 0, 0);

            commandBuffer.SetGraphicPipeline(PipelineState);
            commandBuffer.BindDescriptorSets(DescriptorSet);
            commandBuffer.SetVertexBuffer(VertexBuffer);
            commandBuffer.SetIndexBuffer(IndexBuffer);
            commandBuffer.DrawIndexed(3, 1, 0, 0, 0);
        }



        public void Dispose()
        {
            Adapter.Dispose();
        }
    }
}
