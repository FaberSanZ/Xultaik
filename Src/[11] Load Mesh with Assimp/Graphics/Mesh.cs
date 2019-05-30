using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using SharpDX;


namespace Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position { get; set; }
        public Vector2 Tex { get; set; }

        public Vertex(Vector3 Pos, Vector2 tex)
        {
            Position = Pos;
            Tex = tex;
        }
    }


    public class Mesh
    {
        public int SizeInBytes { get; set; }

        public int Size { get; set; }


        public int IndexSizeInBytes { get; set; }

        public int IndexSize { get; set; }


        public List<Vertex> Vertices { get; set; }


        public List<int> Indices { get; set; }


        public int VertexCount { get; set; }


        public int IndexCount { get; set; }



        public Mesh(string FileName, bool flipUv = false)
        {

            Vertices = new List<Vertex>();

            Indices = new List<int>();

            AssimpContext importer = new AssimpContext();

            if (!importer.IsImportFormatSupported(Path.GetExtension(FileName)))
            {
                throw new ArgumentException("Model format " + Path.GetExtension(FileName) + " is not supported!  Cannot load {1}", "filename");
            }


            ConsoleLogStream logStream = new ConsoleLogStream();
            logStream.Attach();

            PostProcessSteps postProcessFlags = PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.CalculateTangentSpace;
            if (flipUv)
            {
                postProcessFlags |= PostProcessSteps.FlipUVs;
            }


            Scene model = importer.ImportFile(FileName, postProcessFlags);

            int[] indices = new int[model.Meshes.Sum(m => m.FaceCount * 3)];

            int vertexOffSet = 0;

            int indexOffSet = 0;

            foreach (Assimp.Mesh mesh in model.Meshes)
            {
                List<Vertex> verts = new List<Vertex>();

                List<Face> faces = mesh.Faces;

                for (int i = 0; i < mesh.VertexCount; i++)
                {
                    Vector3 pos = mesh.HasVertices ? new Vector3(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z) : new Vector3();

                    Vector3D texC = mesh.HasTextureCoords(0) ? mesh.TextureCoordinateChannels[0][i] : new Vector3D();
                    Vector2 TeC = new Vector2(texC.X, texC.Y);
                    Vertex v = new Vertex(pos, TeC);
                    verts.Add(v);
                }

                Vertices.AddRange(verts);

                for (int i = 0; i < mesh.FaceCount; i++)
                {
                    indices[i * 3 + 0] = (int)faces[i].Indices[2];
                    indices[i * 3 + 1] = (int)faces[i].Indices[1];
                    indices[i * 3 + 2] = (int)faces[i].Indices[0];
                    Indices.Add(indices[i * 3 + 2] + vertexOffSet);
                    Indices.Add(indices[i * 3 + 1] + vertexOffSet);
                    Indices.Add(indices[i * 3 + 0] + vertexOffSet);

                }
                vertexOffSet += mesh.VertexCount;
                indexOffSet += mesh.FaceCount * 3;
            }




            VertexCount = Vertices.Count(); 

            IndexCount = Indices.Count();

            Size = Utilities.SizeOf<Vertex>();

            SizeInBytes = Utilities.SizeOf<Vertex>() * Vertices.Count();

            IndexSize = Utilities.SizeOf<int>();

            IndexSizeInBytes = Utilities.SizeOf<int>() * Indices.Count();

        }
    }
}
