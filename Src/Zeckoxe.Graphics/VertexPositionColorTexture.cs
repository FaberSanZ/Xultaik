using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Zeckoxe.Graphics
{

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct VertexPositionColorTexture : IEquatable<VertexPositionColorTexture>
    {

        public VertexPositionColorTexture(Vector3 position, Vector3 color, Vector2 textureCoordinate) : this()
        {
            Position = position;
            Color = color;
            TextureCoordinate = textureCoordinate;
        }


        public Vector3 Position;


        public Vector3 Color;


        public Vector2 TextureCoordinate;


        public static readonly int Size = 24;



        public bool Equals(VertexPositionColorTexture other)
        {
            return Position.Equals(other.Position) && Color.Equals(other.Color) && TextureCoordinate.Equals(other.TextureCoordinate);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is VertexPositionColorTexture && Equals((VertexPositionColorTexture)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                hashCode = (hashCode * 397) ^ TextureCoordinate.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(VertexPositionColorTexture left, VertexPositionColorTexture right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VertexPositionColorTexture left, VertexPositionColorTexture right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("Position: {0}, Color: {1}, Texcoord: {2}", Position, Color, TextureCoordinate);
        }


        public void FlipWinding()
        {
            TextureCoordinate.X = (1.0f - TextureCoordinate.X);
        }
    }
}
