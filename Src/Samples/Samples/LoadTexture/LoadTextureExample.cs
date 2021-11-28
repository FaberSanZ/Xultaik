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

namespace Samples.LoadTexture
{
    public class LoadTextureExample : ExampleBase, IDisposable
    {
        private const int TextureWidth = 256; //Texture Data
        private const int TextureHeight = 256; //Texture Data
        private const int TexturePixelSize = 4;  // The number of bytes used to represent a pixel in the texture. RGBA


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
        private Dictionary<string, Buffer> Buffers = new();
        private TransformUniform uniform;



        public LoadTextureExample() : base()
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


            Camera.SetPosition(0, 0.34f, -2.5f);
            uniform = new(Camera.Projection, Model, Camera.View);


            CreateBuffers();
            CreatePipelineState();
        }



        public void CreateBuffers()
        {

            int[] Indices = new[]
            {
                0, 1, 2, 2, 3, 0
            };

            VertexPositionTexture[] Vertices = new VertexPositionTexture[]
            {
                new(new(0.5f, 0.5f, -0.5f), new(1.0f, 1.0f)) ,
                new(new(-0.5f, 0.5f, -0.5f), new(0.0f, 1.0f)) ,
                new(new(-0.5f, -0.5f, -0.5f),  new(0.0f, 0.0f)) ,
                new(new(0.5f,  -0.5f, -0.5f),  new(1.0f, 0.0f)) ,
            };


            Buffers["VertexBuffer"] = new(Device, new()
            {
                BufferFlags = BufferFlags.VertexBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = Interop.SizeOf(Vertices),
            });
            Buffers["VertexBuffer"].SetData(Vertices);


            Buffers["IndexBuffer"] = new(Device, new()
            {
                BufferFlags = BufferFlags.IndexBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = Interop.SizeOf(Indices),
            });
            Buffers["IndexBuffer"].SetData(Indices);


            Buffers["ConstBuffer1"] = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            });

            Buffers["ConstBuffer2"] = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            });

        }

        public void CreatePipelineState()
        {


            string shaders = Constants.ShadersFile;
            string images = Constants.ImagesFile;


            string Fragment = shaders + "LoadTexture/Fragment.hlsl";
            string Vertex = shaders + "LoadTexture/Vertex.hlsl";



            //var img0 = Image(GenerateTextureData(), TextureWidth, TextureWidth, 1, 1, TextureWidth * TextureWidth * 4, false, vkf.R8G8B8A8UNorm);
            Image text1 = ImageFile.Load2DFromFile(Device, images + "IndustryForgedDark512.ktx");
            Image text2 = ImageFile.Load2DFromFile(Device, images + "UVCheckerMap09-512.png");
            Sampler sampler = new Sampler(Device);


            GraphicsPipelineDescription Pipelinedescription_0 = new();
            Pipelinedescription_0.SetFramebuffer(Framebuffer);
            Pipelinedescription_0.SetShader(new ShaderBytecode(Fragment, ShaderStage.Fragment));
            Pipelinedescription_0.SetShader(new ShaderBytecode(Vertex, ShaderStage.Vertex));
            Pipelinedescription_0.SetVertexBinding(VertexInputRate.Vertex, VertexPositionTexture.Size);
            Pipelinedescription_0.SetVertexAttribute(VertexType.Position);
            Pipelinedescription_0.SetVertexAttribute(VertexType.TextureCoordinate);
            PipelineState_0 = new(Pipelinedescription_0);

            DescriptorData descriptorData_0 = new();
            descriptorData_0.SetUniformBuffer(0, Buffers["ConstBuffer1"]);
            descriptorData_0.SetImage(1, text1);
            descriptorData_0.SetSampler(2, sampler);
            DescriptorSet_0 = new(PipelineState_0, descriptorData_0);



            GraphicsPipelineDescription Pipelinedescription_1 = new();
            Pipelinedescription_1.SetFramebuffer(Framebuffer);
            Pipelinedescription_1.SetShader(new ShaderBytecode(Fragment, ShaderStage.Fragment));
            Pipelinedescription_1.SetShader(new ShaderBytecode(Vertex, ShaderStage.Vertex));
            Pipelinedescription_1.SetVertexBinding(VertexInputRate.Vertex, VertexPositionTexture.Size);
            Pipelinedescription_1.SetVertexAttribute(VertexType.Position);
            Pipelinedescription_1.SetVertexAttribute(VertexType.TextureCoordinate);
            PipelineState_1 = new(Pipelinedescription_1);

            DescriptorData descriptorData_1 = new();
            descriptorData_1.SetUniformBuffer(0, Buffers["ConstBuffer2"]);
            descriptorData_1.SetImage(1, text2);
            descriptorData_1.SetSampler(2, sampler);
            DescriptorSet_1 = new(PipelineState_1, descriptorData_1);
        }



        public override void Update(ApplicationTime time)
        {
            Camera.Update();


            Model = Matrix4x4.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z) * Matrix4x4.CreateTranslation(-0.8f, -0.4f, 0.0f);
            uniform.Update(Camera, Model);
            Buffers["ConstBuffer1"].SetData(ref uniform);


            Model = Matrix4x4.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, -Rotation.Z) * Matrix4x4.CreateTranslation(0.8f, -0.4f, 0.0f);
            uniform.Update(Camera, Model);
            Buffers["ConstBuffer2"].SetData(ref uniform);



            Rotation.Z -= 0.0004f * MathF.PI;

        }




        public override void Draw(ApplicationTime time)

        {

            Device.WaitIdle();
            CommandBuffer commandBuffer = Context.CommandBuffer;
            commandBuffer.Begin();


            commandBuffer.BeginFramebuffer(Framebuffer);
            commandBuffer.SetScissor(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);
            commandBuffer.SetViewport(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);





            commandBuffer.SetVertexBuffers(new Buffer[] { Buffers["VertexBuffer"] });
            commandBuffer.SetIndexBuffer(Buffers["IndexBuffer"]);


            commandBuffer.SetGraphicPipeline(PipelineState_0);
            commandBuffer.BindDescriptorSets(DescriptorSet_0);
            commandBuffer.DrawIndexed(6, 1, 0, 0, 0);

            commandBuffer.SetGraphicPipeline(PipelineState_1);
            commandBuffer.BindDescriptorSets(DescriptorSet_1);
            commandBuffer.DrawIndexed(6, 1, 0, 0, 0);



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


        internal byte[] GenerateTextureData()
        {
            byte r = 255;
            byte g = 255;
            byte b = 255;
            byte a = 255;

            int color = default;
            color |= r << 24;
            color |= g << 16;
            color |= b << 8;
            color |= a;

            uint color_value = (uint)color; // RBGA

            byte color_r = (byte)((color_value >> 24) & 0xFF);
            byte color_g = (byte)((color_value >> 16) & 0xFF);
            byte color_b = (byte)((color_value >> 8) & 0xFF);
            byte color_a = (byte)(color_value & 0xFF);


            int row_pitch = TextureWidth * TexturePixelSize;
            int cell_pitch = row_pitch >> 3;       // The width of a cell in the checkboard texture.
            int cell_height = TextureWidth >> 3;  // The height of a cell in the checkerboard texture.
            int texture_size = row_pitch * TextureHeight; // w * h * rgba = 4
            byte[] data = new byte[texture_size];

            for (int n = 0; n < texture_size; n += TexturePixelSize)
            {
                int x = n % row_pitch;
                int y = n / row_pitch;
                int i = x / cell_pitch;
                int j = y / cell_height;

                if (i % 2 == j % 2)
                {
                    data[n + 0] = 1; // R
                    data[n + 1] = 1; // G
                    data[n + 2] = 1; // B
                    data[n + 3] = 1; // A
                }
                else
                {
                    data[n + 0] = 0xff; // R
                    data[n + 1] = 0xff; // G
                    data[n + 2] = 0xff; // B
                    data[n + 3] = 0xff; // A
                }
            }

            return data;
        }

        public void Dispose()
        {
            Adapter.Dispose();
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
}
