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

namespace Samples.ComputeTexture
{
    public class ComputeTexture : ExampleBase, IDisposable
    {

        private AdapterConfig AdapterConfig;
        private Adapter Adapter;
        private Device Device;
        private Framebuffer Framebuffer;
        private SwapChain SwapChain;
        private GraphicsContext Context;
        private DescriptorSet DescriptorSet_0;
        private DescriptorSet DescriptorSet_1;
        private GraphicsPipeline PipelineState_0;
        private ComputePipeline PipelineState_1;
        private CommandBuffer cmd_compute;

        public ComputeTexture() : base()
        {

        }


        public override void Initialize()
        {

            AdapterConfig = new()
            {
                SwapChain = true,
                Debug = true
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


            cmd_compute = new(Device, CommandBufferType.AsyncCompute);
            CreatePipelineState();
            
        }








        public void CreatePipelineState()
        {

            string fragment = Constants.ShadersFile + @"ComputeTexture\Fragment.hlsl";
            string vertex = Constants.ShadersFile + @"ComputeTexture\Vertex.hlsl";
            string compute = Constants.ShadersFile + @"ComputeTexture\Compute.hlsl";

            Image image = new(Device, new()
            {
                ImageType = VkImageType.Image2D,
                Usage = ResourceUsage.GPU_Only,
                Flags = ImageFlags.ReadWriteImage | ImageFlags.ShaderResource,
                Format = VkFormat.R8G8B8A8SNorm,
                Width = Framebuffer.SwapChain.Width,
                Height = Framebuffer.SwapChain.Height,
                Depth = 1,
                MipLevels = 1,
                ArraySize = 1,
            });


            Sampler sampler = new Sampler(Device);




            ComputePipelineDescription ComputePipelineDescription = new();
            ComputePipelineDescription.Shader = new ShaderBytecode(compute, ShaderStage.Compute);
            PipelineState_1 = new(Device, ComputePipelineDescription);

            DescriptorData descriptorData_1 = new();
            descriptorData_1.SetReadWriteImage(0, image);
            DescriptorSet_1 = new(PipelineState_1, descriptorData_1);



            GraphicsPipelineDescription Pipelinedescription0 = new();
            Pipelinedescription0.SetFramebuffer(Framebuffer);
            Pipelinedescription0.SetShader(new ShaderBytecode(fragment, ShaderStage.Fragment));
            Pipelinedescription0.SetShader(new ShaderBytecode(vertex, ShaderStage.Vertex));
            PipelineState_0 = new(Pipelinedescription0);

            DescriptorData descriptorData_0 = new();
            descriptorData_0.SetImage(1, image);
            descriptorData_0.SetSampler(2, sampler);
            DescriptorSet_0 = new(PipelineState_0, descriptorData_0);




        }

        public override void Update(ApplicationTime time)
        {

        }
   

        public override void Draw(ApplicationTime time)
        {
            Device.WaitIdle();

            cmd_compute.Begin();
            cmd_compute.SetComputePipeline(PipelineState_1);
            cmd_compute.BindDescriptorSets(DescriptorSet_1);

            cmd_compute.Dispatch2D(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 8, 8);

            cmd_compute.Close();
            Device.Submit(cmd_compute);
            Device.ComputeQueueWaitIdle();


            CommandBuffer cmd = Context.CommandBuffer;

            cmd.Begin();
            cmd.BeginFramebuffer(Framebuffer);
            cmd.SetScissor(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);
            cmd.SetViewport(Window.FramebufferSize.Width, Window.FramebufferSize.Height, 0, 0);


            cmd.BindDescriptorSets(DescriptorSet_0);
            cmd.SetGraphicPipeline(PipelineState_0);
            cmd.Draw(3, 1, 0, 0);


            cmd.Close();
            Device.Submit(cmd);
            SwapChain.Present();
        }

        public override void Resize(int width, int height)
        {
            Device.WaitIdle();
            SwapChain.Resize(width, height);
            Framebuffer.Resize();

        }




        public void Dispose()
        {

        }
    }

}
