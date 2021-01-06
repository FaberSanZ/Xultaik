// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	CommandList.cs
=============================================================================*/

namespace Zeckoxe.Vulkan
{
    public enum OptionalDeviceExtensions
    {
        RayTracing = 1 << 0,
        ConservativeRasterization = 1 << 1,
        CopyCommands2 = 1 << 2,
        BindMemory2 = 1 << 3,
        Multiview = 1 << 4,
        ConditionalRendering = 1 << 5,
        ShadingRate = 1 << 6,
    }
}
