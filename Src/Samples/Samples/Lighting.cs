using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Zeckoxe.Core;
using Zeckoxe.Desktop;
using Zeckoxe.Engine;
using Zeckoxe.Games;
using Zeckoxe.GLTF;
using Zeckoxe.Graphics;
using Zeckoxe.Graphics.Toolkit;
using Buffer = Zeckoxe.Graphics.Buffer;


namespace Samples.Samples
{
    public class Lighting : Game, IDisposable
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


        public Lighting() : base()
        {

        }




        public Camera camera { get; set; }
        public GameTime GameTime { get; set; }
        public GLTFLoader<VertexPositionNormalTexture> GLTFModel { get; set; }
        //public List<Mesh> Meshes { get; private set; }


        public DescriptorSet Descriptor;
        public DescriptorSet Descriptor2;
        public DescriptorSet Descriptor3;
        public Buffer ConstBuffer;
        public Buffer ConstBuffer2;
        public Buffer ConstBuffer3;
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
            Window.Title += " - (Lighting) ";
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
            camera.SetPosition(0, -8, -40.0f);
            camera.AspectRatio = (float)Window.Width / Window.Height;
            //Camera.SetLens(Window.Width, Window.Height);


            // Reset Model
            Model = Matrix4x4.Identity;

            uniform = new(camera.Projection, Model, camera.View);


            CreatePipelineState();


            Texture2D text1 = Texture2D.LoadFromFile(Device, "UVCheckerMap08-512.png");
            Texture2D text2 = Texture2D.LoadFromFile(Device, "IndustryForgedDark512.ktx");
            Texture2D text3 = Texture2D.LoadFromFile(Device, "floor_tiles.bmp");

            Sampler sampler = new Sampler(Device);


            ConstBuffer = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            });

            
            Descriptor = new(PipelineState);
            Descriptor.SetUniformBuffer(0, ConstBuffer);
            Descriptor.SetTexture2DSampler(1, text1, sampler);
            Descriptor.Build();



            ConstBuffer2 = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            });
            Descriptor2 = new(PipelineState);
            Descriptor2.SetUniformBuffer(0, ConstBuffer2);
            Descriptor2.SetTexture2DSampler(1, text2, sampler);
            Descriptor2.Build();



            ConstBuffer3 = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            });


            Descriptor3 = new(PipelineState);
            Descriptor3.SetUniformBuffer(0, ConstBuffer3);
            Descriptor3.SetTexture2DSampler(1, text3, sampler);
            Descriptor3.Build();


            GLTFModel = new(Device, "Models/mesh_mat.gltf");

            yaw = 0f;
            pitch = 0;
            roll = 0;
        }








        public void CreatePipelineState()
        {
            Shaders["Fragment"] = ShaderBytecode.LoadFromFile("Shaders/Lighting/shader.frag", ShaderStage.Fragment);
            Shaders["Vertex"] = ShaderBytecode.LoadFromFile("Shaders/Lighting/shader.vert", ShaderStage.Vertex);


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
                    Offset = Interop.SizeOf<Vector3>(),
                },
                new()
                {
                    Binding = 0,
                    Location = 2,
                    Format = PixelFormat.R32G32SFloat,
                    Offset = Interop.SizeOf<Vector2>() + Interop.SizeOf<Vector3>(),
                },
            };

            List<VertexInputBinding> VertexBindingDescriptions = new()
            {
                new()
                {
                    Binding = 0,
                    InputRate = VertexInputRate.Vertex,
                    Stride = VertexPositionNormalTexture.Size,
                },
            };


            PipelineState = new(new()
            {
                Framebuffer = Framebuffer,

                Layouts =
                {
                    new()
                    {
                        Stage = ShaderStage.Vertex,
                        Type = DescriptorType.UniformBuffer,
                        Binding = 0,
                    },

                    new()
                    {
                        Stage = ShaderStage.Fragment,
                        Type = DescriptorType.CombinedImageSampler,
                        Binding = 1,
                    },
                },

                PushConstants =
                {
                    new(ShaderStage.Vertex, 0 , Interop.SizeOf<Matrix4x4>())
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


            Model = Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix4x4.CreateTranslation(11.0f, .3f, 0.0f);
            uniform.Update(camera, Model);
            ConstBuffer.SetData(ref uniform);


            Model = Matrix4x4.CreateFromYawPitchRoll(-yaw, pitch, roll) * Matrix4x4.CreateTranslation(0, 1.0f, 0.0f);
            uniform.Update(camera, Model);
            ConstBuffer2.SetData(ref uniform);


            Model = Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix4x4.CreateTranslation(-11.0f, .3f, 0.0f);
            uniform.Update(camera, Model);
            ConstBuffer3.SetData(ref uniform);


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
            commandBuffer.BindDescriptorSets(Descriptor);
            GLTFModel.Draw(commandBuffer, PipelineState);


            commandBuffer.SetGraphicPipeline(PipelineState);
            commandBuffer.BindDescriptorSets(Descriptor2);
            GLTFModel.Draw(commandBuffer, PipelineState);


            commandBuffer.SetGraphicPipeline(PipelineState);
            commandBuffer.BindDescriptorSets(Descriptor3);
            GLTFModel.Draw(commandBuffer, PipelineState);
        }



        public void Dispose()
        {
            Adapter.Dispose();
        }
    }
}
