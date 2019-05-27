using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX;


namespace Systems
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Transform
    {
        public Matrix W;

        public Matrix V;

        public Matrix P;

        public Transform(Matrix w, Matrix v, Matrix p)
        {
            W = Matrix.Transpose(w);
            V = Matrix.Transpose(v);
            P = Matrix.Transpose(p);
        }
    }
}
