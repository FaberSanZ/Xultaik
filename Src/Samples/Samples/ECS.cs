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
using Buffer = Zeckoxe.Vulkan.Buffer;

namespace Samples.Samples
{

    public interface IComponent
    {
        public Entity Entity { get; set; }
        public DeviceInfo DeviceInfo { get; set; }

        public Transform Transform { get; set; }
        public void Update();
        public void Draw(CommandBuffer command);
        public void Dispose();
    }


    public class DeviceInfo
    {
        public Entity Entity { get; set; }

        public Device Device { get; set; }
        public Framebuffer Framebuffer { get; set; }
    }


    public class Entity
    {
        public Transform Transform;

        public List<IComponent> Components { get; set; } = new List<IComponent>();
        public List<string> ComponentsNames { get; set; } = new List<string>();
        public DeviceInfo DeviceInfo { get; }

        public Entity(DeviceInfo deviceInfo)
        {
            Transform = new Transform();
            DeviceInfo = deviceInfo;

            //SceneNode.Set
        }


        public Entity(Application game)
        {
            Transform = new Transform();
            DeviceInfo = new()
            {
                Device = game.Device,
                Framebuffer = game.Framebuffer,
            };
        }

        public void Update()
        {
            foreach (IComponent component in Components)
            {
                component.Update();
            }
        }

        public void Draw(CommandBuffer command)
        {
            foreach (IComponent component in Components)
            {
                component.Draw(command);
            }
        }

        public void Dispose()
        {
            foreach (IComponent component in Components)
            {
                component.Dispose();
            }
        }

        public void AddComponent(IComponent Component, string name = "")
        {
            Component.DeviceInfo = DeviceInfo;
            ComponentsNames.Add(name);
            Components.Add(Component);
            Component.Entity = this;
        }


        public IComponent Get(string name)
        {
            for (int i = 0; i < ComponentsNames.Count; i++)
            {
                if (ComponentsNames[i] == name)
                    return Components[i];
            }

            return null;

        }
    }


    public class Transform
    {
        /// <summary>
        /// Is the entity static or can the entity change every frame
        /// </summary>
        public bool IsStatic = false;
        /// <summary>
        /// Position of the entity
        /// </summary>
        public Vector3 Position = new Vector3(0, 0, 0);
        /// <summary>
        /// Rotation of the entity
        /// </summary>
        public Quaternion Rotation = Quaternion.Identity;
        /// <summary>
        /// Scale of the entity
        /// </summary>
        public Vector3 Scale = new Vector3(1, 1, 1);

        /// <summary>
        /// Returns model matrix of the entity
        /// </summary>
        public Matrix4x4 Matrix
        {
            get
            {
                Matrix4x4 mat = Matrix4x4.Identity;
                mat *= Matrix4x4.CreateScale(Scale);
                mat *= Matrix4x4.CreateFromQuaternion(Rotation);
                mat *= Matrix4x4.CreateTranslation(Position);
                return mat;
            }
            set => Matrix4x4.Decompose(value, out Scale, out Rotation, out Position);
        }
        /// <summary>
        /// Returns right vector of the entity
        /// </summary>
        public Vector3 Right
        {
            get
            {
                Matrix4x4 mat = Matrix;
                return Vector3.Normalize(new Vector3(mat.M11, mat.M12, mat.M13));
            }
        }
        /// <summary>
        /// Returns up vector of the entity
        /// </summary>
        public Vector3 Up
        {
            get
            {
                Matrix4x4 mat = Matrix;
                return Vector3.Normalize(new Vector3(mat.M21, mat.M22, mat.M23));
            }
        }
        /// <summary>
        /// Returns forward vector of the entity
        /// </summary>
        public Vector3 Forward
        {
            get
            {
                Matrix4x4 mat = Matrix;
                return Vector3.Normalize(new Vector3(mat.M31, mat.M32, mat.M33));
            }
        }

    }


    public class CameraComponent : IComponent
    {
        public class Camera
        {
        

            public enum CamType { LookAt, FirstPerson };

            private float fov;
            private float aspectRatio;
            private readonly float zNear = 0.1f;
            private readonly float zFar = 128f;
            private float zoom = 1.0f;
            private readonly float moveSpeed = 0.1f, rotSpeed = 0.01f, zoomSpeed = 0.01f;
            private Vector3 rotation = Vector3.Zero;
            private Vector3 position = Vector3.Zero;
            private Matrix4x4 model = Matrix4x4.Identity;

