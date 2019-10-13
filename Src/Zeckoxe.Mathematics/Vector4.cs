using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Zeckoxe.Mathematics
{
    /// <summary>
    /// Represents a four dimensional mathematical vector.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vector4 
    {

        public static readonly Vector4 Zero = new Vector4();


        public static readonly Vector4 UnitX = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);


        public static readonly Vector4 UnitY = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);


        public static readonly Vector4 UnitZ = new Vector4(0.0f, 0.0f, 1.0f, 0.0f);


        public static readonly Vector4 UnitW = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);


        public static readonly Vector4 One = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

 
        public float X;

 
        public float Y;


        public float Z;


        public float W;



        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }



        public bool IsZero => X == 0 && Y == 0 && Z == 0 && W == 0;
        



        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    case 3: return W;
                }

                throw new ArgumentOutOfRangeException("index", "Indices for Vector4 run from 0 to 3, inclusive.");
            }

            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    case 3: W = value; break;
                    default: throw new ArgumentOutOfRangeException("index", "Indices for Vector4 run from 0 to 3, inclusive.");
                }
            }
        }


        public float Length() => (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));
        


        public float LengthSquared() => (X * X) + (Y * Y) + (Z * Z) + (W * W);



        public float[] ToArray() => new float[] { X, Y, Z, W };
        


        public static Vector4 Add(Vector4 left, Vector4 right) => new Vector4(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);



        public static Vector4 Subtract(Vector4 left, Vector4 right) => new Vector4(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);



        public static Vector4 Divide(Vector4 value, float scale) => new Vector4(value.X / scale, value.Y / scale, value.Z / scale, value.W / scale);
        


        public static Vector4 Negate(Vector4 value) => new Vector4(-value.X, -value.Y, -value.Z, -value.W);


        

        public static float Dot(Vector4 left, Vector4 right) => (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z) + (left.W * right.W);
        
        


        public static Vector4 operator +(Vector4 left, Vector4 right) => new Vector4(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
        


        public static Vector4 operator *(Vector4 left, Vector4 right) => new Vector4(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);
        


        public static Vector4 operator +(Vector4 value) => value;
        


        public static Vector4 operator -(Vector4 left, Vector4 right) => new Vector4(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
        


        public static Vector4 operator -(Vector4 value) => new Vector4(-value.X, -value.Y, -value.Z, -value.W);
        


        public static Vector4 operator *(float scale, Vector4 value) => new Vector4(value.X * scale, value.Y * scale, value.Z * scale, value.W * scale);
        


        public static Vector4 operator *(Vector4 value, float scale) => new Vector4(value.X * scale, value.Y * scale, value.Z * scale, value.W * scale);
        


        public static Vector4 operator /(Vector4 value, float scale) => new Vector4(value.X / scale, value.Y / scale, value.Z / scale, value.W / scale);
        


        public static Vector4 operator /(float scale,Vector4 value) => new Vector4(scale / value.X, scale / value.Y, scale / value.Z, scale / value.W);
        


        public static Vector4 operator /(Vector4 value, Vector4 scale) => new Vector4(value.X / scale.X, value.Y / scale.Y, value.Z / scale.Z, value.W / scale.W);
        


        public static Vector4 operator +(Vector4 value, float scalar) => new Vector4(value.X + scalar, value.Y + scalar, value.Z + scalar, value.W + scalar);
        


        public static Vector4 operator +(float scalar, Vector4 value) => new Vector4(scalar + value.X, scalar + value.Y, scalar + value.Z, scalar + value.W);
        


        public static Vector4 operator -(Vector4 value, float scalar) => new Vector4(value.X - scalar, value.Y - scalar, value.Z - scalar, value.W - scalar);
        


        public static Vector4 operator -(float scalar, Vector4 value) => new Vector4(scalar - value.X, scalar - value.Y, scalar - value.Z, scalar - value.W);
        



        public static explicit operator Vector2(Vector4 value) => new Vector2(value.X, value.Y);
        


        public static explicit operator Vector3(Vector4 value) => new Vector3(value.X, value.Y, value.Z);
        


        public override string ToString() => string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", X, Y, Z, W);
       
    }
}
