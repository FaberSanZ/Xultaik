// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	Primitive.cs
=============================================================================*/


using Zeckoxe.Graphics;
using Zeckoxe.Physics;

namespace Zeckoxe.GLTF
{
    public class Primitive
    {
        public Primitive()
        {
        }

        public Primitive(int vertexCount, int indexCount, int firstVertex = 0, int firstIndex = 0)
        {
            VertexCount = vertexCount;
            IndexCount = indexCount;
            FirstVertex = firstVertex;
            FirstIndex = firstIndex;
        }

        public BoundingBox BoundingBox;

        public string Name { get; set; }
        public int FirstIndex { get; set; }
        public int FirstVertex { get; set; }
        public int VertexCount { get; set; }
        public int IndexCount { get; set; }
        public int Material { get; set; }





        public void Draw(CommandBuffer cmd, int instanceCount = 1, int firstInstance = 0)
        {
            //cmd.DrawIndexed(IndexCount, instanceCount, IndexBase, VertexBase, firstInstance);
        }
    }
}
