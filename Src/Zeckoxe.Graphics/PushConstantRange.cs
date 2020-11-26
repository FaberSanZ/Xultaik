// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	PushConstantRange.cs
=============================================================================*/

namespace Zeckoxe.Graphics
{
    public class PushConstantRange
    {
        public PushConstantRange(ShaderStage stage, int offset, int size)
        {
            Stage = stage;
            Offset = offset;
            Size = size;
        }

        public PushConstantRange()
        {

        }

        public ShaderStage Stage { get; set; }
        public int Offset { get; set; }
        public int Size { get; set; }

    }
}
