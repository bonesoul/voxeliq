/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;

namespace Engine.Universe
{
    /// <summary>
    /// Processes universe time.
    /// </summary>
    public static class Time
    {
        private const float RealTimeDivisor = 24*60*60;
        private const float GameHourInRealMinutes = (float) 2/60;
        private const float GameHourInRealSeconds = GameHourInRealMinutes*60;

        public static float GetRealTimeOfDay()
        {
            return (float) (DateTime.Now.TimeOfDay.TotalSeconds*24)/RealTimeDivisor;
        }

        public static float GetGameTimeOfDay()
        {
            //return (float)((DateTime.Now.TimeOfDay.TotalSeconds / GameHourInRealSeconds) % 24); // quick demonstration of day & night cycles.
            return 12; // this disables the day & night cycle.
        }
    }
}