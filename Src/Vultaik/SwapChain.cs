// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using static Vortice.Vulkan.VkUtils;


namespace Vultaik
{
    public unsafe class SwapChain : GraphicsResource, IDisposable
    {
        private delegate VkResult PFN_vkCreateWin32SurfaceKHRDelegate(VkInstance instance, VkWin32SurfaceCreateInfoKHR* createInfo, VkAllocationCallbacks* allocator, VkSurfaceKHR* surface);
        private delegate VkResult PFN_vkCreateXlibSurfaceKHR(VkInstance instance, VkXlibSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface);
        private delegate VkResult PFN_vkCreateWaylandSurfaceKHR(VkInstance instance, VkWaylandSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface);

        internal VkSurfaceKHR surface;
        internal VkFormat color_format;
        internal VkColorSpaceKHR color_space;
        internal VkSwapchainKHR handle;
        internal VkImage[] images;
        internal VkImageView[] swapChain_image_views;

        internal bool vultaik_debug => AdapterConfig.Debug;


        public SwapChain(Device device, SwapchainDescription description) : base(device)
        {
            Description = description;
            AdapterConfig = NativeDevice.AdapterConfig;

            SwapchainSource = description.Source;

            surface = CreateSurface();
            init_queue_family();

            CreateSwapChain(description.Width, description.Height);

            CreateBackBuffers();

            SetupDepthStencil();

        }


        public void SetupDepthStencil()
        {
            if (Description.DepthFormat is not null)
            {
                DepthStencil = new Image(NativeDevice, new ImageDescription
                {
                    Flags = ImageFlags.DepthStencil,
                    Usage = ResourceUsage.CPU_To_GPU,
                    ArraySize = 1,
                    MipLevels = 1,
                    Data = null,
                    Depth = 1,
                    ImageType = VkImageType.Image2D,
                    IsCubeMap = false,
                    Size = 0,
                    Width = Width,
                    Height = Height,
                    Format = Description.DepthFormat.Value,
                });
            }

            //DepthStencil.Initialize();
        }

        public AdapterConfig AdapterConfig { get; set; }
        public VkFormat ColorFormat { get; private set; }
        public Image DepthStencil { get; private set; }
        public SwapchainSource SwapchainSource { get; set; }
        public Framebuffer Framebuffer { get; private set; }
        public SwapchainDescription Description { get; }

        public unsafe void CreateBackBuffers()
        {

            swapChain_image_views = new VkImageView[images.Length];

            for (int i = 0; i < images.Length; i++)
            {

                VkImageViewCreateInfo image_view_info = new VkImageViewCreateInfo()
                {
                    sType = VkStructureType.ImageViewCreateInfo,
                    pNext = null,
                    flags = VkImageViewCreateFlags.None,
                    components = VkComponentMapping.Identity, // TODO: VkComponentMapping
                    image = images[i],
                    viewType = VkImageViewType.Image2D,
                    format = color_format,
                    subresourceRange = new VkImageSubresourceRange()
                    {
                        aspectMask = VkImageAspectFlags.Color,
                        baseMipLevel = 0,
                        levelCount = 1,
                        baseArrayLayer = 0,
                        layerCount = 1,
                    }
                };

                vkCreateImageView(NativeDevice.handle, &image_view_info, null, out swapChain_image_views[i]);


                if (vultaik_debug)
                {
                    string type = "SwapChain - BackImageView";
                    ConsoleLog.Info(type, $"Handle = 0x{swapChain_image_views[i].Handle.ToString("X")}");
                    ConsoleLog.InfoNode(type, $"Image = 0x{image_view_info.image.Handle.ToString("X")}");
                    ConsoleLog.InfoNode(type, $"Format = {image_view_info.format}");
                    ConsoleLog.InfoNode(type, $"Flags = {image_view_info.flags}");
                    ConsoleLog.InfoNode(type, $"Components R = {image_view_info.components.r}");
                    ConsoleLog.InfoNode(type, $"Components G = {image_view_info.components.g}");
                    ConsoleLog.InfoNode(type, $"Components B = {image_view_info.components.b}");
                    ConsoleLog.InfoNode(type, $"Components A = {image_view_info.components.a}");
                    ConsoleLog.InfoNode(type, $"AspectMask = {image_view_info.subresourceRange.aspectMask}");
                    ConsoleLog.InfoNode(type, $"MipLevel = {image_view_info.subresourceRange.baseMipLevel}");
                    ConsoleLog.InfoNode(type, $"ArrayLayer = {image_view_info.subresourceRange.baseArrayLayer}");
                    ConsoleLog.InfoNode(type, $"ViewType = {image_view_info.viewType}", true);
                }
            }
        }



