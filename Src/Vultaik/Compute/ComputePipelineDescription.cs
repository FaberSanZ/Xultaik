// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;



namespace Vultaik
{
    public class ComputePipelineDescription
    {
        public ComputePipelineDescription()
        {

        }

        public ComputePipelineDescription(ShaderBytecode shader)
        {
            Shader = shader;
        }

        public ShaderBytecode Shader { get; set; }
    }
}
