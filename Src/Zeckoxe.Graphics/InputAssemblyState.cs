// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	InputAssemblyState.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{
    public class InputAssemblyState
    {
        public PrimitiveType PrimitiveType { get; set; }
        public bool PrimitiveRestartEnable { get; set; }

        public InputAssemblyState(PrimitiveType Type, bool RestartEnable)
        {
            PrimitiveType = Type;
            PrimitiveRestartEnable = RestartEnable;
        }

        public InputAssemblyState()
        {
            PrimitiveType = PrimitiveType.TriangleList;
            PrimitiveRestartEnable = false;
        }


        public static InputAssemblyState Default() => new InputAssemblyState()
        {
            PrimitiveRestartEnable = false,
            PrimitiveType = PrimitiveType.TriangleList
        };

    }
}
