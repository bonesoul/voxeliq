/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqGame.Chunks;
using VolumetricStudios.VoxeliqGame.Chunks.Builders;
using VolumetricStudios.VoxeliqGame.Common.Logging;
using VolumetricStudios.VoxeliqGame.Graphics;
using VolumetricStudios.VoxeliqGame.Utils.Vector;

namespace VolumetricStudios.VoxeliqGame.Universe
{
    /// <summary>
    /// World service interface.
    /// </summary>
    public interface IWorld
    { }

    /// <summary>
    /// Statistics interface.
    /// </summary>
    public interface IWorldStatisticsService
    {
        int GenerationQueueCount { get; }
        int BuildingQueueCount { get; }
    }

    /// <summary>
    /// World.
    /// </summary>
    public class World : DrawableGameComponent, IWorld, IWorldStatisticsService
    {
        public ChunkStorage Chunks { get; set; } // chunk storage.
        private readonly ChunkCache _chunkCache;// chunk cache.

        public ChunkBuilder ChunkBuilder { get; protected set; } // Chunk builder.       

        public int GenerationQueueCount { get { return this.ChunkBuilder.GenerationQueueCount; } } // Generation queue count.
        public int BuildingQueueCount { get { return this.ChunkBuilder.BuildingQueueCount; } } // Building queue count.

        // required services.
        private ICameraControlService _cameraController;
        private IPlayer _player;


        #region Atmospheric Settings

            public static Vector4 NightColor = Color.Black.ToVector4();
            public static Vector4 SunColor = Color.White.ToVector4();
            public static Vector4 HorizonColor = Color.White.ToVector4();

            public Vector4 EveningTint = Color.Red.ToVector4();
            public Vector4 MorningTint = Color.Gold.ToVector4();

        #endregion

        // misc.
        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility.

        /// <summary>
        /// Creates a new world.
        /// </summary>
        /// <param name="game"> </param>
        /// <param name="chunkStorage"> </param>
        /// <param name="chunkCache"> </param>
        public World(Game game,ChunkStorage chunkStorage, ChunkCache chunkCache)
            :base(game)
        {
            this.Chunks = chunkStorage;
            this._chunkCache = chunkCache;
             
            // export services.
            this.Game.Services.AddService(typeof(IWorldStatisticsService), this);
            this.Game.Services.AddService(typeof(IWorld), this);
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            // import required services.
            this._cameraController = (ICameraControlService)this.Game.Services.GetService(typeof(ICameraControlService));
            this._player = (IPlayer)this.Game.Services.GetService(typeof(IPlayer));           

            this.ChunkBuilder = new QueuedBuilder(this.Game, this._player, this); // the chunk builder.        
            this.Game.Components.Add(this.ChunkBuilder);

            this._cameraController.LookAt(Vector3.Down);
            this._player.SpawnPlayer(new Vector2Int(1000, 1000));

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

            this.Chunks.SouthWestEdge = new Vector2Int(relativePosition.X - ChunkCache.ViewRange, relativePosition.Z - ChunkCache.ViewRange);
            this.Chunks.NorthEastEdge = new Vector2Int(relativePosition.X + ChunkCache.ViewRange, relativePosition.Z + ChunkCache.ViewRange);

            this._chunkCache.BoundingBox = new BoundingBox(new Vector3(this.Chunks.SouthWestEdge.X * Chunk.WidthInBlocks, 0, this.Chunks.SouthWestEdge.Z * Chunk.LenghtInBlocks), new Vector3((this.Chunks.NorthEastEdge.X + 1) * Chunk.WidthInBlocks, Chunk.HeightInBlocks, (this.Chunks.NorthEastEdge.Z + 1) * Chunk.LenghtInBlocks));
        }
    }
}
