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


        public Vertex[] Vertices { get; set; }


        public int VertexCount { get; set; }




        public Mesh()
        {
            Vertices = new Vertex[3];
            Vertices[0] = new Vertex(new Vector3(0.0f, 0.5f, 0.0f), new Color(1.0f, 0.0f, 0.0f, 1.0f));
            Vertices[1] = new Vertex(new Vector3(0.5f, -0.5f, 0.0f), new Color(0.0f, 1.0f, 0.0f, 1.0f));
            Vertices[2] = new Vertex(new Vector3(-0.5f, -0.5f, 0.0f), new Color(0.0f, 0.0f, 1.0f, 1.0f));




            Size = Utilities.SizeOf<Vertex>();

            SizeInBytes = Utilities.SizeOf<Vertex>() * Vertices.Count();

            VertexCount = Vertices.Count();
        }
    }
}
