// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. Faber.reach@gmail.com

/*===================================================================================
	MathUtil.cs
====================================================================================*/


using System;

namespace Zeckoxe.Physics
{
    public static class MathUtil
    {

        public const float ZeroTolerance = 1e-6f; // Value a 8x higher than 1.19209290E-07F


        public const float Pi = (float)Math.PI;


        public const float TwoPi = (float)(2 * Math.PI);


        public const float PiOverTwo = (float)(Math.PI / 2);


        public const float PiOverFour = (float)(Math.PI / 4);


        // From http://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/ Comparing Floating point numbers 2012 edition
        public static unsafe bool NearEqual(float a, float b)
        {
            // Check if the numbers are really close -- needed
            // when comparing numbers near zero.
            if (IsZero(a - b))
            {
                return true;
            }

            // Original from Bruce Dawson: http://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/
            int aInt = *(int*)&a;
            int bInt = *(int*)&b;

            // Different signs means they do not match.
            if ((aInt < 0) != (bInt < 0))
            {
                return false;
            }

            // Find the difference in ULPs.
            int ulp = Math.Abs(aInt - bInt);

            // Choose of maxUlp = 4
            // according to http://code.google.com/p/googletest/source/browse/trunk/include/gtest/internal/gtest-internal.h
            const int maxUlp = 4;
            return (ulp <= maxUlp);
        }


        public static bool IsZero(float a) => Math.Abs(a) < ZeroTolerance;


        public static bool IsOne(float a) => IsZero(a - 1.0f);


        public static bool WithinEpsilon(float a, float b, float epsilon)
        {
            float num = a - b;
            return ((-epsilon <= num) && (num <= epsilon));
        }


        public static float RevolutionsToDegrees(float revolution) => revolution * 360.0f;



        public static float RevolutionsToRadians(float revolution) => revolution * TwoPi;



        public static float RevolutionsToGradians(float revolution) => revolution * 400.0f;



        public static float DegreesToRevolutions(float degree) => degree / 360.0f;



        public static float DegreesToRadians(float degree) => degree * (Pi / 180.0f);


        public static float RadiansToRevolutions(float radian) => radian / TwoPi;



        public static float RadiansToGradians(float radian) => radian * (200.0f / Pi);



        public static float GradiansToRevolutions(float gradian) => gradian / 400.0f;



        public static float GradiansToDegrees(float gradian) => gradian * (9.0f / 10.0f);



        public static float GradiansToRadians(float gradian) => gradian * (Pi / 200.0f);



        public static float RadiansToDegrees(float radian) => radian * (180.0f / Pi);



        public static float Clamp(float value, float min, float max) => value < min ? min : value > max ? max : value;


        public static int Clamp(int value, int min, int max) => value < min ? min : value > max ? max : value;



        // See http://www.encyclopediaofmath.org/index.php/Linear_interpolation and
        // http://fgiesen.wordpress.com/2012/08/15/linear-interpolation-past-present-and-future/
        public static double Lerp(double from, double to, double amount) => (1 - amount) * from + amount * to;



        // See http://www.encyclopediaofmath.org/index.php/Linear_interpolation and
        // http://fgiesen.wordpress.com/2012/08/15/linear-interpolation-past-present-and-future/

        public static float Lerp(float from, float to, float amount) => (1 - amount) * from + amount * to;



        // See http://www.encyclopediaofmath.org/index.php/Linear_interpolation and
        // http://fgiesen.wordpress.com/2012/08/15/linear-interpolation-past-present-and-future/
        public static byte Lerp(byte from, byte to, float amount) => (byte)Lerp(from, (float)to, amount);


        // See https://en.wikipedia.org/wiki/Smoothstep
        public static float SmoothStep(float amount) => (amount <= 0) ? 0 : (amount >= 1) ? 1 : amount * amount * (3 - (2 * amount));


        /// See https://en.wikipedia.org/wiki/Smoothstep
        public static float SmootherStep(float amount) => (amount <= 0) ? 0 : (amount >= 1) ? 1 : amount * amount * amount * (amount * ((amount * 6) - 15) + 10);



        public static float Mod(float value, float modulo) => modulo is 0.0f ? value : value % modulo;



        public static float Mod2PI(float value) => Mod(value, TwoPi);



        public static int Wrap(int value, int min, int max)
        {
            if (min > max)
            {
                throw new ArgumentException(string.Format("min {0} should be less than or equal to max {1}", min, max), "min");
            }

            // Code from http://stackoverflow.com/a/707426/1356325
            int range_size = max - min + 1;

            if (value < min)
            {
                value += range_size * ((min - value) / range_size + 1);
            }

            return min + (value - min) % range_size;
        }

        public static float Wrap(float value, float min, float max)
        {
            if (NearEqual(min, max))
            {
                return min;
            }

            double mind = min;
            double maxd = max;
            double valued = value;

            if (mind > maxd)
            {
                throw new ArgumentException(string.Format("min {0} should be less than or equal to max {1}", min, max), "min");
            }

            double range_size = maxd - mind;
            return (float)(mind + (valued - mind) - (range_size * Math.Floor((valued - mind) / range_size)));
        }


        // http://en.wikipedia.org/wiki/Gaussian_function#Two-dimensional_Gaussian_function
        public static float Gauss(float amplitude, float x, float y, float centerX, float centerY, float sigmaX, float sigmaY) => (float)Gauss((double)amplitude, x, y, centerX, centerY, sigmaX, sigmaY);



        // http://en.wikipedia.org/wiki/Gaussian_function#Two-dimensional_Gaussian_function
        public static double Gauss(double amplitude, double x, double y, double centerX, double centerY, double sigmaX, double sigmaY)
        {
            double cx = x - centerX;
            double cy = y - centerY;

            double componentX = (cx * cx) / (2 * sigmaX * sigmaX);
            double componentY = (cy * cy) / (2 * sigmaY * sigmaY);

            return amplitude * Math.Exp(-(componentX + componentY));
        }
    }
}