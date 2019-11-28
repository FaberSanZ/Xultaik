
// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

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
