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
        public CommandBuffer CommandBuffer { get; set; }


        public GraphicsContext(GraphicsDevice graphicsDevice, CommandBuffer? commandBuffer = null)
        {
            CommandBuffer = commandBuffer is null ? graphicsDevice.NativeCommand : new CommandBuffer(graphicsDevice);
        }
    }
}
