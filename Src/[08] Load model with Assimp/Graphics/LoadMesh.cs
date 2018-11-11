using Assimp;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace _08__Load_model_with_Assimp
{
    public class LoadMesh
    {
        // Properties
        private Buffer VertexBuffer { get; set; }
        private Buffer IndexBuffer { get; set; }

        public int VertexCount { get; set; }
        public int IndexCount { get; set; }

        public ShaderResourceView Texture { get; set; }


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

            foreach (Mesh mesh in scene.Meshes)
            {
                List<Vector3D> positions = mesh.Vertices;
                List<Vector3D> texs = mesh.TextureCoordinateChannels[0];

                List<Face> faces = mesh.Faces;

                for (int i = 0; i < mesh.VertexCount; i++)
                {
                    vertices[i] = new Vertex();
                    vertices[i].Position = new Vector3(positions[i].X, positions[i].Y, positions[i].Z);
                    vertices[i].TexC = new Vector2(texs[i].X, texs[i].Y);
                }


                for (int i = 0; i < mesh.FaceCount; i++)
                {
                    indices[i * 3 + 0] = (int)faces[i].Indices[2] + vertexOffSet;
                    //Console.Write("{0} ", indices[i * 3 + 0]);

                    indices[i * 3 + 1] = (int)faces[i].Indices[1] + vertexOffSet;
                    //Console.Write("{0} ", indices[i * 3 + 1]);

                    indices[i * 3 + 2] = (int)faces[i].Indices[0] + vertexOffSet;
                    //Console.WriteLine("{0} ", indices[i * 3 + 2]);


                    //indices[i] = i;
                }

                vertexOffSet += mesh.VertexCount;
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

                Texture = BitmapLoader.LoadTextureFromFile(Device, TFileName);
            }

        }


        public void RenderBuffers(DeviceContext deviceContext, PrimitiveTopology PrimitiveTopology = PrimitiveTopology.TriangleList)
        {
            VertexBufferBinding vertexBuffer = new VertexBufferBinding();
            vertexBuffer.Buffer = VertexBuffer;
            vertexBuffer.Stride = Utilities.SizeOf<Vertex>();
            vertexBuffer.Offset = 0;

            // Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, vertexBuffer);

            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);


            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology;
        }
    }
}
