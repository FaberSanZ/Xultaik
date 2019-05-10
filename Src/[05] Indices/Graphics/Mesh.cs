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

            Vertices = new Vertex[4];
            Vertices[0] = new Vertex(new Vector3(-0.5f, +0.5f, 0.0f), new Color(0.0f, 1.0f, 0.0f, 1.0f));
            Vertices[1] = new Vertex(new Vector3(+0.5f, +0.5f, 0.0f), new Color(1.0f, 0.0f, 0.0f, 1.0f));
            Vertices[2] = new Vertex(new Vector3(-0.5f, -0.5f, 0.0f), new Color(0.0f, 1.0f, 0.0f, 1.0f));
            Vertices[3] = new Vertex(new Vector3(+0.5f, -0.5f, 0.0f), new Color(1.0f, 0.0f, 0.0f, 1.0f));


            Indices = new int[] 
            {
                0, 1, 2,
                2, 1, 3,

                //0, 3, 2,
                //0, 1, 3,
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
