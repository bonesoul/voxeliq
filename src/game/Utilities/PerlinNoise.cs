/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using System;
using VolumetricStudios.VoxeliqGame.Common.Helpers.Math;
using VolumetricStudios.VoxeliqGame.Utilities;

/**
 *  Inspired & based on Benjamin Glatzel's improved perlin noise implementation - https://github.com/MovingBlocks/Terasology/blob/develop/src/org/terasology/utilities/PerlinNoise.java.
 */

namespace VolumetricStudios.VoxeliqGame.Utilities
{
    /// <summary>
    /// Perlin simplex noise
    /// </summary>
    public class PerlinNoise
    {
        private static double LACUNARITY = 2.1379201;
        private static double H = 0.836281;

        private double[] _spectralWeights;

        private int[] _noisePermutations;
        private bool _recomputeSpectralWeights = true;
        private int _octaves = 9;

        /// <summary>
        /// Init. a new generator with a given seed value.
        /// </summary>
        /// <param name="seed"></param>
        public PerlinNoise(int seed)
        {
            FastRandom rand = new FastRandom(seed);

            _noisePermutations = new int[512];
            int[] _noiseTable = new int[256];

            // Init. the noise table
            for (int i = 0; i < 256; i++)
                _noiseTable[i] = i;

            // Shuffle the array
            for (int i = 0; i < 256; i++)
            {
                int j = rand.NextInt() % 256;
                j = (j < 0) ? -j : j;

                int swap = _noiseTable[i];
                _noiseTable[i] = _noiseTable[j];
                _noiseTable[j] = swap;
            }

            // Finally replicate the noise permutations in the remaining 256 index positions
            for (int i = 0; i < 256; i++)
                _noisePermutations[i] = _noisePermutations[i + 256] = _noiseTable[i];
        }

        public double noise(double x, double y, double z)
        {
            int X = (int)MathHelper.FastFloor(x) & 255, Y = (int)MathHelper.FastFloor(y) & 255, Z = (int)MathHelper.FastFloor(z) & 255;

            x -= MathHelper.FastFloor(x);
            y -= MathHelper.FastFloor(y);
            z -= MathHelper.FastFloor(z);

            double u = fade(x), v = fade(y), w = fade(z);
            int A = _noisePermutations[X] + Y, AA = _noisePermutations[A] + Z, AB = _noisePermutations[(A + 1)] + Z,
                    B = _noisePermutations[(X + 1)] + Y, BA = _noisePermutations[B] + Z, BB = _noisePermutations[(B + 1)] + Z;

            return Lerp(w, Lerp(v, Lerp(u, grad(_noisePermutations[AA], x, y, z),
                    grad(_noisePermutations[BA], x - 1, y, z)),
                    Lerp(u, grad(_noisePermutations[AB], x, y - 1, z),
                            grad(_noisePermutations[BB], x - 1, y - 1, z))),
                    Lerp(v, Lerp(u, grad(_noisePermutations[(AA + 1)], x, y, z - 1),
                            grad(_noisePermutations[(BA + 1)], x - 1, y, z - 1)),
                            Lerp(u, grad(_noisePermutations[(AB + 1)], x, y - 1, z - 1),
                                    grad(_noisePermutations[(BB + 1)], x - 1, y - 1, z - 1))));
        }

        /// <summary>
        /// Returns Fractional Brownian Motion at the given position.
        /// </summary>
        /// <param name="x">Position on the x-axis</param>
        /// <param name="y">Position on the y-axis</param>
        /// <param name="z">Position on the z-axis</param>
        /// <returns></returns>
        public double fBm(double x, double y, double z)
        {
            double result = 0.0;

            if (_recomputeSpectralWeights)
            {
                _spectralWeights = new double[_octaves];

                for (int i = 0; i < _octaves; i++)
                    _spectralWeights[i] = Math.Pow(LACUNARITY, -H*i);

                _recomputeSpectralWeights = false;
            }

            for (int i = 0; i < _octaves; i++)
            {
                result += noise(x, y, z)*_spectralWeights[i];

                x *= LACUNARITY;
                y *= LACUNARITY;
                z *= LACUNARITY;
            }
            return result;
        }

        private static double fade(double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static double Lerp(double t, double a, double b)
        {
            return a + t * (b - a);
        }

        private static double grad(int hash, double x, double y, double z)
        {
            int h = hash & 15;
            double u = h < 8 ? x : y, v = h < 4 ? y : h == 12 || h == 14 ? x : z;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        public void setOctaves(int octaves)
        {
            _octaves = octaves;
            _recomputeSpectralWeights = true;
        }

        public int getOctaves()
        {
            return _octaves;
        }

    }   
}
