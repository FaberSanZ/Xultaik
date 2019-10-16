
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


        public BufferFlags BufferFlags;




        public BufferDescription(int sizeInBytes, BufferFlags bufferFlags, int structureByteStride = 0)
        {
            SizeInBytes = sizeInBytes;

            BufferFlags = bufferFlags;

            StructureByteStride = structureByteStride;
        }

    }
}
