// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using Vortice.Vulkan;


namespace Vultaik
{
    public class MultisampleState
    {
        public VkSampleCountFlags MultisampleCount { get; set; }
        public bool SampleShadingEnable { get; set; }
        public float MinSampleShading { get; set; }
        public bool AlphaToCoverageEnable { get; set; }
        public bool AlphaToOneEnable { get; set; }


        public MultisampleState()
        {
            MultisampleCount = VkSampleCountFlags.Count1;
        }

    }
}
