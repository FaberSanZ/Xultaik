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



    [StructLayout(LayoutKind.Sequential)]
    public struct LightBuffer
    {
        public Vector4 AmbientColor;

        public Vector4 Diffuse;

        public Vector3 LightDirection;

        public float padding;

        public LightBuffer(Vector4 D, Vector3 DI, Vector4 AM)
        {
            Diffuse = D;
            LightDirection = DI;
            AmbientColor = AM;
            padding = 0;
        }

    }
}