        internal VkSurfaceKHR CreateSurface()
        {
            VkInstance instance = NativeDevice.NativeAdapter.instance;
            VkSurfaceKHR surface = 0;

            //PFN_vkCreateWin32SurfaceKHRDelegate vkCreateWin32SurfaceKHR = NativeDevice.GetInstanceProcAddr<PFN_vkCreateWin32SurfaceKHRDelegate>("vkCreateWin32SurfaceKHR");
            //PFN_vkCreateXlibSurfaceKHR vkCreateXlibSurfaceKHR = NativeDevice.GetInstanceProcAddr<PFN_vkCreateXlibSurfaceKHR>("vkCreateXlibSurfaceKHR");
            //PFN_vkCreateWaylandSurfaceKHR vkCreateWaylandSurfaceKHR = NativeDevice.GetInstanceProcAddr<PFN_vkCreateWaylandSurfaceKHR>("vkCreateWaylandSurfaceKHR");


            //if (SwapchainSource is WindowSwapchainSource sourcewin)
            //{
            //    surface = new VkSurfaceKHR(sourcewin.Surface);
            //}


            if (SwapchainSource is Win32SwapchainSource sourcewin32 && NativeDevice.NativeAdapter.SupportsWin32Surface)
            {
                VkWin32SurfaceCreateInfoKHR win32_surface_create_info = new()
                {
                    sType = VkStructureType.Win32SurfaceCreateInfoKHR,
                    pNext = null,
                    flags = VkWin32SurfaceCreateFlagsKHR.None,
                    hinstance = sourcewin32.Hinstance,
                    hwnd = sourcewin32.Hwnd,
                };

                vkCreateWin32SurfaceKHR(instance, &win32_surface_create_info, null, out surface);

                if (vultaik_debug)
                {
                    string message = $"Win32, Handle = 0x{surface.Handle.ToString("X")}, Hwnd = 0x{win32_surface_create_info.hwnd.ToString("X")}";
                    ConsoleLog.Info("SwapChain - Surface", message, true);
                }
            }



            if (SwapchainSource is XlibSwapchainSource xlibsource)
            {

                VkXlibSurfaceCreateInfoKHR xlib_surface_create_info = new()
                {
                    sType = VkStructureType.XlibSurfaceCreateInfoKHR,
                    pNext = null,
                    flags = 0,
                    display = xlibsource.Display,
                    window = xlibsource.Window
                };

                vkCreateXlibSurfaceKHR(instance, &xlib_surface_create_info, null, out surface);

                if (vultaik_debug)
                {
                    string message = $"SwapChain - Xlib, Handle = 0x{surface.Handle.ToString("X")}, Display = 0x{xlib_surface_create_info.display.ToString("X")}, window = 0x{xlib_surface_create_info.window.ToString("X")}";
                    ConsoleLog.Info("Surface", message);
                }
            }

            if (SwapchainSource is WaylandSwapchainSource Waylandsource)
            {

                VkWaylandSurfaceCreateInfoKHR XlibSurfaceCreateInfo = new()
                {
                    sType = VkStructureType.WaylandSurfaceCreateInfoKHR,
                    pNext = null,
                    flags = 0,
                    display = Waylandsource.Display, 
                    surface = Waylandsource.Surface 
                };

                vkCreateWaylandSurfaceKHR(instance, &XlibSurfaceCreateInfo, null, out surface);
            }


            return surface;


        }


