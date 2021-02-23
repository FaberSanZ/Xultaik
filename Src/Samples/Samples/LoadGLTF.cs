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
using Zeckoxe.GLTF;
using Zeckoxe.Vulkan;
using Zeckoxe.Vulkan.Toolkit;
//using Zeckoxe.Physics;
using Buffer = Zeckoxe.Vulkan.Buffer;

namespace Samples.Samples
{

    public class LoadGLTF : Application, IDisposable
    {
        public class Camera
        {
            /// <summary>Corection matrix for vulkan projection</summary>
            public static readonly Matrix4x4 VKProjectionCorrection = new Matrix4x4(
                    1, 0, 0, 0,
                    0, -1, 0, 0,
                    0, 0, 1f / 2, 0,
                    0, 0, 1f / 2, 1
                );

            public enum CamType { LookAt, FirstPerson };

            float fov, aspectRatio, zNear = 0.1f, zFar = 128f, zoom = 1.0f;
            float moveSpeed = 0.1f, rotSpeed = 0.01f, zoomSpeed = 0.01f;

            Vector3 rotation = Vector3.Zero;
            Vector3 position = Vector3.Zero;
            Matrix4x4 model = Matrix4x4.Identity;

            public Vector3 Position => position;
            public Vector3 Rotation => rotation;
            public float NearPlane => zNear;
            public float FarPlane => zFar;

            public CamType Type;

            public float AspectRatio
            {
                get { return aspectRatio; }
                set
                {
                    aspectRatio = value;
                    Update();
                }
            }
            public float FieldOfView
            {
                get { return fov; }
                set
                {
                    fov = value;
                    Update();
                }
            }
            public Matrix4x4 Perspective
            {
                get { return Matrix4x4.CreatePerspectiveFieldOfView(fov, aspectRatio, zNear, zFar); }
            }

            public Camera(float fieldOfView, float aspectRatio, float nearPlane = 0.1f, float farPlane = 16f)
            {
                fov = fieldOfView;
                this.aspectRatio = aspectRatio;
                zNear = nearPlane;
                zFar = farPlane;
                Update();
            }

            public void Rotate(float x, float y, float z = 0)
            {
                rotation.X += rotSpeed * x;
                rotation.Y += rotSpeed * y;
                rotation.Z += rotSpeed * z;
                Update();
            }
            public float Zoom
            {
                get { return zoom; }
                set
                {
                    zoom = value;
                    Update();
                }
            }
            public void SetRotation(float x, float y, float z = 0)
            {
                rotation.X = x;
                rotation.Y = y;
                rotation.Z = z;
                Update();
            }
            public void SetPosition(float x, float y, float z = 0)
            {
                position.X = x;
                position.Y = y;
                position.Z = z;
                Update();
            }
            public void Move(float x, float y, float z = 0)
            {
                position.X += moveSpeed * x;
                position.Y += moveSpeed * y;
                position.Z += moveSpeed * z;
                Update();
            }
            public void SetZoom(float factor)
            {
                zoom += zoomSpeed * factor;
                Update();
            }

            public Matrix4x4 Projection { get; private set; }
            public Matrix4x4 View { get; private set; }
            public Matrix4x4 Model
            {
                get { return model; }
                set
                {
                    model = value;
                    Update();
                }
            }

            public Matrix4x4 SkyboxView
            {
                get
                {
                    return
                        Matrix4x4.CreateFromAxisAngle(Vector3.UnitZ, rotation.Z) *
                        Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, rotation.Y) *
                        Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, rotation.X);
                }
            }

            public Matrix4x4 CreatePerspectiveFieldOfView(float fov, float aspectRatio, float zNear, float zFar)
            {
                float f = (float)(1.0 / System.Math.Tan(0.5 * fov));
                return new Matrix4x4(
                    f / aspectRatio, 0, 0, 0,
                    0, -f, 0, 0,
                    0, 0, zFar / (zNear - zFar), -1,
                    0, 0, zNear * zFar / (zNear - zFar), 0
                );
            }

