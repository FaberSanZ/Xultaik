// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	GraphicsResource.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;

namespace Vultaik
{
    public class GraphicsResource
    {
        public Device NativeDevice { get; private set; }

        public GraphicsResource()
        {
        }


        public GraphicsResource(Device device)
        {
            AttachToGraphicsDevice(device);
        }

        public void AttachToGraphicsDevice(Device device)
        {
            NativeDevice = device;
        }
    }
}
