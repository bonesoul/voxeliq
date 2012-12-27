/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxeliqEngine.Assets;
using VoxeliqEngine.Blocks;
using VoxeliqEngine.Chunks.Generators.Biomes;
using VoxeliqEngine.Chunks.Generators.Terrain;
using VoxeliqEngine.Chunks.Processors;
using VoxeliqEngine.Debugging;
using VoxeliqEngine.Debugging.Profiling;
using VoxeliqEngine.Engine;
using VoxeliqEngine.Graphics;
using VoxeliqEngine.Logging;
using VoxeliqEngine.Universe;
using VoxeliqEngine.Utils.Vector;

namespace VoxeliqEngine.Chunks
{
    public interface IChunkCache
    {
        /// <summary>
        /// Returns the chunk in given x-z position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        Chunk GetChunk(int x, int z);

        /// <summary>
        /// Sets a block in given x-y-z coordinate.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="block"></param>
        void SetBlockAt(int x, int y, int z, Block block);

        /// <summary>
        /// Sets a block in given x-y-z coordinate.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="block"></param>
        void SetBlockAt(Vector3Int position, Block block);

        /// <summary>
        /// Returns chunks drawn in last draw() call.
        /// </summary>
        int ChunksDrawn { get; }

        /// <summary>
        /// Returns true if given chunk is in view range.
        /// </summary>
        /// <param name="chunk"></param>
        /// <returns></returns>
        bool IsChunkInViewRange(Chunk chunk);

        /// <summary>
        /// Returns true if given chunk is in cache range.
        /// </summary>
        /// <param name="chunk"></param>
        /// <returns></returns>
        bool IsChunkInCacheRange(Chunk chunk);

        Dictionary<ChunkState, int> StateStatistics { get; }
    }

    /// <summary>
    /// The chunk cache & manager.
    /// </summary>
    public class ChunkCache : DrawableGameComponent, IChunkCache
    {
        /// <summary>
        /// Range of viewable chunks.
        /// </summary>
        public const byte ViewRange = 10;

        /// <summary>
        /// Bounding box for view range.
        /// </summary>
        public BoundingBox ViewRangeBoundingBox { get; set; }

        /// <summary>
        /// Chunk range cache.
        /// </summary>
        public const byte CacheRange = 10;

        /// <summary>
        /// Bounding box for cache range.
        /// </summary>
        public BoundingBox CacheRangeBoundingBox { get; set; }

        /// <summary>
        /// Chunks drawn statistics.
        /// </summary>
        public int ChunksDrawn { get; protected set; }

        // assets & resources
        private Effect _blockEffect; // block effect.
        private Texture2D _blockTextureAtlas; // block texture atlas
        private Texture2D _crackTextureAtlas; // crack texture atlas // TODO: implement crack textures!

        /// <summary>
        /// Bounding box for chunk cache.
        /// </summary>
        public static BoundingBox BoundingBox { get; set; }

        /// <summary>
        /// The terrain generator.
        /// </summary>
        protected TerrainGenerator Generator { get; set; }

        /// <summary>
        /// The chunk vertex builder.
        /// </summary>
        protected IVertexBuilder VertexBuilder { get; set; }        

        // required services.
        private IChunkStorage _chunkStorage;
        private ICamera _camera;
        private IPlayer _player;
        private IFogger _fogger;
        private TimeRuler _timeRuler;

        public bool CacheThreadStarted { get; private set; }

        public Dictionary<ChunkState, int> StateStatistics { get; private set; }

        // misc.
        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility.

