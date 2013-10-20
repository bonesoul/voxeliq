/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System.Collections.Generic;

namespace Engine.Common.Extensions
{
    public static class ListExtensions
    {
        public static bool ContainsAtLeastOne<T>(this List<T> list1, List<T> list2)
        {
            foreach (T m in list2)
            {
                if (list1.Contains(m))
                    return true;
            }
            return false;
        }

        public static bool ContainsAtLeastOne<T>(this List<T> list, T[] array)
        {
            foreach (T m in array)
            {
                if (list.Contains(m))
                    return true;
            }
            return false;
        }
    }
}