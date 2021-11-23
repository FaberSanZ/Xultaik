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
using System.Linq;

namespace Samples.Bindless
{
    public class Bindless : ExampleBase, IDisposable
    {
        private const int TextureWidth = 64; //Texture Data
        private const int TextureHeight = 64; //Texture Data
        private const int TexturePixelSize = 4;  // The number of bytes used to represent a pixel in the texture. RGBA


        private AdapterConfig AdapterConfig;
        private Adapter Adapter;
        private Device Device;
        private Framebuffer Framebuffer;
        private SwapChain SwapChain;
        private GraphicsContext Context;
        private GraphicsPipeline PipelineState_0;
        private DescriptorSet DescriptorSet_0;
        private Buffer VertexBuffer;
        private Buffer IndexBuffer;
        private Buffer CameraBuffer;

        
        private TransformUniform uniform;
        private Vector3[] rotationSpeeds = new Vector3[OBJECT_INSTANCES];

        private int[] random_texture = new int[OBJECT_INSTANCES]; 
        private const uint OBJECT_INSTANCES = 75;


        public Bindless() : base()
        {

        }


        public override void Initialize()
        {

            AdapterConfig = new()
            {
                SwapChain = true,
                Debug = false,
                Bindless = true,
                //SingleQueue
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


            Camera.SetPosition(0, 0.34f, -18.5f);
            uniform = new(Camera.Projection, Camera.View);


            CreateBuffers();
            CreatePipelineState();
        }

        int[] Indices = new[]
        {
            0,1,2, 0,2,3, 4,5,6,  
            4,6,7, 8,9,10, 8,10,11, 
            12,13,14, 12,14,15, 
            16,17,18, 16,18,19, 
            20,21,22, 20,22,23
        };

        VertexPositionTexture[] Vertices = new VertexPositionTexture[]
        {
            new(new(-0.5f, -0.5f,  0.5f), new(0.0f, 0.0f)),
            new(new(0.5f, -0.5f,  0.5f), new(1.0f, 0.0f)),
            new(new(0.5f,  0.5f,  0.5f),  new(1.0f, 1.0f)),
            new(new(-0.5f, 0.5f,  0.5f ),  new(0.0f, 1.0f)),

            new(new(0.5f,  0.5f,  0.5f), new(0.0f, 0.0f)),
            new(new(0.5f,  0.5f, -0.5f), new(1.0f, 0.0f)),
            new(new(0.5f, -0.5f, -0.5f),  new(1.0f, 1.0f)),
            new(new(0.5f, -0.5f, -0.5f ),  new(0.0f, 1.0f)),

                new(new(0.5f, -0.5f, -0.5f ), new(0.0f, 0.0f)),
                new(new( 0.5f, -1.5f, -0.5f ), new(1.0f, 0.0f)),
                new(new( 0.5f,  0.5f, -0.5f),  new(1.0f, 1.0f)),
                new(new(-1.0f,  1.0f, -1.0f),  new(0.0f, 1.0f)),


                new(new(-0.5f, -0.5f, -0.5f ), new(0.0f, 0.0f)),
                new(new(-0.5f, -0.5f,  0.5f ), new(1.0f, 0.0f)),
                new(new( -0.5f,  0.5f,  0.5f ),  new(1.0f, 1.0f)),
                new(new(-0.5f,  0.5f, -0.5f ),  new(0.0f, 1.0f)),

                new(new( 0.5f,  1.5f,  0.5f), new(0.0f, 0.0f)),
                new(new(-0.5f,  0.5f,  0.5f ), new(1.0f, 0.0f)),
                new(new( -0.5f,  0.5f, -0.5f  ),  new(1.0f, 1.0f)),
                new(new( 1.0f,  1.0f, -1.0f ),  new(0.0f, 1.0f)),

                new(new( -0.5f, -0.5f, -0.5f), new(0.0f, 0.0f)),
                new(new(0.5f, 0.5f, -0.5f), new(1.0f, 0.0f)),
                new(new(0.5f, -0.5f,  0.5f ),  new(1.0f, 1.0f)),
                new(new( -0.5f, -0.5f,  0.5f ),  new(0.0f, 1.0f)),
        };


        public void CreateBuffers()
        {




            VertexBuffer = new(Device, new()
            {
                BufferFlags = BufferFlags.VertexBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = Interop.SizeOf(Vertices),
            });
            VertexBuffer.SetData(Vertices);


            IndexBuffer = new(Device, new()
            {
                BufferFlags = BufferFlags.IndexBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = Interop.SizeOf(Indices),
            });
            IndexBuffer.SetData(Indices);


            CameraBuffer = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            });