        internal void init_queue_family()
        {

            VkPhysicalDevice PhysicalDevice = NativeDevice.NativeAdapter.handle;



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
            {
                vkGetPhysicalDeviceSurfaceSupportKHR(PhysicalDevice, i, surface, out supportsPresent[i]);
            }



            // Search for a graphics and a present queue in the array of queue
            // families, try to find one that supports both
            uint graphicsQueueNodeIndex = uint.MaxValue;
            uint presentQueueNodeIndex = uint.MaxValue;


            for (uint i = 0; i < queueCount; i++)
            {
                if ((queueProps[i].queueFlags & VkQueueFlags.Graphics) != 0)
                {
                    if (graphicsQueueNodeIndex is uint.MaxValue)
                    {
                        graphicsQueueNodeIndex = i;
                    }

                    if (supportsPresent[i] == true)
                    {
                        graphicsQueueNodeIndex = i;
                        presentQueueNodeIndex = i;
                        break;
                    }
                }
            }

            if (presentQueueNodeIndex is uint.MaxValue)
            {
                // If there's no queue that supports both present and graphics
                // try to find a separate present queue
                for (uint i = 0; i < queueCount; ++i)
                {
                    if (supportsPresent[i] == true)
                    {
                        presentQueueNodeIndex = i;
                        break;
                    }
                }
            }

            // Exit if either a graphics or a presenting queue hasn't been found
            if (graphicsQueueNodeIndex is uint.MaxValue || presentQueueNodeIndex is uint.MaxValue)
            {
                throw new InvalidOperationException("Could not find a graphics and/or presenting queue!");
            }


            // TODO : Add support for separate graphics and presenting queue
            if (graphicsQueueNodeIndex != presentQueueNodeIndex)
            {
                throw new InvalidOperationException("Separate graphics and presenting queues are not supported yet!");
            }



            if (vultaik_debug)
            {
                string message = $"QueueNodeIndex = {presentQueueNodeIndex}, Type = {queueProps[presentQueueNodeIndex].queueFlags}";
                ConsoleLog.Info("SwapChain", message);
            }

            // Get list of supported Surface formats
            uint formatCount;
            vkGetPhysicalDeviceSurfaceFormatsKHR(PhysicalDevice, surface, &formatCount, null);
            VkSurfaceFormatKHR* surfaceFormats = stackalloc VkSurfaceFormatKHR[(int)formatCount];
            vkGetPhysicalDeviceSurfaceFormatsKHR(PhysicalDevice, surface, &formatCount, surfaceFormats);



            // If the Surface format list only includes one entry with VK_FORMAT_UNDEFINED,
            // there is no preferered format, so we assume VK_FORMAT_B8G8R8A8_UNORM
            if ((formatCount is 1) && (surfaceFormats[0].format is VkFormat.Undefined))
            {
                color_format = VkFormat.B8G8R8A8UNorm;
                color_space = surfaceFormats[0].colorSpace;
            }
            else
            {
                // iterate over the list of available Surface format and
                // check for the presence of VK_FORMAT_B8G8R8A8_UNORM
                bool found_B8G8R8A8_UNORM = false;


                // TODO: VkSurfaceFormatKHR -> stackalloc
                List<VkSurfaceFormatKHR> Formats = new List<VkSurfaceFormatKHR>();

                for (int i = 0; i < formatCount; i++)
                {
                    Formats.Add(surfaceFormats[i]);
                }

                foreach (VkSurfaceFormatKHR surfaceFormat in Formats)
                {
                    if (surfaceFormat.format is VkFormat.B8G8R8A8UNorm)
                    {
                        color_format = surfaceFormat.format;
                        color_space = surfaceFormat.colorSpace;
                        found_B8G8R8A8_UNORM = true;
                        break;
                    }
                }

                // in case VK_FORMAT_B8G8R8A8_UNORM is not available
                // select the first available color format
                if (!found_B8G8R8A8_UNORM)
                {
                    color_format = surfaceFormats[0].format;
                    color_space = surfaceFormats[0].colorSpace;
                }
            }

            if (vultaik_debug)
            {
                string message = $"ColorFormat = {color_format}, ColorSpace = {color_space}";
                ConsoleLog.Info("SwapChain", message);
            }



        }

        public void Resize(int width, int height)
        {
            CreateSwapChain(width, height);
            CreateBackBuffers();
        }


