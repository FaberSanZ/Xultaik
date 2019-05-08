using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _13__Framework
{
    public static class MathHelper
    {
        private static readonly Random _random = new Random();

        public static int Rand(int minValue, int maxValue) => _random.Next(minValue, maxValue);
        public static float Randf() => _random.NextFloat(0.0f, 1.0f);
        public static float Randf(float minValue, float maxValue) => _random.NextFloat(minValue, maxValue);

        public static float Sinf(double a) => (float)Math.Sin(a);
        public static float Cosf(double d) => (float)Math.Cos(d);
        public static float Tanf(double a) => (float)Math.Tan(a);
        public static float Atanf(double d) => (float)Math.Atan(d);
        public static float Atan2f(double y, double x) => (float)Math.Atan2(y, x);
        public static float Acosf(double d) => (float)Math.Acos(d);
        public static float Expf(double d) => (float)Math.Exp(d);
        public static float Sqrtf(double d) => (float)Math.Sqrt(d);

        public static Vector3 SphericalToCartesian(float radius, float theta, float phi) => new Vector3(
            radius * Sinf(phi) * Cosf(theta),
            radius * Cosf(phi),
            radius * Sinf(phi) * Sinf(theta));

        public static Matrix InverseTranspose(Matrix m)
        {
            // Inverse-transpose is just applied to normals. So zero out
            // translation row so that it doesn't get into our inverse-transpose
            // calculation--we don't want the inverse-transpose of the translation.
            m.Row4 = Vector4.UnitW;

            return Matrix.Transpose(Matrix.Invert(m));
        }
    }
}
