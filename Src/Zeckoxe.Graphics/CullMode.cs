// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	CullMode.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{
    public enum CullMode : int
    {
        None = unchecked(0),

        Front = unchecked(1),

        Back = unchecked(2),
    }
}
