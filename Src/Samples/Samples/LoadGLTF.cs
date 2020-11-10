// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	LoadGLTF.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Zeckoxe.Core;
using Zeckoxe.Desktop;
using Zeckoxe.Engine;
using Zeckoxe.Games;
using Zeckoxe.GLTF;
using Zeckoxe.Graphics;
using Zeckoxe.Graphics.Toolkit;
using Zeckoxe.Physics;
using Buffer = Zeckoxe.Graphics.Buffer;

namespace Samples.Samples
{

    public class LoadGLTF : Game, IDisposable
    {
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


        public LoadGLTF() : base()
        {

        }




        public Camera Camera { get; set; }
        public GameTime GameTime { get; set; }
        public GLTFLoader GLTFModel { get; set; }
        public int[] Indices { get; set; }
        public VertexPositionNormal[] Vertices { get; set; }


        public Dictionary<string, DescriptorSet> DescriptorSets = new();
        public Dictionary<string, Buffer> Buffers = new();
        public Dictionary<string, GraphicsPipelineState> PipelineStates = new();
        public Dictionary<string, ShaderBytecode> Shaders = new();

        // TransformUniform 
        public TransformUniform uniform;
        public float yaw;
        public float pitch;
        public float roll;


        public override void InitializeSettings()
        {
            base.InitializeSettings();
            Parameters.Settings.Validation = ValidationType.None;
            Window.Title += " - (LoadGLTF) ";
        }

        public override void Initialize()
        {
            base.Initialize();

            Camera = new()
            {
                Mode = CameraType.Free,
                Position = new(1, -.3f, -3.5f),
            };

            Camera.SetLens(Window.Width, Window.Height);


            // Reset Model
            Model = Matrix4x4.Identity;

            uniform = new(Camera.Projection, Model, Camera.View);


            GLTFModel = new("Models/DamagedHelmet.gltf");

            Indices = GLTFModel.Indices;
            Vertices = GLTFModel.GetPositionNormalAsArray();


            CreateBuffers();
            CreatePipelineState();



            List<DescriptorPool> pool = new()
            {
                new DescriptorPool(DescriptorType.UniformBuffer, 1),
            };


            DescriptorSets["Descriptor1"] = new(PipelineStates["Solid"], pool);
            DescriptorSets["Descriptor1"].SetUniformBuffer(0, Buffers["ConstBuffer1"]); // Binding 0: Uniform buffer (Vertex shader)
            DescriptorSets["Descriptor1"].Build();

            yaw = 0f;
            pitch = 4.5f;
            roll = 0.15f;
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


        }


        public void CreatePipelineState()
        {
            Shaders["Fragment"] = ShaderBytecode.LoadFromFile("Shaders/PositionColor/shader.frag", ShaderStage.Fragment);
            Shaders["Vertex"] = ShaderBytecode.LoadFromFile("Shaders/PositionColor/shader.vert", ShaderStage.Vertex);


            List<VertexInputAttribute> VertexAttributeDescriptions = new()
            {

                new()
                {
                    Binding = 0,
                    Location = 0,
                    Format = PixelFormat.R32G32B32SFloat,
                    Offset = 0,
                },
                new()
                {
                    Binding = 0,
                    Location = 1,
                    Format = PixelFormat.R32G32B32SFloat,
                    Offset = 12,
                }
            };

            List<VertexInputBinding> VertexBindingDescriptions = new()
            {
                new()
                {
                    Binding = 0,
                    InputRate = VertexInputRate.Vertex,
                    Stride = VertexPositionNormal.Size,
                }
            };


            PipelineStates["Solid"] = new(new() 
            {
                Framebuffer = Framebuffer,

                Layouts =
                {
                    // Binding 0: Uniform buffer (Vertex shader)
                    new()
                    {
                        Stage = ShaderStage.Vertex,
                        Type = DescriptorType.UniformBuffer,
                        Binding = 0,
                    }
                },

                InputAssemblyState = InputAssemblyState.Default(),

                RasterizationState = new()
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.None,
                    FrontFace = FrontFace.Clockwise,
                },
                PipelineVertexInput = new()
                {
                    VertexAttributeDescriptions = VertexAttributeDescriptions,
                    VertexBindingDescriptions = VertexBindingDescriptions,
                },
                Shaders =
                {
                    Shaders["Fragment"],
                    Shaders["Vertex"],
                },

            });
        }



        public override void Update(GameTime game)
        {
            Camera.Update(game);


            Model = Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix4x4.CreateTranslation(0.0f, 0.0f, 0.0f);
            uniform.Update(Camera, Model);
            Buffers["ConstBuffer1"].SetData(ref uniform);



            yaw += 0.0006f * MathF.PI;

        }




        public override void BeginDraw()
        {
            base.BeginDraw();

            CommandBuffer commandBuffer = Context.CommandBuffer;

            commandBuffer.BeginFramebuffer(Framebuffer, .5f, .5f, .5f);
            commandBuffer.SetViewport(Window.Width, Window.Height, 0, 0);
            commandBuffer.SetScissor(Window.Width, Window.Height, 0, 0);

            commandBuffer.SetVertexBuffers(new Buffer[] { Buffers["VertexBuffer"] });
            commandBuffer.SetIndexBuffer(Buffers["IndexBuffer"]);



            commandBuffer.SetGraphicPipeline(PipelineStates["Solid"]);
            commandBuffer.BindDescriptorSets(DescriptorSets["Descriptor1"]);
            commandBuffer.DrawIndexed(Indices.Length, 1, 0, 0, 0);


        }



        public void Dispose()
        {
            Adapter.Dispose();
        }
    }
}
