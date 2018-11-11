using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace _08__Load_model_with_Assimp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position { get; set; }
        public Vector2 TexC { get; set; }

        public Vertex(Vector3 pos, Vector2 uv)
            : this()
        {
            Position = pos;
            TexC = uv;
        }
    }

    public class MeshData
    {
        public List<Vertex> Vertices { get; } = new List<Vertex>();
        public List<int> Indices32 { get; } = new List<int>();

        public List<short> GetIndices16() => Indices32.Select(i => (short)i).ToList();
    }


    // Structures.
    [StructLayout(LayoutKind.Sequential)]
    internal struct MatrixB
    {
        public Matrix World { get; set; }
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }
    }

}
