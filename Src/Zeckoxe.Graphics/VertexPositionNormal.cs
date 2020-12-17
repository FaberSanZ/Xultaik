// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	VertexPositionNormal.cs
=============================================================================*/


using System.Numerics;
using Zeckoxe.Core;

namespace Zeckoxe.Graphics
{
    public struct VertexPositionNormal
    {
        public VertexPositionNormal(Vector3 position, Vector3 normal)
        {
            Position = position;
            Normal = normal;
        }


        [VertexAttribute(VertexAttributeType.Position, PixelFormat.R32G32B32SFloat)]
        public Vector3 Position { get; set; }


        [VertexAttribute(VertexAttributeType.Normal, PixelFormat.R32G32B32SFloat)]
        public Vector3 Normal { get; set; }



        public static readonly int Size = Interop.SizeOf<VertexPositionNormal>();


        public bool Equals(VertexPositionNormal other)
        {
            return Position.Equals(other.Position) && Normal.Equals(other.Normal);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is VertexPositionNormal v && Equals(v);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Normal.GetHashCode();
                return hashCode;
            }
        }

        public void FlipWinding()
        {
        }

        public static bool operator ==(VertexPositionNormal left, VertexPositionNormal right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VertexPositionNormal left, VertexPositionNormal right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("Position: {0}, Normal: {1}", Position, Normal);
        }
    }
}
