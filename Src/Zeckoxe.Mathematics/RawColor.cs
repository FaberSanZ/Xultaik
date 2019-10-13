using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Mathematics
{
    public struct RawColor
    {
        public float R;


        public float G;


        public float B;


        public float A;



        public RawColor(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public RawColor(float value)
        {
            R = G = B = A = value;
        }



        /// <summary>
        /// Red color.
        /// </summary>
        public static RawColor Red => new RawColor(1.0f, 0.0f, 0.0f, 1.0f);



        /// <summary>
        /// Green color.
        /// </summary>
        public static RawColor Green => new RawColor(0.0f, 1.0f, 0.0f, 1.0f);



        /// <summary>
        /// Blue color.
        /// </summary>
        public static RawColor Blue => new RawColor(0.0f, 0.0f, 1.0f, 1.0f);



        /// <summary>
        /// Maroon color.
        /// </summary>
        public static RawColor Maroon => new RawColor(0.501f, 0.0f, 0.0f, 1.0f);



        /// <summary>
        /// DarkRed color.
        /// </summary>
        public static RawColor DarkRed => new RawColor(0.545f, 0.0f, 0.0f, 1.0f);



        /// <summary>
        /// Brown color.
        /// </summary>
        public static RawColor Brown => new RawColor(0.647f, 0.164f, 0.164f, 1.0f);



        /// <summary>
        /// Firebrick color.
        /// </summary>
        public static RawColor Firebrick => new RawColor(0.698f, 0.133f, 0.133f, 1.0f);



        /// <summary>
        /// Crimson color.
        /// </summary>
        public static RawColor Crimson => new RawColor(0.862f, 0.078f, 0.235f, 1.0f);



        /// <summary>
        /// Tomato color.
        /// </summary>
        public static RawColor Tomato => new RawColor(1.0f, 0.388f, 0.278f, 1.0f);



        /// <summary>
        /// Coral color.
        /// </summary>
        public static RawColor Coral => new RawColor(1.0f, 0.498f, 0.313f, 1.0f);



        /// <summary>
        /// IndianRed color.
        /// </summary>
        public static RawColor IndianRed => new RawColor(0.803f, 0.360f, 0.360f, 1.0f);



        /// <summary>
        /// LightCoral color.
        /// </summary>
        public static RawColor LightCoral => new RawColor(0.941f, 0.501f, 0.501f, 1.0f);



        /// <summary>
        /// DarkSalmon color.
        /// </summary>
        public static RawColor DarkSalmon => new RawColor(0.913f, 0.588f, 0.478f, 1.0f);



        /// <summary>
        /// Salmon color.
        /// </summary>
        public static RawColor Salmon => new RawColor(0.980f, 0.501f, 0.447f, 1.0f);



        /// <summary>
        /// LightSalmon color.
        /// </summary>
        public static RawColor LightSalmon => new RawColor(1.0f, 0.627f, 0.478f, 1.0f);



        /// <summary>
        /// OrangeRed color.
        /// </summary>
        public static RawColor OrangeRed => new RawColor(1.0f, 0.270f, 0.0f, 1.0f);
    }



}
