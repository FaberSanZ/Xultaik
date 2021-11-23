// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using Interop = Vultaik.Interop;

namespace Vultaik
{

    public unsafe class Adapter : IDisposable
    {
        internal bool? _supportInitialized;
        internal VkInstance instance;

        internal VkDebugUtilsMessengerEXT _debugMessenger = VkDebugUtilsMessengerEXT.Null;

        internal uint instance_extensions_count;
        internal uint device_count; // number of GPUs we're rendering to --- if DG is disabled, this is 1


        internal List<string> device_extensions_names { get; private set; } = new();


        internal VkPhysicalDevice handle;
        internal VkPhysicalDevice[] handles;
        internal VkPhysicalDeviceProperties device_properties;


        public Adapter(AdapterConfig config)
        {
            AdapterConfig = config;

            Recreate();


            vultaik_debug();
        }

        public bool IsSupported { get; private set; }

        public VkFormat DepthFormat => get_supported_depth_format(FormatExtensions.depth_formats);

        public Version EngineVersion { get; internal set; } = new Version(1, 2, 155);

        public AdapterConfig AdapterConfig { get; set; }

        public List<string> InstanceExtensionsNames { get; private set; } = new();

        public List<string> ValidationLayer { get; private set; } = new();

        public VkPhysicalDeviceType DeviceType => device_properties.deviceType;

        public VendorId VendorId => (VendorId)device_properties.vendorID;

        public float TimestampPeriod => device_properties.limits.timestampPeriod;

        public uint MaxDrawIndirectCount => device_properties.limits.maxDrawIndirectCount;

        public ulong MinUniformBufferOffsetAlignment => device_properties.limits.minUniformBufferOffsetAlignment;

        public VkSampleCountFlags MultisampleCount => Helpers.ExtractMaxSampleCount(device_properties);

        public bool SupportsPhysicalDeviceProperties2 { get; private set; }

        public bool SupportsSurface { get; private set; }

        public bool SupportsWin32Surface { get; private set; }

        public bool SupportsWaylandSurface { get; private set; }

        public bool SupportsMacOSSurface { get; private set; }

        public bool SupportsX11Surface { get; private set; }

        public bool SupportsAndroidSurface { get; private set; }
        
        public bool SupportsExternal { get; private set; }

        public bool SupportsVulkan11Instance { get; private set; }

        public bool SupportsVulkan11Device { get; private set; }

        public bool SupportsDebugUtils { get; set; }

        public bool SupportsValidationGpuAssisted { get; set; }

        public DeviceExtension SwapChain { get; set; }
        public DeviceExtension Bindless { get; set; }
        public DeviceExtension ConservativeRasterization { get; set; }
        
        public DeviceExtension Arithmetic16BitStorage { get; set; }
        public DeviceExtension StorageBufferStorageclass { get; set; }
        public DeviceExtension Maintenance1 { get; set; }
        public DeviceExtension Maintenance2 { get; set; }
        public DeviceExtension Maintenance3 { get; set; }


        public string DeviceName
        {
            get
            {
                VkPhysicalDeviceProperties properties = device_properties;
                return Interop.String.FromPointer(properties.deviceName);
            }
        }

        public string Description
        {
            get
            {
                VkPhysicalDeviceProperties properties = device_properties;
                return Interop.String.FromPointer(properties.deviceName) + $" - {VendorId.ToString()}";
            }
        }



        public IntPtr Instance => instance.Handle;
        public bool IsNull => instance.IsNull || instance == VkInstance.Null;

        private bool is_supported()
        {
            if (_supportInitialized.HasValue)
                return _supportInitialized.Value;

            try
            {
                VkResult result = vkInitialize();
                _supportInitialized = result == VkResult.Success;
                return _supportInitialized.Value;
            }
            catch
            {
                _supportInitialized = false;
                return false;
            }
        }