            // Prepare per-object matrices with random rotations and texture index
            Random random = new();
            Func<Random, float> rnd_dist = rand => (float)(rand.NextDouble() * 2 - 1.0);

            for (uint i = 0; i < OBJECT_INSTANCES; i++)
            {
                random_texture[i] = random.Next(0, 64);
                rotationSpeeds[i] = new Vector3(rnd_dist(random), rnd_dist(random), rnd_dist(random));

            }
        }


        public void AddCube(CommandBuffer cmd, Vector3 position, Vector3 rotation, bool r, int text)
        {

            var model = Matrix4x4.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z) * Matrix4x4.CreateTranslation(position) * Matrix4x4.CreateScale(.8f);


            cmd.PushConstant(PipelineState_0, ShaderStage.Vertex, model);
            cmd.PushConstant<int>(PipelineState_0, ShaderStage.Fragment, text);
            cmd.DrawIndexed(Indices.Length, 1, 0, 0, 0);

        }


        public void GenerateCubes(CommandBuffer cmd, bool r)
        {


            float rotation = Time.TotalMilliseconds / 600;

            uint dim = (uint)(Math.Pow(OBJECT_INSTANCES, (1.0f / 3.0f)));
            Vector3 offset = new Vector3(5.0f);

            for (uint x = 0; x < dim; x++)
            {
                for (uint y = 0; y < dim; y++)
                {
                    for (uint z = 0; z < dim; z++)
                    {
                        uint index = x * dim * dim + y * dim + z;

                        Vector3 rotations = rotation * rotationSpeeds[index];
                        int r_t = random_texture[index];
                        Vector3 pos = new Vector3(-((dim * offset.X) / 2.0f) + offset.X / 2.0f + x * offset.X, -((dim * offset.Y) / 2.0f) + offset.Y / 2.0f + y * offset.Y, -((dim * offset.Z) / 2.0f) + offset.Z / 2.0f + z * offset.Z);
                        AddCube(cmd, pos, rotations, r, r_t);
                    }
                }
            }
        }


        public unsafe void CreatePipelineState()
        {
            Random random = new();

            Image[] random_images = new Image[64];

            Console.WriteLine(TextureHeight * TextureWidth * 4);
            ImageDescription image_description = new ImageDescription()
            {
                Flags = ImageFlags.ShaderResource,
                Usage = ResourceUsage.GPU_Only,
                Width = TextureWidth,
                Height = TextureHeight,
                Size = 16384, // TextureHeight * TextureWidth * R8G8B8A8UNorm
                Format = VkFormat.R8G8B8A8UNorm,
                ImageType = VkImageType.Image2D,

            };


            for (int i = 0; i < random_images.Length; i++)
            {
                byte[] data = GenerateTextureData((byte)random.Next(0, 254), (byte)random.Next(0, 254), (byte)random.Next(0, 254), 
                                                  (byte)random.Next(0, 256), (byte)random.Next(0, 256), (byte)random.Next(0, 256));

                random_images[i] = new(Device, image_description);
                random_images[i].SetData(data); // set and copy data

            }



            GraphicsPipelineDescription Pipelinedescription_0 = new();
            Pipelinedescription_0.SetFramebuffer(Framebuffer);
            Pipelinedescription_0.SetShader(new ShaderBytecode(Constants.ShadersFile + "Bindless/Fragment.hlsl", ShaderStage.Fragment));
            Pipelinedescription_0.SetShader(new ShaderBytecode(Constants.ShadersFile + "Bindless/Vertex.hlsl", ShaderStage.Vertex));
            Pipelinedescription_0.SetVertexBinding(VertexInputRate.Vertex, VertexPositionTexture.Size);
            Pipelinedescription_0.SetVertexAttribute(VertexType.Position);
            Pipelinedescription_0.SetVertexAttribute(VertexType.TextureCoordinate);
            PipelineState_0 = new(Pipelinedescription_0);

            // Set image without binding last allows to use the same DescriptorSet for all types,
            // Otherwise use a different DescriptorSet.

            // Correct use
            // .SetUniformBuffer(0, buffer);
            // .SetSampler(1, sampler1);
            // .SetSampler(2, sampler2);
            // .SetSampler(3, sampler3);
            // .SetImageSampler(4, image, sampler1);
            // .SetStructuredBuffer(5, structured_buffer)
            // .SetReadWriteImage(6, image_read)
            // .SetBindlessImage(7, images);

            // Incorrect use
            // .SetUniformBuffer(0, buffer);
            // .SetSampler(1, sampler1);
            // .SetSampler(2, sampler2);
            // .SetBindlessImage(3, images);
            // .SetSampler(5, sampler3);
            // .SetImageSampler(6, image, sampler1);
            // .SetStructuredBuffer(7, structured_buffer)
            // .SetReadWriteImage(8, image_read)


            DescriptorData descriptorData_0 = new();
            descriptorData_0.SetUniformBuffer(0, CameraBuffer);
            descriptorData_0.SetSampler(1, new Sampler(Device));
            descriptorData_0.SetBindlessImage(2, random_images);

            DescriptorSet_0 = new(PipelineState_0, descriptorData_0);

        }



        public override void Update(ApplicationTime time)
        {
            Camera.Update();
            uniform.Update(Camera);
            CameraBuffer.SetData(ref uniform);


            Rotation.Z += 0.010f * MathF.PI;
        }



        public override void Draw(ApplicationTime time)

        {

            Device.WaitIdle();
            CommandBuffer commandBuffer = Context.CommandBuffer;
            commandBuffer.Begin();


            commandBuffer.BeginFramebuffer(Framebuffer);
            commandBuffer.SetScissor(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);
            commandBuffer.SetViewport(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);





            commandBuffer.SetVertexBuffers(new Buffer[] { VertexBuffer });
            commandBuffer.SetIndexBuffer(IndexBuffer);


            commandBuffer.SetGraphicPipeline(PipelineState_0);
            commandBuffer.BindDescriptorSets(DescriptorSet_0);

            GenerateCubes(commandBuffer, true);


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


        internal byte[] GenerateTextureData(byte r = 255, byte g = 255,byte b = 255, byte r2 = 255, byte g2 = 255, byte b2 = 255)
        {

            int color = default;
            color |= r << 24;
            color |= g << 16;
            color |= b << 8;
            color |= 0xff;

            uint color_value = (uint)color; // RBGA

            byte color_r = (byte)((color_value >> 24) & 0xFF);
            byte color_g = (byte)((color_value >> 16) & 0xFF);
            byte color_b = (byte)((color_value >> 8) & 0xFF);
            byte color_a = (byte)(color_value & 0xFF);


            Random random = new();
            int row_pitch = TextureWidth * TexturePixelSize;
            int cell_pitch = row_pitch >> 2;       // The width of a cell in the checkboard texture.
            int cell_height = TextureWidth >> 2;  // The height of a cell in the checkerboard texture.
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
                    data[n + 0] = color_r; // R
                    data[n + 1] = color_g; // G
                    data[n + 2] = color_b; // B
                    data[n + 3] = 0xff; // A
                }
                else
                {
                    data[n + 3] = r2; // R
                    data[n + 2] = g2; // G
                    data[n + 1] = b2; // B
                    data[n + 0] = 0xff; // A
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
        public TransformUniform(Matrix4x4 p,Matrix4x4 v)
        {
            P = p;
            V = v;
        }

        public Matrix4x4 V;
        public Matrix4x4 P;



        public void Update(Camera camera)
        {
            P = camera.Projection;
            V = camera.View;
        }
    }
}