            public void Update()
            {
                Projection = CreatePerspectiveFieldOfView(fov, aspectRatio, zNear, zFar);

                Matrix4x4 translation = Matrix4x4.CreateTranslation(position * zoom);// * new Vector3(1,1,-1)) ;
                if (Type == CamType.LookAt)
                {
                    View =
                            Matrix4x4.CreateFromAxisAngle(Vector3.UnitZ, rotation.Z) *
                            Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, rotation.Y) *
                            Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, rotation.X) *
                            translation;
                }
                else
                {
                    View = translation *
                            Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, rotation.X) *
                            Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, rotation.Y) *
                            Matrix4x4.CreateFromAxisAngle(Vector3.UnitZ, rotation.Z);
                }
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


        public LoadGLTF() : base()
        {

        }




        public Camera camera { get; set; }
        public ApplicationTime GameTime { get; set; }
        public GLTFLoader<VertexPositionNormal> GLTFModel { get; set; }
        //public List<Mesh> Meshes { get; private set; }


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

        public float DegreesToRadians(float degrees)
        {
            return degrees * (float)Math.PI / 180f;
        }

        public override void Initialize()
        {
            base.Initialize();

            camera = new Camera(DegreesToRadians(45f), 1f, 0.1f, 64f);
            camera.SetRotation(DegreesToRadians(0), 0, 0);
            camera.SetPosition(0, 0, -4.0f);
            camera.AspectRatio = (float)Window.Width / Window.Height;
            //Camera.SetLens(Window.Width, Window.Height);


            // Reset Model
            Model = Matrix4x4.Identity;

            uniform = new(camera.Projection, Model, camera.View);





            ConstBuffer = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            });


            CreatePipelineState();


            GLTFModel = new(Device, "Models/DamagedHelmet.gltf");

            yaw = 0f;
            pitch = 0;
            roll = 0;
        }








        public void CreatePipelineState()
        {
            Shaders["Fragment"] = ShaderBytecode.LoadFromFile("Shaders/LoadGLTF/shader.frag", ShaderStage.Fragment);
            Shaders["Vertex"] = ShaderBytecode.LoadFromFile("Shaders/LoadGLTF/shader.vert", ShaderStage.Vertex);


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

            PipelineStateDescription pipelineStateDescription = new()
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

            };
            pipelineStateDescription.SetUniformBuffer(0, ShaderStage.Vertex, ConstBuffer); // Binding 0: Uniform buffer (Vertex shader)

            PipelineState = new(pipelineStateDescription);
        }



        public override void Update(ApplicationTime game)
        {

            Model = Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix4x4.CreateTranslation(0.0f, .0f, 0.0f);
            uniform.Update(camera, Model);
            ConstBuffer.SetData(ref uniform);

            yaw += 0.0006f * MathF.PI;
        }




        public override void BeginDraw()
        {
            base.BeginDraw();

            CommandBuffer commandBuffer = Context.CommandBuffer;

            commandBuffer.BeginFramebuffer(Framebuffer, .5f, .5f, .5f);
            commandBuffer.SetScissor(Window.Width, Window.Height, 0, 0);
            commandBuffer.SetViewport(Window.Width, Window.Height, 0, 0);

            commandBuffer.SetGraphicPipeline(PipelineState);


            commandBuffer.SetVertexBuffers(new[] { GLTFModel.VertexBuffer });
            commandBuffer.SetIndexBuffer(GLTFModel.IndexBuffer, 0, GLTFModel.IndexType);


            foreach (Scene sc in GLTFModel.Scenes)
            {
                foreach (Node node in sc.Root.Children)
                {
                    RenderNode(commandBuffer, node, sc.Root.LocalMatrix);
                }
            }

        }

        public void RenderNode(CommandBuffer cmd, Node node, Matrix4x4 currentTransform)
        {
            Matrix4x4 localMat = node.LocalMatrix * currentTransform;

            cmd.PushConstant<Matrix4x4>(PipelineState, ShaderStage.Vertex, localMat);

            if (node.Mesh is not null)
                foreach (Primitive p in node.Mesh.Primitives)
                    cmd.DrawIndexed(p.IndexCount, 1, p.FirstIndex, p.FirstVertex, 0);


            if (node.Children is null)
                return;

            foreach (Node child in node.Children)
                RenderNode(cmd, child, localMat);
        }



        public void Dispose()
        {
            Adapter.Dispose();
        }
    }
}
