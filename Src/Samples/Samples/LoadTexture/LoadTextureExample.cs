using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Vultaik.Desktop;
using Vultaik.Engine;
using Vultaik;
using Vultaik.Physics;
using Buffer = Vultaik.Buffer;
using Vortice.Vulkan;
using Interop = Vultaik.Interop;
using Samples.Common;

namespace Samples.LoadTexture
{
    public class LoadTextureExample : Application, IDisposable
    {
        internal int TextureWidth = 256; //Texture Data
        internal int TextureHeight = 256; //Texture Data
        internal int TexturePixelSize = 4;  // The number of bytes used to represent a pixel in the texture. RGBA






        public Camera Camera { get; set; }
        public ApplicationTime GameTime { get; set; }

        public int[] Indices = new[]
        {
            0, 1, 2, 2, 3, 0
        };

        public VertexPositionTexture[] Vertices = new VertexPositionTexture[]
        {
            new(new(0.5f, 0.5f, -0.5f), new Vector2(1.0f, 1.0f)) ,
            new(new(-0.5f, 0.5f, -0.5f), new Vector2(0.0f, 1.0f)) ,
            new(new(-0.5f, -0.5f, -0.5f),  new Vector2(0.0f, 0.0f)) ,
            new(new(0.5f,  -0.5f, -0.5f),  new Vector2(1.0f, 0.0f)) ,
        };



        public GraphicsPipeline PipelineState_0;
        public GraphicsPipeline PipelineState_1;

        public DescriptorSet DescriptorSet_0;
        public DescriptorSet DescriptorSet_1;

        public Dictionary<string, Buffer> Buffers = new();

        // TransformUniform 
        public TransformUniform uniform;
        public float yaw;
        public float pitch;
        public float roll;


        public LoadTextureExample() : base()
        {

        }


        public override void InitializeSettings()
        {
            base.InitializeSettings();
            Parameters.Settings.Validation = ValidationType.None;
            Window.Title += " - (LoadTexture) ";
        }

        public override void Initialize()
        {
            base.Initialize();

            Camera = new(45f, 1f, 0.1f, 64f);
            Camera.SetPosition(0, 0.34f, -3.5f);
            Camera.AspectRatio = (float)Window.Width / Window.Height;


            // Reset Model
            Model = Matrix4x4.Identity;

            uniform = new(Camera.Projection, Model, Camera.View);


            CreateBuffers();
            CreatePipelineState();


            yaw = 0;
            pitch = 3.0f;
            roll = 0;
        }


        public void CreateBuffers()
        {
            Buffers["VertexBuffer"] = new(Device, new()
            {
                BufferFlags = BufferFlags.VertexBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf(Vertices),
            });
            Buffers["VertexBuffer"].SetData(Vertices);


            Buffers["IndexBuffer"] = new(Device, new()
            {
                BufferFlags = BufferFlags.IndexBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf(Indices),
            });
            Buffers["IndexBuffer"].SetData(Indices);


            Buffers["ConstBuffer1"] = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            });

            Buffers["ConstBuffer2"] = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
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


            PipelineStateDescription Pipelinedescription_0 = new();
            Pipelinedescription_0.SetFramebuffer(Framebuffer);
            Pipelinedescription_0.SetShader(new ShaderBytecode(Fragment, ShaderStage.Fragment, ShaderBackend.Hlsl));
            Pipelinedescription_0.SetShader(new ShaderBytecode(Vertex, ShaderStage.Vertex, ShaderBackend.Hlsl));
            Pipelinedescription_0.SetVertexBinding(VkVertexInputRate.Vertex, VertexPositionTexture.Size);
            Pipelinedescription_0.SetVertexAttribute(VertexType.Position);
            Pipelinedescription_0.SetVertexAttribute(VertexType.TextureCoordinate);
            PipelineState_0 = new(Pipelinedescription_0);

