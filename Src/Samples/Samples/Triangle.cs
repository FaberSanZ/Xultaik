using System;
using System.Numerics;
using Zeckoxe.Core;
using Zeckoxe.Desktop;
using Zeckoxe.Engine;
using Zeckoxe.Graphics;
using Zeckoxe.Graphics.Toolkit;
using Zeckoxe.ShaderCompiler;
using Buffer = Zeckoxe.Graphics.Buffer;

namespace Samples.Samples
{
    public class Triangle : Game, IDisposable
    {
        public struct Vertex
        {
            public Vertex(Vector2 position, Vector3 color)
            {
                Position = position;
                Color = color;
            }

            public Vector2 Position;
            public Vector3 Color;
        }



        public Vertex[] vertices = new[]
        {
                new Vertex(new Vector2(0.0f, -0.65f), new Vector3(1.8f, 0.0f, 0.0f)),
                new Vertex(new Vector2(0.5f, 0.65f), new Vector3(0.0f, 1.8f, 0.0f)),
                new Vertex(new Vector2(-0.5f, 0.65f), new Vector3(0.0f, 0.0f, 1.8f)),
        };


        public int[] indices = new[]
        {
            0, 1, 2
        };


        public PipelineState PipelineState { get; set; }
        public Buffer VertexBuffer { get; set; }
        public Buffer IndexBuffer { get; set; }


        public Triangle() : base()
        {
            Parameters.Settings.Validation = false;
            Window.Title += "Zeckoxe Engine - (LoadGLTF) ";
        }


        public override void Initialize()
        {
            base.Initialize();

            CreateBuffers();

            CreatePipelineState();
        }




        public void CreateBuffers()
        {
            VertexBuffer = new Buffer(Device, new BufferDescription()
            {
                BufferFlags = BufferFlags.VertexBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<Vertex>(vertices),
            });


            IndexBuffer = new Buffer(Device, new BufferDescription()
            {
                BufferFlags = BufferFlags.IndexBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<int>(indices),
            });


            VertexBuffer.SetData(vertices);
            IndexBuffer.SetData(indices);
        }


        public void CreatePipelineState()
        {
            PipelineStateDescription Pipelinedescription = new PipelineStateDescription()
            {
                Framebuffer = Framebuffer,

                InputAssemblyState = new InputAssemblyState()
                {
                    PrimitiveType = PrimitiveType.TriangleList,
                },
                RasterizationState = new RasterizationState()
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.Back,
                    FrontFace = FrontFace.Clockwise
                },
                PipelineVertexInput = new PipelineVertexInput
                {
                    VertexAttributeDescriptions =
                    {
                        new VertexInputAttribute
                        {
                            Binding = 0,
                            Location = 0,
                            Format = PixelFormat.R32G32SFloat,
                            Offset = 0,
                        },
                        new VertexInputAttribute
                        {
                            Binding = 0,
                            Location = 1,
                            Format = PixelFormat.R32G32B32SFloat,
                            Offset = 8,
                        }
                    },
                    VertexBindingDescriptions =
                    {
                        new VertexInputBinding
                        {
                            Binding = 0,
                            InputRate = VertexInputRate.Vertex,
                            Stride = 20,
                        }
                    },
                },
                Shaders = 
                { 
                    ShaderBytecode.LoadFromFile("Shaders/shader.frag", ShaderStage.Fragment),
                    ShaderBytecode.LoadFromFile("Shaders/shader.vert", ShaderStage.Vertex),
                },
            };

            PipelineState = new PipelineState(Pipelinedescription);
        }


        public override void BeginDraw()
        {
            base.BeginDraw();

            CommandBuffer commandBuffer = Context.CommandBuffer;


            commandBuffer.BeginFramebuffer(Framebuffer);

            commandBuffer.SetViewport(Window.Width, Window.Height, 0, 0);
            commandBuffer.SetScissor(Window.Width, Window.Height, 0, 0);

            commandBuffer.SetGraphicPipeline(PipelineState);
            commandBuffer.SetVertexBuffers(new Buffer[] { VertexBuffer });
            commandBuffer.SetIndexBuffer(IndexBuffer);
            commandBuffer.DrawIndexed(3, 1, 0, 0, 0);
        }



        public void Dispose()
        {
            Adapter.Dispose();
        }
    }
}
