// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	CommandListType.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{
    public enum CommandListType
    {
        Direct = 0,

        Bundle = 1,

        Compute = 2,

        Copy = 3,

        VideoDecode = 4,

        VideoProcess = 5,

        VideoEncode = 6
    }
}
