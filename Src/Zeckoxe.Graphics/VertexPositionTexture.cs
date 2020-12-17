// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	VertexPositionTexture.cs
=============================================================================*/


using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Zeckoxe.Graphics
{
    public interface IVertex
    {

    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct VertexPositionTexture : IEquatable<VertexPositionTexture>, IVertex
    {

        public VertexPositionTexture(Vector3 position, Vector2 textureCoordinate, Vector3 _color)
            : this()
        {
            Position = position;
            TextureCoordinate = textureCoordinate;
            _color = color;

        }


        public Vector3 Position;
        public Vector3 color;


        public Vector2 TextureCoordinate;


        public static readonly int Size = Marshal.SizeOf<VertexPositionTexture>();



        public bool Equals(VertexPositionTexture other)
        {
            return Position.Equals(other.Position) && TextureCoordinate.Equals(other.TextureCoordinate);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is VertexPositionTexture && Equals((VertexPositionTexture)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ TextureCoordinate.GetHashCode();
                return hashCode;
            }
        }


        public void FlipWinding()
        {
            TextureCoordinate.X = (1.0f - TextureCoordinate.X);
        }

        public static bool operator ==(VertexPositionTexture left, VertexPositionTexture right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VertexPositionTexture left, VertexPositionTexture right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("Position: {0}, Texcoord: {1}", Position, TextureCoordinate);
        }
    }
}
