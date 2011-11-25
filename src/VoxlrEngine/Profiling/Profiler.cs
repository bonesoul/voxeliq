/*
 * Copyright (C) 2011 voxlr project 
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VolumetricStudios.VoxlrEngine.Profiling
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
