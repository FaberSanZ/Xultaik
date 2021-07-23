using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Vortice.Vulkan;
using Vultaik.Desktop;
using Vultaik.Physics;
using Vultaik.GLTF;
using Vultaik;
using Buffer = Vultaik.Buffer;
using Interop = Vultaik.Interop;
using Samples.Common;
using Vultaik.Toolkit;
using Vultaik.Toolkit.GeometricPrimitives;

namespace Samples.SpecularLighting
{
    public class Primitive
    {


        List<VertexPositionNormalTexture> vertices = new();
        List<ushort> indices = new();
        public Primitive(Device device, Vultaik.Toolkit.GeometricPrimitives.PrimitiveType type)
        {

            switch (type)
            {
                case Vultaik.Toolkit.GeometricPrimitives.PrimitiveType.Sphere:
                    Primitives.Sphere(2.0f, 32, out vertices, out indices);
                    break;

                case Vultaik.Toolkit.GeometricPrimitives.PrimitiveType.Torus:
                    Primitives.Torus(2.0f, 0.6f, 32, out vertices, out indices);
                    break;

                case Vultaik.Toolkit.GeometricPrimitives.PrimitiveType.Capsule:
                    Primitives.Capsule(3.0f, 2.0f, 32, out vertices, out indices);
                    break;
                case Vultaik.Toolkit.GeometricPrimitives.PrimitiveType.Cube:
                    Primitives.Cube(1.5f, out vertices, out indices);
                    break;
                case Vultaik.Toolkit.GeometricPrimitives.PrimitiveType.Quad:
                    Primitives.Quad(1.5f, out vertices, out indices);
                    break;

                case Vultaik.Toolkit.GeometricPrimitives.PrimitiveType.Pyramid:
                    Primitives.Pyramid(1.5f, out vertices, out indices);
                    break;
                case Vultaik.Toolkit.GeometricPrimitives.PrimitiveType.Plane:
                    Primitives.Plane(1.0f, out vertices, out indices);
                    break;
                default:
                    break;
            }


            VertexBuffer = new(device, new()
            {
                BufferFlags = BufferFlags.VertexBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = Interop.SizeOf<VertexPositionNormalTexture>(vertices.ToArray()),
            });
            VertexBuffer.SetData(vertices.ToArray());


            IndexBuffer = new(device, new()
            {
                BufferFlags = BufferFlags.IndexBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = Interop.SizeOf<ushort>(indices.ToArray()),
            });
            IndexBuffer.SetData(indices.ToArray());

        }

        public Buffer VertexBuffer;
        public Buffer IndexBuffer;
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Size { get; set; } = new Vector3(1);

        public Matrix4x4 Model { get; set; }

        private void UpdateModel()
        {
            Model = Matrix4x4.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z) * Matrix4x4.CreateTranslation(Position) * Matrix4x4.CreateScale(Size);
        }


