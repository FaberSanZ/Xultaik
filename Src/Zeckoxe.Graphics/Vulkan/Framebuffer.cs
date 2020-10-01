using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Zeckoxe.Graphics
{
    // TODO: Framebuffer

    public unsafe class Framebuffer : GraphicsResource
    {


        internal VkRenderPass renderPass;
        internal VkFramebuffer[] framebuffers;


        public Framebuffer(SwapChain swapChain) : base(swapChain.NativeDevice)
        {
            SwapChain = swapChain;

            Recreate();
        }

        public SwapChain SwapChain { get; }

        public void Recreate()
        {
            CreateRenderPass();
            CreateFrameBuffers();
        }

        internal void CreateFrameBuffers()
        {
            VkImageView[] SwapChainImageViews = SwapChain.SwapChainImageViews;
            framebuffers = new VkFramebuffer[SwapChainImageViews.Length];

            for (uint i = 0; i < SwapChainImageViews.Length; i++)
            {

                VkImageView* attachments = stackalloc VkImageView[2];
                attachments[0] = SwapChainImageViews[i];                                 // Color attachment is the view of the swapchain image			
                attachments[1] = SwapChain.DepthStencil._depthStencil.View;

                VkFramebufferCreateInfo frameBufferInfo = new VkFramebufferCreateInfo()
                {
                    sType = VkStructureType.FramebufferCreateInfo,
                    renderPass = renderPass,
                    attachmentCount = 2,
                    pAttachments = attachments,
                    width = (uint)NativeDevice.NativeParameters.BackBufferWidth,
                    height = (uint)NativeDevice.NativeParameters.BackBufferWidth,
                    layers = 1,
                };

                vkCreateFramebuffer(NativeDevice.handle, &frameBufferInfo, null, out framebuffers[i]);
            }

        }


        internal void CreateRenderPass()
        {
            VkFormat ColorFormat = SwapChain.VkColorFormat;
            VkFormat DepthFormat = SwapChain.DepthStencil.vkformat;


            // Descriptors for the attachments used by this renderpass
            VkAttachmentDescription* attachments = stackalloc VkAttachmentDescription[2];

            // Color attachment
            attachments[0] = new VkAttachmentDescription
            {
                format = ColorFormat,                                  // Use the color format selected by the swapchain
                samples = VkSampleCountFlags.Count1,                             // We don't use multi sampling in this example
                loadOp = VkAttachmentLoadOp.Clear,                               // Clear this attachment at the start of the render pass
                storeOp = VkAttachmentStoreOp.Store,                             // Keep it's contents after the render pass is finished (for displaying it)
                stencilLoadOp = VkAttachmentLoadOp.DontCare,                     // We don't use stencil, so don't care for load
                stencilStoreOp = VkAttachmentStoreOp.DontCare,                   // Same for store
                initialLayout = VkImageLayout.Undefined,                         // Layout at render pass start. Initial doesn't matter, so we use undefined
                finalLayout = VkImageLayout.PresentSrcKHR                        // Layout to which the attachment is transitioned when the render pass is finished
            };                                                                   // Use the color format selected by the swapchain
                                                                                 // As we want to present the color buffer to the swapchain, we transition to PRESENT_KHR	
                                                                                 // Depth attachment
            attachments[1] = new VkAttachmentDescription
            {
                format = DepthFormat,                                            // A proper depth format is selected in the example base
                samples = VkSampleCountFlags.Count1,
                loadOp = VkAttachmentLoadOp.Clear,                               // Clear depth at start of first subpass
                storeOp = VkAttachmentStoreOp.DontCare,                          // We don't need depth after render pass has finished (DONT_CARE may result in better performance)
                stencilLoadOp = VkAttachmentLoadOp.DontCare,                     // No stencil
                stencilStoreOp = VkAttachmentStoreOp.DontCare,                   // No Stencil
                initialLayout = VkImageLayout.Undefined,                         // Layout at render pass start. Initial doesn't matter, so we use undefined
                finalLayout = VkImageLayout.ColorAttachmentOptimal               // Transition to depth/stencil attachment
            };                                                                   // A proper depth format is selected in the example base

            // Setup attachment references
            VkAttachmentReference colorReference = new VkAttachmentReference
            {
                attachment = 0,                                                             // Attachment 0 is color
                layout = VkImageLayout.ColorAttachmentOptimal                               // Attachment layout used as color during the subpass
            };

            VkAttachmentReference depthReference = new VkAttachmentReference
            {

                attachment = 1,                                                             // Attachment 1 is color
                layout = VkImageLayout.DepthStencilAttachmentOptimal                        // Attachment used as depth/stemcil used during the subpass
            };

            // Setup a single subpass reference
            VkSubpassDescription subpassDescription = new VkSubpassDescription
            {
                pipelineBindPoint = VkPipelineBindPoint.Graphics,
                colorAttachmentCount = 1,                                                   // Subpass uses one color attachment
                pColorAttachments = &colorReference,                                        // Reference to the color attachment in slot 0
                pDepthStencilAttachment = &depthReference,                                  // Reference to the depth attachment in slot 1
                inputAttachmentCount = 0,                                    // Input attachments can be used to sample from contents of a previous subpass
                pInputAttachments = null,                                 // (Input attachments not used by this example)
                preserveAttachmentCount = 0,                                 // Preserved attachments can be used to loop (and preserve) attachments through subpasses
                pPreserveAttachments = null,                              // (Preserve attachments not used by this example)
                pResolveAttachments = null                               // Resolve attachments are resolved at the end of a sub pass and can be used for e.g. multi sampling
            };

            // Setup subpass dependencies
            // These will add the implicit ttachment layout transitionss specified by the attachment descriptions
            // The actual usage layout is preserved through the layout specified in the attachment reference		
            // Each subpass dependency will introduce a memory and execution dependency between the source and dest subpass described by
            // srcStageMask, dstStageMask, srcAccessMask, dstAccessMask (and dependencyFlags is set)
            // Note: VK_SUBPASS_EXTERNAL is a special constant that refers to all commands executed outside of the actual renderpass)
            VkSubpassDependency* dependencies = stackalloc VkSubpassDependency[2];

            // First dependency at the start of the renderpass
            // Does the transition from final to initial layout 
            dependencies[0].srcSubpass = SubpassExternal;                               // Producer of the dependency 
            dependencies[0].dstSubpass = 0;                                                 // Consumer is our single subpass that will wait for the execution depdendency
            dependencies[0].srcStageMask = VkPipelineStageFlags.BottomOfPipe;
            dependencies[0].dstStageMask = VkPipelineStageFlags.ColorAttachmentOutput;
            dependencies[0].srcAccessMask = VkAccessFlags.MemoryRead;
            dependencies[0].dstAccessMask = (VkAccessFlags.ColorAttachmentRead | VkAccessFlags.ColorAttachmentWrite);
            dependencies[0].dependencyFlags = (VkDependencyFlags.ByRegion);

            // Second dependency at the end the renderpass
            // Does the transition from the initial to the final layout
            dependencies[1].srcSubpass = 0;                                                 // Producer of the dependency is our single subpass
            dependencies[1].dstSubpass = SubpassExternal;                               // Consumer are all commands outside of the renderpass
            dependencies[1].srcStageMask = VkPipelineStageFlags.ColorAttachmentOutput;
            dependencies[1].dstStageMask = VkPipelineStageFlags.BottomOfPipe;
            dependencies[1].srcAccessMask = (VkAccessFlags.ColorAttachmentRead | VkAccessFlags.ColorAttachmentWrite);
            dependencies[1].dstAccessMask = VkAccessFlags.MemoryRead;
            dependencies[1].dependencyFlags = VkDependencyFlags.ByRegion;

            // Create the actual renderpass
            VkRenderPassCreateInfo renderPassInfo = new VkRenderPassCreateInfo()
            {
                sType = VkStructureType.RenderPassCreateInfo,
            };
            renderPassInfo.attachmentCount = 2;                                             // Number of attachments used by this render pass
            renderPassInfo.pAttachments = attachments;                                      // Descriptions of the attachments used by the render pass
            renderPassInfo.subpassCount = 1;                                                // We only use one subpass in this example
            renderPassInfo.pSubpasses = &subpassDescription;                                // Description of that subpass
            renderPassInfo.dependencyCount = 2;                                             // Number of subpass dependencies
            renderPassInfo.pDependencies = dependencies;




            vkCreateRenderPass(NativeDevice.handle, &renderPassInfo, null, out VkRenderPass RenderPass);
            renderPass = RenderPass;

        }
    }
}
