using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace Zeckoxe.Mathematics
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vector2 
    {
        public static readonly Vector2 Zero = new Vector2();


        public static readonly Vector2 UnitX = new Vector2(1.0f, 0.0f);


        public static readonly Vector2 UnitY = new Vector2(0.0f, 1.0f);


        public static readonly Vector2 One = new Vector2(1.0f, 1.0f);


        public float X;


        public float Y;



        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }




        public bool IsZero=>  X == 0 && Y == 0;
        


        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                }

                throw new ArgumentOutOfRangeException("index", "Indices for Vector2 run from 0 to 1, inclusive.");
            }

            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    default: throw new ArgumentOutOfRangeException("index", "Indices for Vector2 run from 0 to 1, inclusive.");
                }
            }
        }



        public float Length() => (float)Math.Sqrt((X * X) + (Y * Y));



        public float LengthSquared() => (X * X) + (Y * Y);



        public float[] ToArray() => new float[] { X, Y };




        public static Vector2 Add(Vector2 left, Vector2 right) => new Vector2(left.X + right.X, left.Y + right.Y);



        public static Vector2 Subtract(Vector2 left, Vector2 right) => new Vector2(left.X - right.X, left.Y - right.Y);

        

        public static Vector2 Multiply(Vector2 left, Vector2 right) => new Vector2(left.X * right.X, left.Y * right.Y);
        


        public static Vector2 Divide(Vector2 value, float scale) => new Vector2(value.X / scale, value.Y / scale);



        public static Vector2 Negate(Vector2 value) => new Vector2(-value.X, -value.Y);




        public static float Dot(Vector2 left, Vector2 right) => (left.X * right.X) + (left.Y * right.Y);



        public static Vector2 operator +(Vector2 left, Vector2 right) => new Vector2(left.X + right.X, left.Y + right.Y);



        public static Vector2 operator *(Vector2 left, Vector2 right) => new Vector2(left.X * right.X, left.Y * right.Y);



        public static Vector2 operator +(Vector2 value) => value;



        public static Vector2 operator -(Vector2 left, Vector2 right) => new Vector2(left.X - right.X, left.Y - right.Y);



        public static Vector2 operator -(Vector2 value) => new Vector2(-value.X, -value.Y);



        public static Vector2 operator *(float scale, Vector2 value) => new Vector2(value.X * scale, value.Y * scale);



        public static Vector2 operator *(Vector2 value, float scale) => new Vector2(value.X * scale, value.Y * scale);



        public static Vector2 operator /(Vector2 value, float scale) => new Vector2(value.X / scale, value.Y / scale);



        public static Vector2 operator /(float scale, Vector2 value) => new Vector2(scale / value.X, scale / value.Y);



        public static Vector2 operator /(Vector2 value, Vector2 scale) => new Vector2(value.X / scale.X, value.Y / scale.Y);



        public static Vector2 operator +(Vector2 value, float scalar) => new Vector2(value.X + scalar, value.Y + scalar);



        public static Vector2 operator +(float scalar, Vector2 value) => new Vector2(scalar + value.X, scalar + value.Y);



        public static Vector2 operator -(Vector2 value, float scalar) => new Vector2(value.X - scalar, value.Y - scalar);



        public static Vector2 operator -(float scalar, Vector2 value) => new Vector2(scalar - value.X, scalar - value.Y);



        public override string ToString() => string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1}", X, Y);

    }
}
