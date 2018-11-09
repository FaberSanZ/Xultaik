using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace _06__Render_states
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position { get; set; }
        public Color4 Color { get; set; }

        public Vertex(Vector3 vector3, Color color)
        {
            Position = vector3;
            Color = color;
        }
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
