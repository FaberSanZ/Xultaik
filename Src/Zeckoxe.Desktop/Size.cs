// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Size.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace Zeckoxe.Desktop
{

    public struct Size
    {

        public static readonly Size Empty = new Size();


        public readonly int Width;


        public readonly int Height;

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public static Size operator +(Size p1, Size p2) => new Size(p1.Width + p2.Width, p1.Height + p2.Height);
        

        public static Size operator -(Size p1, Size p2) => new Size(p1.Width - p2.Width, p1.Height - p2.Height);
        

        public static Size operator *(int s, Size p) => new Size(s * p.Width, s * p.Height);


        public static Size operator *(Size p, int s) => new Size(s * p.Width, s * p.Height);
        

        public static implicit operator Size(Point p) => new Size(p.X, p.Y);
        

        public bool Equals(Size other) => Width.Equals(other.Width) && Height.Equals(other.Height);
        

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (obj.GetType() != this.GetType())
                return false;

            return Equals((Size)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Width.GetHashCode();
                hashCode = (hashCode * 397) ^ Height.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Size left, Size right) => Equals(left, right);
        

        public static bool operator !=(Size left, Size right) => !Equals(left, right);
        

        public override string ToString() => $"{Width} {Height}";
        
    }
}