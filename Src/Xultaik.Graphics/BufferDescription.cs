// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


using System;
using System.Collections.Generic;
using System.Text;

namespace Xultaik.Graphics
{
    public struct BufferDescription
    {
        public int SizeInBytes;

        public int StructureByteStride;

        public HeapType HeapType;

        public BufferFlags Flags;



        public BufferDescription(int sizeInBytes, BufferFlags bufferFlags, HeapType heapType, int structureByteStride = 0)
        {
            SizeInBytes = sizeInBytes;
            Flags = bufferFlags;
            HeapType = heapType;
            StructureByteStride = structureByteStride;
        }

    }
}
