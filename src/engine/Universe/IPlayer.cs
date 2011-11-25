/*
 * Copyright (C) 2011 voxeliq project 
 *
 */

using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqEngine.Utils.Vector;

namespace VolumetricStudios.VoxeliqEngine.Universe
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
