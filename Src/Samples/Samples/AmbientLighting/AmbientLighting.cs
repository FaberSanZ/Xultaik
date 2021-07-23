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

namespace Samples.AmbientLighting
{
    public class AmbientLighting : ExampleBase, IDisposable
    {

        private ModelAssetImporter<VertexPositionNormalTexture> GLTFModel;
        private AdapterConfig AdapterConfig;
        private Adapter Adapter;
        private Device Device;
        private Framebuffer Framebuffer;
        private SwapChain SwapChain;
        private GraphicsContext Context;
        private DescriptorSet DescriptorSet_0;

        private Buffer ConstBuffer;
        private Buffer ConstBuffer2;
        private GraphicsPipeline PipelineState_0;


        private TransformUniform uniform;
        private Light light;
        private float yaw, pitch, roll = 0;


        public AmbientLighting() : base()
        {

        }


        public override void Initialize()
        {

            AdapterConfig = new()
            {
                SwapChain = true,
            };


            Camera.SetPosition(0, -0.55f, -8.0f);
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

            light = new()
            {
                Ambient = new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
                Diffuse = new Vector4(0.8f, 0.8f, 0.8f, 1.0f),
                Direction = new Vector3(1, 1, -1.05f),
            };



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
                SizeInBytes = Interop.SizeOf<Light>(),
            });


            GLTFModel = new(Device, Constants.ModelsFile + "chinesedragon.gltf");

            CreatePipelineState();

            
        }








        public void CreatePipelineState()
        {

            string images = Constants.ImagesFile;
            string fragment = Constants.ShadersFile + @"AmbientLighting\Fragment.hlsl";
            string vertex = Constants.ShadersFile + @"AmbientLighting\Vertex.hlsl";

            Image text1 = ImageFile.Load2DFromFile(Device, images + "UV_Grid_Sm.jpg");

            Sampler sampler = new Sampler(Device);


            GraphicsPipelineDescription Pipelinedescription0 = new();
            Pipelinedescription0.SetFramebuffer(Framebuffer);
            Pipelinedescription0.SetShader(new ShaderBytecode(fragment, ShaderStage.Fragment));
            Pipelinedescription0.SetShader(new ShaderBytecode(vertex, ShaderStage.Vertex));
            Pipelinedescription0.SetVertexBinding(VertexInputRate.Vertex, VertexPositionNormalTexture.Size );
            Pipelinedescription0.SetVertexAttribute(VertexType.Position);
            Pipelinedescription0.SetVertexAttribute(VertexType.Normal);
            Pipelinedescription0.SetVertexAttribute(VertexType.TextureCoordinate);
            PipelineState_0 = new(Pipelinedescription0);

            DescriptorData descriptorData_0 = new();
            descriptorData_0.SetUniformBuffer(0, ConstBuffer);
            descriptorData_0.SetImage(1, text1);
            descriptorData_0.SetSampler(2, sampler);
            descriptorData_0.SetUniformBuffer(3, ConstBuffer2);
            DescriptorSet_0 = new(PipelineState_0, descriptorData_0);
             



        }

        public override void Update(ApplicationTime time)
        {
            var timer = time.TotalMilliseconds / (3600);

            Camera.Update();
            light.Direction.X = -14.0f + MathF.Abs(MathF.Sin(MathUtil.Radians(timer * 360.0f)) * 2.0f);
            light.Direction.X = 0.0f + MathF.Sin(MathUtil.Radians(timer * 360.0f)) * MathF.Cos(MathUtil.Radians(timer * 360.0f)) * 2.0f;
            light.Direction.Y = 0.0f + MathF.Sin(MathUtil.Radians(timer * 360.0f)) * 2.0f;
            light.Direction.Z = 0.0f + MathF.Cos(MathUtil.Radians(timer * 360.0f)) * 2.0f;




            ConstBuffer2.SetData(ref light);


            Model = Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix4x4.CreateTranslation(0.0f, .0f, 0.0f);
            uniform.Update(Camera, Model);
            ConstBuffer.SetData(ref uniform);



            yaw = timer;
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

        public Vector4 Ambient;

        public Vector4 Diffuse;

        public Vector3 Direction;

        public float padding;

        public Light(Vector4 D, Vector3 DI, Vector4 AM)
        {
            Diffuse = D;
            Direction = DI;
            Ambient = AM;
            padding = 0;
        }
    }
}
