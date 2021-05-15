// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	Scene.cs
=============================================================================*/



using System.Collections.Generic;
using System.Numerics;
using Vultaik.Physics;

namespace Vultaik.GLTF
{
    public class Scene
    {
        public Scene()
        {

        }

        public string Name { get; set; }
        public Node Root { get; set; }
        public List<Node> GetNodes => Root?.Children;
        public BoundingBox AABB => Root.GetAABB(Matrix4x4.Identity);
        


        public Node FindNode(string name)
        {
            return Root is null ? null : Root.FindNode(name);
        }

    }
}
