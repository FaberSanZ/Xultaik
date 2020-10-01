using System;
using System.Numerics;
using Zeckoxe.Core;

namespace Zeckoxe.Graphics
{
    public struct VertexPositionNormalColor : IEquatable<VertexPositionNormalColor>
    {

        public VertexPositionNormalColor(Vector3 position, Vector3 normal, Vector3 color) : this()
        {
            Position = position;
            Normal = normal;
            Color = color;
        }


        public Vector3 Position;

        public Vector3 Normal;

        public Vector3 Color;

        public static readonly int Size = Interop.SizeOf<VertexPositionNormalColor>();


        public bool Equals(VertexPositionNormalColor other)
        {
            return Position.Equals(other.Position) && Normal.Equals(other.Normal) && Color.Equals(other.Color);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is VertexPositionNormalColor v && Equals(v);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Normal.GetHashCode();
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                return hashCode;
            }
        }

        public void FlipWinding()
        {
        }

        public static bool operator ==(VertexPositionNormalColor left, VertexPositionNormalColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VertexPositionNormalColor left, VertexPositionNormalColor right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("Position: {0}, Normal: {1}, Color: {2}", Position, Normal, Color);
        }
    }
}
