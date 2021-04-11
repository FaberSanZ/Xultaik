using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Vortice.Vulkan;
using Zeckoxe.Core;
using Zeckoxe.Desktop;
using Zeckoxe.Physics;
using Zeckoxe.Engine;
using Zeckoxe.GLTF;
using Zeckoxe.Vulkan;
using Zeckoxe.Vulkan.Toolkit;
using Buffer = Zeckoxe.Vulkan.Buffer;
using Interop = Zeckoxe.Core.Interop;

namespace Samples.Samples
{
    public class Lighting : Application, IDisposable
    {


        [StructLayout(LayoutKind.Sequential)]
        public struct TransformUniform
        {
            public TransformUniform(Matrix4x4 p, Matrix4x4 m, Matrix4x4 v)
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


        [StructLayout(LayoutKind.Sequential)]
        public struct Light
        {
            public Vector3 Pos;
            public Vector3 ViewPos;
            public Vector3 Color;
            public Vector3 Pad;

            public Light(Vector3 p, Vector3 v, Vector3 c)
            {
                Pos = p;
                ViewPos = v;
                Color = c;
                Pad = Vector3.One;
            }
        }


        public Lighting() : base()
        {

        }




        public Camera camera { get; set; }
        public ApplicationTime GameTime { get; set; }

        public ModelAssetImporter<VertexPositionNormalTexture> GLTFModel { get; set; }

        public DescriptorSet DescriptorSet_0 { get; set; }
        public DescriptorSet DescriptorSet_1 { get; set; }
        public DescriptorSet DescriptorSet_2 { get; set; }

        public Buffer ConstBuffer;
        public Buffer ConstBuffer2;
        public Buffer ConstBuffer3;
        public Buffer ConstBuffer4;
        public GraphicsPipeline PipelineState_0;
        public GraphicsPipeline PipelineState_1;
        public GraphicsPipeline PipelineState_2;
        public Dictionary<string, ShaderBytecode> Shaders = new();

        // TransformUniform 
        public TransformUniform uniform;
        public Light light;

        public float yaw;
        public float pitch;
        public float roll;


        public override void InitializeSettings()
        {
            base.InitializeSettings();
            Parameters.Settings.Validation = ValidationType.Info;
            Window.Title += " - (Lighting) ";
        }





        public override void Initialize()
        {
            base.Initialize();

            camera = new Camera(45f, 1f, 0.1f, 64f);
            camera.SetRotation(0, 0, 0);
            camera.SetPosition(0, -8, -40.0f);
            camera.AspectRatio = (float)Window.Width / Window.Height;


            // Reset Model
            Model = Matrix4x4.Identity;

            uniform = new(camera.Projection, Model, camera.View);
            light = new(new(1.2f, 1.0f, 1.0f), camera.Position, new(1, 1, 1));


            BufferDescription bufferDescription = new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            };

            ConstBuffer = new(Device, bufferDescription);
            ConstBuffer2 = new(Device, bufferDescription);
            ConstBuffer3 = new(Device, bufferDescription);

            ConstBuffer4 = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<Light>(),
            });



            CreatePipelineState();


            GLTFModel = new(Device, "Models/mesh_mat.gltf");

