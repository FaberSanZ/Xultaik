using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Vortice.Vulkan;
using Vultaik.Desktop;
using Vultaik.Physics;
using Vultaik.Engine;
using Vultaik.GLTF;
using Vultaik;
using Buffer = Vultaik.Buffer;
using Interop = Vultaik.Interop;
using Samples.Common;

namespace Samples.DiffuseLighting
{
    public class DiffuseLightingExample : Application, IDisposable
    {
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




        public DiffuseLightingExample() : base()
        {

        }


        public TransformUniform uniform;
        public Light light;

        public float yaw;
        public float pitch;
        public float roll;


        public override void InitializeSettings()
        {
            base.InitializeSettings();
            Parameters.Settings.Validation = ValidationType.None;
            Window.Title += " - (Diffuse Lighting) ";
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
            light = new(new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f));


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

            GLTFModel = new(Device, Constants.ModelsFile + "mesh_mat.gltf");

            yaw = 0f;
            pitch = 0;
            roll = 0;
        }








        public void CreatePipelineState()
        {

            string shaders = Constants.ShadersFile;
            string images = Constants.ImagesFile;

            string Fragment = shaders + "Lighting/Fragment.hlsl";
            string Vertex = shaders + "Lighting/Vertex.hlsl";



            Image text1 = ImageFile.Load2DFromFile(Device, images + "UVCheckerMap08-512.png");
            Image text2 = ImageFile.Load2DFromFile(Device, images + "UVCheckerMap02-512.png");
            Image text3 = ImageFile.Load2DFromFile(Device, images + "UVCheckerMap09-512.png");

            Sampler sampler = new Sampler(Device);


            PipelineStateDescription Pipelinedescription0 = new();
            Pipelinedescription0.SetFramebuffer(Framebuffer);
            Pipelinedescription0.SetShader(new ShaderBytecode(Fragment, ShaderStage.Fragment, ShaderBackend.Hlsl));
            Pipelinedescription0.SetShader(new ShaderBytecode(Vertex, ShaderStage.Vertex, ShaderBackend.Hlsl));
            Pipelinedescription0.SetVertexBinding(VkVertexInputRate.Vertex, VertexPositionNormalTexture.Size);
            Pipelinedescription0.SetVertexAttribute(VertexType.Position);
            Pipelinedescription0.SetVertexAttribute(VertexType.TextureCoordinate);
            Pipelinedescription0.SetVertexAttribute(VertexType.Normal);
            PipelineState_0 = new(Pipelinedescription0);

            DescriptorData descriptorData_0 = new();
            descriptorData_0.SetUniformBuffer(0, ConstBuffer);
            descriptorData_0.SetImage(1, text1);
            descriptorData_0.SetSampler(2, sampler);
            descriptorData_0.SetUniformBuffer(3, ConstBuffer4);
            DescriptorSet_0 = new(PipelineState_0, descriptorData_0);
             




            PipelineStateDescription Pipelinedescription1 = new();
            Pipelinedescription1.SetFramebuffer(Framebuffer);
            Pipelinedescription1.SetShader(new ShaderBytecode(Fragment, ShaderStage.Fragment, ShaderBackend.Hlsl));
            Pipelinedescription1.SetShader(new ShaderBytecode(Vertex, ShaderStage.Vertex, ShaderBackend.Hlsl)); 
            Pipelinedescription1.SetVertexBinding(VkVertexInputRate.Vertex, VertexPositionNormalTexture.Size);
            Pipelinedescription1.SetVertexAttribute(VertexType.Position);
            Pipelinedescription1.SetVertexAttribute(VertexType.TextureCoordinate);
            Pipelinedescription1.SetVertexAttribute(VertexType.Normal);
            PipelineState_1 = new(Pipelinedescription1);

            DescriptorData descriptorData_1 = new();
            descriptorData_1.SetUniformBuffer(0, ConstBuffer2);
            descriptorData_1.SetImage(1, text2);
            descriptorData_1.SetSampler(2, sampler);
            descriptorData_1.SetUniformBuffer(3, ConstBuffer4);
            DescriptorSet_1 = new(PipelineState_1, descriptorData_1);




            PipelineStateDescription Pipelinedescription2 = new();
            Pipelinedescription2.SetFramebuffer(Framebuffer);
            Pipelinedescription2.SetShader(new ShaderBytecode(Fragment, ShaderStage.Fragment, ShaderBackend.Hlsl));
            Pipelinedescription2.SetShader(new ShaderBytecode(Vertex, ShaderStage.Vertex, ShaderBackend.Hlsl)); 
            Pipelinedescription2.SetVertexBinding(VkVertexInputRate.Vertex, VertexPositionNormalTexture.Size);
            Pipelinedescription2.SetVertexAttribute(VertexType.Position);
            Pipelinedescription2.SetVertexAttribute(VertexType.TextureCoordinate);
            Pipelinedescription2.SetVertexAttribute(VertexType.Normal); 
            PipelineState_2 = new(Pipelinedescription2);

            DescriptorData descriptorData_2 = new();
            descriptorData_2.SetUniformBuffer(0, ConstBuffer3);
            descriptorData_2.SetImage(1, text3);
            descriptorData_2.SetSampler(2, sampler);
            descriptorData_2.SetUniformBuffer(3, ConstBuffer4);
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
    
        public Vector4 Diffuse;

        public Vector3 LightDirection;

        public float Padding;

        public Light(Vector4 D, Vector3 L)
        {
            Diffuse = D;
            LightDirection = L;
            Padding = 0;
        }
    }
}