        public ChunkCache(Game game)
            : base(game)
        {
            this.Game.Services.AddService(typeof (IChunkCache), this); // export service.

            this.CacheThreadStarted = false;

            this.StateStatistics = new Dictionary<ChunkState, int>
                                       {
                                           {ChunkState.AwaitingGenerate, 0},
                                           {ChunkState.Generating, 0},
                                           {ChunkState.AwaitingLighting, 0},
                                           {ChunkState.Lighting, 0},
                                           {ChunkState.AwaitingBuild, 0},
                                           {ChunkState.Building, 0},
                                           {ChunkState.Ready, 0},
                                           {ChunkState.AwaitingRelighting, 0},
                                           {ChunkState.AwaitingRebuild, 0},
                                           {ChunkState.AwaitingRemoval, 0},
                                       };
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            // import required services.
            this._chunkStorage = (IChunkStorage) this.Game.Services.GetService(typeof (IChunkStorage));
            this._camera = (ICamera) this.Game.Services.GetService(typeof (ICamera));
            this._player = (IPlayer) this.Game.Services.GetService(typeof (IPlayer));
            this._fogger = (IFogger) this.Game.Services.GetService(typeof (IFogger));
            this.VertexBuilder = (IVertexBuilder) this.Game.Services.GetService(typeof (IVertexBuilder));
            this._timeRuler = (TimeRuler) this.Game.Services.GetService(typeof (TimeRuler));

            this.Generator = new BiomedTerrain(new RainForest());
            base.Initialize();
        }

        protected override void LoadContent()
        {
            this._blockEffect = AssetManager.Instance.BlockEffect;
            this._blockTextureAtlas = AssetManager.Instance.BlockTextureAtlas;
            this._crackTextureAtlas = AssetManager.Instance.CrackTextureAtlas;
        }

        public bool IsChunkInViewRange(Chunk chunk)
        {
            return ViewRangeBoundingBox.Contains(chunk.BoundingBox) == ContainmentType.Contains;
        }

        public bool IsChunkInCacheRange(Chunk chunk)
        {
            return CacheRangeBoundingBox.Contains(chunk.BoundingBox) == ContainmentType.Contains;
        }

        public override void Update(GameTime gameTime)
        {
            this.ViewRangeBoundingBox = new BoundingBox(
                new Vector3(this._player.CurrentChunk.WorldPosition.X - (ViewRange*Chunk.WidthInBlocks), 0,
                            this._player.CurrentChunk.WorldPosition.Z - (ViewRange*Chunk.LenghtInBlocks)),
                new Vector3(this._player.CurrentChunk.WorldPosition.X + ((ViewRange + 1)*Chunk.WidthInBlocks),
                            Chunk.HeightInBlocks,
                            this._player.CurrentChunk.WorldPosition.Z + ((ViewRange + 1)*Chunk.LenghtInBlocks))
                );

            this.CacheRangeBoundingBox = new BoundingBox(
                new Vector3(this._player.CurrentChunk.WorldPosition.X - (CacheRange*Chunk.WidthInBlocks), 0,
                            this._player.CurrentChunk.WorldPosition.Z - (CacheRange*Chunk.LenghtInBlocks)),
                new Vector3(this._player.CurrentChunk.WorldPosition.X + ((CacheRange + 1)*Chunk.WidthInBlocks),
                            Chunk.HeightInBlocks,
                            this._player.CurrentChunk.WorldPosition.Z + ((CacheRange + 1)*Chunk.LenghtInBlocks))
                );

                if (!this.CacheThreadStarted)
                {
                    var cacheThread = new Thread(CacheThread) {IsBackground = true};
                    cacheThread.Start();

                    this.CacheThreadStarted = true;
                }

        }

        private void CacheThread()
        {
            while (true)
            {
                if (this._player.CurrentChunk == null)
                    continue;

                this.Process();
            }
        }

