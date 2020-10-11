// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	Texture2D.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{
    public class Texture2D : Texture
    {

        public Texture2D(Device device) : base(device, new TextureDescription() 
        { 
            Flags = TextureFlags.ShaderResource,
            
        })
        {
            Recreate();
        }


        public void Recreate()
        {

        }
    }
}
