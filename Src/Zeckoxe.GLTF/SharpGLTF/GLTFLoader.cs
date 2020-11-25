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

            List<VertexPositionColor> vertexBuffer = new(); List<int> indexBuffe = new();

            Meshes = new();
            foreach (SharpGLTF.Schema2.Mesh mesh in model.LogicalMeshes)
            {
                Meshes.Add(create_buffers(mesh, vertexBuffer, indexBuffe));

                
            }


            foreach (SharpGLTF.Schema2.Mesh mesh in model.LogicalMeshes)
            {
                foreach (var item in mesh.LogicalParent.LogicalNodes)
                {
                    System.Console.WriteLine(item.Mesh);
                }
            }



            VertexBuffer = new(_device, new()
            {
                BufferFlags = BufferFlags.VertexBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = vertexBuffer.Count * Marshal.SizeOf<TVertex>(),
            });


            IndexBuffer = new(_device, new()
            {
                BufferFlags = BufferFlags.IndexBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = indexBuffe.Count * Marshal.SizeOf<int>(),
            });


            VertexBuffer.SetData<VertexPositionColor>(vertexBuffer.ToArray());
            IndexBuffer.SetData<int>(indexBuffe.ToArray());


            //Meshes = LoadMeshes();
        }


        public Graphics.Buffer VertexBuffer { get; private set; }
        public int VertexCount { get; private set; }

        public Graphics.Buffer IndexBuffer { get; private set; }
        public int IndexCount { get; private set; }

        public List<Mesh> Meshes { get; set; }

        public IndexType IndexType { get; set; }




        internal Mesh create_buffers(SharpGLTF.Schema2.Mesh mesh, List<VertexPositionColor> vertexBuffer, List<int> indexBuffer)
        {
            Mesh m = new Mesh { Name = mesh.Name };

            foreach (SharpGLTF.Schema2.MeshPrimitive p in mesh.Primitives)
            {

                int indexStart = indexBuffer.Count;
                int vertexStart = vertexBuffer.Count;
                int indexCount = 0;
                int vertexCount = 0;



                IList<System.Numerics.Vector3> positions = p.GetVertexAccessor("POSITION")?.AsVector3Array();
                IList<System.Numerics.Vector3> normals = p.GetVertexAccessor("NORMAL")?.AsVector3Array();



                //prim.BoundingBox.Min.ImportFloatArray(positions.Min().);
                //prim.BoundingBox.Max.ImportFloatArray(AccPos.Max);
                //prim.BoundingBox.IsValid = true;

                //Interleaving vertices

                for (int i = 0; i < positions.Count; i++)
                {
                    //Vector4 tangent = default; // tangents![i];
                    vertexBuffer.Add(new(positions[i], normals[i]));
                }
                //vertexBuffer.AddRange(vertices);
                vertexCount = positions.Count;
                //indices loading

                (int A, int B, int C)[] triangleIndices = p.GetTriangleIndices()?.ToArray();

                int[] indices = triangleIndices?.SelectMany(i => new[] { i.A, i.B, i.C }).ToArray();

                for (int i = 0; i < indices.Length; i++)
                {
                    indexBuffer.Add(indices[i] + vertexStart);

                }
                indexCount = indices.Length;

                Primitive prim = new Primitive
                {
                    FirstVertex = vertexStart,
                    VertexCount = vertexCount,
                    FirstIndex = indexStart,
                    IndexCount = indexCount,
                };

                m.AddPrimitive(prim);




            }

            return m;
        }




        public List<Mesh> LoadMeshes()
        {
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


                            IList<System.Numerics.Vector3> positions = p.GetVertexAccessor("POSITION")?.AsVector3Array();
                            IList<System.Numerics.Vector3> normals = p.GetVertexAccessor("NORMAL")?.AsVector3Array();



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

                            prim.IndexCount = indices.Count();
                            prim.FirstIndex = p.IndexAccessor.LogicalIndex;
                            System.Console.WriteLine(p.IndexAccessor.LogicalIndex);
                            m.AddPrimitive(prim);




                        }
                        meshes.Add(m);
                    }
                }




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
                commandBuffer.DrawIndexed(primitive.IndexCount, 1, primitive.FirstIndex, 0, 0);
            }

        }



    }
}