        protected void Process()
        {
            this.StateStatistics[ChunkState.AwaitingGenerate] = 0;
            this.StateStatistics[ChunkState.Generating] = 0;
            this.StateStatistics[ChunkState.AwaitingLighting] = 0;
            this.StateStatistics[ChunkState.Lighting] = 0;
            this.StateStatistics[ChunkState.AwaitingBuild] = 0;
            this.StateStatistics[ChunkState.Building] = 0;
            this.StateStatistics[ChunkState.Ready] = 0;
            this.StateStatistics[ChunkState.AwaitingRelighting] = 0;
            this.StateStatistics[ChunkState.AwaitingRebuild] = 0;
            this.StateStatistics[ChunkState.AwaitingRemoval] = 0;


            Profiler.Start("chunk-cache-loop");
            //this._timeRuler.BeginMark(1,"Chunk Cache", Color.Green);
            foreach (var chunk in this._chunkStorage.Values)
            {
                if (this.IsChunkInViewRange(chunk))
                {
                    this.ProcessChunkInViewRange(chunk);
                }
                else
                {
                    if (this.IsChunkInCacheRange(chunk))
                        this.ProcessChunkInCacheRange(chunk);
                    else
                    {
                        chunk.ChunkState = ChunkState.AwaitingRemoval;
                        this._chunkStorage.Remove(chunk.RelativePosition.X, chunk.RelativePosition.Z);
                        chunk.Dispose();
                    }
                }

                this.StateStatistics[chunk.ChunkState]++;
            }
            //this._timeRuler.EndMark(1, "Chunk Cache");
            Profiler.Stop("chunk-cache-loop");

            //if (Profiler.Timers["chunk-cache-loop"].ElapsedMilliseconds > 10)
                //Console.WriteLine("chunk-cache-loop:" + Profiler.Timers["chunk-cache-loop"].ElapsedMilliseconds);

            if (Settings.World.IsInfinitive)
                this.RecacheChunks();
        }


        private void RecacheChunks()
        {
            this._player.CurrentChunk = this.GetChunk((int) _player.Position.X, (int) _player.Position.Z);

            for (int z = -CacheRange; z <= CacheRange; z++)
            {
                for (int x = -CacheRange; x <= CacheRange; x++)
                {
                    if (this._chunkStorage.ContainsKey(this._player.CurrentChunk.RelativePosition.X + x, this._player.CurrentChunk.RelativePosition.Z + z))
                        continue;

                    var chunk = new Chunk(new Vector2Int(this._player.CurrentChunk.RelativePosition.X + x, this._player.CurrentChunk.RelativePosition.Z + z));
                    this._chunkStorage[chunk.RelativePosition.X, chunk.RelativePosition.Z] = chunk;
                }
            }

            var southWestEdge = new Vector2Int(this._player.CurrentChunk.RelativePosition.X - ViewRange, this._player.CurrentChunk.RelativePosition.Z - ViewRange);
            var northEastEdge = new Vector2Int(this._player.CurrentChunk.RelativePosition.X + ViewRange, this._player.CurrentChunk.RelativePosition.Z + ViewRange);

            BoundingBox = new BoundingBox(
                    new Vector3(southWestEdge.X*Chunk.WidthInBlocks, 0, southWestEdge.Z*Chunk.LenghtInBlocks),
                    new Vector3((northEastEdge.X + 1)*Chunk.WidthInBlocks, Chunk.HeightInBlocks,
                                (northEastEdge.Z + 1)*Chunk.LenghtInBlocks));
        }

        private void ProcessChunkInCacheRange(Chunk chunk)
        {
            if (chunk.ChunkState == ChunkState.Ready || chunk.ChunkState != ChunkState.AwaitingGenerate)
                return;

            switch (chunk.ChunkState) // switch on the chunk state.
            {
                case ChunkState.AwaitingGenerate:
                    Generator.Generate(chunk);
                    break;
                case ChunkState.AwaitingLighting:
                    Lightning.Process(chunk);
                    break;
                default:
                    break;
            }
        }

