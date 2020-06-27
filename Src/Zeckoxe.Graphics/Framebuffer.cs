using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using Interop = Zeckoxe.Core.Interop;

namespace Zeckoxe.Graphics
{
    public unsafe class Framebuffer : GraphicsResource
    {


        internal VkRenderPass renderPass;
        internal VkFramebuffer[] framebuffers;


        public Framebuffer(GraphicsDevice device) : base(device)
        {
            Recreate();
        }


        public void Recreate()
        {
            CreateRenderPass();
            CreateFrameBuffers();
        }

        internal void CreateFrameBuffers()
        {
            VkImageView[] SwapChainImageViews = NativeDevice.NativeSwapChain.SwapChainImageViews;
            framebuffers = new VkFramebuffer[SwapChainImageViews.Length];

            for (uint i = 0; i < SwapChainImageViews.Length; i++)
            {
                VkFramebufferCreateInfo frameBufferInfo = new VkFramebufferCreateInfo()
                {
                    sType = VkStructureType.FramebufferCreateInfo,
                    renderPass = renderPass,
                    attachmentCount = 1,
                    pAttachments = Interop.AllocToPointer(ref SwapChainImageViews[i]),
                    width = (uint)NativeDevice.NativeParameters.BackBufferWidth,
                    height = (uint)NativeDevice.NativeParameters.BackBufferWidth,
                    layers = 1,
                };

                vkCreateFramebuffer(NativeDevice.Device, &frameBufferInfo, null, out framebuffers[i]);
            }

        }


        internal void CreateRenderPass()
        {
            VkFormat ColorFormat = NativeDevice.NativeSwapChain.VkColorFormat;


            VkAttachmentDescription colorAttachment = new VkAttachmentDescription()
            {
                format = ColorFormat,
                samples = VkSampleCountFlags.Count1,
                loadOp = VkAttachmentLoadOp.Clear,
                storeOp = VkAttachmentStoreOp.Store,
                stencilLoadOp = VkAttachmentLoadOp.DontCare,
                stencilStoreOp = VkAttachmentStoreOp.DontCare,
                initialLayout = VkImageLayout.Undefined,
                finalLayout = VkImageLayout.PresentSrcKHR,
            };


            VkAttachmentReference colorAttachmentRef = new VkAttachmentReference()
            {
                attachment = 0,
                layout = VkImageLayout.ColorAttachmentOptimal,
            };


            VkSubpassDescription subpass = new VkSubpassDescription()
            {
                pipelineBindPoint = VkPipelineBindPoint.Graphics,
                colorAttachmentCount = 1,
                pColorAttachments = &colorAttachmentRef,
            };


            VkSubpassDependency dependency = new VkSubpassDependency()
            {
                srcSubpass = SubpassExternal,
                dstSubpass = 0,
                srcStageMask = VkPipelineStageFlags.ColorAttachmentOutput,
                srcAccessMask = 0,
                dstStageMask = VkPipelineStageFlags.ColorAttachmentOutput,
                dstAccessMask = VkAccessFlags.ColorAttachmentRead | VkAccessFlags.ColorAttachmentWrite,
            };


            VkRenderPassCreateInfo renderPassCI = new VkRenderPassCreateInfo()
            {
                sType = VkStructureType.RenderPassCreateInfo,
                attachmentCount = 1,
                pAttachments = &colorAttachment,
                subpassCount = 1,
                pSubpasses = &subpass,
                dependencyCount = 1,
                pDependencies = &dependency,
            };

            vkCreateRenderPass(NativeDevice.Device, &renderPassCI, null, out VkRenderPass RenderPass);
            renderPass = RenderPass;

        }
    }
}
