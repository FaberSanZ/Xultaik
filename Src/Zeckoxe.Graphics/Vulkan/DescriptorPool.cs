// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	DescriptorPool.cs
=============================================================================*/

namespace Zeckoxe.Graphics
{
    public class DescriptorPool
    {
        public DescriptorPool(DescriptorType type, int count)
        {
            Type = type;
            Count = count;
        }

        public DescriptorType Type { get; set; }
        public int Count { get; set; }
    }
}
