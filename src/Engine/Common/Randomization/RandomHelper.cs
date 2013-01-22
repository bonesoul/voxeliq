﻿/*
 * Copyright (C) 2011 - 2013 Int6 Studios - http://www.int6.org,
 * Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Linq;
using System.Collections.Generic;

namespace VoxeliqEngine.Common.Randomization
{
    public class RandomHelper
    {
        private readonly static Random _random;

        static RandomHelper()
        {
            _random = new Random();
        }

        public static int Next()
        {
            return _random.Next();
        }

        public static int Next(Int32 maxValue)
        {
            return _random.Next(maxValue);
        }

        public static int Next(Int32 minValue, Int32 maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        public static void NextBytes(byte[] buffer)
        {
            _random.NextBytes(buffer);
        }

        public static double NextDouble()
        {
            return _random.NextDouble();
        }

        /*IEnumerable<TValue>*/
        public static TValue RandomValue<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            List<TValue> values = Enumerable.ToList(dictionary.Values);
            int size = dictionary.Count;
            /*while (true)
            {
                yield return values[_random.Next(size)];
            }*/
            return values[_random.Next(size)];
        }

        /// <summary>
        /// Picks a random item from a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="probability">A function that assigns each item a probability. If the probabilities dont sum up to 1, they are normalized</param>
        /// <returns></returns>
        public static T RandomItem<T>(IEnumerable<T> list, Func<T, float> probability)
        {
            int cumulative = (int)list.Select(x => probability(x)).Sum();

            int randomRoll = RandomHelper.Next(cumulative);
            float cumulativePercentages = 0;

            foreach (T element in list)
            {
                cumulativePercentages += probability(element);
                if (cumulativePercentages > randomRoll)
                    return element;
            }

            return list.First();
        }

    }

    public class ItemRandomHelper
    {
        uint a;
        uint b;
        public ItemRandomHelper(int seed)
        {
            a = (uint)seed;
            b = 666;
        }

        public void ReinitSeed()
        {
            b = 666;
        }

        public uint Next()
        {
            ulong temp = 1791398085UL * (ulong)a + (ulong)b;
            a = (uint)temp;
            b = (uint)(temp >> 32);

            return (uint)a;
        }

        public float Next(float min, float max)
        {
            return min + Next() % (uint)(max - min + 1);
        }
    }
}