// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Point.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace Zeckoxe.Desktop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {

        public static readonly Point Zero;

        public readonly int X;

        public readonly int Y;



        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Point operator +(Point p1, Point p2) => new Point(p1.X + p2.X, p1.Y + p2.Y);
        

        public static Point operator -(Point p1, Point p2) => new Point(p1.X - p2.X, p1.Y - p2.Y);
        

        public static Point operator -(Point p, Size s) => new Point(p.X - s.Width, p.Y - s.Height);
        

        public static Point operator *(int s, Point p) => new Point(s * p.X, s * p.Y);
        

        public static Point operator *(Point p, int s) => new Point(s * p.X, s * p.Y);
        


        public bool Equals(Point other) => X.Equals(other.X) && Y.Equals(other.Y);
        

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (obj.GetType() != this.GetType())
                return false;

            return Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Point left, Point right) => Equals(left, right);
        

        public static bool operator !=(Point left, Point right) => !Equals(left, right);
        

        public override string ToString() => $"{X} {Y}";
        
    }
}
