using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Zeckoxe.Core;
using Zeckoxe.Desktop;
using Zeckoxe.Engine;
using Zeckoxe.Vulkan;
using Zeckoxe.Vulkan.Toolkit;
using Zeckoxe.Physics;
using Buffer = Zeckoxe.Vulkan.Buffer;

namespace Samples.Samples
{
    public class LoadTexture : Application, IDisposable
    {
        internal int TextureWidth = 256; //Texture Data
        internal int TextureHeight = 256; //Texture Data
        internal int TexturePixelSize = 4;  // The number of bytes used to represent a pixel in the texture. RGBA


        [StructLayout(LayoutKind.Sequential)]
        public struct TransformUniform
        {
            public TransformUniform(Matrix4x4 p, Matrix4x4 m, Matrix4x4 v)
            {
                P = p;
                M = m;
                V = v;
            }

            public Matrix4x4 P;

            public Matrix4x4 M;

            public Matrix4x4 V;

            public void Update(Camera camera, Matrix4x4 m)
            {
                P = camera.Projection;
                M = m;
                V = camera.View;
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct Vertex
        {
            public Vertex(Vector3 position, Vector3 color, Vector2 texCoord)
            {
                Position = position;
                Color = color;
                TexCoord = texCoord;
            }

            public Vector3 Position;

            public Vector3 Color;
            public Vector2 TexCoord;





            public static readonly int Size = Interop.SizeOf<Vertex>();
        }




        public Camera Camera { get; set; }
        public ApplicationTime GameTime { get; set; }

        public int[] Indices = new[]
        {
            0, 1, 2,
            2, 3, 0
        };

        public Vertex[] Vertices = new[]
        {
            new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(1.0f, 0.0f, 0.0f), new Vector2(1.0f, 0.0f)) ,
            new Vertex(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3(0.0f, 1.0f, 0.0f), new Vector2(0.0f, 0.0f)) ,
            new Vertex(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.0f, 0.0f, 1.0f), new Vector2(0.0f, 1.0f)) ,
            new Vertex(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(1.0f, 1.0f, 1.0f), new Vector2(1.0f, 1.0f)) ,
        };


        public Dictionary<string, Buffer> Buffers = new();
        public Dictionary<string, GraphicsPipelineState> PipelineStates = new();
        public Dictionary<string, ShaderBytecode> Shaders = new();

        // TransformUniform 
        public TransformUniform uniform;
        public float yaw;
        public float pitch;
        public float roll;


        public LoadTexture() : base()
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

            Camera = new()
            {
                Mode = CameraType.Free,
                Position = new(0, -0.0f, -2.1f),
            };

            Camera.InvertY = true;
            Camera.SetLens(Window.Width, Window.Height);

            // Reset Model
            Model = Matrix4x4.Identity;

            uniform = new(Camera.Projection, Model, Camera.View);






            CreateBuffers();
            CreatePipelineState();



            //var img = new TextureData(GenerateTextureData(), TextureWidth, TextureWidth, 1, 1, TextureWidth * TextureWidth * 4, false, PixelFormat.R8G8B8A8UNorm);




            yaw = 0;
            pitch = 2.5f;
            roll = 4.7f;
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
            Shaders["Fragment"] = ShaderBytecode.LoadFromFile("Shaders/Texture/shader.frag", ShaderStage.Fragment);
            Shaders["Vertex"] = ShaderBytecode.LoadFromFile("Shaders/Texture/shader.vert", ShaderStage.Vertex);


            Image2D text1 = Image2D.LoadFromFile(Device, "UVCheckerMap08-512.png");
            Image2D text2 = Image2D.LoadFromFile(Device, "IndustryForgedDark512.ktx");
            Sampler sampler = new Sampler(Device);


            PipelineStateDescription Pipelinedescription1 = new();
            Pipelinedescription1.SetFramebuffer(Framebuffer);
            Pipelinedescription1.AddShader(Shaders["Fragment"]);
            Pipelinedescription1.AddShader(Shaders["Vertex"]);
            Pipelinedescription1.AddVertexBinding(VertexInputRate.Vertex, Vertex.Size);
            Pipelinedescription1.AddVertexAttribute(VertexType.Position);
            Pipelinedescription1.AddVertexAttribute(VertexType.Color);
            Pipelinedescription1.AddVertexAttribute(VertexType.TextureCoordinate);
            Pipelinedescription1.SetUniformBuffer(0, ShaderStage.Vertex, Buffers["ConstBuffer1"]);
            Pipelinedescription1.SetImageSampler(1, ShaderStage.Fragment, text1, sampler);



            PipelineStateDescription Pipelinedescription2 = new();
            Pipelinedescription2.SetFramebuffer(Framebuffer);
            Pipelinedescription2.AddShader(Shaders["Fragment"]);
            Pipelinedescription2.AddShader(Shaders["Vertex"]);
            Pipelinedescription2.AddVertexBinding(VertexInputRate.Vertex, Vertex.Size);
            Pipelinedescription2.AddVertexAttribute(VertexType.Position);
            Pipelinedescription2.AddVertexAttribute(VertexType.Color);
            Pipelinedescription2.AddVertexAttribute(VertexType.TextureCoordinate);
            Pipelinedescription2.SetUniformBuffer(0, ShaderStage.Vertex, Buffers["ConstBuffer2"]);
            Pipelinedescription2.SetImageSampler(1, ShaderStage.Fragment, text2, sampler);



            PipelineStates["Texture1"] = new(Pipelinedescription1);
            PipelineStates["Texture2"] = new(Pipelinedescription2);

        }



        public override void Update(ApplicationTime game)
        {
            Camera.Update(game);


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

            commandBuffer.BeginFramebuffer(Framebuffer, 0.6f, 0.8f, 0.4f, 1.0f);
            commandBuffer.SetViewport(Window.Width, Window.Height, 0, 0);
            commandBuffer.SetScissor(Window.Width, Window.Height, 0, 0);





            commandBuffer.SetVertexBuffers(new Buffer[] { Buffers["VertexBuffer"] });
            commandBuffer.SetIndexBuffer(Buffers["IndexBuffer"]);


            commandBuffer.SetGraphicPipeline(PipelineStates["Texture1"]);
            commandBuffer.DrawIndexed(Indices.Length, 1, 0, 0, 0);


            commandBuffer.SetGraphicPipeline(PipelineStates["Texture2"]);
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
}
