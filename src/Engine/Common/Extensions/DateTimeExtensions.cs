/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;

namespace VoxeliqEngine.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static uint ToUnixTime(this DateTime time)
        {
            return (uint)((time.ToUniversalTime().Ticks - 621355968000000000L) / 10000000L);
        }

        public static ulong ToExtendedEpoch(this DateTime time)
        {
            return (ulong)((time.ToUniversalTime().Ticks - 621355968000000000L) / 10L);
        }
    }
}