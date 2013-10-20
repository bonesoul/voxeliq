/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Engine.Chunks;
using Engine.Common.Logging;
using Engine.Common.Vector;
using Engine.Graphics;
using Microsoft.Xna.Framework;

namespace Engine.Universe
{
    /// <summary>
    /// World service interface.
    /// </summary>
    public interface IWorld
    { }

    /// <summary>
    /// World.
    /// </summary>
    public class World : DrawableGameComponent, IWorld
    {
        public ChunkStorage Chunks { get; set; } // chunk storage.
        private readonly ChunkCache _chunkCache; // chunk cache.

        // required services.
        private ICameraControlService _cameraController;
        private IPlayer _player;

        #region Atmospheric Settings

        public static Vector4 NightColor = Color.Black.ToVector4();
        public static Vector4 SunColor = Color.White.ToVector4();
        public static Vector4 HorizonColor = Color.DarkGray.ToVector4();

        public static Vector4 EveningTint = Color.Red.ToVector4();
        public static Vector4 MorningTint = Color.Gold.ToVector4();

        #endregion

        // misc.
        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility.

        /// <summary>
        /// Creates a new world.
        /// </summary>
        /// <param name="game"> </param>
        /// <param name="chunkStorage"> </param>
        /// <param name="chunkCache"> </param>
        public World(Game game, ChunkStorage chunkStorage, ChunkCache chunkCache)
            : base(game)
        {
            this.Chunks = chunkStorage;
            this._chunkCache = chunkCache;

            // export services.
            this.Game.Services.AddService(typeof (IWorld), this);
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            // import required services.
            this._cameraController = (ICameraControlService) this.Game.Services.GetService(typeof (ICameraControlService));
            this._player = (IPlayer) this.Game.Services.GetService(typeof (IPlayer));

            this._cameraController.LookAt(Vector3.Down);
            this._player.SpawnPlayer(new Vector2Int(0, 0));

            base.Initialize();
        }

        public void SpawnPlayer(Vector2Int relativePosition)
        {
            //Profiler.Start("terrain-generation");
            for (int z = -ChunkCache.CacheRange; z <= ChunkCache.CacheRange; z++)
            {
                for (int x = -ChunkCache.CacheRange; x <= ChunkCache.CacheRange; x++)
                {
                    var chunk = new Chunk(new Vector2Int(relativePosition.X + x, relativePosition.Z + z));
                    this.Chunks[chunk.RelativePosition.X, chunk.RelativePosition.Z] = chunk;

                    if (chunk.RelativePosition == relativePosition) this._player.CurrentChunk = chunk;
                }
            }

            this.Chunks.SouthWestEdge = new Vector2Int(relativePosition.X - ChunkCache.ViewRange,
                                                       relativePosition.Z - ChunkCache.ViewRange);
            this.Chunks.NorthEastEdge = new Vector2Int(relativePosition.X + ChunkCache.ViewRange,
                                                       relativePosition.Z + ChunkCache.ViewRange);

            ChunkCache.BoundingBox =
                new BoundingBox(
                    new Vector3(this.Chunks.SouthWestEdge.X*Chunk.WidthInBlocks, 0, this.Chunks.SouthWestEdge.Z*Chunk.LenghtInBlocks),
                    new Vector3((this.Chunks.NorthEastEdge.X + 1)*Chunk.WidthInBlocks, Chunk.HeightInBlocks, (this.Chunks.NorthEastEdge.Z + 1)*Chunk.LenghtInBlocks));
        }
    }
}