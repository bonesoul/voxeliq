/*
 * Copyright (C) 2011 - Hüseyin Uslu shalafiraistlin@gmail.com
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

namespace VolumetricStudios.VoxlrEngine.Utils.Vector
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
            if (obj is Vector2Int) return this.Equals((Vector2Int)obj);
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