        public void Recreate()
        {
            IsSupported = is_supported();

            if (!IsSupported)
                return;

            supports_extensions();

            CreateInstance(InstanceExtensionsNames.ToArray());

            CreatePhysicalDevice();

            CreatePhysicalDeviceProperties();

            device_extension();


            SupportsVulkan11Instance = vkEnumerateInstanceVersion() >= VkVersion.Version_1_1;


            if (device_properties.apiVersion >= VkVersion.Version_1_1) 
                SupportsVulkan11Device = SupportsVulkan11Instance;

        }


        internal void device_extension()
        {
            Maintenance1 = new("VK_KHR_maintenance1", false);
            Maintenance2 = new("VK_KHR_maintenance2", false);
            Maintenance3 = new("VK_KHR_maintenance3", false);
            SwapChain = new("VK_KHR_swapchain", false, AdapterConfig.SwapChain);
            Bindless = new("VK_EXT_descriptor_indexing", false, AdapterConfig.Bindless);
            ConservativeRasterization = new("VK_EXT_conservative_rasterization", AdapterConfig.ConservativeRasterization);
            StorageBufferStorageclass = new("VK_KHR_storage_buffer_storage_class", false);
            Arithmetic16BitStorage = new("VK_KHR_16bit_storage", false, AdapterConfig.Arithmetic16BitStorage);

            foreach (VkExtensionProperties item in vkEnumerateDeviceExtensionProperties(handle))
            {
                string name = Interop.String.FromPointer(item.extensionName);

                device_extensions_names.Add(name);


                if (Maintenance1.Name == name)
                    Maintenance1.Support = true;

                if (Maintenance2.Name == name)
                    Maintenance2.Support = true;

                if (Maintenance3.Name == name)
                    Maintenance3.Support = true;

                if (SwapChain.Name == name)
                    SwapChain.Support = true;

                if (Bindless.Name == name)
                    Bindless.Support = true;

                if (ConservativeRasterization.Name == name)
                    ConservativeRasterization.Support = true;

                if (StorageBufferStorageclass.Name == name)
                    StorageBufferStorageclass.Support = true;

                if (Arithmetic16BitStorage.Name == name)
                    Arithmetic16BitStorage.Support = true;


            }
        }


