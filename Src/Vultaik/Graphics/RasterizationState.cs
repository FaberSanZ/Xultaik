// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using Vortice.Vulkan;


namespace Vultaik
{
    public class RasterizationState
    {
        public bool DepthClampEnable { get; set; }
        public bool RasterizerDiscardEnable { get; set; }
        public FillMode FillMode { get; set; }
        public VkCullModeFlags CullMode { get; set; }
        public VkFrontFace FrontFace { get; set; }
        public bool DepthBiasEnable { get; set; }
        public float DepthBiasConstantFactor { get; set; }
        public float DepthBiasClamp { get; set; }
        public float DepthBiasSlopeFactor { get; set; }
        public float LineWidth { get; set; } = 1.0F;


        public static RasterizationState Default() => new()
        {
            FillMode = FillMode.Solid,
            CullMode = VkCullModeFlags.None,
            FrontFace = VkFrontFace.Clockwise,
        };
    }
}
