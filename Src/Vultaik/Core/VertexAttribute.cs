// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	VertexAttribute.cs
=============================================================================*/


using System;

namespace Vultaik
{
    public class VertexAttribute : Attribute
    {
        public VertexType Type;

        public VertexAttribute(VertexType type)
        {
            Type = type;
        }
    }
}
