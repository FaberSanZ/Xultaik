using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
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

        public Vector3 Position;

        public Vector3 Normal;



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
