/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

namespace Engine.Common.Math
{
    public static class MathHelper
    {
        /// <summary>
        /// Returns the absolute value.
        /// </summary>
        /// <param name="i"></param>
        /// <returns>the absolute value</returns>
        public static int FastAbs(int i)
        {
            return (i >= 0) ? i : -i;
        }

        public static double FastFloor(double d)
        {
            int i = (int)d;
            return (d < 0 && d != i) ? i - 1 : i;
        }

        /// <summary>
        /// Clamps a given value to be an element of [0..1].
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Clamp(double value)
        {
            if (value > 1.0)
                return 1.0;
            if (value < 0.0)
                return 0.0;
            return value;
        }

        /// <summary>
        /// Linear interpolation.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="q00"></param>
        /// <param name="q01"></param>
        /// <returns></returns>
        public static double Lerp(double x, double x1, double x2, double q00, double q01)
        {
            return ((x2 - x) / (x2 - x1)) * q00 + ((x - x1) / (x2 - x1)) * q01;
        }

        /// <summary>
        /// Linear interpolation.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double Lerp(double x1, double x2, double p)
        {
            return x1 * (1.0 - p) + x2 * p;
        }


        /// <summary>
        /// Bilinear interpolation.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="q11"></param>
        /// <param name="q12"></param>
        /// <param name="q21"></param>
        /// <param name="q22"></param>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static double BiLerp(double x, double y, double q11, double q12, double q21, double q22, double x1, double x2, double y1, double y2)
        {
            double r1 = Lerp(x, x1, x2, q11, q21);
            double r2 = Lerp(x, x1, x2, q12, q22);
            return Lerp(y, y1, y2, r1, r2);
        }

        /// <summary>
        /// Trilinear interpolation.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="q000"></param>
        /// <param name="q001"></param>
        /// <param name="q010"></param>
        /// <param name="q011"></param>
        /// <param name="q100"></param>
        /// <param name="q101"></param>
        /// <param name="q110"></param>
        /// <param name="q111"></param>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="z1"></param>
        /// <param name="z2"></param>
        /// <returns></returns>
        public static double TriLerp(double x, double y, double z, double q000, double q001, double q010, double q011, double q100, double q101, double q110, double q111, double x1, double x2, double y1, double y2, double z1, double z2)
        {
            double x00 = Lerp(x, x1, x2, q000, q100);
            double x10 = Lerp(x, x1, x2, q010, q110);
            double x01 = Lerp(x, x1, x2, q001, q101);
            double x11 = Lerp(x, x1, x2, q011, q111);
            double r0 = Lerp(y, y1, y2, x00, x01);
            double r1 = Lerp(y, y1, y2, x10, x11);
            return Lerp(z, z1, z2, r0, r1);
        }

        /// <summary>
        /// Maps any given value to be positive only.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static int MapToPositive(int x)
        {
            if (x >= 0)
                return x * 2;

            return -x * 2 - 1;
        }

        /// <summary>
        /// Recreates the original value after applying "mapToPositive".
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static int RedoMapToPositive(int x)
        {
            if (x % 2 == 0)
            {
                return x / 2;
            }

            return -(x / 2) - 1;
        }

        /// <summary>
        /// Applies Cantor's pairing function to 2D coordinates.
        /// </summary>
        /// <param name="k1">X-coordinate</param>
        /// <param name="k2">Y-coordinate</param>
        /// <returns>Unique 1D value</returns>
        public static int Cantorize(int k1, int k2)
        {
            return ((k1 + k2) * (k1 + k2 + 1) / 2) + k2;
        }

        /// <summary>
        /// Inverse function of Cantor's pairing function.
        /// </summary>
        /// <param name="c">Cantor value</param>
        /// <returns>Value along the y-axis</returns>
        public static int CantorY(int c)
        {
            int j = (int)(System.Math.Sqrt(0.25 + 2 * c) - 0.5);
            return c - j * (j + 1) / 2;
        }

        /// <summary>
        /// Inverse function of Cantor's pairing function.
        /// </summary>
        /// <param name="c">Cantor value</param>
        /// <returns>Value along the x-axis</returns>
        public static int CantorX(int c)
        {
            int j = (int)(System.Math.Sqrt(0.25 + 2 * c) - 0.5);
            return j - CantorY(c);
        }

    }
}
