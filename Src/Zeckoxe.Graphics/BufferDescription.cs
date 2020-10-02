// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	BufferDescription.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{
    public struct BufferDescription
    {

        public int SizeInBytes;


        public int ByteStride;


        public BufferFlags BufferFlags;


        public GraphicsResourceUsage Usage;




        public BufferDescription(int sizeInBytes, BufferFlags bufferFlags, GraphicsResourceUsage usage, int byteStride = 0)
        {
            SizeInBytes = sizeInBytes;

            BufferFlags = bufferFlags;

            Usage = usage;

            ByteStride = byteStride;
        }

    }
}
