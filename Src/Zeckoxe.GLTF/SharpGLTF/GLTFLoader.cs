using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Zeckoxe.Graphics;

namespace Zeckoxe.GLTF.SharpGLTFNew
{
    public class GLTFLoader<TVertex>
    {
        internal Device _device;
        private readonly SharpGLTF.Schema2.ModelRoot model;
        private List<Mesh> meshes;

        public GLTFLoader(Device device, string path)
        {
            _device = device;

            model = SharpGLTF.Schema2.ModelRoot.Load(path);

            create_buffers();


            Meshes = LoadMeshes();
        }


        public Graphics.Buffer VertexBuffer { get; private set; }
        public int VertexCount { get; private set; }

        public Graphics.Buffer IndexBuffer { get; private set; }
        public int IndexCount { get; private set; }

        public List<Mesh> Meshes { get; set; }

        public IndexType IndexType { get; set; }




        internal void create_buffers()
        {

        }




        public List<Mesh> LoadMeshes()
        {



            int vertexCount = 0, indexCount = 0;
            int autoNamedMesh = 1;

            meshes = new List<Mesh>();


            //VertexBuffer.Map();
            //IndexBuffer.Map();

            List<VertexPositionColor> vertex = new();
            List<int> Indices = new();

            {

                unsafe
                {

                    foreach (SharpGLTF.Schema2.Mesh mesh in model.LogicalMeshes)
                    {

                        string meshName = mesh.Name;
                        if (string.IsNullOrEmpty(meshName))
                        {
                            meshName = "mesh_" + autoNamedMesh.ToString();
                            autoNamedMesh++;
                        }
                        Mesh m = new Mesh { Name = meshName };
                        System.Console.WriteLine(m.Name);
                        foreach (SharpGLTF.Schema2.MeshPrimitive p in mesh.Primitives)
                        {
   

                            var positions = p.GetVertexAccessor("POSITION")?.AsVector3Array();
                            var normals = p.GetVertexAccessor("NORMAL")?.AsVector3Array();



                            //prim.BoundingBox.Min.ImportFloatArray(positions.Min().);
                            //prim.BoundingBox.Max.ImportFloatArray(AccPos.Max);
                            //prim.BoundingBox.IsValid = true;

                            //Interleaving vertices

                            VertexPositionColor[] vertices = new VertexPositionColor[positions?.Count ?? 0];
                            for (int i = 0; i < vertices.Length; i++)
                            {
                                //Vector4 tangent = default; // tangents![i];
                                vertices![i] = new VertexPositionColor(positions![i], normals![i]);

                            }
                            vertex.AddRange(vertices);

                            //indices loading

                            (int A, int B, int C)[] triangleIndices = p.GetTriangleIndices()?.ToArray();

                            int[] indices = triangleIndices?.SelectMany(i => new[] { i.A, i.B, i.C }).ToArray();
                            Indices.AddRange(indices);


                            Primitive prim = new Primitive
                            {
                                //IndexBase = indexCount,
                                //VertexBase = vertexCount,
                                //VertexCount = positions.Count,
                            };

                            indexCount += indices.Count();
                            vertexCount += positions.Count;
                            prim.IndexCount = indices.Count();

                            m.AddPrimitive(prim);




                        }
                        meshes.Add(m);
                    }
                }


                VertexBuffer = new(_device, new()
                {
                    BufferFlags = BufferFlags.VertexBuffer,
                    Usage = GraphicsResourceUsage.Dynamic,
                    SizeInBytes = vertex.Count * Marshal.SizeOf<TVertex>(),
                });


                IndexBuffer = new(_device, new()
                {
                    BufferFlags = BufferFlags.IndexBuffer,
                    Usage = GraphicsResourceUsage.Dynamic,
                    SizeInBytes = Indices.Count *  Marshal.SizeOf<int>(),
                });


                VertexBuffer.SetData<VertexPositionColor>(vertex.ToArray());
                IndexBuffer.SetData<int>(Indices.ToArray());

            }

            return meshes;
        }


        public void Draw(CommandBuffer commandBuffer)
        {
            commandBuffer.SetVertexBuffers(new[] { VertexBuffer });
            commandBuffer.SetIndexBuffer(IndexBuffer, 0, IndexType.Uint32);


            foreach (Mesh m in Meshes)
            {
                DrawNode(commandBuffer, m);
            }

        }

        public void DrawNode(CommandBuffer commandBuffer, Mesh m)
        {
            foreach (Primitive primitive in m.Primitives)
            {
                commandBuffer.DrawIndexed(primitive.IndexCount, 1, primitive.FirstIndex, primitive.FirstVertex, 0);
            }

        }



    }
}