        private void ProcessChunkInViewRange(Chunk chunk)
        {
            if (chunk.ChunkState == ChunkState.Ready || chunk.ChunkState == ChunkState.AwaitingRemoval)
                return;

            switch (chunk.ChunkState) // switch on the chunk state.
            {
                case ChunkState.AwaitingGenerate:
                    Generator.Generate(chunk);
                    break;
                case ChunkState.AwaitingLighting:
                case ChunkState.AwaitingRelighting:
                    Lightning.Process(chunk);
                    break;
                case ChunkState.AwaitingBuild:
                case ChunkState.AwaitingRebuild:
                    this.VertexBuilder.Build(chunk);
                    break;
                default:
                    break;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var viewFrustrum = new BoundingFrustum(this._camera.View*this._camera.Projection);

            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game.GraphicsDevice.BlendState = BlendState.Opaque;

            // general parameters
            _blockEffect.Parameters["World"].SetValue(Matrix.Identity);
            _blockEffect.Parameters["View"].SetValue(this._camera.View);
            _blockEffect.Parameters["Projection"].SetValue(this._camera.Projection);
            _blockEffect.Parameters["CameraPosition"].SetValue(this._camera.Position);

            // texture parameters
#if XNA
            _blockEffect.Parameters["BlockTextureAtlas"].SetValue(_blockTextureAtlas);
#elif MONOGAME
            _blockEffect.Parameters["BlockTextureAtlasSampler"].SetValue(_blockTextureAtlas);
#endif

            // atmospheric settings
            _blockEffect.Parameters["SunColor"].SetValue(World.SunColor);
            _blockEffect.Parameters["NightColor"].SetValue(World.NightColor);
            _blockEffect.Parameters["HorizonColor"].SetValue(World.HorizonColor);
            _blockEffect.Parameters["MorningTint"].SetValue(World.MorningTint);
            _blockEffect.Parameters["EveningTint"].SetValue(World.EveningTint);

            // time of day parameters
            _blockEffect.Parameters["TimeOfDay"].SetValue(Time.GetGameTimeOfDay());

            // fog parameters
            _blockEffect.Parameters["FogNear"].SetValue(this._fogger.FogVector.X);
            _blockEffect.Parameters["FogFar"].SetValue(this._fogger.FogVector.Y);


            this.ChunksDrawn = 0;
            foreach (EffectPass pass in this._blockEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                foreach (Chunk chunk in this._chunkStorage.Values)
                {
                    if (chunk.IndexBuffer == null || chunk.VertexBuffer == null)
                        continue;

                    if (chunk.VertexBuffer.VertexCount == 0)
                        continue;

                    if (chunk.IndexBuffer.IndexCount == 0)
                        continue;

                    if (!IsChunkInViewRange(chunk))
                        continue;

                    if (!chunk.BoundingBox.Intersects(viewFrustrum)) // if chunk is not in view frustrum,
                        continue; // pas it.

                    Game.GraphicsDevice.SetVertexBuffer(chunk.VertexBuffer);
                    Game.GraphicsDevice.Indices = chunk.IndexBuffer;
                    Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, chunk.VertexBuffer.VertexCount, 0, chunk.IndexBuffer.IndexCount/3);

                    this.ChunksDrawn++;
                }
            }
        }

        // Returns the chunk in given x-z position.
        public Chunk GetChunk(int x, int z)
        {
            return !this._chunkStorage.ContainsKey(x/Chunk.WidthInBlocks, z/Chunk.LenghtInBlocks) ? null : this._chunkStorage[x/Chunk.WidthInBlocks, z/Chunk.LenghtInBlocks];
        }

        // Sets a block in given x-y-z coordinate.
        public void SetBlockAt(Vector3Int position, Block block)
        {
            this.SetBlockAt(position.X, position.Y, position.Z, block);
        }

        // Sets a block in given x-y-z coordinate.
        public void SetBlockAt(int x, int y, int z, Block block)
        {
            var chunk = this.GetChunk(x, z);
            if (chunk == null)
                return;

            chunk.FastSetBlockAt((byte) (x%Chunk.WidthInBlocks), (byte) y, (byte) (z%Chunk.LenghtInBlocks), block); // use FastSetBlock as we already do bounds check by finding the chunk block is owned by.
        }

        /// <summary>
        /// Check if given x, y and z coordinates are in bounds of chunk cache.
        /// </summary>
        /// <param name="x">The x coordinate to check.</param>
        /// <param name="y">The y coordinate to check.</param>
        /// <param name="z">The z coordinate to check.</param>
        /// <returns>True if given point/block is in bounds of chunk-cache.</returns>
        /// <remarks>Prefer this method instead of BoundingBox.Contains as blocks need special handling!</remarks>
        public static bool IsInBounds(int x, int y, int z)
        {
            if (x < BoundingBox.Min.X || z < BoundingBox.Min.Z || x >= BoundingBox.Max.X ||
                z >= BoundingBox.Max.Z || y < BoundingBox.Min.Y || y >= BoundingBox.Max.Y)                 
                return false;

            return true;
        }
    }
}