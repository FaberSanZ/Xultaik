using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using Buffer = SharpDX.Direct3D11.Buffer;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using Assimp;
using System.Linq;

namespace _13__Framework
{
    public class LoadModel
    {
        // Properties
        private Buffer VertexBuffer { get; set; }
        private Buffer IndexBuffer { get; set; }
        public int VertexCount { get; set; }
        public int IndexCount { get; set; }

        public ShaderResourceView Texture { get; set; }


        public LoadModel() { }

        // Methods.
        public void InitializeBuffer(Device Device, string FileName, string TFileName)
        {


            AssimpContext importer = new AssimpContext();
            Scene scene = importer.ImportFile(
                FileName,
                PostProcessSteps.GenerateSmoothNormals |
                PostProcessSteps.FlipUVs |
                PostProcessSteps.PreTransformVertices);


            Vertex[] vertices = new Vertex[scene.Meshes.Sum(m => m.VertexCount)];
            int[] indices = new int[scene.Meshes.Sum(m => m.FaceCount * 3)];

            int vertexOffSet = 0;
            int indexOffSet = 0;
            foreach (Mesh mesh in scene.Meshes)
            {
                List<Vector3D> positions = mesh.Vertices;
                List<Vector3D> normals = mesh.Normals;
                List<Vector3D> texs = mesh.TextureCoordinateChannels[0];

                for (int i = 0; i < mesh.VertexCount; i++)
                {
                    vertices[vertexOffSet + i] = new Vertex()
                    {
                        Position = new Vector3(positions[i].X, positions[i].Y, positions[i].Z),
                        Normal = new Vector3(normals[i].X, normals[i].Y, normals[i].Z),
                        TexC = new Vector2(texs[i].X, texs[i].Y)
                    };
                }

                List<Face> faces = mesh.Faces;

                for (int i = 0; i < mesh.FaceCount; i++)
                {
                    indices[i * 3 + 0 + indexOffSet]  = (int)faces[i].Indices[2] + vertexOffSet;
                    indices[i * 3 + 1 + indexOffSet]  = (int)faces[i].Indices[1] + vertexOffSet;
                    indices[i * 3 + 2 + indexOffSet]  = (int)faces[i].Indices[0] + vertexOffSet;
                }

                vertexOffSet += mesh.VertexCount;
                indexOffSet += mesh.FaceCount * 3;
            }


            IndexCount = indices.Count();

            // Create the vertex buffer.
            VertexBuffer = Buffer.Create(Device, BindFlags.VertexBuffer, vertices);

            // Create the index buffer.
            IndexBuffer = Buffer.Create(Device, BindFlags.IndexBuffer, indices);

            string ext = Path.GetExtension(TFileName);

            if (ext.ToLower() == ".dds")
            {

                Texture = DDSLoader.LoadTextureFromFile(Device, Device.ImmediateContext, TFileName);
            }
            else
            {
                BitmapLoader res = new BitmapLoader();
                res.Initialize(Device, TFileName);
                Texture = res.TextureResource;
            }

        }

        public void ShutDownBuffers()
        {
            // Release the index buffer.
            IndexBuffer?.Dispose();
            IndexBuffer = null;
            // Release the vertex buffer.
            VertexBuffer?.Dispose();
            VertexBuffer = null;
        }

        public void RenderBuffers(DeviceContext deviceContext)
        {
            // Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<Vertex>(), 0));

            // Set the index buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, SharpDX.DXGI.Format.R32_UInt, 0);

            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            //deviceContext.DrawIndexed(IndexCount, 0, 0);
        }

    }
}
