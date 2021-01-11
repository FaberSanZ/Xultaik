// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	VertexInputAttribute.cs
=============================================================================*/

namespace Zeckoxe.Vulkan
{

    public class VertexInputAttribute
    {
        public VertexInputAttribute()
        {

        }

        public int Location { get; set; }
        public int Binding { get; set; }
        public PixelFormat Format { get; set; }
        public int Offset { get; set; }
    }
}
