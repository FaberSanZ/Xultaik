// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	GraphicsSwapChain.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Vulkan;
using Zeckoxe.Core;
using static Vulkan.VulkanNative;

namespace Zeckoxe.Graphics
{
    public unsafe class GraphicsSwapChain : GraphicsResource
    {
        public PresentationParameters Parameters { get; set; }

        public PixelFormat ColorFormat { get; private set; }

        public Texture Backbuffer { get; private set; }





        internal VkSurfaceKHR Surface;
        internal VkFormat VkColorFormat;
        internal VkColorSpaceKHR ColorSpace;
        internal VkSwapchainKHR SwapChain;
        internal List<VkImage> Images;
        internal VkImageView[] SwapChainImageViews;




        public GraphicsSwapChain(GraphicsDevice device) : base(device)
        {
            Parameters = NativeDevice.NativeParameters;



            Surface = CreateSurface();

            CreateSwapChain();

            Backbuffer = new Texture(device);

            CreateBackBuffers();

        }



        private unsafe void CreateBackBuffers()
        {

            SwapChainImageViews = new VkImageView[Images.Count];


            // Get the images
            uint Count = 0;
            vkGetSwapchainImagesKHR(NativeDevice.Device, SwapChain, ref Count, null);
            VkImage* vkImages = stackalloc VkImage[(int)Count];
            vkGetSwapchainImagesKHR(NativeDevice.Device, SwapChain, ref Count, vkImages);


            for (int i = 0; i < Count; i++)
            {
                VkImageViewCreateInfo imageViewCI = new VkImageViewCreateInfo()
                {
                    sType = VkStructureType.ImageViewCreateInfo,
                    image = vkImages[i],
                    viewType = VkImageViewType.Image2D,
                    format = VkColorFormat,
                    subresourceRange = new VkImageSubresourceRange()
                    {
                        aspectMask = VkImageAspectFlags.Color,
                        baseMipLevel = 0,
                        levelCount = 1,
                        baseArrayLayer = 0,
                        layerCount = 1,
                    }
                };

                vkCreateImageView(NativeDevice.Device, ref imageViewCI, null, out SwapChainImageViews[i]);
            }
        }



        internal VkSurfaceKHR CreateSurface()
        {
            VkInstance instance = NativeDevice.NativeInstance.NativeInstance;
            VkSurfaceKHR defSurface = default;

            VkWin32SurfaceCreateInfoKHR Win32SurfaceCreateInfo = new VkWin32SurfaceCreateInfoKHR()
            {
                sType = VkStructureType.Win32SurfaceCreateInfoKHR,
                hinstance = Process.GetCurrentProcess().Handle,
                hwnd = Parameters.DeviceHandle,
            };




            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                vkCreateWin32SurfaceKHR(instance, &Win32SurfaceCreateInfo, null, &defSurface);



            return defSurface;


        }



