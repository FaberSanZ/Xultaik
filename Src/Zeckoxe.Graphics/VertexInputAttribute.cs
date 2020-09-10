// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	VertexInputAttribute.cs
=============================================================================*/

namespace Zeckoxe.Graphics
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
