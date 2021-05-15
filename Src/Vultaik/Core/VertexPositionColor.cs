// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	VertexPositionColor.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Numerics;

namespace Vultaik
{



    [Flags]
    public enum AttachmentType : UInt32
    {
        None,
        Color = 0x1,
        Normal = 0x2,
        AmbientOcclusion = 0x4,
        Metal = 0x8,
        Roughness = 0x10,
        PhysicalProps = 0x20,
        Emissive = 0x40,
    };


    public class AttachmentAttribute : Attribute
    {
        public AttachmentType AttachmentType;
        public Type DataType;
        public AttachmentAttribute(AttachmentType attachmentType, Type dataType)
        {
            AttachmentType = attachmentType;
            DataType = dataType;
        }
    }



    public struct VertexPositionColor
    {

        public VertexPositionColor(Vector3 position, Vector3 color)
        {
            Position = position;
            Color = color;
        }

        [VertexAttribute(VertexType.Position)]
        public Vector3 Position { get; set; }


        [VertexAttribute(VertexType.Color)]
        public Vector3 Color { get; set; }



        public static readonly int Size = Interop.SizeOf<VertexPositionColor>();


        public bool Equals(VertexPositionColor other)
        {
            return Position.Equals(other.Position) && Color.Equals(other.Color);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is VertexPositionColor v && Equals(v);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                return hashCode;
            }
        }

        public void FlipWinding()
        {
        }

        public static bool operator ==(VertexPositionColor left, VertexPositionColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VertexPositionColor left, VertexPositionColor right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("Position: {0}, Color: {1}", Position, Color);
        }
    }


    public struct VertexPositionColor4 
    {

        public VertexPositionColor4(Vector3 position, Vector3 color)
        {
            Position = position;
            Color = new Vector4(color, 0);
        }

        public VertexPositionColor4(Vector3 position, Vector4 color)
        {
            Position = position;
            Color = color;
        }

        [VertexAttribute(VertexType.Position)]
        public Vector3 Position { get; set; }


        [VertexAttribute(VertexType.Color)]
        public Vector4 Color { get; set; }



        public static readonly int Size = Interop.SizeOf<VertexPositionColor4>();


        public bool Equals(VertexPositionColor4 other)
        {
            return Position.Equals(other.Position) && Color.Equals(other.Color);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is VertexPositionColor4 v && Equals(v);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                return hashCode;
            }
        }

        public void FlipWinding()
        {
        }

        public static bool operator ==(VertexPositionColor4 left, VertexPositionColor4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VertexPositionColor4 left, VertexPositionColor4 right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("Position: {0}, Color: {1}", Position, Color);
        }
    }
}
