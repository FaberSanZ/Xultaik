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
using Zeckoxe.GLTF.SharpGLTFNew;
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
        public GLTFLoader<VertexPositionNormal> GLTFModel { get; set; }
        //public List<Mesh> Meshes { get; private set; }


        public DescriptorSet Descriptor;
        public Buffer ConstBuffer;
        public GraphicsPipelineState PipelineState;
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



            CreatePipelineState();


            ConstBuffer = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            });



            Descriptor = new(PipelineState);
            Descriptor.SetUniformBuffer(0, ConstBuffer); // Binding 0: Uniform buffer (Vertex shader)
            Descriptor.Build();

            GLTFModel = new(Device, "Models/DamagedHelmet.gltf");

            yaw = 0f;
            pitch = 4.5f;
            roll = 0.15f;
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


            PipelineState = new(new() 
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
                    FrontFace = FrontFace.CounterClockwise,
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
            ConstBuffer.SetData(ref uniform);



            yaw += 0.0006f * MathF.PI;

        }




        public override void BeginDraw()
        {
            base.BeginDraw();

            CommandBuffer commandBuffer = Context.CommandBuffer;

            commandBuffer.BeginFramebuffer(Framebuffer, .5f, .5f, .5f);
            commandBuffer.SetViewport(Window.Width, Window.Height, 0, 0);
            commandBuffer.SetScissor(Window.Width, Window.Height, 0, 0);

            commandBuffer.SetGraphicPipeline(PipelineState);
            commandBuffer.BindDescriptorSets(Descriptor);

            GLTFModel.Draw(commandBuffer);
            //GLTFModel.DrawAll(commandBuffer);


        }



        public void Dispose()
        {
            Adapter.Dispose();
        }
    }
}
