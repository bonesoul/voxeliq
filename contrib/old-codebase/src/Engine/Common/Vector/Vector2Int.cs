/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

namespace Engine.Common.Vector
{
    public struct Vector2Int
    {
        public int X;
        public int Z;

        public Vector2Int(int x, int z)
        {
            this.X = x;
            this.Z = z;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2Int) return this.Equals((Vector2Int) obj);
            else return false;
        }

        public bool Equals(Vector2Int other)
        {
            return ((this.X == other.X) && (this.Z == other.Z));
        }

        public static bool operator ==(Vector2Int value1, Vector2Int value2)
        {
            return ((value1.X == value2.X) && (value1.Z == value2.Z));
        }

        public static bool operator !=(Vector2Int value1, Vector2Int value2)
        {
            if (value1.X == value2.X) return value1.Z != value2.Z;
            return true;
        }

        public override int GetHashCode()
        {
            return (this.X.GetHashCode() + this.Z.GetHashCode());
        }

        public override string ToString()
        {
            return string.Format("{{X:{0} Z:{1}}}", this.X, this.Z);
        }
    }
}