        public int Width { get; set; }
        public int Height { get; set; }
        public void CreateSwapChain(int width, int height)
        {

            VkPhysicalDevice PhysicalDevice = NativeDevice.NativeAdapter.handle;


            Width = width;
            Height = height;


            bool vsync = Description.VSync;
            // Get physical Device Surface properties and formats
            vkGetPhysicalDeviceSurfaceCapabilitiesKHR(PhysicalDevice, surface, out VkSurfaceCapabilitiesKHR surfCaps);



            // Get available present modes
            uint presentModeCount;
            vkGetPhysicalDeviceSurfacePresentModesKHR(PhysicalDevice, surface, &presentModeCount, null);
            VkPresentModeKHR* presentModes = stackalloc VkPresentModeKHR[(int)presentModeCount];
            vkGetPhysicalDeviceSurfacePresentModesKHR(PhysicalDevice, surface, &presentModeCount, presentModes);

            VkExtent2D swapchainExtent = default;
            // If width (and height) equals the special value 0xFFFFFFFF, the size of the Surface will be set by the swapchain
            if (surfCaps.currentExtent.width == unchecked(-1))
            {
                // If the Surface size is undefined, the size is set to
                // the size of the Images requested.

                swapchainExtent = new(width, height);
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
            VkPresentModeKHR swapchainPresentMode = VkPresentModeKHR.Fifo;

            // If v-sync is not requested, try to find a mailbox mode
            // It's the lowest latency non-tearing present mode available
            if (!vsync)
            {
                for (uint i = 0; i < presentModeCount; i++)
                {
                    if (presentModes[i] is VkPresentModeKHR.Mailbox)
                    {
                        swapchainPresentMode = VkPresentModeKHR.Mailbox;
                        break;
                    }
                    if ((swapchainPresentMode is not VkPresentModeKHR.Mailbox) && (presentModes[i] is VkPresentModeKHR.Immediate))
                    {
                        swapchainPresentMode = VkPresentModeKHR.Immediate;
                    }
                }
            }




            // Determine the number of Images
            uint desiredNumberOfSwapchainImages = surfCaps.minImageCount + 1;
            if ((surfCaps.maxImageCount > 0) && (desiredNumberOfSwapchainImages > surfCaps.maxImageCount))
            {
                desiredNumberOfSwapchainImages = surfCaps.maxImageCount;
            }



            // Find the transformation of the Surface
            VkSurfaceTransformFlagsKHR preTransform;
            if ((surfCaps.supportedTransforms & VkSurfaceTransformFlagsKHR.Identity) is not 0)
            {
                // We prefer a non-rotated transform
                preTransform = VkSurfaceTransformFlagsKHR.Identity;
            }
            else
            {
                preTransform = surfCaps.currentTransform;
            }



            // Find a supported composite alpha format (not all devices support alpha opaque)
            VkCompositeAlphaFlagsKHR compositeAlpha = VkCompositeAlphaFlagsKHR.Opaque;

            // Simply select the first composite alpha format available
            VkCompositeAlphaFlagsKHR[] compositeAlphaFlags = new[]
            {
                VkCompositeAlphaFlagsKHR.Opaque,
                VkCompositeAlphaFlagsKHR.PreMultiplied,
                VkCompositeAlphaFlagsKHR.PostMultiplied,
                VkCompositeAlphaFlagsKHR.Inherit,
            };

            foreach (VkCompositeAlphaFlagsKHR compositeAlphaFlag in compositeAlphaFlags)
            {
                if ((surfCaps.supportedCompositeAlpha & compositeAlphaFlag) is not 0)
                {
                    compositeAlpha = compositeAlphaFlag;
                    break;
                }
            }




            VkSwapchainKHR oldSwapchain = handle;

            VkSwapchainCreateInfoKHR swapchain_info = new()
            {
                sType = VkStructureType.SwapchainCreateInfoKHR,
                pNext = null,
                
                surface = surface,
                minImageCount = desiredNumberOfSwapchainImages,
                imageFormat = color_format,
                imageColorSpace = color_space,
                imageExtent = swapchainExtent,

                imageUsage = VkImageUsageFlags.ColorAttachment,
                preTransform = preTransform,
                imageArrayLayers = 1,
                imageSharingMode = VkSharingMode.Exclusive,
                queueFamilyIndexCount = 0,
                pQueueFamilyIndices = null,
                presentMode = swapchainPresentMode,
                oldSwapchain = oldSwapchain,
                
                // TODO: Setting clipped to VK_TRUE allows the implementation to discard rendering outside of the Surface area
                clipped = true,
                compositeAlpha = compositeAlpha,
            };


            // Enable transfer source on swap chain images if supported
            if ((surfCaps.supportedUsageFlags & VkImageUsageFlags.TransferSrc) is not 0)
            {
                swapchain_info.imageUsage |= VkImageUsageFlags.TransferSrc;
            }

            // Enable transfer destination on swap chain images if supported
            if ((surfCaps.supportedUsageFlags & VkImageUsageFlags.TransferDst) is not 0)
            {
                swapchain_info.imageUsage |= VkImageUsageFlags.TransferDst;
            }


            if (vultaik_debug)
            {
                ConsoleLog.Info("SwapChain", $"Width = {width}, Height = {height}");
                ConsoleLog.Info("SwapChain", $"PresentMode = {swapchainPresentMode}");
                ConsoleLog.Info("SwapChain", $"VSync = {vsync}");
                ConsoleLog.Info("SwapChain", $"SwapchainImages = {desiredNumberOfSwapchainImages}");
                ConsoleLog.Info("SwapChain", $"SurfaceTransform = {preTransform}");
                ConsoleLog.Info("SwapChain", $"CompositeAlpha = {compositeAlpha}");
                ConsoleLog.Info("SwapChain", $"ImageUsage = {swapchain_info.imageUsage}");
            }

            vkCreateSwapchainKHR(NativeDevice.handle, &swapchain_info, null, out handle).CheckResult();


            if (vultaik_debug)
            {
                ConsoleLog.Info("SwapChain", $"Handle = 0x{handle.Handle.ToString("X")}");
            }


            if (oldSwapchain.Handle != VkSwapchainKHR.Null)
            {

                if (vultaik_debug)
                {
                    ConsoleLog.Info("SwapChain Old", $"Handle = 0x{oldSwapchain.Handle.ToString("X")}");
                }

                for (uint i = 0; i < images.Length; i++)
                {
                    vkDestroyImageView(NativeDevice.handle, swapChain_image_views[i], null);
                }
                vkDestroySwapchainKHR(NativeDevice.handle, oldSwapchain, null);
            }


            uint imageCount;
            vkGetSwapchainImagesKHR(NativeDevice.handle, handle, &imageCount, null);
            images = new VkImage[imageCount];

            fixed (VkImage* images_ptr = images)
            {
                vkGetSwapchainImagesKHR(NativeDevice.handle, handle, &imageCount, images_ptr);
            }


        }


        public uint AcquireNextImage()
        {
            // By setting timeout to UINT64_MAX we will always wait until the next image has been acquired or an actual error is thrown
            // With that we don't have to handle VK_NOT_READY
            vkAcquireNextImageKHR(NativeDevice.handle, handle, ulong.MaxValue, NativeDevice.image_available_semaphore, new VkFence(), out uint i);
            return i;
        }




        public void Present()
        {
            VkSemaphore semaphore = NativeDevice.render_finished_semaphore;
            VkSwapchainKHR swapchain = handle;
            CommandBuffer commandBuffer = NativeDevice.GraphicsCommandBuffer;
            uint imageIndex = commandBuffer.imageIndex;

            VkPresentInfoKHR present_info = new()
            {
                sType = VkStructureType.PresentInfoKHR,
                pNext = null,
                pResults = null,
                waitSemaphoreCount = 1,
                pWaitSemaphores = &semaphore,
                swapchainCount = 1,
                pSwapchains = &swapchain,
                pImageIndices = &imageIndex /*Interop.AllocToPointer(ref commandBuffer.imageIndex)*/,
            };



            vkQueuePresentKHR(NativeDevice.graphics_queue, &present_info);
        }

        public void Dispose()
        {
            vkDestroySwapchainKHR(NativeDevice.handle, handle, null);
            vkDestroySurfaceKHR(NativeDevice.NativeAdapter.instance, surface, null);
        }
    }

}
