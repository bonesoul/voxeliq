﻿/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

namespace VoxeliqEngine.Common.Extensions
{
    public static class NumberExtensions
    {
        public static string GetKiloString(this int value)
        {
            int i;
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            double dblSByte = 0;
            for (i = 0; (int)(value / 1024) > 0; i++, value /= 1024) dblSByte = value / 1024.0;
            return dblSByte.ToString("0.00") + suffixes[i];
        }

        public static string GetKiloString(this long value)
        {
            int i;
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            double dblSByte = 0;
            for (i = 0; (int)(value / 1024) > 0; i++, value /= 1024) dblSByte = value / 1024.0;
            return dblSByte.ToString("0.00") + suffixes[i];
        }
    }
}
