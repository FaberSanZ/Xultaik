// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	GraphicsResource.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{
    public class GraphicsResource
    {
        public GraphicsDevice NativeDevice { get; private set; }

        public GraphicsResource(GraphicsDevice device)
        {
            AttachToGraphicsDevice(device);
        }

        public void AttachToGraphicsDevice(GraphicsDevice device)
        {
            NativeDevice = device;
        }
    }
}
