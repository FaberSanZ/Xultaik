using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Vortice.Vulkan;
using Vultaik.Desktop;
using Vultaik.Physics;
using Vultaik.GLTF;
using Vultaik;
using Buffer = Vultaik.Buffer;
using Interop = Vultaik.Interop;
using Samples.Common;
using Vultaik.Toolkit;

namespace Samples.DiffuseLighting
{
    public class DiffuseLightingExample : ExampleBase, IDisposable
    {

        private ModelAssetImporter<VertexPositionNormalTexture> GLTFModel;
        private AdapterConfig AdapterConfig;
        private Adapter Adapter;
        private Device Device;
        private Framebuffer Framebuffer;
        private SwapChain SwapChain;
        private GraphicsContext Context;
        private DescriptorSet DescriptorSet_0;
        private DescriptorSet DescriptorSet_1;
        private DescriptorSet DescriptorSet_2;
        private Buffer ConstBuffer;
        private Buffer ConstBuffer2;
        private Buffer ConstBuffer3;
        private Buffer ConstBuffer4;
        private GraphicsPipeline PipelineState_0;
        private GraphicsPipeline PipelineState_1;
        private GraphicsPipeline PipelineState_2;

        private TransformUniform uniform;
        private Light light;
        private float yaw, pitch, roll = 0;


        public DiffuseLightingExample() : base()
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


            Camera.SetPosition(0, -8, -40.0f);
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

            Context = new(Device);
            Framebuffer = new(SwapChain);

            uniform = new(Camera.Projection, Model, Camera.View);
            light = new(new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector3(0.0f, 0.0f, 0.0f));


            BufferDescription bufferDescription = new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            };

            ConstBuffer = new(Device, bufferDescription);
            ConstBuffer2 = new(Device, bufferDescription);
            ConstBuffer3 = new(Device, bufferDescription);

