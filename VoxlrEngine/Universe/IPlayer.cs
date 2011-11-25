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
using VolumetricStudios.VoxlrEngine.Utils.Vector;

namespace VolumetricStudios.VoxlrEngine.Universe
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
        /// Last chunk the player was on.
        /// </summary>
        Chunk LastChunk { get; set; }

        /// <summary>
        /// The current chunk the player is in.
        /// </summary>
        Chunk CurrentChunk { get; set; }

        /// <summary>
        /// The weapon player is wielding.
        /// </summary>
        Weapon Weapon { get; set; }

        PositionedBlock? AimedSolidBlock { get; }

        PositionedBlock? AimedEmptyBlock { get; }

        /// <summary>
        /// Moves camera in given direction.
        /// </summary>
        /// <param name="direction"><see cref="MoveDirection"/></param>
        /// <param name="useElevationandRotatin"></param>
        void Move(GameTime gameTime, MoveDirection direction);

        /// <summary>
        /// Let's the player jump.
        /// </summary>
        void Jump();

        /// <summary>
        /// Sets camera position.
        /// </summary>
        /// <param name="position">The position in Vector3.</param>
        void SpawnPlayer(Vector2Int position);

        /// <summary>
        /// Is flying enabled?
        /// </summary>
        /// <returns></returns>
        bool FlyingEnabled { get; }

        /// <summary>
        /// Toggles fly form.
        /// </summary>
        void ToggleFlyForm();
    }

    public enum MoveDirection
    {
        Forward,
        Backward,
        Left,
        Right,
    }
}
