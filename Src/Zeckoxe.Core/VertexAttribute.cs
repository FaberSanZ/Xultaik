// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	VertexAttribute.cs
=============================================================================*/


using System;

namespace Zeckoxe.Core
{
    public class VertexAttribute : Attribute
    {
        public VertexAttributeType Type;

        public VertexAttribute(VertexAttributeType type)
        {
            Type = type;
        }
    }
}
