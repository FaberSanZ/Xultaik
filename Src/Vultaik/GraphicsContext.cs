// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	GraphicsContext.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Vultaik
{
    public class GraphicsContext
    {
        public CommandBuffer CommandBuffer { get; set; }

        public GraphicsContext(Device graphicsDevice, CommandBuffer? commandBuffer = null)
        {
            CommandBuffer = commandBuffer is null ? graphicsDevice.GraphicsCommandBuffer : commandBuffer;
        }
    }
}
