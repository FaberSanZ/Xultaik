// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	Mesh.cs
=============================================================================*/


using System.Collections.Generic;
using Vultaik.Physics;

namespace Vultaik.GLTF
{
    public class Mesh
    {

        public Mesh()
        {
            Primitives = new();
        }


        public BoundingBox BoundingBox;

        public string Name { get; set; }
        public List<Primitive> Primitives { get; set; }




        /// <summary>
        /// Add primitive and update mesh bounding box
        /// </summary>
        public void AddPrimitive(Primitive p)
        {
            if (Primitives.Count == 0)
            {
                BoundingBox = p.BoundingBox;
            }
            else
            {
                BoundingBox += p.BoundingBox;
            }

            Primitives.Add(p);
        }
    }
}
