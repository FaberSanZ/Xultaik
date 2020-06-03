// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	GLTFLoader.cs
=============================================================================*/


using SharpGLTF.Runtime;
using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zeckoxe.GLTF
{
    internal class GLTFLoader
    {
        private readonly IList<System.Numerics.Vector3> _Positions;
        private readonly IList<System.Numerics.Vector3> _NORMAL;
        private readonly IList<System.Numerics.Vector2> _Textute;


        public GLTFLoader(string FileName)
        {

            //Vertices = new ModelPart<Vertex>();

            //Indices = new ModelPart<int>();



            ModelRoot model = ModelRoot.Load(FileName);
            Console.WriteLine(model.LogicalTextures[0].Name);

            SceneTemplate[] templates = model.LogicalScenes.Select(item => SceneTemplate.Create(item, true)).ToArray();
            Dictionary<int, SharpGLTF.Schema2.Mesh> meshes = templates.SelectMany(item => item.LogicalMeshIds).ToDictionary(k => k, k => model.LogicalMeshes[k]);

            //Console.WriteLine("Logical Images Count {0}", model.LogicalImages.Count);

            foreach (Image image in model.LogicalImages)
            {

            }



            List<MeshPrimitive> srcPrims = GetValidPrimitives(meshes[0]).ToList();

            foreach (MeshPrimitive srcPrim in srcPrims)
            {
                _Positions = srcPrim.GetVertexAccessor("POSITION")?.AsVector3Array();
                _NORMAL = srcPrim.GetVertexAccessor("NORMAL")?.AsVector3Array();
                _Textute = srcPrim.GetVertexAccessor("TEXCOORD_0")?.AsVector2Array();
                float s = srcPrim.Material.AlphaCutoff;

            }

            //for (int i = 0; i < _Positions.Count; i++)
            //{
            //    Vertices.Data.Add(new Vertex()
            //    {
            //        Position = _Positions[i],
            //        Normal = _NORMAL[i],
            //        Tex = _Textute[i],
            //    });
            //}



            foreach (MeshPrimitive srcPrim in srcPrims)
            {

                IEnumerable<(int A, int B, int C)> front = srcPrim.GetTriangleIndices();
                IEnumerable<(int A, int C, int B)> back = front.Select(item => (item.A, item.C, item.B));
                (int A, int, int)[] _Triangles = front.Concat(back).ToArray();
                //Indices.Data = CreateIndexBuffer(srcPrim.GetTriangleIndices())/*.Select(item => item).ToArray().ToList()*/;
            }






            //Vertices.Count = Vertices.Data.Count();

            //Vertices.SizeInBytes = Utilities.SizeOf<Vertex>() * Vertices.Data.Count();

            //Vertices.Size = Utilities.SizeOf<Vertex>();





            //Indices.Count = Indices.Data.Count();

            //Indices.SizeInBytes = Utilities.SizeOf<int>() * Indices.Data.Count();

            //Indices.Size = Utilities.SizeOf<int>();

        }




        private static IEnumerable<MeshPrimitive> GetValidPrimitives(Mesh srcMesh)
        {
            foreach (MeshPrimitive srcPrim in srcMesh.Primitives)
            {
                Accessor ppp = srcPrim.GetVertexAccessor("POSITION");
                if (ppp.Count < 3)
                {
                    continue;
                }

                if (srcPrim.DrawPrimitiveType == PrimitiveType.POINTS)
                {
                    continue;
                }

                if (srcPrim.DrawPrimitiveType == PrimitiveType.LINES)
                {
                    continue;
                }

                if (srcPrim.DrawPrimitiveType == PrimitiveType.LINE_LOOP)
                {
                    continue;
                }

                if (srcPrim.DrawPrimitiveType == PrimitiveType.LINE_STRIP)
                {
                    continue;
                }

                yield return srcPrim;
            }
        }


        private List<int> CreateIndexBuffer(IEnumerable<(int A, int B, int C)> triangles)
        {
            int[] sequence32 = triangles.SelectMany(item => new[] { item.C, item.B, item.A }).ToArray();

            int max = sequence32.Max();


            return sequence32.ToList();

        }
    }
}
