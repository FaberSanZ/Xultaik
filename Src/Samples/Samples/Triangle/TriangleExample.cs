using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Vultaik;
using Vultaik.Desktop;
using Vultaik.Physics;
using Buffer = Vultaik.Buffer;
using Vortice.Vulkan;
using Interop = Vultaik.Interop;
using Samples.Common;

namespace Samples.Triangle
{
    public class TriangleExample : IDisposable
    {

        public TriangleExample() : base()
        {

        }

        public Camera Camera { get; set; }
        public PresentationParameters Parameters { get; set; }
        public Adapter Adapter { get; set; }
        public Device Device { get; set; }
        public Framebuffer Framebuffer { get; set; }
        public SwapChain SwapChain { get; set; }
        public GraphicsContext Context { get; set; }
        public Matrix4x4 Model { get; set; }
        public Window? Window { get; set; }


        public GraphicsPipeline PipelineState { get; set; }
        public DescriptorSet DescriptorSet { get; set; }
        public Buffer VertexBuffer { get; set; }
        public Buffer IndexBuffer { get; set; }
        public Buffer ConstBuffer { get; set; }

        public TransformUniform Uniform;


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
            Camera.SetPosition(0, 0, -3.0f);
            Camera.AspectRatio = (float)Window.Width / Window.Height;
            Camera.Update();


            // Reset Model
            Model = Matrix4x4.Identity;


            Uniform = new(Camera.Projection, Model, Camera.View);

            CreateBuffers();

            CreatePipelineState();


        }




        public void CreateBuffers()
        {


            VertexPositionColor[] vertices = new[]
            {
                new VertexPositionColor(new Vector3(0.0f, -0.65f, -0.5f), new Vector3(1.6f, 0.0f, 0.0f)),
                new VertexPositionColor(new Vector3(0.65f, 0.65f, -0.5f), new Vector3(0.0f, 1.6f, 0.0f)),
                new VertexPositionColor(new Vector3(-0.65f, 0.65f, -0.5f), new Vector3(0.0f, 0.0f, 1.6f)),
            };


            int[] indices = new[]
            {
                0, 1, 2
            };


            VertexBuffer = new(Device, new()
            {
                BufferFlags = BufferFlags.VertexBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<VertexPositionColor>(vertices),
            });
            VertexBuffer.SetData(vertices);


            IndexBuffer = new(Device, new()
            {
                BufferFlags = BufferFlags.IndexBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<int>(indices),
            });
            IndexBuffer.SetData(indices);


            ConstBuffer = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            });


        }


        public void CreatePipelineState()
        {

            var file = Constants.ShadersFile;

            PipelineStateDescription Pipelinedescription = new();
            Pipelinedescription.SetFramebuffer(Framebuffer);
            Pipelinedescription.SetShader(new ShaderBytecode(file + "Triangle/Fragment.hlsl", ShaderStage.Fragment, ShaderBackend.Hlsl));
            Pipelinedescription.SetShader(new ShaderBytecode(file + "Triangle/Vertex.hlsl", ShaderStage.Vertex, ShaderBackend.Hlsl));
            Pipelinedescription.SetVertexBinding(VkVertexInputRate.Vertex, VertexPositionColor.Size);
            Pipelinedescription.SetVertexAttribute(VertexType.Position);
            Pipelinedescription.SetVertexAttribute(VertexType.Color);
            Pipelinedescription.SetFillMode(VkPolygonMode.Fill);
            Pipelinedescription.SetCullMode(VkCullModeFlags.None);
            Pipelinedescription.SetPrimitiveType(VkPrimitiveTopology.TriangleList);

            PipelineState = new(Pipelinedescription);


            DescriptorData descriptorData = new();
            descriptorData.SetUniformBuffer(0, ConstBuffer);

            DescriptorSet = new(PipelineState, descriptorData);
        }

        public void Update()
        {
            Camera.Update();

            Uniform.Update(Camera, Model);

            ConstBuffer.SetData(ref Uniform);
        }


        public void Draw()
        {
            Device.WaitIdle();
            CommandBuffer commandBuffer = Context.CommandBuffer;
            commandBuffer.Begin();


            commandBuffer.BeginFramebuffer(Framebuffer);
            commandBuffer.SetViewport(Window.Width, Window.Height, 0, 0);
            commandBuffer.SetScissor(Window.Width, Window.Height, 0, 0);

            commandBuffer.SetGraphicPipeline(PipelineState);
            commandBuffer.BindDescriptorSets(DescriptorSet);
            commandBuffer.SetVertexBuffer(VertexBuffer);
            commandBuffer.SetIndexBuffer(IndexBuffer);
            commandBuffer.DrawIndexed(3, 1, 0, 0, 0);


            commandBuffer.Close();
            commandBuffer.Submit();
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
            Adapter.Dispose();
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct TransformUniform
    {
        public TransformUniform(Matrix4x4 m, Matrix4x4 v, Matrix4x4 p)
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
