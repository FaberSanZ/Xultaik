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

namespace Samples.LoadTexture
{
    public class LoadTextureExample : IDisposable
    {
        internal int TextureWidth = 256; //Texture Data
        internal int TextureHeight = 256; //Texture Data
        internal int TexturePixelSize = 4;  // The number of bytes used to represent a pixel in the texture. RGBA


        public Camera Camera { get; set; }

        public int[] Indices = new[]
        {
            0, 1, 2, 2, 3, 0
        };

        public VertexPositionTexture[] Vertices = new VertexPositionTexture[]
        {
            new(new(0.5f, 0.5f, -0.5f), new(1.0f, 1.0f)) ,
            new(new(-0.5f, 0.5f, -0.5f), new(0.0f, 1.0f)) ,
            new(new(-0.5f, -0.5f, -0.5f),  new(0.0f, 0.0f)) ,
            new(new(0.5f,  -0.5f, -0.5f),  new(1.0f, 0.0f)) ,
        };


        public PresentationParameters Parameters { get; set; }
        public Adapter Adapter { get; set; }
        public Device Device { get; set; }
        public Framebuffer Framebuffer { get; set; }
        public SwapChain SwapChain { get; set; }
        public GraphicsContext Context { get; set; }
        public Matrix4x4 Model { get; set; }
        public Window? Window { get; set; }


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


            GraphicsPipelineDescription Pipelinedescription_0 = new();
            Pipelinedescription_0.SetFramebuffer(Framebuffer);
            Pipelinedescription_0.SetShader(new ShaderBytecode(Fragment, ShaderStage.Fragment));
            Pipelinedescription_0.SetShader(new ShaderBytecode(Vertex, ShaderStage.Vertex));
            Pipelinedescription_0.SetVertexBinding(VkVertexInputRate.Vertex, VertexPositionTexture.Size);
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



        public void Update()
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




        public void Draw()
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
            commandBuffer.DrawIndexed(Indices.Length, 1, 0, 0, 0);

            commandBuffer.SetGraphicPipeline(PipelineState_1);
            commandBuffer.BindDescriptorSets(DescriptorSet_1);
            commandBuffer.DrawIndexed(Indices.Length, 1, 0, 0, 0);



            commandBuffer.Close();
            Device.Submit(commandBuffer);
            SwapChain.Present();
        }

        private void Window_Resize((int Width, int Height) obj)
        {
            Device.WaitIdle();
            SwapChain.Resize(obj.Width, obj.Height);
            Framebuffer.Resize();

            Camera.AspectRatio = (float)obj.Width / obj.Height;
        }


        public void Run()
        {

            Initialize();

            Window?.Show();
            Window!.Resize += Window_Resize;
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
