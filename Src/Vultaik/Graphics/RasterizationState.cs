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
        public CullMode CullMode { get; set; }
        public FrontFace FrontFace { get; set; }
        public bool DepthBiasEnable { get; set; }
        public float DepthBiasConstantFactor { get; set; } = 0;
        public float DepthBiasClamp { get; set; } = 0;
        public float DepthBiasSlopeFactor { get; set; }
        public float LineWidth { get; set; } = 1.0F;


        public static RasterizationState Default() => new()
        {
            FillMode = FillMode.Solid,
            CullMode = CullMode.None,
            FrontFace = FrontFace.Clockwise,
            DepthBiasClamp = 0,
            DepthBiasConstantFactor = 0,
        };
    }
}
