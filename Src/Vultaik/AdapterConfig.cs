// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using System.Collections.Generic;
using System.Text;

namespace Vultaik
{
    public class AdapterConfig
    {
        public bool Debug { get; set; }

        public bool ValidationGpuAssisted { get; set; }

        public bool Fullscreen { get; set; } // TODO: Implement Fullscreen 

        public bool RayTracing { get; set; } // TODO: Implement RayTracing 

        public bool ConservativeRasterization { get; set; } // TODO: Implement ConservativeRasterization 

        public bool ConditionalRendering { get; set; } // TODO: Implement ConditionalRendering 

        public bool ShadingRate { get; set; } // TODO: Implement ShadingRate 

        public bool Bindless { get; set; }

        public bool SwapChain { get; set; } 

        public bool Arithmetic16BitStorage { get; set; }

        public bool IntegratedGpu { get; set; }

    }



}
