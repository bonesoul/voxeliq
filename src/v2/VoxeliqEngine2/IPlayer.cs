using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using VolumetricStudios.VoxeliqEngine.Movement;

namespace VolumetricStudios.VoxeliqEngine
{
    /// <summary>
    /// Interface for controlling player.
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// The real player position.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// Moves player in given direction.
        /// </summary>
        /// <param name="direction"><see cref="MoveDirection"/></param>
        void Move(MoveDirection direction);
    }
}
