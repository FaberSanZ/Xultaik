// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	GraphicsAdapter.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Vortice.Vulkan;
using Zeckoxe.Core;
using static Vortice.Vulkan.Vulkan;
using Interop = Zeckoxe.Core.Interop;

namespace Zeckoxe.Vulkan
{
    public unsafe class Adapter : IDisposable
    {
        internal VkInstance instance;

        internal vkDebugUtilsMessengerCallbackEXT _debugMessengerCallbackFunc;
        internal VkDebugUtilsMessengerEXT _debugMessenger = VkDebugUtilsMessengerEXT.Null;

        internal uint instance_extensions_count;
        internal uint device_count; // number of GPUs we're rendering to --- if DG is disabled, this is 1


        internal List<string> device_extensions_names { get; private set; } = new();


        internal VkPhysicalDevice handle;
        internal VkPhysicalDevice[] handles;
        internal VkPhysicalDeviceProperties device_properties;





        public Adapter(PresentationParameters parameters)
        {
            Parameters = parameters;

            Recreate();
        }


        public Version EngineVersion { get; internal set; } = new Version(1, 2, 155);

        public PresentationParameters Parameters { get; set; }

        public List<string> InstanceExtensionsNames { get; private set; } = new();

        public List<string> ValidationLayer { get; private set; } = new();

        public List<ILog> Log { get; set; }

        public DeviceType DeviceType => (DeviceType)device_properties.deviceType;

        public uint VendorId => device_properties.vendorID;

        public bool Validation = false;

        public bool RayTracingSupport => false;

        public float TimestampPeriod => device_properties.limits.timestampPeriod;

        public uint MaxDrawIndirectCount => device_properties.limits.maxDrawIndirectCount;

        public MultisampleCount MultisampleCount => (MultisampleCount)Tools.ExtractMaxSampleCount(device_properties); // TODO: MultisampleCount.ToVkSampleCountFlags

        public bool SupportsPhysicalDeviceProperties2 { get; private set; }

        public bool SupportsSurface { get; private set; }

        public bool SupportsWin32Surface { get; private set; }
        public bool SupportsWaylandSurface { get; private set; }
        public bool SupportsMacOSSurface { get; private set; }
        public bool SupportsX11Surface { get; private set; }
        public bool SupportsAndroidSurface { get; private set; }
        

        public bool SupportsExternal { get; private set; }

        public bool SupportsDedicated { get; private set; }

        public bool SupportsImageFormatList { get; private set; }

        public bool SupportsDebugMarker { get; private set; }

        public bool SupportsDebugUtils { get; private set; }

        public bool SupportsMirrorClampToEdge { get; private set; }

        public bool SupportsGoogleDisplay_timing { get; private set; }

        public bool SupportsNV_DeviceDiagnosticCheckPoints { get; private set; }

        public bool SupportsVulkan11Instance { get; private set; }

        public bool SupportsVulkan11Device { get; private set; }

        public bool SupportsExternalMemoryHost { get; private set; }

        public bool SupportsSurfaceCapabilities2 { get; private set; }

        public bool SupportsFullScreenExclusive { get; private set; }

        public bool SupportsUpdateTemplate { get; private set; }

        public bool SupportsMaintenance_1 { get; private set; }

        public bool SupportsMaintenance_2 { get; private set; }

        public bool SupportsMaintenance_3 { get; private set; }

        public bool SupportsDescriptorIndexing { get; private set; }

        public bool SupportsConservativeRasterization { get; private set; }

        public bool SupportsBindMemory2 { get; private set; }

        public bool SupportsGetMemoryRequirements2 { get; private set; }

        public bool SupportsDrawIndirectCount { get; private set; }

        public bool SupportsDrawParameters { get; private set; }

        public bool SupportsDriverProperties { get; private set; }

