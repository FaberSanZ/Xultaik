// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)




using System;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Vultaik
{

    public unsafe class Framebuffer : GraphicsResource, IDisposable
    {


        internal VkRenderPass renderPass;
        internal VkFramebuffer[] framebuffers;


        public Framebuffer(FramebufferDescription description)
        {

        }


        public Framebuffer(SwapChain swapChain) : base(swapChain.NativeDevice)
        {
            SwapChain = swapChain;

            Recreate();
        }
        public VkFramebuffer CurrentFramebuffer { get; }

        public SwapChain SwapChain { get; }


        public void Recreate()
        {
            CreateRenderPass();
            CreateFrameBuffers();
        }

        public void CreateFrameBuffers()
        {
            VkImageView[] SwapChainImageViews = SwapChain.swapChain_image_views;
            framebuffers = new VkFramebuffer[SwapChainImageViews.Length];

            for (uint i = 0; i < SwapChainImageViews.Length; i++)
            {

#pragma warning disable CA2014 
                VkImageView* attachments = stackalloc VkImageView[2]
                {
                    SwapChainImageViews[i],                          	
                    SwapChain.DepthStencil.image_view,
                };
#pragma warning restore CA2014 


                VkFramebufferCreateInfo frameBufferInfo = new VkFramebufferCreateInfo()
                {
                    sType = VkStructureType.FramebufferCreateInfo,
                    pNext = null,
                    flags = VkFramebufferCreateFlags.None,
                    renderPass = renderPass,
                    attachmentCount = 2,
                    pAttachments = attachments,
                    width = (uint)SwapChain.Width,
                    height = (uint)SwapChain.Height,
                    layers = 1,
                };

                vkCreateFramebuffer(NativeDevice.handle, &frameBufferInfo, null, out framebuffers[i]);
            }

        }


        internal void CreateRenderPass()
        {
            VkFormat color_format = SwapChain.color_format;
            VkFormat depth_format = SwapChain.DepthStencil.Format;


            // Descriptors for the attachments used by this renderpass
            VkAttachmentDescription* attachments = stackalloc VkAttachmentDescription[2]
            {
                // Color attachment
                new VkAttachmentDescription
                {
                    format = color_format,                                           // Use the color format selected by the swapchain
                    samples = VkSampleCountFlags.Count1,                             // We don't use multi sampling in this example
                    loadOp = VkAttachmentLoadOp.Clear,                               // Clear this attachment at the start of the render pass
                    storeOp = VkAttachmentStoreOp.Store,                             // Keep it's contents after the render pass is finished (for displaying it)
                    stencilLoadOp = VkAttachmentLoadOp.DontCare,                     // We don't use stencil, so don't care for load
                    stencilStoreOp = VkAttachmentStoreOp.DontCare,                   // Same for store
                    initialLayout = VkImageLayout.Undefined,                         // Layout at render pass start. Initial doesn't matter, so we use undefined
                    finalLayout = VkImageLayout.PresentSrcKHR                        // Layout to which the attachment is transitioned when the render pass is finished
                },



                // Use the color format selected by the swapchain
                // As we want to present the color buffer to the swapchain, we transition to PRESENT_KHR	
                // Depth attachment
                // A proper depth format is selected in the example base

                new VkAttachmentDescription
                {
                    format = depth_format,                                           // A proper depth format is selected in the example base
                    samples = VkSampleCountFlags.Count1,
                    loadOp = VkAttachmentLoadOp.Clear,                               // Clear depth at start of first subpass
                    storeOp = VkAttachmentStoreOp.DontCare,                          // We don't need depth after render pass has finished (DONT_CARE may result in better performance)
                    stencilLoadOp = VkAttachmentLoadOp.DontCare,                     // No stencil
                    stencilStoreOp = VkAttachmentStoreOp.DontCare,                   // No Stencil
                    initialLayout = VkImageLayout.Undefined,                         // Layout at render pass start. Initial doesn't matter, so we use undefined
                    finalLayout = VkImageLayout.ColorAttachmentOptimal               // Transition to depth/stencil attachment
                },

            };


            // Setup attachment references
            VkAttachmentReference* colorReferences = stackalloc VkAttachmentReference[1]
            {
                new VkAttachmentReference                                               // Attachment layout used as color during the subpass
                {
                    attachment = 0,                                                     // Attachment 0 is color
                    layout = VkImageLayout.ColorAttachmentOptimal
                }
            };


            VkAttachmentReference depthReference = new VkAttachmentReference
            {
                attachment = 1,                                                             // Attachment 1 is color
                layout = VkImageLayout.DepthStencilAttachmentOptimal                        // Attachment used as depth/stemcil used during the subpass
            };


            // Setup a single subpass reference
            VkSubpassDescription* subpass_description = stackalloc VkSubpassDescription[1]
            {
                new VkSubpassDescription
                {
                    pipelineBindPoint = VkPipelineBindPoint.Graphics,
                    colorAttachmentCount = 1,                                                   // Subpass uses one color attachment
                    pColorAttachments = colorReferences,                                        // Reference to the color attachment in slot 0
                    pDepthStencilAttachment = &depthReference,                                  // Reference to the depth attachment in slot 1
                    inputAttachmentCount = 0,                                                   // Input attachments can be used to sample from contents of a previous subpass
                    pInputAttachments = null,                                                   // (Input attachments not used by this example)
                    preserveAttachmentCount = 0,                                                // Preserved attachments can be used to loop (and preserve) attachments through subpasses
                    pPreserveAttachments = null,                                                // (Preserve attachments not used by this example)
                    pResolveAttachments = null                                                  // Resolve attachments are resolved at the end of a sub pass and can be used for e.g. multi sampling
                },
            };



            // Setup subpass dependencies
            // These will add the implicit ttachment layout transitionss specified by the attachment descriptions
            // The actual usage layout is preserved through the layout specified in the attachment reference		
            // Each subpass dependency will introduce a memory and execution dependency between the source and dest subpass described by
            // srcStageMask, dstStageMask, srcAccessMask, dstAccessMask (and dependencyFlags is set)
            // Note: VK_SUBPASS_EXTERNAL is a special constant that refers to all commands executed outside of the actual renderpass)
            VkSubpassDependency* dependencies = stackalloc VkSubpassDependency[2]
            {
                // First dependency at the start of the renderpass
                // Does the transition from final to initial layout 
                new VkSubpassDependency
                {
                    srcSubpass = SubpassExternal,                                   // Producer of the dependency 
                    dstSubpass = 0,                                                 // Consumer is our single subpass that will wait for the execution depdendency
                    srcStageMask = VkPipelineStageFlags.BottomOfPipe,
                    dstStageMask = VkPipelineStageFlags.ColorAttachmentOutput,
                    srcAccessMask = VkAccessFlags.MemoryRead,
                    dstAccessMask = VkAccessFlags.ColorAttachmentRead | VkAccessFlags.ColorAttachmentWrite,
                    dependencyFlags = VkDependencyFlags.ByRegion
                },

                // Second dependency at the end the renderpass
                // Does the transition from the initial to the final layout
                new VkSubpassDependency
                {
                    srcSubpass = 0,                                                 // Producer of the dependency is our single subpass
                    dstSubpass = SubpassExternal,                                   // Consumer are all commands outside of the renderpass
                    srcStageMask = VkPipelineStageFlags.ColorAttachmentOutput,
                    dstStageMask = VkPipelineStageFlags.BottomOfPipe,
                    srcAccessMask = VkAccessFlags.ColorAttachmentRead | VkAccessFlags.ColorAttachmentWrite,
                    dstAccessMask = VkAccessFlags.MemoryRead,
                    dependencyFlags = VkDependencyFlags.ByRegion
                }
            };



            // Create the actual renderpass
            VkRenderPassCreateInfo render_pass_info = new VkRenderPassCreateInfo()
            {
                sType = VkStructureType.RenderPassCreateInfo,
                attachmentCount = 2,                                             // Number of attachments used by this render pass
                pAttachments = attachments,                                      // Descriptions of the attachments used by the render pass
                subpassCount = 1,                                                // We only use one subpass in this example
                pSubpasses = subpass_description,                                // Description of that subpass
                dependencyCount = 2,                                             // Number of subpass dependencies
                pDependencies = dependencies,
            };

            vkCreateRenderPass(NativeDevice.handle, &render_pass_info, null, out renderPass).CheckResult();
        }




        private (VkAttachmentDescription, VkAttachmentReference) CreateAttachment(
          VkFormat format,
          VkSampleCountFlags samples,
          uint index,
          VkAttachmentLoadOp loadOp,
          VkAttachmentStoreOp storeOp,
          VkAttachmentStoreOp stencilStoreOp,
          VkImageLayout initialLayout,
          VkImageLayout finalLayout)
        {
            return (new VkAttachmentDescription()
            {
                format = format,
                samples = samples,
                loadOp = loadOp,
                storeOp = storeOp,
                stencilLoadOp = VkAttachmentLoadOp.Load,
                stencilStoreOp = stencilStoreOp,
                initialLayout = initialLayout,
                finalLayout = finalLayout
            }, new VkAttachmentReference()
            {
                attachment = index,
                layout = finalLayout
            });
        }


        public void Resize()
        {
            Free();
            CreateFrameBuffers();
        }
        public void Free()
        {

            //vkDestroyImageView(NativeDevice.handle, depthImageView, nullptr);
            //vkDestroyImage(NativeDevice.handle, depthImage, nullptr);
            //vkFreeMemory(NativeDevice.handle, depthImageMemory, nullptr);
            SwapChain.DepthStencil.Dispose();

            SwapChain.DepthStencil.Initialize(SwapChain.Width, SwapChain.Height);
            for (int i = 0; i < SwapChain.swapChain_image_views.Length; i++)
            {
                vkDestroyFramebuffer(NativeDevice.handle, framebuffers[i], null);
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < SwapChain.swapChain_image_views.Length; i++)
            {
                vkDestroyImageView(NativeDevice.handle, SwapChain.swapChain_image_views[i], null);
                vkDestroyFramebuffer(NativeDevice.handle, framebuffers[i], null);
            }
            SwapChain.DepthStencil.Dispose();


            vkDestroyRenderPass(NativeDevice.handle, renderPass, null);
        }
    }
}
