// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Vortice.Vulkan;


namespace Vultaik
{
    public class ComputePipelineDescription
    {

        //public VkPipelineCreateFlags flags;
        public ShaderBytecode Shader { get; set; }
        public VkPipelineLayout layout;

        public VkPipeline basePipelineHandle;
        public int basePipelineIndex;
    }
}
