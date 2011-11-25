/*
* Hüseyin Uslu, shalafiraistlin@gmail.com
* This code is provided as is.
* Original code by: http://techcraft.codeplex.com/discussions/247791
*/

using System;
using System.Diagnostics;

namespace arraytests.Tests
{
    public sealed class Multidimensional: ArrayTest
    {
        private int[,,] _storage;

        public Multidimensional(int dimensionSize)
            : base(dimensionSize) { }

        public override TimeSpan AccessSequental()
        {
            _storage = null;
            _storage = new int[DimensionSize, DimensionSize, DimensionSize];

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            int counter = 0;
            for (int x = 0; x < DimensionSize; x++)
            {
                for (int y = 0; y < DimensionSize; y++)
                {
                    for (int z = 0; z < DimensionSize; z++)
                    {
                        _storage[x, y, z] = counter;
                        counter++;
                    }
                }
            }

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public override TimeSpan AccessRandom()
        {
            _storage = null;
            _storage = new int[DimensionSize, DimensionSize, DimensionSize];
            int dimensionMinusOne = DimensionSize - 1;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            int counter = 0;
            bool bSwitch = false;
            for (int x = 0; x < DimensionSize; x++)
            {
                for (int y = 0; y < DimensionSize; y++)
                {
                    for (int z = 0; z < DimensionSize; z++)
                    {
                        if (bSwitch)
                        {
                            _storage[x, y, z] = counter;
                        }
                        else
                        {
                            _storage[dimensionMinusOne - x, dimensionMinusOne - y, dimensionMinusOne - z] = counter;
                        }
                        counter++;
                        bSwitch = !bSwitch;
                    }
                }
            }

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }
}
