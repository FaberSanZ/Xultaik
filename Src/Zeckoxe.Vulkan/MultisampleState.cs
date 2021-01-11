// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	InputAssemblyState.cs
=============================================================================*/

namespace Zeckoxe.Vulkan
{
    public class MultisampleState
    {
        public MultisampleCount MultisampleCount { get; set; }
        public bool SampleShadingEnable { get; set; }
        public float MinSampleShading { get; set; }
        public bool AlphaToCoverageEnable { get; set; }
        public bool AlphaToOneEnable { get; set; }


        public MultisampleState()
        {
            MultisampleCount = MultisampleCount.X1;
        }

    }
}