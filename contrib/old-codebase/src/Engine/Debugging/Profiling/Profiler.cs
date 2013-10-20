/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine.Debugging.Profiling
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