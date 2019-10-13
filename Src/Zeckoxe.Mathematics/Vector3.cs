using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Zeckoxe.Mathematics
{
    /// <summary>
    /// Represents a three dimensional mathematical vector.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vector3
    {
        public static readonly Vector3 Zero = new Vector3();


        public static readonly Vector3 UnitX = new Vector3(1.0f, 0.0f, 0.0f);


        public static readonly Vector3 UnitY = new Vector3(0.0f, 1.0f, 0.0f);


        public static readonly Vector3 UnitZ = new Vector3(0.0f, 0.0f, 1.0f);


        public static readonly Vector3 One = new Vector3(1.0f, 1.0f, 1.0f);


        public static readonly Vector3 Up = new Vector3(0.0f, 1.0f, 0.0f);


        public static readonly Vector3 Down = new Vector3(0.0f, -1.0f, 0.0f);


        public static readonly Vector3 Left = new Vector3(-1.0f, 0.0f, 0.0f);


        public static readonly Vector3 Right = new Vector3(1.0f, 0.0f, 0.0f);


        public static readonly Vector3 ForwardRH = new Vector3(0.0f, 0.0f, -1.0f);


        public static readonly Vector3 ForwardLH = new Vector3(0.0f, 0.0f, 1.0f);


        public static readonly Vector3 BackwardRH = new Vector3(0.0f, 0.0f, 1.0f);


        public static readonly Vector3 BackwardLH = new Vector3(0.0f, 0.0f, -1.0f);


        public float X;


        public float Y;


        public float Z;




        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }



        public bool IsZero => X == 0 && Y == 0 && Z == 0;
        



        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                }

                throw new ArgumentOutOfRangeException("index", "Indices for Vector3 run from 0 to 2, inclusive.");
            }

            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    default: throw new ArgumentOutOfRangeException("index", "Indices for Vector3 run from 0 to 2, inclusive.");
                }
            }
        }



        public float Length() => (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z));



        public float LengthSquared() => (X * X) + (Y * Y) + (Z * Z);



        public float[] ToArray() => new float[] { X, Y, Z };



        public static Vector3 Add(Vector3 left, Vector3 right) => new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);



        public static Vector3 Subtract(Vector3 left, Vector3 right) => new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);



        public static Vector3 Multiply(Vector3 left, Vector3 right) => new Vector3(left.X * right.X, left.Y * right.Y, left.Z * right.Z);



        public static Vector3 Divide(Vector3 value, float scale) => new Vector3(value.X / scale, value.Y / scale, value.Z / scale);




        public static Vector3 Divide(float scale, Vector3 value) => new Vector3(scale / value.X, scale / value.Y, scale / value.Z);



        public static Vector3 Negate(Vector3 value) => new Vector3(-value.X, -value.Y, -value.Z);
        


        public static float Dot(Vector3 left, Vector3 right)
        {
            return (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z);
        }



        public static Vector3 operator +(Vector3 left, Vector3 right) => new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);



        public static Vector3 operator *(Vector3 left, Vector3 right) => new Vector3(left.X * right.X, left.Y * right.Y, left.Z * right.Z);



        public static Vector3 operator +(Vector3 value) => value;
        


        public static Vector3 operator -(Vector3 left, Vector3 right) => new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        


        public static Vector3 operator -(Vector3 value) => new Vector3(-value.X, -value.Y, -value.Z);
        


        public static Vector3 operator *(float scale, Vector3 value) => new Vector3(value.X * scale, value.Y * scale, value.Z * scale);
        


        public static Vector3 operator *(Vector3 value, float scale) => new Vector3(value.X * scale, value.Y * scale, value.Z * scale);
        


        public static Vector3 operator /(Vector3 value, float scale) => new Vector3(value.X / scale, value.Y / scale, value.Z / scale);
        


        public static Vector3 operator /(float scale, Vector3 value) => new Vector3(scale / value.X, scale / value.Y, scale / value.Z);
        


        public static Vector3 operator /(Vector3 value, Vector3 scale) => new Vector3(value.X / scale.X, value.Y / scale.Y, value.Z / scale.Z);
        
        

        public static Vector3 operator +(Vector3 value, float scalar) => new Vector3(value.X + scalar, value.Y + scalar, value.Z + scalar);
        


        public static Vector3 operator +(float scalar, Vector3 value) => new Vector3(scalar + value.X, scalar + value.Y, scalar + value.Z);
        


        public static Vector3 operator -(Vector3 value, float scalar) => new Vector3(value.X - scalar, value.Y - scalar, value.Z - scalar);
        


        public static Vector3 operator -(float scalar, Vector3 value) => new Vector3(scalar - value.X, scalar - value.Y, scalar - value.Z);



        public static explicit operator Vector2(Vector3 value) => new Vector2(value.X, value.Y);
        


        public override string ToString() => string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2}", X, Y, Z);

    }
}
