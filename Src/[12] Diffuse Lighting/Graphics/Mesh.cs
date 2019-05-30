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
        public Vector3 Normal { get; set; }

        public Vertex(Vector3 pos, Vector3 norm, Vector2 uv)
            : this()
        {
            Position = pos;
            Tex = uv;
            Normal = norm;
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



        public Mesh(string FileName)
        {

            Vertices = new List<Vertex>();

            Indices = new List<int>();


            PostProcessSteps Flags = PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.CalculateTangentSpace | PostProcessSteps.Triangulate;

            AssimpContext importer = new AssimpContext();

            Scene model = importer.ImportFile(FileName, Flags);



            foreach (Assimp.Mesh mesh in model.Meshes)
            {

                for (int i = 0; i < mesh.VertexCount; i++)
                {
                    Vector3D Pos = mesh.Vertices[i];
                    Vector3D Normal = mesh.Normals[i];
                    Vector3D Tex = mesh.HasTextureCoords(0) ? mesh.TextureCoordinateChannels[0][i] : new Vector3D();

                    Vertices.Add(new Vertex(new Vector3(Pos.X, Pos.Y, Pos.Z), new Vector3(Normal.X, -Normal.Y, Normal.Z), new Vector2(Tex.X, Tex.Y)));
                }



                int indexBase = Indices.Count();

                foreach (Face Faces in mesh.Faces)
                {
                    if (Faces.IndexCount != 3)
                        continue;

                    Indices.Add(indexBase + Faces.Indices[0]);
                    Indices.Add(indexBase + Faces.Indices[1]);
                    Indices.Add(indexBase + Faces.Indices[2]);
                }
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
