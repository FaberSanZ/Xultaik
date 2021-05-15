// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Vortice.Vulkan;
using Vultaik.Desktop;
using Vultaik.Engine;
using Vultaik.GLTF;
using Vultaik;
using Vultaik.Physics;
using Buffer = Vultaik.Buffer;
using Interop = Vultaik.Interop;
using Samples.Common;

namespace Samples.LoadGLTF
{


    public class LoadGLTFExample : Application, IDisposable
    {

        public Camera camera { get; set; }
        public ApplicationTime GameTime { get; set; }
        public ModelAssetImporter<VertexPositionNormal> GLTFModel { get; set; }
        //public List<Mesh> Meshes { get; private set; }


        public Buffer ConstBuffer;
        public DescriptorSet DescriptorSet { get; set; }
        public GraphicsPipeline PipelineState;


        public LoadGLTFExample() : base()
        {

        }



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

            camera = new Camera(45f, 1f, 0.1f, 64f);
            camera.SetPosition(0, 0, -4.0f);
            camera.AspectRatio = (float)Window.Width / Window.Height;
            camera.Update();


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

            GLTFModel = new(Device, Constants.ModelsFile + "DamagedHelmet.gltf");

            yaw = 0f;
            pitch = 0;
            roll = 0;
        }

        public void CreatePipelineState()
        {


            string shaders = Constants.ShadersFile;

            string Fragment = shaders + "LoadGLTF/shader.frag";
            string Vertex = shaders + "LoadGLTF/shader.vert";


            PipelineStateDescription pipelineStateDescription = new();
            pipelineStateDescription.SetFramebuffer(Framebuffer);
            pipelineStateDescription.SetProgram(new[] { Fragment, Vertex });
            pipelineStateDescription.SetVertexBinding(VkVertexInputRate.Vertex, VertexPositionNormal.Size);
            pipelineStateDescription.SetVertexAttribute(VertexType.Position);
            pipelineStateDescription.SetVertexAttribute(VertexType.Color);
            PipelineState = new(pipelineStateDescription);

            DescriptorData descriptorData = new();
            descriptorData.SetUniformBuffer(0, ConstBuffer);
            DescriptorSet = new DescriptorSet(PipelineState, descriptorData);
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

            commandBuffer.BeginFramebuffer(Framebuffer);
            commandBuffer.SetScissor(Window.Width, Window.Height, 0, 0);
            commandBuffer.SetViewport(Window.Width, Window.Height, 0, 0);

            commandBuffer.SetGraphicPipeline(PipelineState);
            commandBuffer.BindDescriptorSets(DescriptorSet);

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
