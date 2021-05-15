using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vultaik.Physics
{
    // TODO: Quaternion

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Quaternion : IEquatable<Quaternion>, IFormattable
    {

        public Quaternion(float value)
        {
            X = value;
            Y = value;
            Z = value;
            W = value;
        }

        public Quaternion(Vector4 value)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
            W = value.W;
        }

        public Quaternion(Vector3 value, float w)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
            W = w;
        }

        public Quaternion(Vector2 value, float z, float w)
        {
            X = value.X;
            Y = value.Y;
            Z = z;
            W = w;
        }

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Quaternion(float[] values)
        {
            if (values is null)
            {
                throw new ArgumentNullException("values");
            }

            if (values.Length != 4)
            {
                throw new ArgumentOutOfRangeException("values", "There must be four and only four input values for Quaternion.");
            }

            X = values[0];
            Y = values[1];
            Z = values[2];
            W = values[3];
        }



        //public static readonly int SizeInBytes = Interop.SizeOf<Quaternion>();

        public static readonly Quaternion Zero = new Quaternion();

        public static readonly Quaternion One = new Quaternion(1.0f, 1.0f, 1.0f, 1.0f);

        public static readonly Quaternion Identity = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);

        public float X;

        public float Y;

        public float Z;

        public float W;

        public bool IsIdentity => Equals(Identity);

        public bool IsNormalized => MathUtil.IsOne((X * X) + (Y * Y) + (Z * Z) + (W * W));

        public float Angle => MathUtil.IsZero((X * X) + (Y * Y) + (Z * Z)) ? 0.0f : (float)(2.0 * Math.Acos(MathUtil.Clamp(W, -1f, 1f)));



        public Vector3 Axis
        {
            get
            {
                float length = (X * X) + (Y * Y) + (Z * Z);
                if (MathUtil.IsZero(length))
                {
                    return Vector3.UnitX;
                }

                float inv = 1.0f / (float)Math.Sqrt(length);
                return new Vector3(X * inv, Y * inv, Z * inv);
            }
        }


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

                throw new ArgumentOutOfRangeException("index", "Indices for Quaternion run from 0 to 3, inclusive.");
            }

            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    case 3: W = value; break;
                    default: throw new ArgumentOutOfRangeException("index", "Indices for Quaternion run from 0 to 3, inclusive.");
                }
            }
        }


        public void Conjugate()
        {
            X = -X;
            Y = -Y;
            Z = -Z;
        }


        public void Invert()
        {
            float lengthSq = LengthSquared();
            if (!MathUtil.IsZero(lengthSq))
            {
                lengthSq = 1.0f / lengthSq;

                X = -X * lengthSq;
                Y = -Y * lengthSq;
                Z = -Z * lengthSq;
                W = W * lengthSq;
            }
        }


        public float Length() => (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));

        public float LengthSquared() => (X * X) + (Y * Y) + (Z * Z) + (W * W);

        public void Normalize()
        {
            float length = Length();
            if (!MathUtil.IsZero(length))
            {
                float inverse = 1.0f / length;
                X *= inverse;
                Y *= inverse;
                Z *= inverse;
                W *= inverse;
            }
        }


        public float[] ToArray() => new float[] { X, Y, Z, W };
        


        public static void Add(ref Quaternion left, ref Quaternion right, out Quaternion result)
        {
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = left.Z + right.Z;
            result.W = left.W + right.W;
        }


        public static Quaternion Add(Quaternion left, Quaternion right)
        {
            Add(ref left, ref right, out Quaternion result);
            return result;
        }


        public static void Subtract(ref Quaternion left, ref Quaternion right, out Quaternion result)
        {
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
            result.W = left.W - right.W;
        }


        public static Quaternion Subtract(Quaternion left, Quaternion right)
        {
            Subtract(ref left, ref right, out Quaternion result);
            return result;
        }


        public static void Multiply(ref Quaternion value, float scale, out Quaternion result)
        {
            result.X = value.X * scale;
            result.Y = value.Y * scale;
            result.Z = value.Z * scale;
            result.W = value.W * scale;
        }


        public static Quaternion Multiply(Quaternion value, float scale)
        {
            Multiply(ref value, scale, out Quaternion result);
            return result;
        }


        public static void Multiply(ref Quaternion left, ref Quaternion right, out Quaternion result)
        {
            float lx = left.X;
            float ly = left.Y;
            float lz = left.Z;
            float lw = left.W;
            float rx = right.X;
            float ry = right.Y;
            float rz = right.Z;
            float rw = right.W;
            float a = (ly * rz - lz * ry);
            float b = (lz * rx - lx * rz);
            float c = (lx * ry - ly * rx);
            float d = (lx * rx + ly * ry + lz * rz);
            result.X = (lx * rw + rx * lw) + a;
            result.Y = (ly * rw + ry * lw) + b;
            result.Z = (lz * rw + rz * lw) + c;
            result.W = lw * rw - d;
        }


        public static Quaternion Multiply(Quaternion left, Quaternion right)
        {
            Multiply(ref left, ref right, out Quaternion result);
            return result;
        }

        public static void Negate(ref Quaternion value, out Quaternion result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            result.W = -value.W;
        }


        public static Quaternion Negate(Quaternion value)
        {
            Negate(ref value, out Quaternion result);
            return result;
        }


        public static void Barycentric(ref Quaternion value1, ref Quaternion value2, ref Quaternion value3, float amount1, float amount2, out Quaternion result)
        {
            Slerp(ref value1, ref value2, amount1 + amount2, out Quaternion start);
            Slerp(ref value1, ref value3, amount1 + amount2, out Quaternion end);
            Slerp(ref start, ref end, amount2 / (amount1 + amount2), out result);
        }


        public static Quaternion Barycentric(Quaternion value1, Quaternion value2, Quaternion value3, float amount1, float amount2)
        {
            Barycentric(ref value1, ref value2, ref value3, amount1, amount2, out Quaternion result);
            return result;
        }


        public static void Conjugate(ref Quaternion value, out Quaternion result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            result.W = value.W;
        }


        public static Quaternion Conjugate(Quaternion value)
        {
            Conjugate(ref value, out Quaternion result);
            return result;
        }

        public static void Dot(ref Quaternion left, ref Quaternion right, out float result) => result = (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z) + (left.W * right.W);
        
        public static float Dot(Quaternion left, Quaternion right) => (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z) + (left.W * right.W);
        


        public static void Exponential(ref Quaternion value, out Quaternion result)
        {
            float angle = (float)Math.Sqrt((value.X * value.X) + (value.Y * value.Y) + (value.Z * value.Z));
            float sin = (float)Math.Sin(angle);

            if (!MathUtil.IsZero(sin))
            {
                float coeff = sin / angle;
                result.X = coeff * value.X;
                result.Y = coeff * value.Y;
                result.Z = coeff * value.Z;
            }
            else
            {
                result = value;
            }

            result.W = (float)Math.Cos(angle);
        }


        public static Quaternion Exponential(Quaternion value)
        {
            Exponential(ref value, out Quaternion result);
            return result;
        }


        public static void Invert(ref Quaternion value, out Quaternion result)
        {
            result = value;
            result.Invert();
        }


        public static Quaternion Invert(Quaternion value)
        {
            Invert(ref value, out Quaternion result);
            return result;
        }


        public static void Lerp(ref Quaternion start, ref Quaternion end, float amount, out Quaternion result)
        {
            float inverse = 1.0f - amount;

            if (Dot(start, end) >= 0.0f)
            {
                result.X = (inverse * start.X) + (amount * end.X);
                result.Y = (inverse * start.Y) + (amount * end.Y);
                result.Z = (inverse * start.Z) + (amount * end.Z);
                result.W = (inverse * start.W) + (amount * end.W);
            }
            else
            {
                result.X = (inverse * start.X) - (amount * end.X);
                result.Y = (inverse * start.Y) - (amount * end.Y);
                result.Z = (inverse * start.Z) - (amount * end.Z);
                result.W = (inverse * start.W) - (amount * end.W);
            }

            result.Normalize();
        }


        public static Quaternion Lerp(Quaternion start, Quaternion end, float amount)
        {
            Lerp(ref start, ref end, amount, out Quaternion result);
            return result;
        }


        public static void Logarithm(ref Quaternion value, out Quaternion result)
        {
            if (Math.Abs(value.W) < 1.0)
            {
                float angle = (float)Math.Acos(value.W);
                float sin = (float)Math.Sin(angle);

                if (!MathUtil.IsZero(sin))
                {
                    float coeff = angle / sin;
                    result.X = value.X * coeff;
                    result.Y = value.Y * coeff;
                    result.Z = value.Z * coeff;
                }
                else
                {
                    result = value;
                }
            }
            else
            {
                result = value;
            }

            result.W = 0.0f;
        }


        public static Quaternion Logarithm(Quaternion value)
        {
            Logarithm(ref value, out Quaternion result);
            return result;
        }

        public static void Normalize(ref Quaternion value, out Quaternion result)
        {
            Quaternion temp = value;
            result = temp;
            result.Normalize();
        }


        public static Quaternion Normalize(Quaternion value)
        {
            value.Normalize();
            return value;
        }

        public static void RotationAxis(ref Vector3 axis, float angle, out Quaternion result)
        {
            Vector3 normalized = Vector3.Normalize(axis);

            float half = angle * 0.5f;
            float sin = (float)Math.Sin(half);
            float cos = (float)Math.Cos(half);

            result.X = normalized.X * sin;
            result.Y = normalized.Y * sin;
            result.Z = normalized.Z * sin;
            result.W = cos;
        }


        public static Quaternion RotationAxis(Vector3 axis, float angle)
        {
            RotationAxis(ref axis, angle, out Quaternion result);
            return result;
        }


        public static void RotationMatrix(ref Matrix4x4 matrix, out Quaternion result)
        {
            float sqrt;
            float half;
            float scale = matrix.M11 + matrix.M22 + matrix.M33;

            if (scale > 0.0f)
            {
                sqrt = (float)Math.Sqrt(scale + 1.0f);
                result.W = sqrt * 0.5f;
                sqrt = 0.5f / sqrt;

                result.X = (matrix.M23 - matrix.M32) * sqrt;
                result.Y = (matrix.M31 - matrix.M13) * sqrt;
                result.Z = (matrix.M12 - matrix.M21) * sqrt;
            }
            else if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
            {
                sqrt = (float)Math.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
                half = 0.5f / sqrt;

                result.X = 0.5f * sqrt;
                result.Y = (matrix.M12 + matrix.M21) * half;
                result.Z = (matrix.M13 + matrix.M31) * half;
                result.W = (matrix.M23 - matrix.M32) * half;
            }
            else if (matrix.M22 > matrix.M33)
            {
                sqrt = (float)Math.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
                half = 0.5f / sqrt;

                result.X = (matrix.M21 + matrix.M12) * half;
                result.Y = 0.5f * sqrt;
                result.Z = (matrix.M32 + matrix.M23) * half;
                result.W = (matrix.M31 - matrix.M13) * half;
            }
            else
            {
                sqrt = (float)Math.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
                half = 0.5f / sqrt;

                result.X = (matrix.M31 + matrix.M13) * half;
                result.Y = (matrix.M32 + matrix.M23) * half;
                result.Z = 0.5f * sqrt;
                result.W = (matrix.M12 - matrix.M21) * half;
            }
        }

        public static Quaternion RotationMatrix(Matrix4x4 matrix)
        {
            RotationMatrix(ref matrix, out Quaternion result);
            return result;
        }

        public static void RotationYawPitchRoll(float yaw, float pitch, float roll, out Quaternion result)
        {
            float halfRoll = roll * 0.5f;
            float halfPitch = pitch * 0.5f;
            float halfYaw = yaw * 0.5f;

            float sinRoll = (float)Math.Sin(halfRoll);
            float cosRoll = (float)Math.Cos(halfRoll);
            float sinPitch = (float)Math.Sin(halfPitch);
            float cosPitch = (float)Math.Cos(halfPitch);
            float sinYaw = (float)Math.Sin(halfYaw);
            float cosYaw = (float)Math.Cos(halfYaw);

            result.X = (cosYaw * sinPitch * cosRoll) + (sinYaw * cosPitch * sinRoll);
            result.Y = (sinYaw * cosPitch * cosRoll) - (cosYaw * sinPitch * sinRoll);
            result.Z = (cosYaw * cosPitch * sinRoll) - (sinYaw * sinPitch * cosRoll);
            result.W = (cosYaw * cosPitch * cosRoll) + (sinYaw * sinPitch * sinRoll);
        }

        public static Quaternion RotationYawPitchRoll(float yaw, float pitch, float roll)
        {
            RotationYawPitchRoll(yaw, pitch, roll, out Quaternion result);
            return result;
        }

        public static void Slerp(ref Quaternion start, ref Quaternion end, float amount, out Quaternion result)
        {
            float opposite;
            float inverse;
            float dot = Dot(start, end);

            if (Math.Abs(dot) > 1.0f - MathUtil.ZeroTolerance)
            {
                inverse = 1.0f - amount;
                opposite = amount * Math.Sign(dot);
            }
            else
            {
                float acos = (float)Math.Acos(Math.Abs(dot));
                float invSin = (float)(1.0 / Math.Sin(acos));

                inverse = (float)Math.Sin((1.0f - amount) * acos) * invSin;
                opposite = (float)Math.Sin(amount * acos) * invSin * Math.Sign(dot);
            }

            result.X = (inverse * start.X) + (opposite * end.X);
            result.Y = (inverse * start.Y) + (opposite * end.Y);
            result.Z = (inverse * start.Z) + (opposite * end.Z);
            result.W = (inverse * start.W) + (opposite * end.W);
        }

        public static Quaternion Slerp(Quaternion start, Quaternion end, float amount)
        {
            Slerp(ref start, ref end, amount, out Quaternion result);
            return result;
        }

        public static void Squad(ref Quaternion value1, ref Quaternion value2, ref Quaternion value3, ref Quaternion value4, float amount, out Quaternion result)
        {
            Slerp(ref value1, ref value4, amount, out Quaternion start);
            Slerp(ref value2, ref value3, amount, out Quaternion end);
            Slerp(ref start, ref end, 2.0f * amount * (1.0f - amount), out result);
        }

        public static Quaternion Squad(Quaternion value1, Quaternion value2, Quaternion value3, Quaternion value4, float amount)
        {
            Squad(ref value1, ref value2, ref value3, ref value4, amount, out Quaternion result);
            return result;
        }

        public static Quaternion[] SquadSetup(Quaternion value1, Quaternion value2, Quaternion value3, Quaternion value4)
        {
            Quaternion q0 = (value1 + value2).LengthSquared() < (value1 - value2).LengthSquared() ? -value1 : value1;
            Quaternion q2 = (value2 + value3).LengthSquared() < (value2 - value3).LengthSquared() ? -value3 : value3;
            Quaternion q3 = (value3 + value4).LengthSquared() < (value3 - value4).LengthSquared() ? -value4 : value4;
            Quaternion q1 = value2;

            Exponential(ref q1, out Quaternion q1Exp);
            Exponential(ref q2, out Quaternion q2Exp);

            Quaternion[] results = new Quaternion[3];
            results[0] = q1 * Exponential(-0.25f * (Logarithm(q1Exp * q2) + Logarithm(q1Exp * q0)));
            results[1] = q2 * Exponential(-0.25f * (Logarithm(q2Exp * q3) + Logarithm(q2Exp * q1)));
            results[2] = q2;

            return results;
        }
        public static Quaternion operator +(Quaternion left, Quaternion right)
        {
            Add(ref left, ref right, out Quaternion result);
            return result;
        }

        public static Quaternion operator -(Quaternion left, Quaternion right)
        {
            Subtract(ref left, ref right, out Quaternion result);
            return result;
        }

        public static Quaternion operator -(Quaternion value)
        {
            Negate(ref value, out Quaternion result);
            return result;
        }

        public static Quaternion operator *(float scale, Quaternion value)
        {
            Multiply(ref value, scale, out Quaternion result);
            return result;
        }

        public static Quaternion operator *(Quaternion value, float scale)
        {
            Multiply(ref value, scale, out Quaternion result);
            return result;
        }

        public static Quaternion operator *(Quaternion left, Quaternion right)
        {
            Multiply(ref left, ref right, out Quaternion result);
            return result;
        }

        [MethodImpl((MethodImplOptions)0x100)] // MethodImplOptions.AggressiveInlining
        public static bool operator ==(Quaternion left, Quaternion right)
        {
            return left.Equals(ref right);
        }

        [MethodImpl((MethodImplOptions)0x100)] // MethodImplOptions.AggressiveInlining
        public static bool operator !=(Quaternion left, Quaternion right)
        {
            return !left.Equals(ref right);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", X, Y, Z, W);
        }


        public string ToString(string format)
        {
            if (format == null)
            {
                return ToString();
            }

            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", X.ToString(format, CultureInfo.CurrentCulture),
                Y.ToString(format, CultureInfo.CurrentCulture), Z.ToString(format, CultureInfo.CurrentCulture), W.ToString(format, CultureInfo.CurrentCulture));
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2} W:{3}", X, Y, Z, W);
        }


        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
            {
                return ToString(formatProvider);
            }

            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2} W:{3}", X.ToString(format, formatProvider),
                Y.ToString(format, formatProvider), Z.ToString(format, formatProvider), W.ToString(format, formatProvider));
        }


        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                hashCode = (hashCode * 397) ^ W.GetHashCode();
                return hashCode;
            }
        }

        [MethodImpl((MethodImplOptions)0x100)] // MethodImplOptions.AggressiveInlining
        public bool Equals(ref Quaternion other)
        {
            return MathUtil.NearEqual(other.X, X) && MathUtil.NearEqual(other.Y, Y) && MathUtil.NearEqual(other.Z, Z) && MathUtil.NearEqual(other.W, W);
        }


        [MethodImpl((MethodImplOptions)0x100)] // MethodImplOptions.AggressiveInlining
        public bool Equals(Quaternion other)
        {
            return Equals(ref other);
        }

        public override bool Equals(object value)
        {
            if (!(value is Quaternion))
            {
                return false;
            }

            Quaternion strongValue = (Quaternion)value;
            return Equals(ref strongValue);
        }
    }
}