        public void CreateSwapChain()
        {

            var PhysicalDevice = NativeDevice.NativeAdapter.NativePhysicalDevice;

            var width = Parameters.BackBufferWidth;

            var height = Parameters.BackBufferHeight;

            var vsync = Parameters.Settings.VSync;


            // Get available queue family properties
            uint queueCount;
            vkGetPhysicalDeviceQueueFamilyProperties(PhysicalDevice, &queueCount, null);
            VkQueueFamilyProperties* queueProps = stackalloc VkQueueFamilyProperties[(int)queueCount];
            vkGetPhysicalDeviceQueueFamilyProperties(PhysicalDevice, &queueCount, queueProps);



            // Iterate over each queue to learn whether it supports presenting:
            // Find a queue with present support
            // Will be used to present the swap chain Images to the windowing system
            VkBool32* supportsPresent = stackalloc VkBool32[(int)queueCount];
            for (uint i = 0; i < queueCount; i++)
                vkGetPhysicalDeviceSurfaceSupportKHR(PhysicalDevice, i, Surface, &supportsPresent[i]);
            


            // Search for a graphics and a present queue in the array of queue
            // families, try to find one that supports both
            uint graphicsQueueNodeIndex = uint.MaxValue;
            uint presentQueueNodeIndex = uint.MaxValue;
            for (uint i = 0; i < queueCount; i++)
            {
                if ((queueProps[i].queueFlags & VkQueueFlags.Graphics) != 0)
                {
                    if (graphicsQueueNodeIndex == uint.MaxValue)
                        graphicsQueueNodeIndex = i;
                    

                    if (supportsPresent[i] == 1)
                    {
                        graphicsQueueNodeIndex = i;
                        presentQueueNodeIndex = i;
                        break;
                    }
                }
            }

            if (presentQueueNodeIndex == uint.MaxValue)
            {
                // If there's no queue that supports both present and graphics
                // try to find a separate present queue
                for (uint i = 0; i < queueCount; ++i)
                {
                    if (supportsPresent[i] == 1)
                    {
                        presentQueueNodeIndex = i;
                        break;
                    }
                }
            }

            // Exit if either a graphics or a presenting queue hasn't been found
            if (graphicsQueueNodeIndex == uint.MaxValue || presentQueueNodeIndex == uint.MaxValue)
                throw new InvalidOperationException("Could not find a graphics and/or presenting queue!");
            

            // todo : Add support for separate graphics and presenting queue
            if (graphicsQueueNodeIndex != presentQueueNodeIndex)
                throw new InvalidOperationException("Separate graphics and presenting queues are not supported yet!");
            



            // Get list of supported Surface formats
            uint formatCount;
            vkGetPhysicalDeviceSurfaceFormatsKHR(PhysicalDevice, Surface, &formatCount, null);
            VkSurfaceFormatKHR* surfaceFormats = stackalloc VkSurfaceFormatKHR[(int)formatCount];
            vkGetPhysicalDeviceSurfaceFormatsKHR(PhysicalDevice, Surface, &formatCount, surfaceFormats);



            // If the Surface format list only includes one entry with VK_FORMAT_UNDEFINED,
            // there is no preferered format, so we assume VK_FORMAT_B8G8R8A8_UNORM
            if ((formatCount == 1) && (surfaceFormats[0].format == VkFormat.Undefined))
            {
                VkColorFormat = VkFormat.B8g8r8a8Unorm;
                ColorSpace = surfaceFormats[0].colorSpace;
            }
            else
            {
                // iterate over the list of available Surface format and
                // check for the presence of VK_FORMAT_B8G8R8A8_UNORM
                bool found_B8G8R8A8_UNORM = false;

                List<VkSurfaceFormatKHR> Formats = new List<VkSurfaceFormatKHR>();

                for (int i = 0; i < formatCount; i++)
                    Formats.Add(surfaceFormats[i]);




                foreach (var surfaceFormat in Formats)
                {
                    if (surfaceFormat.format == VkFormat.B8g8r8a8Unorm)
                    {
                        VkColorFormat = surfaceFormat.format;
                        ColorSpace = surfaceFormat.colorSpace;
                        found_B8G8R8A8_UNORM = true;
                        break;
                    }
                }

                // in case VK_FORMAT_B8G8R8A8_UNORM is not available
                // select the first available color format
                if (!found_B8G8R8A8_UNORM)
                {
                    VkColorFormat = surfaceFormats[0].format;
                    ColorSpace = surfaceFormats[0].colorSpace;
                }
            }


            // Get physical Device Surface properties and formats
            VkSurfaceCapabilitiesKHR surfCaps;
            vkGetPhysicalDeviceSurfaceCapabilitiesKHR(PhysicalDevice, Surface, &surfCaps);



            // Get available present modes
            uint presentModeCount;
            vkGetPhysicalDeviceSurfacePresentModesKHR(PhysicalDevice, Surface, &presentModeCount, null);
            VkPresentModeKHR* presentModes = stackalloc VkPresentModeKHR[(int)presentModeCount];
            vkGetPhysicalDeviceSurfacePresentModesKHR(PhysicalDevice, Surface, &presentModeCount, (VkPresentModeKHR*)presentModes);



            VkExtent2D swapchainExtent;
            // If width (and height) equals the special value 0xFFFFFFFF, the size of the Surface will be set by the swapchain
            if (surfCaps.currentExtent.width == unchecked((uint)-1))
            {
                // If the Surface size is undefined, the size is set to
                // the size of the Images requested.
                swapchainExtent.width = (uint)width;
                swapchainExtent.height = (uint)height;
            }
            else
            {
                // If the Surface size is defined, the swap chain size must match
                swapchainExtent = surfCaps.currentExtent;
                width = (int)surfCaps.currentExtent.width;
                height = (int)surfCaps.currentExtent.height;
            }


            // Select a present mode for the swapchain

            // The VK_PRESENT_MODE_FIFO_KHR mode must always be present as per spec
            // This mode waits for the vertical blank ("v-sync")
            VkPresentModeKHR swapchainPresentMode = VkPresentModeKHR.FifoKHR;

            // If v-sync is not requested, try to find a mailbox mode
            // It's the lowest latency non-tearing present mode available
            if (!vsync)
            {
                for (uint i = 0; i < presentModeCount; i++)
                {
                    if (presentModes[i] == VkPresentModeKHR.MailboxKHR)
                    {
                        swapchainPresentMode = VkPresentModeKHR.MailboxKHR;
                        break;
                    }
                    if ((swapchainPresentMode != VkPresentModeKHR.MailboxKHR) && (presentModes[i] == VkPresentModeKHR.ImmediateKHR))
                        swapchainPresentMode = VkPresentModeKHR.ImmediateKHR;

                }
            }

            // Determine the number of Images
            uint desiredNumberOfSwapchainImages = surfCaps.minImageCount + 1;
            if ((surfCaps.maxImageCount > 0) && (desiredNumberOfSwapchainImages > surfCaps.maxImageCount))
                desiredNumberOfSwapchainImages = surfCaps.maxImageCount;


            // Find the transformation of the Surface
            VkSurfaceTransformFlagsKHR preTransform;
            if ((surfCaps.supportedTransforms & VkSurfaceTransformFlagsKHR.IdentityKHR) != 0)
                // We prefer a non-rotated transform
                preTransform = VkSurfaceTransformFlagsKHR.IdentityKHR;

            else
                preTransform = surfCaps.currentTransform;


            VkSwapchainCreateInfoKHR swapchainCI = new VkSwapchainCreateInfoKHR()
            {
                sType = VkStructureType.SwapchainCreateInfoKHR,
                pNext = null,
                surface = Surface,
                minImageCount = desiredNumberOfSwapchainImages,
                imageFormat = VkColorFormat,
                imageColorSpace = ColorSpace,
                imageExtent = new VkExtent2D()
                {
                    width = swapchainExtent.width,
                    height = swapchainExtent.height
                },
                imageUsage = VkImageUsageFlags.ColorAttachment,
                preTransform = preTransform,
                imageArrayLayers = 1,
                imageSharingMode = VkSharingMode.Exclusive,
                queueFamilyIndexCount = 0,
                pQueueFamilyIndices = null,
                presentMode = swapchainPresentMode,
                oldSwapchain = SwapChain,

                // Setting clipped to VK_TRUE allows the implementation to discard rendering outside of the Surface area
                clipped = True,
                compositeAlpha = VkCompositeAlphaFlagsKHR.OpaqueKHR,
            };


            // Set additional usage flag for blitting from the swapchain Images if supported
            VkFormatProperties formatProps;
            vkGetPhysicalDeviceFormatProperties(PhysicalDevice, VkColorFormat, out formatProps);
            if ((formatProps.optimalTilingFeatures & VkFormatFeatureFlags.BlitDst) != 0)
                swapchainCI.imageUsage |= VkImageUsageFlags.TransferSrc;
            

            VkSwapchainKHR swapChain;
            vkCreateSwapchainKHR(NativeDevice.Device, &swapchainCI, null, &swapChain);
            SwapChain = swapChain;


            //vkDestroySwapchainKHR(NativeDevice.Device, SwapChain, null);



            uint imageCount;
            vkGetSwapchainImagesKHR(NativeDevice.Device, SwapChain, &imageCount, null);
            VkImage* VkImages = stackalloc VkImage[(int)imageCount];
            vkGetSwapchainImagesKHR(NativeDevice.Device, swapChain, &imageCount, VkImages);



            Images = new List<VkImage>();

            for (int i = 0; i < imageCount; i++)
                Images.Add(VkImages[i]);

        }




        public void Present()
        {
            VkSemaphore Semaphore = NativeDevice.RenderFinishedSemaphore;
            VkSwapchainKHR swapchain = SwapChain;
            CommandBuffer commandBuffer = NativeDevice.NativeCommand;

            VkPresentInfoKHR presentInfo = new VkPresentInfoKHR()
            {
                sType = VkStructureType.PresentInfoKHR,
                waitSemaphoreCount = 1,
                pWaitSemaphores = &Semaphore,
                swapchainCount = 1,
                pSwapchains = &swapchain,
                pImageIndices = Interop.AllocToPointer(ref commandBuffer.imageIndex),
            };


            vkQueuePresentKHR(NativeDevice.NativeCommandQueue, &presentInfo);
        }

    }

}
