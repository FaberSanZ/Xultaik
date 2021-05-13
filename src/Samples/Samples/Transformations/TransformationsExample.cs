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

namespace Samples.Transformations
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

    public class TransformationsExample : Application, IDisposable
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





        public GraphicsPipeline PipelineState_0 { get; set; }
        public GraphicsPipeline PipelineState_1 { get; set; }

        public DescriptorSet DescriptorSet_0 { get; set; }
        public DescriptorSet DescriptorSet_1 { get; set; }


        public Buffer VertexBuffer { get; set; }
        public Buffer IndexBuffer { get; set; }
        public Buffer ConstBuffer { get; set; }
        public Buffer ConstBuffer2 { get; set; }
        public Camera Camera { get; set; }
        public ApplicationTime GameTime { get; set; }



        public TransformationsExample() : base()
        {

        }


        // Transform Uniform 
        public TransformUniform uniform;
        public float yaw;
        public float pitch;
        public float roll;

        public override void InitializeSettings()
        {
            base.InitializeSettings();
            Parameters.Settings.Validation = ValidationType.None;
            Window.Title += " - (Transformations) ";
        }

        public override void Initialize()
        {
            base.Initialize();


            Camera = new(45f, 1f, 0.1f, 64f);
            Camera.SetPosition(0, 0, -3.5f);
            Camera.AspectRatio = (float)Window.Width / Window.Height;
            Camera.Update();


            // Reset Model
            Model = Matrix4x4.Identity;
            Models.Add(Model);


            uniform = new(Camera.Projection, Model, Camera.View);



            CreateBuffers();

            CreatePipelineState();


            yaw = 0;
            pitch = 0;
            roll = 0;
        }




        public void CreateBuffers()
        {
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

            ConstBuffer2 = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            });
        }


        public void CreatePipelineState()
        {
            var file = Constants.ShadersFile;

            ShaderBytecode Fragment = ShaderBytecode.LoadFromFile(file + "Transformations/shader.frag", ShaderStage.Fragment);
            ShaderBytecode Vertex = ShaderBytecode.LoadFromFile(file + "Transformations/shader.vert", ShaderStage.Vertex);



            PipelineStateDescription Pipelinedescription0 = new();
            Pipelinedescription0.SetFramebuffer(Framebuffer);
            Pipelinedescription0.SetShader(Fragment);
            Pipelinedescription0.SetShader(Vertex);
            Pipelinedescription0.SetVertexBinding(VkVertexInputRate.Vertex, VertexPositionColor.Size);
            Pipelinedescription0.AddVertexAttribute<VertexPositionColor>();
            PipelineState_0 = new(Pipelinedescription0);

            DescriptorData descriptorData_0 = new();
            descriptorData_0.SetUniformBuffer(0, ConstBuffer);
            DescriptorSet_0 = new(PipelineState_0, descriptorData_0);




            PipelineStateDescription Pipelinedescription1 = new();
            Pipelinedescription1.SetFramebuffer(Framebuffer);
            Pipelinedescription1.SetShader(Fragment);
            Pipelinedescription1.SetShader(Vertex);
            Pipelinedescription1.SetVertexBinding(VkVertexInputRate.Vertex, VertexPositionColor.Size);
            Pipelinedescription1.AddVertexAttribute<VertexPositionColor>();
            PipelineState_1 = new(Pipelinedescription1);


            DescriptorData descriptorData_1 = new();
            descriptorData_1.SetUniformBuffer(0, ConstBuffer2);
            DescriptorSet_1 = new(PipelineState_1, descriptorData_1);
        }



        public override void Update(ApplicationTime game)
        {
            Camera.Update();


            Model = Matrix4x4.CreateFromYawPitchRoll(-yaw, -pitch, -roll) * Matrix4x4.CreateTranslation(-0.45f, 0.0f, 0.0f);
            uniform.Update(Camera, Model);
            ConstBuffer.SetData(ref uniform);


            Model = Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix4x4.CreateTranslation(0.45f, 0.0f, 0.0f);
            uniform.Update(Camera, Model);
            ConstBuffer2.SetData(ref uniform);


            yaw += 0.0005f * MathF.PI;
            pitch += 0.0005f * MathF.PI;
            roll += 0.0005f * MathF.PI;
        }


        public override void BeginDraw()
        {
            base.BeginDraw();

            CommandBuffer commandBuffer = Context.CommandBuffer;

            commandBuffer.BeginFramebuffer(Framebuffer, .0f, .2f, .4f);
            commandBuffer.SetViewport(Window.Width, Window.Height, 0, 0);
            commandBuffer.SetScissor(Window.Width, Window.Height, 0, 0);

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
        }


        public void Dispose()
        {
            Adapter.Dispose();
        }
    }
}
