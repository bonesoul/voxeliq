/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;

namespace Engine.Common.Vector
{
    public struct Vector4Byte
    {
        public byte X;
        public byte Y;
        public byte Z;
        public byte W;

        public Vector4Byte(byte x, byte y, byte z, byte w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public Vector4Byte(byte x, byte y, byte z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = 1;
        }

        public Vector4Byte(int x, int y, int z)
        {
            this.X = (byte) x;
            this.Y = (byte) y;
            this.Z = (byte) z;
            this.W = 1;
        }

        public Vector4Byte(byte value)
        {
            this.X = this.Y = this.Z = value;
            this.W = 1;
        }

        #region Operators

        public static bool operator ==(Vector4Byte left, Vector4Byte right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector4Byte left, Vector4Byte right)
        {
            return !left.Equals(right);
        }

        public static Vector4Byte operator +(Vector4Byte a, Vector4Byte b)
        {
            Vector4Byte result;

            result.X = (byte) (a.X + b.X);
            result.Y = (byte) (a.Y + b.Y);
            result.Z = (byte) (a.Z + b.Z);
            result.W = 1;

            return result;
        }

        public static Vector4Byte operator -(Vector4Byte a, Vector4Byte b)
        {
            Vector4Byte result;

            result.X = (byte) (a.X - b.X);
            result.Y = (byte) (a.Y - b.Y);
            result.Z = (byte) (a.Z - b.Z);
            result.W = 1;

            return result;
        }

        public static Vector4Byte operator *(Vector4Byte a, Vector4Byte b)
        {
            Vector4Byte result;

            result.X = (byte) (a.X*b.X);
            result.Y = (byte) (a.Y*b.Y);
            result.Z = (byte) (a.Z*b.Z);
            result.W = 1;

            return result;
        }

        public static Vector4Byte operator /(Vector4Byte a, Vector4Byte b)
        {
            Vector4Byte result;

            result.X = (byte) (a.X/b.X);
            result.Y = (byte) (a.Y/b.Y);
            result.Z = (byte) (a.Z/b.Z);
            result.W = 1;

            return result;
        }

        #endregion

        public bool Equals(Vector4Byte other)
        {
            return (((this.X == other.X) && (this.Y == other.Y)) &&
                    (this.Z == other.Z) && (this.W == other.W));
        }

        public override bool Equals(object obj)
        {
            bool flag = false;

            if (obj is Vector4Byte)
            {
                flag = this.Equals((Vector4Byte) obj);
            }

            return flag;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.X.GetHashCode() + this.Y.GetHashCode() +
                       this.Z.GetHashCode() + this.W.GetHashCode();
            }
        }

        public override string ToString()
        {
            return String.Format("{{X:{0} Y:{1} Z:{2} W:{3}}}", X, Y, Z, W);
        }
    }
}