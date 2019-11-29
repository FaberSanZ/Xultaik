// Copyright(c) 2019-2020 Faber Leonardo.All Rights Reserved.

/*=============================================================================
	BufferFlags.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{
    [Flags]
    public enum BufferFlags
    {
        None = 0,

        ConstantBuffer = 1,

        IndexBuffer = 2,

        VertexBuffer = 4,

        RenderTarget = 8,

        ShaderResource = 16,

        UnorderedAccess = 32,

        StructuredBuffer = 64,

        StructuredAppendBuffer = UnorderedAccess | StructuredBuffer | 128,

        StructuredCounterBuffer = UnorderedAccess | StructuredBuffer | 256,

        RawBuffer = 512,

        ArgumentBuffer = 1024,

        StreamOutput = 2048,

    }
}