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

using Microsoft.Xna.Framework;

namespace VolumetricStudios.VoxlrEngine.Utils.Vector
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
            this.X = (int)vector.X;
            this.Y = (int)vector.Y;
            this.Z = (int)vector.Z;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector3Int) return this.Equals((Vector3Int)obj);
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
