// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	BoundingBox.cs
=============================================================================*/


using System.Numerics;

namespace Vultaik.Physics
{
    public struct BoundingBox
    {

        public BoundingBox(Vector3 min, Vector3 max, bool isValid = false)
        {
            Min = min;
            Max = max;
            IsValid = isValid;
        }

        public Vector3 Min;
        public Vector3 Max;
        public bool IsValid;


        public float Width => Max.X - Min.X;
        public float Height => Max.Y - Min.Y;
        public float Depth => Max.Z - Min.Z;

        public Vector3 Center => new Vector3(Width / 2f + Min.X, Height / 2f + Min.Y, Depth / 2f + Min.Z);


        public BoundingBox GetAABB(Matrix4x4 m)
        {
            if (!IsValid)
            {
                return default;
            }

            Vector3 mini = new Vector3(m.M41, m.M42, m.M43);
            Vector3 maxi = mini;
            Vector3 v0, v1;

            Vector3 right = new Vector3(m.M11, m.M12, m.M13);
            v0 = right * Min.X;
            v1 = right * Max.X;
            mini += Vector3.Min(v0, v1);
            maxi += Vector3.Max(v0, v1);

            Vector3 up = new Vector3(m.M21, m.M22, m.M23);
            v0 = up * Min.Y;
            v1 = up * Max.Y;
            mini += Vector3.Min(v0, v1);
            maxi += Vector3.Max(v0, v1);

            Vector3 back = new Vector3(m.M31, m.M32, m.M33);
            v0 = back * Min.Z;
            v1 = back * Max.Z;
            mini += Vector3.Min(v0, v1);
            maxi += Vector3.Max(v0, v1);

            return new BoundingBox(mini, maxi, true);
        }


        public static BoundingBox operator +(BoundingBox bb1, BoundingBox bb2)
        {
            return bb1.IsValid ? bb2.IsValid ? new BoundingBox(Vector3.Min(bb1.Min, bb2.Min), Vector3.Min(bb1.Max, bb2.Max), true) : bb1 : bb2.IsValid ? bb2 : default;
        }
        public override string ToString()
        {
            return IsValid ? string.Format($" {Min}->{Max}") : "Invalid";
        }
    }
}
