using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace VolumetricStudios.VoxeliqEngine.Utility.Math
{
    public static class Vectors
    {
        public static readonly Vector3 UpVector = new Vector3(0f, 1f, 0f);
        public static readonly Vector3 DownVector = new Vector3(0f, -1f, 0f);
        public static readonly Vector3 RightVector = new Vector3(1f, 0f, 0f);
        public static readonly Vector3 LeftVector = new Vector3(-1f, 0f, 0f);
        public static readonly Vector3 ForwardVector = new Vector3(0f, 0f, -1f);
        public static readonly Vector3 BackwardVector = new Vector3(0f, 0f, 1f);
    }
}
