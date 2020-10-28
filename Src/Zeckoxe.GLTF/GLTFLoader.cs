// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	GLTFLoader.cs
=============================================================================*/


using SharpGLTF.Runtime;
using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Zeckoxe.Graphics;


namespace Zeckoxe.GLTF
{
    public unsafe class GLTFLoader
    {
        private readonly IList<Vector3> _Positions;
        private readonly IList<Vector3> _Normal;
        private readonly IList<Vector2> _Texture;
        private readonly IList<Vector3> _Color;


        public int[] Indices;

        public GraphicsPipelineState GLTFPipeline { get; set; }



        public GLTFLoader(string FileName)
        {

            //Vertices = new ModelPart<Vertex>();

            //Indices = new ModelPart<int>();



            ModelRoot model = ModelRoot.Load(FileName);

            SceneTemplate[] templates = model.LogicalScenes.Select(item => SceneTemplate.Create(item, true)).ToArray();
            Dictionary<int, Mesh> meshes = templates.SelectMany(item => item.LogicalMeshIds).ToDictionary(k => k, k => model.LogicalMeshes[k]);

            //Console.WriteLine("Logical Images Count {0}", model.LogicalImages.Count);

            foreach (SharpGLTF.Schema2.Image image in model.LogicalImages)
            {
                //var i = image.OpenImageFile();
                //i.
            }



            IEnumerable<MeshPrimitive> srcPrims = GetValidPrimitives(meshes[0]);

            foreach (MeshPrimitive srcPrim in srcPrims)
            {
                _Positions = srcPrim.GetVertexAccessor("POSITION")?.AsVector3Array();
                _Normal = srcPrim.GetVertexAccessor("NORMAL")?.AsVector3Array();
                _Texture = srcPrim.GetVertexAccessor("TEXCOORD_0")?.AsVector2Array();
                _Color = srcPrim.GetVertexAccessor("COLOR_0")?.AsVector3Array();

                IEnumerable<(int A, int B, int C)> front = srcPrim.GetTriangleIndices();
                IEnumerable<(int A, int C, int B)> back = front.Select(item => (item.A, item.C, item.B));
                (int A, int, int)[] _Triangles = front.Concat(back).ToArray();


                Indices = CreateIndexBuffer(srcPrim.GetTriangleIndices());

            }
        }


        public Span<VertexPositionNormal> GetPositionNormalAsSpan()
        {
            VertexPositionNormal[] vertices = new VertexPositionNormal[_Positions.Count];

            for (int i = 0; i < _Positions.Count; i++)
            {
                vertices[i] = new VertexPositionNormal()
                {
                    Position = _Positions[i] != null ? _Positions[i] : Vector3.One,
                    Normal = _Normal[i] != null ? _Normal[i] : Vector3.One,
                };
            }

            return vertices.AsSpan(); // TODO: GetPositionNormal AsSpan
        }

        public VertexPositionNormal[] GetPositionNormalAsArray()
        {
            VertexPositionNormal[] vertices = new VertexPositionNormal[_Positions.Count];

            for (int i = 0; i < _Positions.Count; i++)
            {

                vertices[i] = new VertexPositionNormal()
                {
                    Position = _Positions[i] != null ? _Positions[i] : Vector3.One,
                    Normal = _Normal[i] != null ? _Normal[i] : Vector3.One,
                };
            }

            return vertices;
        }


        public IEnumerable<VertexPositionNormal> GetPositionNormal()
        {
            for (int i = 0; i < _Positions.Count; i++)
            {
                yield return new VertexPositionNormal()
                {
                    Position = _Positions[i] != null ? _Positions[i] : Vector3.One,
                    Normal = _Normal[i] != null ? _Normal[i] : Vector3.One,
                };
            }
        }

        public IEnumerable<VertexPositionNormalColor> GetPositionNormalColor()
        {
            for (int i = 0; i < _Positions.Count; i++)
            {
                yield return new VertexPositionNormalColor()
                {
                    Position = _Positions[i] != null ? _Positions[i] : Vector3.One,
                    Normal = _Normal[i] != null ? _Normal[i] : Vector3.One,
                    Color = _Color[i] != null ? _Color[i] : Vector3.One,
                };
            }
        }


        public void Update()
        {

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

                if (srcPrim.DrawPrimitiveType == SharpGLTF.Schema2.PrimitiveType.POINTS)
                {
                    continue;
                }

                if (srcPrim.DrawPrimitiveType == SharpGLTF.Schema2.PrimitiveType.LINES)
                {
                    continue;
                }

                if (srcPrim.DrawPrimitiveType == SharpGLTF.Schema2.PrimitiveType.LINE_LOOP)
                {
                    continue;
                }

                if (srcPrim.DrawPrimitiveType == SharpGLTF.Schema2.PrimitiveType.LINE_STRIP)
                {
                    continue;
                }

                yield return srcPrim;
            }
        }


        private int[] CreateIndexBuffer(IEnumerable<(int A, int B, int C)> triangles)
        {
            int[] sequence32 = triangles.SelectMany(item => new[] { item.C, item.B, item.A }).ToArray();

            int max = sequence32.Max();


            return sequence32;
        }
    }
}
