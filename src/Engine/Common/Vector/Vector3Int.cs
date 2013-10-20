/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Microsoft.Xna.Framework;

namespace Engine.Common.Vector
{
    /// <summary>
    /// Basic Vector3 class that takes integer values that can be used on coordinate values within the world.
    /// Based on Microsoft.XNA.Framework.Vector3
    /// It's based on value-types.
    /// </summary>
    public struct Vector3Int
    {
        public int X;
        public int Y;
        public int Z;

        public Vector3Int(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector3Int(Vector3 vector)
        {
            this.X = (int) vector.X;
            this.Y = (int) vector.Y;
            this.Z = (int) vector.Z;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector3Int) return this.Equals((Vector3Int) obj);
            else return false;
        }

        public bool Equals(Vector3Int other)
        {
            return (((this.X == other.X) && (this.Y == other.Y)) && (this.Z == other.Z));
        }

        public static bool operator ==(Vector3Int value1, Vector3Int value2)
        {
            return (((value1.X == value2.X) && (value1.Y == value2.Y)) && (value1.Z == value2.Z));
        }

        public static bool operator !=(Vector3Int value1, Vector3Int value2)
        {
            if ((value1.X == value2.X) && (value1.Y == value2.Y)) return value1.Z != value2.Z;
            return true;
        }

        public override int GetHashCode()
        {
            return ((this.X.GetHashCode() + this.Y.GetHashCode()) + this.Z.GetHashCode());
        }

        public override string ToString()
        {
            return string.Format("{{X:{0} Y:{1} Z:{2}}}", this.X, this.Y, this.Z);
        }

        public Vector3 AsVector3()
        {
            return new Vector3(X, Y, Z);
        }
    }
}