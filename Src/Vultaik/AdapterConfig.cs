// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using System.Collections.Generic;
using System.Text;

namespace Vultaik
{
    public class AdapterConfig
    {
        public int BackBufferWidth { get; set; }

        public int BackBufferHeight { get; set; }

        public bool VulkanDebug { get; set; }

        public bool Fullscreen { get; set; }

        public bool VSync { get; set; }

        public bool RayTracing { get; set; }

        public bool ConservativeRasterization { get; set; }

        public bool CopyCommands2 { get; set; }

        public bool BindMemory2 { get; set; }

        public bool Multiview { get; set; }

        public bool ConditionalRendering { get; set; }

        public bool ShadingRate { get; set; }

        public bool Bindless { get; set; }
    }



}
