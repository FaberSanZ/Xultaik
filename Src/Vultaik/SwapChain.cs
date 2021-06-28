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
    public struct QueueFamilyIndices
    {
        public readonly int GraphicsFamily;
        public readonly int PresentFamily;

        public bool IsComplete => GraphicsFamily >= 0 && PresentFamily >= 0;

        public unsafe QueueFamilyIndices(VkPhysicalDevice device, VkSurfaceKHR surface)
        {
            int graphicsIndex = -1;
            int presentIndex = -1;

            uint queueFamilyCount = 0;
            vkGetPhysicalDeviceQueueFamilyProperties(device, &queueFamilyCount, null);
            VkQueueFamilyProperties* queueFamilies = stackalloc VkQueueFamilyProperties[(int)queueFamilyCount];
            vkGetPhysicalDeviceQueueFamilyProperties(device, &queueFamilyCount, queueFamilies);

            for (int i = 0; i < queueFamilyCount; i++)
            {
                VkQueueFamilyProperties q = queueFamilies[i];

                if (q.queueCount > 0 && (q.queueFlags & VkQueueFlags.Graphics) != 0)
                {
                    graphicsIndex = i;
                }

                vkGetPhysicalDeviceSurfaceSupportKHR(device, (uint)i, surface, out VkBool32 presentSupported);
                if (presentIndex < 0 && q.queueCount > 0 && presentSupported)
                {
                    presentIndex = i;
                }


            }

            GraphicsFamily = graphicsIndex;
            PresentFamily = presentIndex;
        }
    }
    public ref struct SwapChainSupportDetails
    {
        public VkSurfaceCapabilitiesKHR capabilities;
        public VkSurfaceFormatKHR[] formats;
        public VkPresentModeKHR[] presentModes;

        public bool IsComplete => formats.Length > 0 && presentModes.Length > 0;

        public unsafe SwapChainSupportDetails(VkPhysicalDevice device, VkSurfaceKHR surface)
        {
            formats = default;
            presentModes = default;
            capabilities = default;

            vkGetPhysicalDeviceSurfaceCapabilitiesKHR(device, surface, out capabilities);

            uint formatCount;
            vkGetPhysicalDeviceSurfaceFormatsKHR(device, surface, &formatCount, null); // Count
            formats = new VkSurfaceFormatKHR[formatCount];
            fixed (VkSurfaceFormatKHR* formatsPtr = formats)
            {
                vkGetPhysicalDeviceSurfaceFormatsKHR(device, surface, &formatCount, formatsPtr);
            }


            uint presentModeCount;
            vkGetPhysicalDeviceSurfacePresentModesKHR(device, surface, &presentModeCount, null); //Count 
            presentModes = new VkPresentModeKHR[presentModeCount];
            fixed (VkPresentModeKHR* presentsPtr = presentModes)
            {
                vkGetPhysicalDeviceSurfacePresentModesKHR(device, surface, &presentModeCount, presentsPtr);
            }

        }
    }




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

        internal bool vultaik_debug => AdapterConfig.VultaikDebug;


        public SwapChain(Device device, SwapchainDescription description) : base(device)
        {
            Description = description;
            AdapterConfig = NativeDevice.AdapterConfig;

            SwapchainSource = description.Source;

            surface = CreateSurface();
            //init_queue_family();

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
            Width = width;
            Height = height;
            VkPhysicalDevice physical_device = NativeDevice.NativeAdapter.handle;




            SwapChainSupportDetails swapChainSupport = new SwapChainSupportDetails(physical_device, surface);

            VkSurfaceFormatKHR surfaceFormat = ChooseSwapSurfaceFormat(swapChainSupport.formats);
            VkPresentModeKHR presentMode = ChooseSwapPresentMode(swapChainSupport.presentModes);
            VkExtent2D extent = ChooseSwapExtent(swapChainSupport.capabilities, (uint)width, (uint)height);

            uint imageCount = swapChainSupport.capabilities.minImageCount + 1;
            if (swapChainSupport.capabilities.maxImageCount > 0 && imageCount > swapChainSupport.capabilities.maxImageCount)
            {
                imageCount = Math.Min(imageCount, swapChainSupport.capabilities.maxImageCount);
            }

            VkSwapchainCreateInfoKHR createInfo = new VkSwapchainCreateInfoKHR()
            {
                sType = VkStructureType.SwapchainCreateInfoKHR,
                surface = surface,
                minImageCount = imageCount,
                imageFormat = surfaceFormat.format,
                imageColorSpace = surfaceFormat.colorSpace,
                imageExtent = extent,
                imageArrayLayers = 1,
                imageUsage = VkImageUsageFlags.ColorAttachment,
                preTransform = swapChainSupport.capabilities.currentTransform,
                compositeAlpha = VkCompositeAlphaFlagsKHR.Opaque,
                presentMode = presentMode,
                clipped = true,
            };

            QueueFamilyIndices indices = new QueueFamilyIndices(NativeDevice.NativeAdapter.handle, surface);

            uint* QueueFamilyIndicesPtr = stackalloc uint[]
            {
                (uint)indices.GraphicsFamily,
                (uint)indices.PresentFamily,
            };

            if (indices.GraphicsFamily != indices.PresentFamily)
            {
                createInfo.imageSharingMode = VkSharingMode.Concurrent;
                createInfo.pQueueFamilyIndices = QueueFamilyIndicesPtr;
            }
            else
            {
                createInfo.imageSharingMode = VkSharingMode.Exclusive;
            }

            createInfo.preTransform = VkSurfaceTransformFlagsKHR.Identity;

            vkCreateSwapchainKHR(NativeDevice.handle, &createInfo, null, out handle);



            //vkGetSwapchainImagesKHR(NativeDevice.handle, handle, &imageCount, null);
            images = vkGetSwapchainImagesKHR(NativeDevice.handle, handle).ToArray();

            //fixed (VkImage* img = images)
            //    vkGetSwapchainImagesKHR(NativeDevice.handle, handle, &imageCount, img);


            color_format = surfaceFormat.format;
            //extent = extent;
        }


        //internal void SetNewSwapchain(VkSwapchainKHR deviceSwapchain)
        //{


        //    // Get the images
        //    uint scImageCount = 0;
        //    VkResult result = vkGetSwapchainImagesKHR(NativeDevice.handle, deviceSwapchain, ref scImageCount, null);
        //    if (images == null)
        //    {
        //        images = new VkImage[(int)scImageCount];
        //    }
        //    result = vkGetSwapchainImagesKHR(NativeDevice.handle, deviceSwapchain, ref scImageCount, out _scImages[0]);
        //}


        private VkSurfaceFormatKHR ChooseSwapSurfaceFormat(VkSurfaceFormatKHR[] formats)
        {
            if (formats.Length == 1 && formats[0].format == VkFormat.Undefined)
            {
                return new VkSurfaceFormatKHR()
                {
                    format = VkFormat.B8G8R8A8UNorm,// 32 BITS BGRA
                    colorSpace = VkColorSpaceKHR.SrgbNonLinear
                };
            }

            foreach (VkSurfaceFormatKHR availableFormat in formats)
            {
                if (availableFormat.format == VkFormat.B8G8R8A8UNorm && availableFormat.colorSpace == VkColorSpaceKHR.SrgbNonLinear)
                {
                    return availableFormat;
                }
            }

            return formats[0];
        }

        private VkPresentModeKHR ChooseSwapPresentMode(VkPresentModeKHR[] presentModes)
        {
            //VkPresentModeKHR bestMode = VkPresentModeKHR.FifoKHR;

            foreach (VkPresentModeKHR availablePresentMode in presentModes)
            {
                if (availablePresentMode == VkPresentModeKHR.Mailbox)
                {
                    return availablePresentMode; // MailboxKHR
                }
                else if (availablePresentMode == VkPresentModeKHR.Immediate)
                {
                    return availablePresentMode; // ImmediateKHR;
                }
            }

            return VkPresentModeKHR.Immediate;
        }

        private VkExtent2D ChooseSwapExtent(VkSurfaceCapabilitiesKHR capabilities, uint width, uint height)
        {
            if (capabilities.currentExtent.width != int.MaxValue)
            {
                return capabilities.currentExtent;
            }

            return new VkExtent2D()
            {
                width = (uint)Math.Max(capabilities.minImageExtent.width, Math.Min(capabilities.maxImageExtent.width, (uint)width)),
                height = (uint)Math.Max(capabilities.minImageExtent.height, Math.Min(capabilities.maxImageExtent.height, (uint)height)),
            };
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
