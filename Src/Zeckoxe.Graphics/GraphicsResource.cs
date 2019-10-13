// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

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
        public GraphicsDevice GraphicsDevice { get; private set; }

        protected GraphicsResource(GraphicsDevice device)
        {
            GraphicsDevice = device;
        }
    }
}
