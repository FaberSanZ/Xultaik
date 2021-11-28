// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Vortice.Vulkan;
using Vultaik.Desktop;
using Vultaik.GLTF;
using Vultaik;
using Vultaik.Physics;
using Buffer = Vultaik.Buffer;
using Interop = Vultaik.Interop;
using Samples.Common;
using Vultaik.Toolkit;

namespace Samples.LoadGLTF
{


    public class LoadGLTFExample : ExampleBase, IDisposable
    {

        private ModelAssetImporter<VertexPositionNormal> GLTFModel { get; set; }
        private AdapterConfig AdapterConfig;
        private Adapter Adapter;
        private Device Device;
        private Framebuffer Framebuffer;
        private SwapChain SwapChain;
        private GraphicsContext Context;
        private Buffer ConstBuffer;
        private DescriptorSet DescriptorSet;
        private GraphicsPipeline PipelineState;

        private TransformUniform uniform;
        private float yaw, pitch, roll = 0;


        public LoadGLTFExample() : base()
        {

        }





        public override void Initialize()
        {

            AdapterConfig = new()
            {
                SwapChain = true,
                Debug = false,
                Fullscreen = false,
            };



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

            Camera.SetPosition(0, 0, -4.0f);
            Camera.Update();

            uniform = new(Camera.Projection, Model, Camera.View);


            ConstBuffer = new(Device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = Interop.SizeOf<TransformUniform>(),
            });


            CreatePipelineState();

            GLTFModel = new(Device, Constants.ModelsFile + "DamagedHelmet.gltf");

        }

        public void CreatePipelineState()
        {


            string shaders = Constants.ShadersFile;


            GraphicsPipelineDescription pipelineStateDescription = new();
            pipelineStateDescription.SetFramebuffer(Framebuffer);
            pipelineStateDescription.SetShader(new ShaderBytecode(shaders + "LoadGLTF/Fragment.hlsl", ShaderStage.Fragment));
            pipelineStateDescription.SetShader(new ShaderBytecode(shaders + "LoadGLTF/Vertex.hlsl", ShaderStage.Vertex)); 
            pipelineStateDescription.SetVertexBinding(VertexInputRate.Vertex, VertexPositionNormal.Size);
            pipelineStateDescription.SetVertexAttribute(VertexType.Position);
            pipelineStateDescription.SetVertexAttribute(VertexType.Color);
            PipelineState = new(pipelineStateDescription);

            DescriptorData descriptorData = new();
            descriptorData.SetUniformBuffer(0, ConstBuffer);
            DescriptorSet = new DescriptorSet(PipelineState, descriptorData);
        }



        public override void Update(ApplicationTime time)
        {

            Model = Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix4x4.CreateTranslation(0.0f, .0f, 0.0f);
            uniform.Update(Camera, Model);
            ConstBuffer.SetData(ref uniform);

            yaw += 0.0006f * MathF.PI;
        }




        public override void Draw(ApplicationTime time)
        {

            Device.WaitIdle();
            CommandBuffer commandBuffer = Context.CommandBuffer;
            commandBuffer.Begin();


            commandBuffer.BeginFramebuffer(Framebuffer);
            commandBuffer.SetScissor(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);
            commandBuffer.SetViewport(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);

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


            commandBuffer.Close();
            Device.Submit(commandBuffer);
            SwapChain.Present();

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


        public override void Resize(int width, int height)
        {
            Device.WaitIdle();
            SwapChain.Resize(width, height);
            Framebuffer.Resize();

            Camera.AspectRatio = (float)width / height;
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
