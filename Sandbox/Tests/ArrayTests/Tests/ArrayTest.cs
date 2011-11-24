/*
* Hüseyin Uslu, shalafiraistlin@gmail.com
* This code is provided as is.
* Original code by: http://techcraft.codeplex.com/discussions/247791
*/

using System;

namespace arraytests.Tests
{
    public class ArrayTest
    {
        protected int DimensionSize;

        public ArrayTest(int dimensionSize)
        {
            DimensionSize = dimensionSize;
        }

        public virtual TimeSpan AccessSequental() { return TimeSpan.Zero; }
        public virtual TimeSpan AccessRandom() { return TimeSpan.Zero; }        
    }
}
