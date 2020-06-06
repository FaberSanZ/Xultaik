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
using Zeckoxe.Core;
using Zeckoxe.Graphics;
using Zeckoxe.Graphics.Toolkit;
using Buffer = Zeckoxe.Graphics.Buffer;


namespace Zeckoxe.GLTF
{

    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 color;
        public Vertex(Vector3 pos, Vector3 color)
        {
            Position = pos;
            this.color = color;
        }
    }


    public unsafe class GLTFLoader
    {
        private readonly IList<Vector3> _Positions;
        private readonly IList<Vector3> _Normal;
        private readonly IList<Vector2> _Texture;
        private readonly IList<Vector3> _Color;

        private List<Vertex> vertices;
        private List<int> indices;


        public Buffer VertexBuffer { get; set; }
        public Buffer IndexBuffer { get; set; }

        public PipelineState GLTFPipeline { get; set; }



        public GLTFLoader(string FileName, GraphicsDevice device)
        {

            //Vertices = new ModelPart<Vertex>();

            //Indices = new ModelPart<int>();



            ModelRoot model = ModelRoot.Load(FileName);

            SceneTemplate[] templates = model.LogicalScenes.Select(item => SceneTemplate.Create(item, true)).ToArray();
            Dictionary<int, Mesh> meshes = templates.SelectMany(item => item.LogicalMeshIds).ToDictionary(k => k, k => model.LogicalMeshes[k]);

            //Console.WriteLine("Logical Images Count {0}", model.LogicalImages.Count);

            foreach (Image image in model.LogicalImages)
            {
                //var i = image.OpenImageFile();
                //i.
            }


            vertices = new List<Vertex>();
            indices = new List<int>();



            List<MeshPrimitive> srcPrims = GetValidPrimitives(meshes[0]).ToList();

            foreach (MeshPrimitive srcPrim in srcPrims)
            {
                _Positions = srcPrim.GetVertexAccessor("POSITION")?.AsVector3Array();
                //_Normal = srcPrim.GetVertexAccessor("NORMAL")?.AsVector3Array();
                //_Texture = srcPrim.GetVertexAccessor("TEXCOORD_0")?.AsVector2Array();
                //_Color = srcPrim.GetVertexAccessor("")?.AsVector3Array();

                IEnumerable<(int A, int B, int C)> front = srcPrim.GetTriangleIndices();
                IEnumerable<(int A, int C, int B)> back = front.Select(item => (item.A, item.C, item.B));
                (int A, int, int)[] _Triangles = front.Concat(back).ToArray();


                indices = CreateIndexBuffer(srcPrim.GetTriangleIndices());

            }

            for (int i = 0; i < _Positions.Count; i++)
            {
                vertices.Add(new Vertex()
                {
                    Position = _Positions[i],
                    //Normal = _NORMAL[i],
                    //Tex = _Textute[i],
                });
            }


            CreateBuffers(device);


            VertexBuffer.SetData(vertices.ToArray());
            IndexBuffer.SetData(indices.ToArray());
        }



        public void Update()
        {

        }


        public void Draw(CommandBuffer commandBuffer)
        {
            commandBuffer.SetVertexBuffers(new Buffer[] { VertexBuffer });
            commandBuffer.SetIndexBuffer(IndexBuffer);
            commandBuffer.DrawIndexed(indices.Count, 1, 0, 0, 0);
        }

        public void CreateBuffers(GraphicsDevice device)
        {
            VertexBuffer = new Buffer(device, new BufferDescription()
            {
                BufferFlags = BufferFlags.VertexBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<Vertex>(vertices.ToArray()),
            });


            IndexBuffer = new Buffer(device, new BufferDescription()
            {
                BufferFlags = BufferFlags.IndexBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = Interop.SizeOf<int>(indices.ToArray()),
            });
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


        private List<int> CreateIndexBuffer(IEnumerable<(int A, int B, int C)> triangles)
        {
            int[] sequence32 = triangles.SelectMany(item => new[] { item.C, item.B, item.A }).ToArray();

            int max = sequence32.Max();


            return sequence32.ToList();

        }
    }
}