        internal void supports_extensions()
        {
            IEnumerable<string> instance_extensions_names = Instance_Extensions()
                                                    .ToArray()
                                                    .Select(_ => Interop.String.FromPointer(_.extensionName));


            if (AdapterConfig.Debug && instance_extensions_names.Contains("VK_EXT_debug_utils"));
            {
                InstanceExtensionsNames.Add("VK_EXT_debug_utils");
                SupportsDebugUtils = true;
            }

            if (SupportsDebugUtils && AdapterConfig.ValidationGpuAssisted && instance_extensions_names.Contains("VK_EXT_validation_features"))
            {
                InstanceExtensionsNames.Add("VK_EXT_validation_features");
                SupportsValidationGpuAssisted = true;
            }


            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (instance_extensions_names.Contains("VK_KHR_win32_surface"))
                {
                    InstanceExtensionsNames.Add("VK_KHR_win32_surface");
                    SupportsWin32Surface = true;
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (instance_extensions_names.Contains("VK_MVK_macos_surface"))
                {
                    InstanceExtensionsNames.Add("VK_MVK_macos_surface");
                    SupportsMacOSSurface = true;
                }

                if (instance_extensions_names.Contains("VK_MVK_ios_surface"))
                {
                    InstanceExtensionsNames.Add("VK_MVK_ios_surface");
                    SupportsMacOSSurface = true;
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (instance_extensions_names.Contains("VK_KHR_android_surface"))
                {
                    InstanceExtensionsNames.Add("VK_KHR_android_surface");
                    SupportsAndroidSurface = true;
                }

                if (instance_extensions_names.Contains("VK_KHR_xlib_surface"))
                {
                    InstanceExtensionsNames.Add("VK_KHR_xlib_surface");
                    SupportsX11Surface = true;
                }

                if (instance_extensions_names.Contains("VK_KHR_wayland_surface"))
                {
                    InstanceExtensionsNames.Add("VK_KHR_wayland_surface");
                    SupportsWaylandSurface = true;
                }
            }

            if (instance_extensions_names.Contains("VK_KHR_surface"))
            {
                InstanceExtensionsNames.Add("VK_KHR_surface");
                SupportsSurface = true;
            }

            if (instance_extensions_names.Contains("VK_KHR_get_physical_device_properties2"))
            {
                InstanceExtensionsNames.Add("VK_KHR_get_physical_device_properties2");
                SupportsPhysicalDeviceProperties2 = true;
            }


            if (SupportsPhysicalDeviceProperties2 && 
                instance_extensions_names.Contains("VK_KHR_external_memory_capabilities") && 
                instance_extensions_names.Contains("VK_KHR_external_semaphore_capabilities"))
            {
                InstanceExtensionsNames.Add("VK_KHR_external_memory_capabilities");
                InstanceExtensionsNames.Add("VK_KHR_external_semaphore_capabilities");
                SupportsExternal = true;
            }


        }

        internal void CreateInstance(string[] extensions)
        {

            VkApplicationInfo app_info = new()
            {
                sType = VkStructureType.ApplicationInfo,
                pNext = null,
                applicationVersion = new VkVersion(0, 0, 1),
                engineVersion = new VkVersion(EngineVersion.Major, EngineVersion.Minor, EngineVersion.Patch),
                pApplicationName = Interop.String.ToPointer("Engine"),
                pEngineName = Interop.String.ToPointer("Zeckoxe"),
                apiVersion = vkEnumerateInstanceVersion()
            };



            VkInstanceCreateInfo inst_info = new()
            {
                sType = VkStructureType.InstanceCreateInfo,
                pNext = null,
                flags = VkInstanceCreateFlags.None,
                pApplicationInfo = &app_info,
                ppEnabledExtensionNames = Interop.String.AllocToPointers(extensions),
                enabledExtensionCount = (uint)extensions.Length,
            };


            VkDebugUtilsMessengerCreateInfoEXT debug_utils_create_info = new() { sType = VkStructureType.DebugUtilsMessengerCreateInfoEXT };



            VkDebugUtilsMessageSeverityFlagsEXT messageSeverity = VkDebugUtilsMessageSeverityFlagsEXT.None;

            messageSeverity |= VkDebugUtilsMessageSeverityFlagsEXT.Info;
            messageSeverity |= VkDebugUtilsMessageSeverityFlagsEXT.Error;
            messageSeverity |= VkDebugUtilsMessageSeverityFlagsEXT.Verbose;
            messageSeverity |= VkDebugUtilsMessageSeverityFlagsEXT.Warning;

            List<string> requested_validation_layers = new();


            ReadOnlySpan<VkLayerProperties> availableLayers = vkEnumerateInstanceLayerProperties();

            if (AdapterConfig.Debug)
                foreach (var layer in availableLayers)
                    if ("VK_LAYER_KHRONOS_validation" == layer.GetLayerName())
                        requested_validation_layers.Add("VK_LAYER_KHRONOS_validation");


            if (requested_validation_layers.Any() && SupportsDebugUtils)
            {
                inst_info.enabledLayerCount = (uint)requested_validation_layers.Count;
                inst_info.ppEnabledLayerNames = new VkStringArray(requested_validation_layers);

                debug_utils_create_info.messageSeverity = messageSeverity;
                debug_utils_create_info.messageType = VkDebugUtilsMessageTypeFlagsEXT.Validation | VkDebugUtilsMessageTypeFlagsEXT.Performance;
                debug_utils_create_info.pfnUserCallback = &DebugMessengerCallback;

                inst_info.pNext = &debug_utils_create_info;
            }

            VkValidationFeaturesEXT validation_features_info = new() { sType = VkStructureType.ValidationFeaturesEXT };

            if (SupportsValidationGpuAssisted && AdapterConfig.ValidationGpuAssisted)
            {
                
                VkValidationFeatureEnableEXT* enable_features = stackalloc VkValidationFeatureEnableEXT[2]
                {
                    VkValidationFeatureEnableEXT.GpuAssistedReserveBindingSlot,
                    VkValidationFeatureEnableEXT.GpuAssisted,
                    //VkValidationFeatureEnableEXT.SynchronizationValidation
                };

                validation_features_info.enabledValidationFeatureCount = 2;
                validation_features_info.pEnabledValidationFeatures = enable_features;
                validation_features_info.pNext = inst_info.pNext;
                inst_info.pNext = &validation_features_info;
            }

            vkCreateInstance(&inst_info, null, out instance);
            vkLoadInstance(instance);

            if (requested_validation_layers.Any() && SupportsDebugUtils)
            {
                vkCreateDebugUtilsMessengerEXT(instance, &debug_utils_create_info, null, out _debugMessenger).CheckResult();
            }
        }




        [UnmanagedCallersOnly]
        private static uint DebugMessengerCallback(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity, VkDebugUtilsMessageTypeFlagsEXT messageTypes,
                                                           VkDebugUtilsMessengerCallbackDataEXT* pCallbackData, void* userData)
        {


            uint[] ignored_ids = new[]
            {
                0xc05b3a9du,
                0x2864340eu,
                0xbfcfaec2u,
                0x96f03c1cu,
                0x8189c842u,
                0x3d492883u,
                0x1608dec0u,

                0x9b4c6071u,    // TODO: VkDebugUtilsObjectNameInfoEXT
                0x90ef715du,    // TODO: UNASSIGNED-CoreValidation-DrawState-InvalidImageAspect
                0xf27b16au,     // TODO: VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL: when using a Depth or Stencil format
                0x34f84ef4u,    // TODO: vkCmdBeginRenderPass-initialLayout: If any of the initialLayout or finalLayout member of the VkAttachmentDescription
                0x4d08326du,    // TODO: vkEndCommandBuffer-commandBuffer  
                0xc7aabc16u,    // TODO: VkPresentInfoKHR-pImageIndices 
            };

            for (int i = 0; i < ignored_ids.Length; i++)
                if ((uint)pCallbackData->messageIdNumber == ignored_ids[i])
                    return VK_FALSE;

            string? message = Interop.String.FromPointer(pCallbackData->pMessage);

            switch (messageTypes)
            {
                case VkDebugUtilsMessageTypeFlagsEXT.Validation:
                    switch (messageSeverity)
                    {
                        case VkDebugUtilsMessageSeverityFlagsEXT.Info:
                            ConsoleLog.Info($"Vulkan", $" {message}");
                            break;
                        case VkDebugUtilsMessageSeverityFlagsEXT.Warning:
                            ConsoleLog.Warn("Vulkan", $" {message}");
                            break;
                        case VkDebugUtilsMessageSeverityFlagsEXT.Error:
                            ConsoleLog.Error($"Vulkan", $" {message}");
                            break;
                    }

                    break;
                default:
                    switch (messageSeverity)
                    {
                        case VkDebugUtilsMessageSeverityFlagsEXT.Info:
                            ConsoleLog.Info($"Vulkan", $" {message}");
                            break;
                        case VkDebugUtilsMessageSeverityFlagsEXT.Warning:
                            ConsoleLog.Warn("Vulkan", $" {message}");
                            break;
                        case VkDebugUtilsMessageSeverityFlagsEXT.Error:
                            ConsoleLog.Error($"Vulkan", $" {message}");
                            break;
                    }

                    break;
            }

            return VK_FALSE;
        }





        internal void CreatePhysicalDevice()
        {
            // Physical Device
            uint count = 0;
            vkEnumeratePhysicalDevices(instance, &count, null);
            VkPhysicalDevice* physicalDevicesptr = stackalloc VkPhysicalDevice[(int)count];
            vkEnumeratePhysicalDevices(instance, &count, physicalDevicesptr);

            device_count = count;

            handles = new VkPhysicalDevice[device_count];


            for (int i = 0; i < device_count; i++)
            {
                vkGetPhysicalDeviceProperties(physicalDevicesptr[i], out var properties);

                if (properties.deviceType != VkPhysicalDeviceType.IntegratedGpu && !AdapterConfig.IntegratedGpu)
                {
                    handle = physicalDevicesptr[i];
                    break;
                }

                handle = physicalDevicesptr[i];
            }


            // TODO: Multi GPU
            for (int i = 0; i < device_count; i++)
                handles[i] = physicalDevicesptr[i];

        }


        internal unsafe ReadOnlySpan<VkExtensionProperties> Instance_Extensions()
        {
            uint count = 0;
            vkEnumerateInstanceExtensionProperties(null, &count, null).CheckResult();

            ReadOnlySpan<VkExtensionProperties> properties = new VkExtensionProperties[count];
            fixed (VkExtensionProperties* ptr = properties)
            {
                vkEnumerateInstanceExtensionProperties(null, &count, ptr).CheckResult();
            }

            return properties;
        }



        internal VkExtensionProperties* Instance_ExtensionsPtr()
        {
            uint count = 0;

            vkEnumerateInstanceExtensionProperties(null, &count, null).CheckResult();
            instance_extensions_count = count;

            VkExtensionProperties* ext = stackalloc VkExtensionProperties[(int)count];
            vkEnumerateInstanceExtensionProperties(null, &count, ext).CheckResult();
            return ext;
        }


        internal VkFormat get_supported_depth_format(IEnumerable<VkFormat> depthFormats)
        {
            // Since all depth formats may be optional, we need to find a suitable depth format to use
            // Start with the highest precision packed format

            VkFormat depthFormat = VkFormat.Undefined;

            foreach (VkFormat format in depthFormats)
            {
                vkGetPhysicalDeviceFormatProperties(handle, format, out VkFormatProperties formatProps);

                // Format must support depth stencil attachment for optimal tiling
                if ((formatProps.optimalTilingFeatures & VkFormatFeatureFlags.DepthStencilAttachment) is not 0)
                {
                    depthFormat = format;
                }
            }

            return depthFormat;
        }


        public PixelFormat GetSupportedDepthFormat(IEnumerable<PixelFormat> formats)
        {
            return (PixelFormat)get_supported_depth_format((IEnumerable<VkFormat>)formats);
        }



       




        internal void CreatePhysicalDeviceProperties()
        {
            vkGetPhysicalDeviceProperties(handle, out device_properties);
        }



        public void Dispose()
        {

            if (_debugMessenger != VkDebugUtilsMessengerEXT.Null)
                vkDestroyDebugUtilsMessengerEXT(instance, _debugMessenger, null);

            vkDestroyInstance(instance, null);
            instance = VkInstance.Null;
        }


        private void vultaik_debug()
        {
            if (AdapterConfig.Debug)
            {
                ConsoleLog.Info("Adapter", $"Handle = 0x{handle.Handle.ToString("X")}");
                ConsoleLog.Info("Adapter", $"DepthFormat = {DepthFormat}");
                ConsoleLog.Info("Adapter", $"MultisampleCount = {MultisampleCount}");
                ConsoleLog.Info("Adapter", $"VendorId = {VendorId}");
                ConsoleLog.Info("Adapter", $"DeviceType = {DeviceType}");
                ConsoleLog.Info("Adapter", $"MaxDrawIndirectCount = {MaxDrawIndirectCount}");
                ConsoleLog.Info("Adapter", $"MinUniformBufferOffsetAlignment = {MinUniformBufferOffsetAlignment}");
                ConsoleLog.Info("Adapter", $"SupportsValidationGpuAssisted = {SupportsValidationGpuAssisted}");
                ConsoleLog.Info("Adapter", $"SupportsDebugUtils = {SupportsDebugUtils}");
                ConsoleLog.Info("Adapter", $"SupportsExternal = {SupportsExternal}");

                ConsoleLog.Info("Adapter - ValidationLayer", $"ValidationLayer = {ValidationLayer.Count}");
                foreach (var v in ValidationLayer)
                    ConsoleLog.InfoNode("ValidationLayer - ValidationLayer", $"Layer = {v}", v == ValidationLayer.Last());

                ConsoleLog.Info("Adapter", $"{EngineVersion.ToString()}");
                ConsoleLog.Info("Adapter", $"TimestampPeriod = {TimestampPeriod}", true);
            }
        }
    }
}