        public bool SupportsCalibratedTimestamps { get; private set; }


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
                return Interop.String.FromPointer(properties.deviceName) + $" - {VendorNameString(VendorId)}";
            }
        }



        public VkInstance GetInstance()
        {
            return instance;
        }



        public void Recreate()
        {

            var _validation = Parameters.Settings.Validation;

            Validation = (_validation & ValidationType.Default) is not 0
                || (_validation & ValidationType.Console) is not 0
                || (_validation & ValidationType.Debug) is not 0
                || (_validation & ValidationType.ImGui) is not 0;

            vkInitialize().CheckResult();

            supports_extensions();

            CreateInstance(InstanceExtensionsNames.ToArray());

            CreatePhysicalDevice();

            CreatePhysicalDeviceProperties();

            device_extension();


            SupportsVulkan11Instance = vkEnumerateInstanceVersion() >= VkVersion.Version_1_1;


            if (device_properties.apiVersion >= VkVersion.Version_1_1) 
                SupportsVulkan11Device = SupportsVulkan11Instance;


            Log = new();


            if ((Parameters.Settings.Validation & ValidationType.Default) != 0)
                Log.Add(new ConsoleLog());

            if ((Parameters.Settings.Validation & ValidationType.Console) != 0)
                Log.Add(new ConsoleLog());

            if ((Parameters.Settings.Validation & ValidationType.Debug) != 0)
                Log.Add(new ConsoleLog()); // TODO: DebugLog

            if ((Parameters.Settings.Validation & ValidationType.ImGui) != 0)
                Log.Add(new ConsoleLog()); // TODO:ImGuiLog

        }


        internal void device_extension()
        {
            foreach (VkExtensionProperties item in vkEnumerateDeviceExtensionProperties(handle))
            {
                device_extensions_names.Add(Interop.String.FromPointer(item.extensionName));
            }
        }

        internal void supports_extensions()
        {
            IEnumerable<string> instance_extensions_names = Instance_Extensions()
                                                    .ToArray()
                                                    .Select(_ => Interop.String.FromPointer(_.extensionName));


            if (!((Parameters.Settings.Validation & ValidationType.None) is not 0) && instance_extensions_names.Contains("VK_EXT_debug_report"))
            {
                InstanceExtensionsNames.Add("VK_EXT_debug_report");
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
                apiVersion = new VkVersion(1, 0, 0),
                applicationVersion = new VkVersion(0, 0, 1),
                engineVersion = new VkVersion(EngineVersion.Major, EngineVersion.Minor, EngineVersion.Patch),
                pApplicationName = Interop.String.ToPointer("Engine"),
                pEngineName = Interop.String.ToPointer("Zeckoxe"),
            };


            // TODO: layers
            string[] layers = new[]
            {
                "VK_LAYER_KHRONOS_validation",
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

            VkDebugUtilsMessengerCreateInfoEXT debugUtilsCreateInfo = new()
            {
                sType = VkStructureType.DebugUtilsMessengerCreateInfoEXT,
                pNext = null,
                flags = VkDebugUtilsMessengerCreateFlagsEXT.None,
                pUserData = null,
                messageSeverity = VkDebugUtilsMessageSeverityFlagsEXT.Error | VkDebugUtilsMessageSeverityFlagsEXT.Warning | VkDebugUtilsMessageSeverityFlagsEXT.Info,
                messageType = VkDebugUtilsMessageTypeFlagsEXT.Validation | VkDebugUtilsMessageTypeFlagsEXT.Performance,
                pfnUserCallback = Interop.GetFunctionPointerForDelegate(_debugMessengerCallbackFunc = DebugMessengerCallback),
            };

            if (Validation)
            {
                inst_info.pNext = &debugUtilsCreateInfo;
                inst_info.ppEnabledLayerNames = Interop.String.AllocToPointers(layers);
                inst_info.enabledLayerCount = (uint)layers.Length;

            }

            vkCreateInstance(&inst_info, null, out instance);
            vkLoadInstance(instance);

            //vkCreateDebugUtilsMessengerEXT(instance, &debugUtilsCreateInfo, null, out _debugMessenger).CheckResult();
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

            if (device_count >= 1 )
                handle = physicalDevicesptr[0];

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


        public PixelFormat GetSupportedDepthFormat(IEnumerable<PixelFormat> depthFormats)
        {
            // Since all depth formats may be optional, we need to find a suitable depth format to use
            // Start with the highest precision packed format

            PixelFormat depthFormat = PixelFormat.Undefined;

            foreach (PixelFormat format in depthFormats)
            {
                vkGetPhysicalDeviceFormatProperties(handle, (VkFormat)format, out VkFormatProperties formatProps);

                // Format must support depth stencil attachment for optimal tiling
                if ((formatProps.optimalTilingFeatures & VkFormatFeatureFlags.DepthStencilAttachment) is not 0)
                {
                    depthFormat = format;
                }
            }



            return depthFormat;
        }



        internal VkBool32 DebugMessengerCallback(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity, VkDebugUtilsMessageTypeFlagsEXT messageTypes, VkDebugUtilsMessengerCallbackDataEXT* pCallbackData, IntPtr userData)
        {
            string message = Interop.String.FromPointer(pCallbackData->pMessage);

            if (messageTypes is VkDebugUtilsMessageTypeFlagsEXT.Validation)
            {
                if (messageSeverity is VkDebugUtilsMessageSeverityFlagsEXT.Error)
                {
                    
                    if (Validation)
                        foreach (var l in Log)
                            l.Error("Vulkan", $"Validation: {messageSeverity} - {message}");

                }
                else if (messageSeverity is VkDebugUtilsMessageSeverityFlagsEXT.Warning)
                {
                    if (Validation)
                        foreach (var l in Log)
                            l.Warn($"[Vulkan]: Validation: {messageSeverity} - {message}");
                }

            }
            else
            {
                if (messageSeverity is VkDebugUtilsMessageSeverityFlagsEXT.Error)
                {
                    if (Validation)
                        foreach (var l in Log)
                            l.Error("Vulkan", $"[Vulkan]: {messageSeverity} - {message}");
                }
                else if (messageSeverity is VkDebugUtilsMessageSeverityFlagsEXT.Warning)
                {
                    if (Validation)
                        foreach (var l in Log)
                            l.Warn($"[Vulkan]: {messageSeverity} - {message}");
                }

                //foreach (var l in Log)
                //    l.WriteLine($"[Vulkan]: {messageSeverity} - {message}");
            }

            return VkBool32.False;
        }





        internal void CreatePhysicalDeviceProperties()
        {
            vkGetPhysicalDeviceProperties(handle, out device_properties);
        }




        internal string VendorNameString(uint vendorId)
        {
            switch (vendorId)
            {
                case 0x1002:
                    return "AMD";

                case 0x1010:
                    return "ImgTec";

                case 0x10DE:
                    return "NVIDIA";

                case 0x13B5:
                    return "ARM";

                case 0x5143:
                    return "Qualcomm";

                case 0x8086:
                    return "Intel";

                default:
                    return "Unknown";
            }
        }






        public void Dispose()
        {
            if (_debugMessenger != VkDebugUtilsMessengerEXT.Null)
                vkDestroyDebugUtilsMessengerEXT(instance, _debugMessenger, null);

            vkDestroyInstance(instance, null);
        }
    }
}
