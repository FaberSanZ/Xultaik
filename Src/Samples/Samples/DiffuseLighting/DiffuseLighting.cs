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

namespace Samples.DiffuseLighting
{
    public class DiffuseLightingExample : IDisposable
    {
        public Camera camera { get; set; }

        public ModelAssetImporter<VertexPositionNormalTexture> GLTFModel { get; set; }

        public Camera Camera { get; set; }
        public PresentationParameters Parameters { get; set; }
        public Adapter Adapter { get; set; }
        public Device Device { get; set; }
        public Framebuffer Framebuffer { get; set; }
        public SwapChain SwapChain { get; set; }
        public GraphicsContext Context { get; set; }
        public Matrix4x4 Model { get; set; }
        public Window? Window { get; set; }

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
        public float timer;





        public void Initialize()
        {
            Window = new Window("Vultaik", 1200, 800);

            Parameters = new PresentationParameters()
            {
                BackBufferWidth = Window.Width,
                BackBufferHeight = Window.Height,
                Settings = new Settings()
                {
                    Validation = ValidationType.None,
                    Fullscreen = false,
                    VSync = false,
                },
            };



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

            Context = new GraphicsContext(Device);
            Framebuffer = new Framebuffer(SwapChain);

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
            timer = 0;
        }








        public void CreatePipelineState()
        {

            string shaders = Constants.ShadersFile;
            string images = Constants.ImagesFile;

            string Fragment = shaders + "DiffuseLighting/Fragment.hlsl";
            string Vertex = shaders + "DiffuseLighting/Vertex.hlsl";



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


        public void Update()
        {

            //light.LightDirection.X = -14.0f + MathF.Abs(MathF.Sin(MathUtil.Radians(timer * 360.0f)) * 2.0f);
            light.LightDirection.X = 0.0f + MathF.Sin(MathUtil.Radians(timer * 360.0f)) * MathF.Cos(MathUtil.Radians(timer * 360.0f)) * 2.0f;
            light.LightDirection.Y = 0.0f + MathF.Sin(MathUtil.Radians(timer * 360.0f)) * 2.0f;
            light.LightDirection.Z = 0.0f + MathF.Cos(MathUtil.Radians(timer * 360.0f)) * 2.0f;
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


            timer += 0.0006f;
            //yaw = timer * MathF.PI;
        }


        public void Draw()
        {

            Device.WaitIdle();
            CommandBuffer cmd = Context.CommandBuffer;

            cmd.Begin();
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


            cmd.Close();
            cmd.Submit();
            SwapChain.Present();
        }


        public void Run()
        {

            Initialize();

            Window?.Show();
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
            ConstBuffer.Dispose();
            ConstBuffer2.Dispose();
            ConstBuffer3.Dispose();
            ConstBuffer4.Dispose();
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
