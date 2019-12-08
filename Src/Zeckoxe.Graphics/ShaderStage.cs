// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	ShaderStage.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{
    public enum ShaderStage
    {
        VertexShader = 0,

        PixelShader = 1,

        GeometryShader = 2,

        HullShader = 3,

        DomainShader = 4,

        ComputeShader = 5,

        Library = 6,

        Count = 7
    }
}