            yaw = 0f;
            pitch = 0;
            roll = 0;
        }








        public void CreatePipelineState()
        {
            Shaders["Fragment"] = ShaderBytecode.LoadFromFile("Shaders/Lighting/shader.frag", ShaderStage.Fragment);
            Shaders["Vertex"] = ShaderBytecode.LoadFromFile("Shaders/Lighting/shader.vert", ShaderStage.Vertex);

            Image2D text1 = Image2D.LoadFromFile(Device, "UVCheckerMap08-512.png");
            Image2D text2 = Image2D.LoadFromFile(Device, "IndustryForgedDark512.ktx");
            Image2D text3 = Image2D.LoadFromFile(Device, "floor_tiles.bmp");

            Sampler sampler = new Sampler(Device);


            PipelineStateDescription Pipelinedescription0 = new();
            Pipelinedescription0.SetFramebuffer(Framebuffer);
            Pipelinedescription0.SetShader(Shaders["Vertex"]);
            Pipelinedescription0.SetShader(Shaders["Fragment"]);
            Pipelinedescription0.SetVertexBinding(VkVertexInputRate.Vertex, VertexPositionNormalTexture.Size);
            Pipelinedescription0.SetVertexAttribute(VertexType.Position);
            Pipelinedescription0.SetVertexAttribute(VertexType.TextureCoordinate);
            Pipelinedescription0.SetVertexAttribute(VertexType.Normal);
            PipelineState_0 = new(Pipelinedescription0);

            DescriptorData descriptorData_0 = new();
            descriptorData_0.SetUniformBuffer(0, ConstBuffer);
            descriptorData_0.SetImageSampler(1, text1, sampler);
            descriptorData_0.SetUniformBuffer(2, ConstBuffer4);
            DescriptorSet_0 = new(PipelineState_0, descriptorData_0);
             




            PipelineStateDescription Pipelinedescription1 = new();
            Pipelinedescription1.SetFramebuffer(Framebuffer);
            Pipelinedescription1.SetShader(Shaders["Fragment"]);
            Pipelinedescription1.SetShader(Shaders["Vertex"]);
            Pipelinedescription1.SetVertexBinding(VkVertexInputRate.Vertex, VertexPositionNormalTexture.Size);
            Pipelinedescription1.SetVertexAttribute(VertexType.Position);
            Pipelinedescription1.SetVertexAttribute(VertexType.TextureCoordinate);
            Pipelinedescription1.SetVertexAttribute(VertexType.Normal);
            PipelineState_1 = new(Pipelinedescription1);

            DescriptorData descriptorData_1 = new();
            descriptorData_1.SetUniformBuffer(0, ConstBuffer2);
            descriptorData_1.SetImageSampler(1, text2, sampler);
            descriptorData_1.SetUniformBuffer(2, ConstBuffer4);
            DescriptorSet_1 = new(PipelineState_1, descriptorData_1);




            PipelineStateDescription Pipelinedescription2 = new();
            Pipelinedescription2.SetFramebuffer(Framebuffer);
            Pipelinedescription2.SetShader(Shaders["Fragment"]);
            Pipelinedescription2.SetShader(Shaders["Vertex"]);
            Pipelinedescription2.SetVertexBinding(VkVertexInputRate.Vertex, VertexPositionNormalTexture.Size);
            Pipelinedescription2.SetVertexAttribute(VertexType.Position);
            Pipelinedescription2.SetVertexAttribute(VertexType.TextureCoordinate);
            Pipelinedescription2.SetVertexAttribute(VertexType.Normal); 
            PipelineState_2 = new(Pipelinedescription2);

            DescriptorData descriptorData_2 = new();
            descriptorData_2.SetUniformBuffer(0, ConstBuffer3);
            descriptorData_2.SetImageSampler(1, text3, sampler);
            descriptorData_2.SetUniformBuffer(2, ConstBuffer4);
            DescriptorSet_2 = new(PipelineState_2, descriptorData_2);
        }



        public override void Update(ApplicationTime game)
        {

            ConstBuffer4.SetData(ref light);


            Model = Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix4x4.CreateTranslation(11.0f, .3f, 0.0f);
            uniform.Update(camera, Model);
            ConstBuffer.SetData(ref uniform);

            Model = Matrix4x4.CreateFromYawPitchRoll(-yaw, pitch, roll) * Matrix4x4.CreateTranslation(0, 1.0f, 0.0f);
            uniform.Update(camera, Model);
            ConstBuffer2.SetData(ref uniform);

            Model = Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix4x4.CreateTranslation(-11.0f, .3f, 0.0f);
            uniform.Update(camera, Model);
            ConstBuffer3.SetData(ref uniform);


            yaw += 0.0006f * MathF.PI;

        }


        public override void BeginDraw()
        {
            base.BeginDraw();


            CommandBuffer cmd = Context.CommandBuffer;
            cmd.BeginFramebuffer(Framebuffer);
            cmd.SetScissor(Window.Width, Window.Height, 0, 0);
            cmd.SetViewport(Window.Width, Window.Height, 0, 0);


            cmd.BindDescriptorSets(DescriptorSet_0);
            cmd.SetGraphicPipeline(PipelineState_0);
            GLTFModel.Draw(cmd, PipelineState_0);


            cmd.BindDescriptorSets(DescriptorSet_1);
            cmd.SetGraphicPipeline(PipelineState_1);
            GLTFModel.Draw(cmd, PipelineState_1);


            cmd.BindDescriptorSets(DescriptorSet_2);
            cmd.SetGraphicPipeline(PipelineState_2);
            GLTFModel.Draw(cmd, PipelineState_2);
        }


        public override void Destroy()
        {
            ConstBuffer.Dispose();
            ConstBuffer2.Dispose();
            ConstBuffer3.Dispose();
            ConstBuffer4.Dispose();
            base.Destroy();
        }


        public void Dispose()
        {
            Destroy();
        }
    }
}
