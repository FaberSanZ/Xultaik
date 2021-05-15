// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)




using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Vultaik
{

    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionNormalTexture : IEquatable<VertexPositionNormalTexture>
    {

        public VertexPositionNormalTexture(Vector3 position, Vector3 normal, Vector2 textureCoordinate) : this()
        {
            Position = position;
            Normal = normal;
            TextureCoordinate = textureCoordinate;
        }

        [VertexAttribute(VertexType.Position)]
        public Vector3 Position { get; set; }



        [VertexAttribute(VertexType.Normal)]
        public Vector3 Normal { get; set; }



        [VertexAttribute(VertexType.TextureCoordinate)]
        public Vector2 TextureCoordinate { get; set; }



        public static readonly int Size = Interop.SizeOf<VertexPositionNormalTexture>();


        public bool Equals(VertexPositionNormalTexture other)
        {
            return Position.Equals(other.Position) && Normal.Equals(other.Normal) && TextureCoordinate.Equals(other.TextureCoordinate);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is VertexPositionNormalTexture && Equals((VertexPositionNormalTexture)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Normal.GetHashCode();
                hashCode = (hashCode * 397) ^ TextureCoordinate.GetHashCode();
                return hashCode;
            }
        }


        public void FlipWinding()
        {
            //TextureCoordinate.X = (1.0f - TextureCoordinate.X);
        }

        public static bool operator ==(VertexPositionNormalTexture left, VertexPositionNormalTexture right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VertexPositionNormalTexture left, VertexPositionNormalTexture right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("Position: {0}, Normal: {1}, Texcoord: {2}", Position, Normal, TextureCoordinate);
        }
    }
}