            public Vector3 Position => position;
            public Vector3 Rotation => rotation;
            public float NearPlane => zNear;
            public float FarPlane => zFar;

            public CamType Type;

            public float AspectRatio
            {
                get => aspectRatio;
                set
                {
                    aspectRatio = value;
                    Update();
                }
            }
            public float FieldOfView
            {
                get => fov;
                set
                {
                    fov = value;
                    Update();
                }
            }
            public Matrix4x4 Perspective => Matrix4x4.CreatePerspectiveFieldOfView(fov, aspectRatio, zNear, zFar);

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
                get => zoom;
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
                get => model;
                set
                {
                    model = value;
                    Update();
                }
            }

            public Matrix4x4 SkyboxView => Matrix4x4.CreateFromAxisAngle(Vector3.UnitZ, rotation.Z) *
                        Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, rotation.Y) *
                        Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, rotation.X);

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


        public Entity Entity { get; set; }
        public DeviceInfo DeviceInfo { get; set; }



        public Camera camera { get; set; }
        public Transform Transform { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public float DegreesToRadians(float degrees)
        {
            return degrees * (float)Math.PI / 180f;
        }


        public CameraComponent(Window window)
        {
            camera = new Camera(DegreesToRadians(45f), 1f, 0.1f, 64f);
            camera.SetRotation(DegreesToRadians(0), 0, 0);
            camera.SetPosition(0, 0, -4.0f);
            camera.AspectRatio = (float)window.Width / window.Height;
        }

        public void Dispose()
        {
        }

        public void Draw(CommandBuffer command)
        {
        }

        public void Update()
        {
        }
    }



    public class Model : IComponent
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct TransformUniform
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



            public void Update(CameraComponent camera, Matrix4x4 m)
            {
                P = camera.camera.Projection;
                M = m;
                V = camera.camera.View;
            }
        }


        public Model(string path, CameraComponent camera, DeviceInfo deviceInfo)
        {
            GLTFModel = new(deviceInfo.Device, path);
            DeviceInfo = deviceInfo;
            Transform = new Transform();
            ConstBuffer = new(deviceInfo.Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            });

            uniform = new(camera.camera.Projection, Matrix4x4.Identity, camera.camera.View);
            CreatePipelineState();

            Camera = camera;
        }

        public Entity Entity { get; set; }
        public Transform Transform { get; set; }


        public Buffer ConstBuffer;
        public GraphicsPipelineState PipelineState;
        public Dictionary<string, ShaderBytecode> Shaders = new();

        public GLTFLoader<VertexPositionNormalTexture> GLTFModel { get; set; }
        public DeviceInfo DeviceInfo { get; set; }
        public CameraComponent Camera { get; }

        internal TransformUniform uniform;

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




            Image2D text1 = Image2D.LoadFromFile(DeviceInfo.Device, "UVCheckerMap08-512.png");
            Image2D text2 = Image2D.LoadFromFile(DeviceInfo.Device, "IndustryForgedDark512.ktx");
            Image2D text3 = Image2D.LoadFromFile(DeviceInfo.Device, "floor_tiles.bmp");

            List<Image2D> textures = new List<Image2D>()
            {
                text1,
                text2,
                text3,
            };

            Sampler sampler = new Sampler(DeviceInfo.Device);


            Random random = new Random();

            int index = random.Next(0, textures.Count);

            //Console.WriteLine(index);

            PipelineStateDescription pipelineStateDescription = new()
            {
                Framebuffer = DeviceInfo.Framebuffer,

                PushConstants =
                {
                    new(ShaderStage.Vertex, 0, Interop.SizeOf<Matrix4x4>())
                },

                InputAssemblyState = InputAssemblyState.Default(),
                RasterizationState = RasterizationState.Default(),

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
            pipelineStateDescription.SetImageSampler(1, ShaderStage.Fragment, textures[index], sampler);

            PipelineState = new(pipelineStateDescription);
        }


        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void Draw(CommandBuffer command)
        {
            command.SetGraphicPipeline(PipelineState);
            GLTFModel.Draw(command, PipelineState);
        }

        public void Update()
        {
            uniform.Update(Camera, Transform.Matrix);
            ConstBuffer.SetData(ref uniform);
        }
    }
    public class ECS : Application, IDisposable
    {
        private Entity Entity;

        public ECS() : base()
        {

        }

        public override void InitializeSettings()
        {
            base.InitializeSettings();
            Parameters.Settings.Validation = ValidationType.None;
            Window.Title += " - (ECS) ";
        }
        CameraComponent cameraComponent;


        public override void Initialize()
        {
            base.Initialize();

            Entity = new Entity(this);
            cameraComponent = new CameraComponent(Window);

            for (int x = -2; x < 3; x++)
            {
                for (int y = -2; y < 3; y++)
                {
                    Entity.AddComponent(new Model("Models/mesh_mat.gltf", cameraComponent, new DeviceInfo() { Device = Device, Framebuffer = Framebuffer })
                    {

                        Transform = new Transform()
                        {
                            Position = new Vector3(x, y, 0),
                            Scale = new Vector3(.05f, .05f, .05f)
                        },

                    }, "Model");
                }


            }
        }

        private void AddComponent()
        {
            var arg3 = Input.Mice[0].Position;

            if (Input.Mice[0].IsButtonPressed(MouseButton.Left))
            {
                Matrix4x4 p = cameraComponent.camera.Projection;

                // Convert screen pixel to view space.
                float vx = (2f * arg3.X / Window.Width - 1f) / p.M11;
                float vy = (-2f * arg3.Y / Window.Height + 1f) / p.M22;

                var ray = new Ray(Vector3.Zero, new Vector3(vx, vy, 1));
                Matrix4x4 v = cameraComponent.camera.View;
                Matrix4x4.Invert(v, out var invView);

                Matrix4x4 toWorld = invView;

                ray = new Ray(Vector3.TransformNormal(ray.Position, toWorld), Vector3.TransformNormal(ray.Direction, toWorld));

                Entity.AddComponent(new Model("Models/mesh_mat.gltf", new CameraComponent(Window), new DeviceInfo() { Device = Device, Framebuffer = Framebuffer })
                {
                      
                    Transform = new Transform()
                    {
                        Position = new Vector3(ray.Direction.X * 3.5f, -(ray.Direction.Y * 3.5f), 0),
                        Scale = new Vector3(.05f, .05f, .05f)
                    },

                });
            }

        }

        private float x = 0.0f;
        public override void Update(ApplicationTime game)
        {
            cameraComponent.camera.Update();


            for (int i = 0; i < Entity.Components.Count; i++)
            {
                Entity.Components[i].Transform.Rotation = Quaternion.CreateFromYawPitchRoll(x, 0, 0);
            }
            //(Entity.Get("Model") as Model).Transform
            AddComponent();

            Entity.Update();

            x += 0.003f;



            Console.WriteLine(Entity.Components.Count);
            //Console.Clear();
        }




        public override void BeginDraw()
        {
            base.BeginDraw();

            CommandBuffer commandBuffer = Context.CommandBuffer;

            commandBuffer.BeginFramebuffer(Framebuffer, .5f, .5f, .5f);
            commandBuffer.SetScissor(Window.Width, Window.Height, 0, 0);
            commandBuffer.SetViewport(Window.Width, Window.Height, 0, 0);

            Entity.Draw(commandBuffer);
        }



        public void Dispose()
        {
            Adapter.Dispose();
        }
    }


    /// <summary>
    /// Represents a three dimensional line based on a point in space and a direction.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Ray : IFormattable
    {
        /// <summary>
        /// The position in three dimensional space where the ray starts.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The normalized direction in which the ray points.
        /// </summary>
        public Vector3 Direction;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ray"/> struct.
        /// </summary>
        /// <param name="position">The position in three dimensional space of the origin of the ray.</param>
        /// <param name="direction">The normalized direction of the ray.</param>
        public Ray(Vector3 position, Vector3 direction)
        {
            this.Position = position;
            this.Direction = direction;
        }

 

       
       

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "Position:{0} Direction:{1}", Position.ToString(format, formatProvider),
                Direction.ToString(format, formatProvider));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Position.GetHashCode() * 397) ^ Direction.GetHashCode();
            }
        }

        

    }
}
