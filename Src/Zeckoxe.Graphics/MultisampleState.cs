// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	InputAssemblyState.cs
=============================================================================*/

namespace Zeckoxe.Graphics
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