using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using Assimp.Configs;
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
        {
            Position = pos;
            Tex = uv;
            Normal = norm;
        }
    }


    public class Mesh
    {
        public ModelPart<Vertex> Vertices { get; set; }


        public ModelPart<int> Indices { get; set; }



        public Mesh(string FileName)
        {

            Vertices = new ModelPart<Vertex>();

            Indices = new ModelPart<int>();


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

                    Vertices.Data.Add(new Vertex(new Vector3(Pos.X, Pos.Y, Pos.Z), new Vector3(Normal.X, Normal.Y, Normal.Z), new Vector2(Tex.X, Tex.Y)));
                }



                int indexBase = Indices.Data.Count();

                foreach (Face Faces in mesh.Faces)
                {
                    if (Faces.IndexCount != 3)
                        continue;

                    Indices.Data.Add(indexBase + Faces.Indices[0]);
                    Indices.Data.Add(indexBase + Faces.Indices[1]);
                    Indices.Data.Add(indexBase + Faces.Indices[2]);
                }
            }



            Vertices.Count = Vertices.Data.Count();

            Vertices.SizeInBytes = Utilities.SizeOf<Vertex>() * Vertices.Data.Count();

            Vertices.Size = Utilities.SizeOf<Vertex>();





            Indices.Count = Indices.Data.Count();

            Indices.SizeInBytes = Utilities.SizeOf<int>() * Indices.Data.Count();

            Indices.Size = Utilities.SizeOf<int>();

        }
    }
}
