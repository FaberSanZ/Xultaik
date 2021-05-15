// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	VertexPositionNormalColor.cs
=============================================================================*/


using System;
using System.Numerics;

namespace Vultaik
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
