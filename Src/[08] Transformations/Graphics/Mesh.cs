using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX;


namespace Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position { get; set; }
        public Color4 Color { get; set; }

        public Vertex(Vector3 Pos, Color Col)
        {
            Position = Pos;
            Color = Col;
        }
    }


    public class Mesh
    {
        public int SizeInBytes { get; set; }

        public int Size { get; set; }


        public int IndexSizeInBytes { get; set; }

        public int IndexSize { get; set; }


        public Vertex[] Vertices { get; set; }


        public int[] Indices { get; set; }


        public int VertexCount { get; set; }


        public int IndexCount { get; set; }



        public Mesh()
        {
            Vertices = new Vertex[24];
            Vertices[0] = new Vertex(new Vector3(-0.5f, 0.5f, -0.5f), new Color(1.0f, 0.0f, 0.0f, 1.0f));
            Vertices[1] = new Vertex(new Vector3(0.5f, -0.5f, -0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f));
            Vertices[2] = new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Color(0.0f, 0.0f, 1.0f, 1.0f));
            Vertices[3] = new Vertex(new Vector3(0.5f, 0.5f, -0.5f), new Color(0.0f, 1.0f, 0.0f, 1.0f));

            Vertices[4] = new Vertex(new Vector3(0.5f, -0.5f, -0.5f), new Color(1.0f, 0.0f, 0.0f, 1.0f));
            Vertices[5] = new Vertex(new Vector3(0.5f, 0.5f, 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f));
            Vertices[6] = new Vertex(new Vector3(0.5f, -0.5f, 0.5f), new Color(0.0f, 0.0f, 1.0f, 1.0f));
            Vertices[7] = new Vertex(new Vector3(0.5f, 0.5f, -0.5f), new Color(0.0f, 1.0f, 0.0f, 1.0f));

            Vertices[8] = new Vertex(new Vector3(-0.5f, 0.5f, 0.5f), new Color(1.0f, 0.0f, 0.0f, 1.0f));
            Vertices[9] = new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f));
            Vertices[10] = new Vertex(new Vector3(-0.5f, -0.5f, 0.5f), new Color(0.0f, 0.0f, 1.0f, 1.0f));
            Vertices[11] = new Vertex(new Vector3(-0.5f, 0.5f, -0.5f), new Color(0.0f, 1.0f, 0.0f, 1.0f));

            Vertices[12] = new Vertex(new Vector3(0.5f, 0.5f, 0.5f), new Color(1.0f, 0.0f, 0.0f, 1.0f));
            Vertices[13] = new Vertex(new Vector3(-0.5f, -0.5f, 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f));
            Vertices[14] = new Vertex(new Vector3(0.5f, -0.5f, 0.5f), new Color(0.0f, 0.0f, 1.0f, 1.0f));
            Vertices[15] = new Vertex(new Vector3(-0.5f, 0.5f, 0.5f), new Color(0.0f, 1.0f, 0.0f, 1.0f));

            Vertices[16] = new Vertex(new Vector3(-0.5f, 0.5f, -0.5f), new Color(1.0f, 0.0f, 0.0f, 1.0f));
            Vertices[17] = new Vertex(new Vector3(0.5f, 0.5f, 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f));
            Vertices[18] = new Vertex(new Vector3(0.5f, 0.5f, -0.5f), new Color(0.0f, 0.0f, 1.0f, 1.0f));
            Vertices[19] = new Vertex(new Vector3(-0.5f, 0.5f, 0.5f), new Color(0.0f, 1.0f, 0.0f, 1.0f));

            Vertices[20] = new Vertex(new Vector3(0.5f, -0.5f, 0.5f), new Color(1.0f, 0.0f, 0.0f, 1.0f));
            Vertices[21] = new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f));
            Vertices[22] = new Vertex(new Vector3(0.5f, -0.5f, -0.5f), new Color(0.0f, 0.0f, 1.0f, 1.0f));
            Vertices[23] = new Vertex(new Vector3(-0.5f, -0.5f, 0.5f), new Color(0.0f, 1.0f, 0.0f, 1.0f));


            Indices = new int[]
            {
                // front face
                0, 1, 2, // first triangle
                0, 3, 1, // second triangle

                // left face
                4, 5, 6, // first triangle
                4, 7, 5, // second triangle

                // right face
                8, 9, 10, // first triangle
                8, 11, 9, // second triangle

                // back face
                12, 13, 14, // first triangle
                12, 15, 13, // second triangle

                // top face
                16, 17, 18, // first triangle
                16, 19, 17, // second triangle

                // bottom face
                20, 21, 22, // first triangle
                20, 23, 21, // second triangle
            };




            VertexCount = Vertices.Count(); 

            IndexCount = Indices.Count();

            Size = Utilities.SizeOf<Vertex>();

            SizeInBytes = Utilities.SizeOf<Vertex>() * Vertices.Count();

            IndexSize = Utilities.SizeOf<int>();

            IndexSizeInBytes = Utilities.SizeOf<int>() * Indices.Count();

        }
    }
}