            ConstBuffer4 = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = Interop.SizeOf<Light>(),
            });



            CreatePipelineState();

            GLTFModel = new(Device, Constants.ModelsFile + "mesh_mat.gltf");
            
        }








        public void CreatePipelineState()
        {

            string images = Constants.ImagesFile;
            string fragment = Constants.ShadersFile + @"DiffuseLighting\Fragment.hlsl";
            string vertex = Constants.ShadersFile + @"DiffuseLighting\Vertex.hlsl";

            Image text1 = ImageFile.Load2DFromFile(Device, images + "UVCheckerMap08-512.png");
            Image text2 = ImageFile.Load2DFromFile(Device, images + "UVCheckerMap02-512.png");
            Image text3 = ImageFile.Load2DFromFile(Device, images + "UVCheckerMap09-512.png");

            Sampler sampler = new Sampler(Device);


            GraphicsPipelineDescription Pipelinedescription0 = new();
            Pipelinedescription0.SetFramebuffer(Framebuffer);
            Pipelinedescription0.SetShader(new ShaderBytecode(fragment, ShaderStage.Fragment));
            Pipelinedescription0.SetShader(new ShaderBytecode(vertex, ShaderStage.Vertex));
            Pipelinedescription0.SetVertexBinding(VertexInputRate.Vertex, VertexPositionNormalTexture.Size);
            Pipelinedescription0.SetVertexAttribute(VertexType.Position);
            Pipelinedescription0.SetVertexAttribute(VertexType.Normal);
            Pipelinedescription0.SetVertexAttribute(VertexType.TextureCoordinate);
            PipelineState_0 = new(Pipelinedescription0);

            DescriptorData descriptorData_0 = new();
            descriptorData_0.SetUniformBuffer(0, ConstBuffer);
            descriptorData_0.SetImage(1, text1);
            descriptorData_0.SetSampler(2, sampler);
            descriptorData_0.SetUniformBuffer(3, ConstBuffer4);
            DescriptorSet_0 = new(PipelineState_0, descriptorData_0);





            GraphicsPipelineDescription Pipelinedescription1 = new();
            Pipelinedescription1.SetFramebuffer(Framebuffer);
            Pipelinedescription1.SetShader(new ShaderBytecode(fragment, ShaderStage.Fragment));
            Pipelinedescription1.SetShader(new ShaderBytecode(vertex, ShaderStage.Vertex)); 
            Pipelinedescription1.SetVertexBinding(VertexInputRate.Vertex, VertexPositionNormalTexture.Size);
            Pipelinedescription1.SetVertexAttribute(VertexType.Position);
            Pipelinedescription1.SetVertexAttribute(VertexType.Normal);
            Pipelinedescription1.SetVertexAttribute(VertexType.TextureCoordinate);
            PipelineState_1 = new(Pipelinedescription1);

            DescriptorData descriptorData_1 = new();
            descriptorData_1.SetUniformBuffer(0, ConstBuffer2);
            descriptorData_1.SetImage(1, text2);
            descriptorData_1.SetSampler(2, sampler);
            descriptorData_1.SetUniformBuffer(3, ConstBuffer4);
            DescriptorSet_1 = new(PipelineState_1, descriptorData_1);




            GraphicsPipelineDescription Pipelinedescription2 = new();
            Pipelinedescription2.SetFramebuffer(Framebuffer);
            Pipelinedescription2.SetShader(new ShaderBytecode(fragment, ShaderStage.Fragment));
            Pipelinedescription2.SetShader(new ShaderBytecode(vertex, ShaderStage.Vertex)); 
            Pipelinedescription2.SetVertexBinding(VertexInputRate.Vertex, VertexPositionNormalTexture.Size);
            Pipelinedescription2.SetVertexAttribute(VertexType.Position);
            Pipelinedescription2.SetVertexAttribute(VertexType.Normal); 
            Pipelinedescription2.SetVertexAttribute(VertexType.TextureCoordinate);
            PipelineState_2 = new(Pipelinedescription2);

            DescriptorData descriptorData_2 = new();
            descriptorData_2.SetUniformBuffer(0, ConstBuffer3);
            descriptorData_2.SetImage(1, text3);
            descriptorData_2.SetSampler(2, sampler);
            descriptorData_2.SetUniformBuffer(3, ConstBuffer4);
            DescriptorSet_2 = new(PipelineState_2, descriptorData_2);
        }

        public override void Update(ApplicationTime time)
        {
            var timer = time.TotalMilliseconds / (3600);

            Camera.Update();
            //light.LightDirection.X = -14.0f + MathF.Abs(MathF.Sin(MathUtil.Radians(timer * 360.0f)) * 2.0f);
            light.LightDirection.X = 0.0f + MathF.Sin(MathUtil.Radians(timer * 360.0f)) * MathF.Cos(MathUtil.Radians(timer * 360.0f)) * 2.0f;
            light.LightDirection.Y = 0.0f + MathF.Sin(MathUtil.Radians(timer * 360.0f)) * 2.0f;
            light.LightDirection.Z = 0.0f + MathF.Cos(MathUtil.Radians(timer * 360.0f)) * 2.0f;




            ConstBuffer4.SetData(ref light);


            Model = Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix4x4.CreateTranslation(11.0f, .3f, 0.0f);
            uniform.Update(Camera, Model);
            ConstBuffer.SetData(ref uniform);

            Model = Matrix4x4.CreateFromYawPitchRoll(-yaw, pitch, roll) * Matrix4x4.CreateTranslation(0, 1.0f, 0.0f);
            uniform.Update(Camera, Model);
            ConstBuffer2.SetData(ref uniform);

            Model = Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix4x4.CreateTranslation(-11.0f, .3f, 0.0f);
            uniform.Update(Camera, Model);
            ConstBuffer3.SetData(ref uniform);

            if (Input.Keyboards[0].IsKeyPressed(Key.T))
            {
                if (light.IsTexture == 1)
                    light.IsTexture = 0;

                else if (light.IsTexture == 0)
                    light.IsTexture = 1;
            }
        }
   

        public override void Draw(ApplicationTime time)
        {

            Device.WaitIdle();
            CommandBuffer cmd = Context.CommandBuffer;

            cmd.Begin();
            cmd.BeginFramebuffer(Framebuffer);
            cmd.SetScissor(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);
            cmd.SetViewport(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);


            cmd.BindDescriptorSets(DescriptorSet_0);
            cmd.SetGraphicPipeline(PipelineState_0);
            GLTFModel.Draw(cmd, PipelineState_0);


            cmd.BindDescriptorSets(DescriptorSet_1);
            cmd.SetGraphicPipeline(PipelineState_1);
            GLTFModel.Draw(cmd, PipelineState_1);


            cmd.BindDescriptorSets(DescriptorSet_2);
            cmd.SetGraphicPipeline(PipelineState_2);
            GLTFModel.Draw(cmd, PipelineState_2);


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
            //ConstBuffer.Dispose();
            //ConstBuffer2.Dispose();
            //ConstBuffer3.Dispose();
            //ConstBuffer4.Dispose();
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

        public float IsTexture;

        public Light(Vector4 D, Vector3 L)
        {
            Diffuse = D;
            LightDirection = L;
            IsTexture = 1;
        }
    }
}
