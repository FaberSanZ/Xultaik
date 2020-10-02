// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	DebugReport.cs
=============================================================================*/


using System;
using System.Runtime.InteropServices;
using Vortice.Vulkan;

namespace Zeckoxe.Graphics
{
    // TODO: Use the new VK_EXT_debug_utils extension 
    public class DebugReport : GraphicsResource
    {
        internal VkInstance _instance;
        internal VkDebugReportCallbackEXT handle;
        //internal PFN_vkDebugReportCallbackEXT debugCallbackDelegate = new PFN_vkDebugReportCallbackEXT(debugCallback);


        public DebugReport(GraphicsDevice device) : base(device)
        {
            _instance = device.NativeAdapter.instance;
        }


        internal VkBool32 debugCallback(VkDebugReportFlagsEXT flags, VkDebugReportObjectTypeEXT objectType, ulong obj, UIntPtr location, int messageCode, IntPtr pLayerPrefix, IntPtr pMessage, IntPtr pUserData)
        {
            string prefix = "";
            switch (flags)
            {
                case 0:
                    prefix = "?";
                    break;
                case VkDebugReportFlagsEXT.Information:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    prefix = "INFO";
                    break;
                case VkDebugReportFlagsEXT.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    prefix = "WARN";
                    break;
                case VkDebugReportFlagsEXT.PerformanceWarning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    prefix = "PERF";
                    break;
                case VkDebugReportFlagsEXT.Error:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    prefix = "EROR";
                    break;
                case VkDebugReportFlagsEXT.Debug:
                    Console.ForegroundColor = ConsoleColor.Red;
                    prefix = "DBUG";
                    break;
            }
            try
            {
                string msg = Marshal.PtrToStringAnsi(pMessage);
                string[] tmp = msg.Split('|');
                Console.WriteLine($"{prefix}:{tmp[1]} |{Marshal.PtrToStringAnsi(pLayerPrefix)}({messageCode}){objectType}:{tmp[0]}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("error parsing debug message: " + ex);
            }
            Console.ForegroundColor = ConsoleColor.White;
            return VkBool32.False;
        }





        internal unsafe delegate uint PFN_vkDebugReportCallbackEXT(uint flags, VkDebugReportObjectTypeEXT objectType, ulong @object, UIntPtr location, int messageCode, byte* pLayerPrefix, byte* pMessage, void* pUserData);

        internal unsafe delegate VkResult vkCreateDebugReportCallbackEXT_d(VkInstance instance, VkDebugReportCallbackCreateInfoEXT* createInfo, IntPtr allocatorPtr, out VkDebugReportCallbackEXT ret);

        internal unsafe delegate void vkDestroyDebugReportCallbackEXT_d(VkInstance instance, VkDebugReportCallbackEXT callback, VkAllocationCallbacks* pAllocator);
    }
}