            DescriptorData descriptorData_0 = new();
            descriptorData_0.SetUniformBuffer(0, Buffers["ConstBuffer1"]);
            descriptorData_0.SetImage(1, text1);
            descriptorData_0.SetSampler(2, sampler);
            DescriptorSet_0 = new(PipelineState_0, descriptorData_0);



            PipelineStateDescription Pipelinedescription_1 = new();
            Pipelinedescription_1.SetFramebuffer(Framebuffer);
            Pipelinedescription_1.SetShader(new ShaderBytecode(Fragment, ShaderStage.Fragment, ShaderBackend.Hlsl));
            Pipelinedescription_1.SetShader(new ShaderBytecode(Vertex, ShaderStage.Vertex, ShaderBackend.Hlsl));
            Pipelinedescription_1.SetVertexBinding(VkVertexInputRate.Vertex, VertexPositionTexture.Size);
            Pipelinedescription_1.SetVertexAttribute(VertexType.Position);
            Pipelinedescription_1.SetVertexAttribute(VertexType.TextureCoordinate);
            PipelineState_1 = new(Pipelinedescription_1);

            DescriptorData descriptorData_1 = new();
            descriptorData_1.SetUniformBuffer(0, Buffers["ConstBuffer2"]);
            descriptorData_1.SetImage(1, text2);
            descriptorData_1.SetSampler(2, sampler);
            DescriptorSet_1 = new(PipelineState_1, descriptorData_1);
        }



        public override void Update(ApplicationTime game)
        {
            Camera.Update();


            Model = Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix4x4.CreateTranslation(-0.8f, -0.4f, 0.0f);
            uniform.Update(Camera, Model);
            Buffers["ConstBuffer1"].SetData(ref uniform);


            Model = Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, -roll) * Matrix4x4.CreateTranslation(0.8f, -0.4f, 0.0f);
            uniform.Update(Camera, Model);
            Buffers["ConstBuffer2"].SetData(ref uniform);



            roll -= 0.0004f * MathF.PI;

        }




        public override void BeginDraw()
        {
            base.BeginDraw();

            CommandBuffer commandBuffer = Context.CommandBuffer;

            commandBuffer.BeginFramebuffer(Framebuffer);
            commandBuffer.SetViewport(Window.Width, Window.Height, 0, 0);
            commandBuffer.SetScissor(Window.Width, Window.Height, 0, 0);





            commandBuffer.SetVertexBuffers(new Buffer[] { Buffers["VertexBuffer"] });
            commandBuffer.SetIndexBuffer(Buffers["IndexBuffer"]);


            commandBuffer.SetGraphicPipeline(PipelineState_0);
            commandBuffer.BindDescriptorSets(DescriptorSet_0);
            commandBuffer.DrawIndexed(Indices.Length, 1, 0, 0, 0);

            commandBuffer.SetGraphicPipeline(PipelineState_1);
            commandBuffer.BindDescriptorSets(DescriptorSet_1);
            commandBuffer.DrawIndexed(Indices.Length, 1, 0, 0, 0);
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

            uint _value = (uint)color; // RBGA

            byte Cr = (byte)((_value >> 24) & 0xFF);
            byte Cg = (byte)((_value >> 16) & 0xFF);
            byte Cb = (byte)((_value >> 8) & 0xFF);
            byte Ca = (byte)(_value & 0xFF);


            int rowPitch = TextureWidth * TexturePixelSize;
            int cellPitch = rowPitch >> 3;       // The width of a cell in the checkboard texture.
            int cellHeight = TextureWidth >> 3;  // The height of a cell in the checkerboard texture.
            int textureSize = rowPitch * TextureHeight;
            byte[] data = new byte[textureSize];

            for (int n = 0; n < textureSize; n += TexturePixelSize)
            {
                int x = n % rowPitch;
                int y = n / rowPitch;
                int i = x / cellPitch;
                int j = y / cellHeight;

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
