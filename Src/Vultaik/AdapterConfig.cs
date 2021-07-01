// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using System.Collections.Generic;
using System.Text;

namespace Vultaik
{
    public class AdapterConfig
    {
        public bool VulkanDebug { get; set; }

        public bool ValidationGpuAssisted { get; set; }

        public bool VultaikDebug { get; set; } // TODO: Implement VultaikDebug 

        public bool Fullscreen { get; set; } // TODO: Implement Fullscreen 

        public bool RayTracing { get; set; } // TODO: Implement RayTracing 

        public bool ConservativeRasterization { get; set; } // TODO: Implement ConservativeRasterization 

        public bool CopyCommands2 { get; set; } // TODO: Implement CopyCommands2 

        public bool BindMemory2 { get; set; } // TODO: Implement BindMemory2 

        public bool Multiview { get; set; } // TODO: Implement Multiview 

        public bool ConditionalRendering { get; set; } // TODO: Implement ConditionalRendering 

        public bool ShadingRate { get; set; } // TODO: Implement ShadingRate 

        public bool Bindless { get; set; } // TODO: Implement Bindless 

        public bool SingleQueue { get; set; } // TODO: Implement SingleQueue 

        public bool ForceExclusiveTransferQueue { get; set; }

        public bool SwapChain { get; set; }
    }



}
