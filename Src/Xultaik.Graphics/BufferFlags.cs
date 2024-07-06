// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using System.Collections.Generic;
using System.Text;

namespace Xultaik.Graphics
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