        public void Draw(CommandBuffer cmd, GraphicsPipeline pipeline)
        {
            UpdateModel();

            cmd.PushConstant(pipeline, ShaderStage.Vertex, Model);
            cmd.SetVertexBuffer(VertexBuffer);
            cmd.SetIndexBuffer(IndexBuffer, indexType: VkIndexType.Uint16);
            cmd.DrawIndexed(indices.Count, 1, 0, 0, 0);
        }
    }
    public class SpecularLighting : ExampleBase, IDisposable
    {

        private AdapterConfig AdapterConfig;
        private Adapter Adapter;
        private Device Device;
        private Framebuffer Framebuffer;
        private SwapChain SwapChain;
        private GraphicsContext Context;
        private DescriptorSet DescriptorSet_0;

        private Buffer ConstBuffer;
        private Buffer ConstBuffer2;
        private GraphicsPipeline PipelineState_0;
        Primitive Sphere;
        Primitive Torus;
        Primitive Cube;
        Primitive Pyramid;
        Primitive Capsule;
        
        private TransformUniform uniform;
        private Light light;
        private float yaw, pitch, roll = 0;


        public SpecularLighting() : base()
        {

        }


        public override void Initialize()
        {

            AdapterConfig = new()
            {
                SwapChain = true,
            };


            Camera.SetPosition(0, 0, -8.0f);
            Camera.Update();

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

            uniform = new(Camera.Projection, Camera.View);

            light = new()
            {
                Ambient = new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
                Diffuse = new Vector4(0.8f, 0.8f, 0.8f, 1.0f),
                Direction = new Vector3(1, 1, -1.05f),
                Specular = new(1,1,1),
                SpecularPower = 30,
            };



            ConstBuffer = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            });


            ConstBuffer2 = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = Interop.SizeOf<Light>(),
            });

            Sphere = new(Device, Vultaik.Toolkit.GeometricPrimitives.PrimitiveType.Sphere);
            Sphere.Position = new(-2.5f, 1.5f, 0.0f);

            Torus = new(Device, Vultaik.Toolkit.GeometricPrimitives.PrimitiveType.Torus);
            Torus.Position = new(2.5f, 1.5f, 0.0f);

            Pyramid = new(Device, Vultaik.Toolkit.GeometricPrimitives.PrimitiveType.Pyramid);
            Pyramid.Position = new(-2.5f, -1.5f, 0.0f);

            Cube = new(Device, Vultaik.Toolkit.GeometricPrimitives.PrimitiveType.Cube);
            Cube.Position = new(2.5f, -1.5f, 0.0f);

            Capsule = new(Device, Vultaik.Toolkit.GeometricPrimitives.PrimitiveType.Capsule);
            Capsule.Position = new(0, 0, 0.0f);
            Capsule.Size = new(0.4f, 0.4f, 0.4f);

            CreatePipelineState();

            
        }








        public void CreatePipelineState()
        {

            string images = Constants.ImagesFile;
            string fragment = Constants.ShadersFile + @"SpecularLighting\Fragment.hlsl";
            string vertex = Constants.ShadersFile + @"SpecularLighting\Vertex.hlsl";

            Image text1 = ImageFile.Load2DFromFile(Device, images + "UV_Grid_Sm.jpg");

            Sampler sampler = new Sampler(Device);


            GraphicsPipelineDescription Pipelinedescription0 = new();
            Pipelinedescription0.SetFramebuffer(Framebuffer);
            Pipelinedescription0.SetShader(new ShaderBytecode(fragment, ShaderStage.Fragment));
            Pipelinedescription0.SetShader(new ShaderBytecode(vertex, ShaderStage.Vertex));
            Pipelinedescription0.SetVertexBinding(VertexInputRate.Vertex, VertexPositionNormalTexture.Size );
            Pipelinedescription0.SetVertexAttribute(VertexType.Position);
            Pipelinedescription0.SetVertexAttribute(VertexType.Normal);
            Pipelinedescription0.SetVertexAttribute(VertexType.TextureCoordinate);
            //Pipelinedescription0.SetFillMode(VkPolygonMode.Line);
            PipelineState_0 = new(Pipelinedescription0);

            DescriptorData descriptorData_0 = new();
            descriptorData_0.SetUniformBuffer(0, ConstBuffer);
            descriptorData_0.SetImage(1, text1);
            descriptorData_0.SetSampler(2, sampler);
            descriptorData_0.SetUniformBuffer(3, ConstBuffer2);
            DescriptorSet_0 = new(PipelineState_0, descriptorData_0);
             



        }

        public override void Update(ApplicationTime time)
        {
            var timer = time.TotalMilliseconds / (3600);

            Camera.Update();
            light.Direction.X = -14.0f + MathF.Abs(MathF.Sin(MathUtil.Radians(timer * 360.0f)) * 2.0f);
            light.Direction.X = 0.0f + MathF.Sin(MathUtil.Radians(timer * 360.0f)) * MathF.Cos(MathUtil.Radians(timer * 360.0f)) * 2.0f;
            light.Direction.Y = 0.0f + MathF.Sin(MathUtil.Radians(timer * 360.0f)) * 2.0f;
            light.Direction.Z = 0.0f + MathF.Cos(MathUtil.Radians(timer * 360.0f)) * 2.0f;

            ConstBuffer2.SetData(ref light);


            uniform.Update(Camera);
            ConstBuffer.SetData(ref uniform);


            Sphere.Rotation = new(yaw, -yaw, yaw);
            Torus.Rotation = new(-yaw, yaw, -yaw);
            Pyramid.Rotation = new(-yaw,- yaw, yaw);
            Cube.Rotation = new(yaw, -yaw, -yaw);
            Capsule.Rotation = new(-yaw, yaw, -yaw);


            yaw = timer;
        }
   

        public override void Draw(ApplicationTime time)
        {

            Device.WaitIdle();
            CommandBuffer cmd = Context.CommandBuffer;

            cmd.Begin();
            cmd.BeginFramebuffer(Framebuffer);
            cmd.SetScissor(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);
            cmd.SetViewport(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);


            cmd.BindDescriptorSets(DescriptorSet_0);
            cmd.SetGraphicPipeline(PipelineState_0);

            Sphere.Draw(cmd, PipelineState_0);
            Torus.Draw(cmd, PipelineState_0);
            Pyramid.Draw(cmd, PipelineState_0);
            Cube.Draw(cmd, PipelineState_0);
            Capsule.Draw(cmd, PipelineState_0);


            cmd.Close();
            Device.Submit(cmd);
            SwapChain.Present();
        }

        public override void Resize(int width, int height)
        {
            Device.WaitIdle();
            SwapChain.Resize(width, height);
            Framebuffer.Resize();

            Camera.AspectRatio = (float)width / height;
        }




        public void Dispose()
        {
            //ConstBuffer.Dispose();
            //ConstBuffer2.Dispose();
            //ConstBuffer3.Dispose();
            //ConstBuffer4.Dispose();
        }
    }



    [StructLayout(LayoutKind.Sequential)]
    public struct TransformUniform
    {
        public TransformUniform(Matrix4x4 p, Matrix4x4 v)
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


    [StructLayout(LayoutKind.Sequential)]
    public struct Light
    {

        public Vector4 Ambient;

        public Vector4 Diffuse;

        public Vector3 Direction;

        public float SpecularPower;
        public Vector3 Specular;

        public Light(Vector4 D, Vector3 DI, Vector4 AM, float sp, Vector3 s)
        {
            Diffuse = D;
            Direction = DI;
            Ambient = AM;
            SpecularPower = sp;
            Specular = s;
        }
    }
}
