/*
 * Copyright (C) 2011 voxeliq project 
 *
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VolumetricStudios.VoxeliqEngine.Profiling
{
    /// <summary>
    /// Provides methods for profiling.
    /// </summary>
    public static class Profiler
    {
        public static readonly Dictionary<string, Stopwatch> Timers;

        static Profiler()
        {
            Timers = new Dictionary<string, Stopwatch>();
        }

        public static void Start(string key)
        {
            if (!Timers.ContainsKey(key)) Timers.Add(key, new Stopwatch());
            else Timers[key].Restart();
            Timers[key].Start();
        }

        public static TimeSpan Stop(string key)
        {
            if (!Timers.ContainsKey(key)) return TimeSpan.Zero;
            Timers[key].Stop();
            return Timers[key].Elapsed;
        }
    }
}
