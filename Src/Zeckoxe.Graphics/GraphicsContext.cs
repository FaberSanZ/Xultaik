// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	GraphicsContext.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{
    public class GraphicsContext
    {
        public CommandList CommandList { get; set; }


        public GraphicsContext(GraphicsDevice graphicsDevice, CommandList? commandList = null)
        {
            CommandList = commandList is null ? graphicsDevice.NativeCommandList : new CommandList(graphicsDevice);
        }
    }
}
