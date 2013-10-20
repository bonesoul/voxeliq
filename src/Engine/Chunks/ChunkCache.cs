/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Engine.Assets;
using Engine.Blocks;
using Engine.Chunks.Generators.Biomes;
using Engine.Chunks.Generators.Terrain;
using Engine.Chunks.Processors;
using Engine.Common.Logging;
using Engine.Common.Vector;
using Engine.Debugging.Timing;
using Engine.Graphics;
using Engine.Universe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Chunks
{
    public interface IChunkCache
    {
        /// <summary>
        /// Returns chunk that exists in given world position.
        /// </summary>
        /// <param name="x">X in world coordinate.</param>
        /// <param name="z">Z in world coordinate</param>
        /// <returns>Returns the <see cref="Chunk"/> that exists in given position or null if otherwise.</returns>
        Chunk GetChunkByWorldPosition(int x, int z);

        /// <summary>
        /// Returns chunk that exists in given relative position.
        /// </summary>
        /// <param name="x">X in relative coordinate.</param>
        /// <param name="z">Z in relative coordinate</param>
        /// <returns>Returns the <see cref="Chunk"/> that exists in given position or null if otherwise.</returns>
        Chunk GetChunkByRelativePosition(int x, int z);

        /// <summary>
        /// Returns the chunk in given neighborhood.
        /// </summary>
        /// <param name="origin">The origin chunk.</param>
        /// <param name="edge">The neighbor edge.</param>
        /// <returns></returns>
        Chunk GetNeighborChunk(Chunk origin, Chunk.Edges edge);

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
    /// The chunk cache that consists of two seperate caches, one for chunks in view range, one for chunks in cache range.
    /// </summary>
    public class ChunkCache : DrawableGameComponent, IChunkCache
    {
        /// <summary>
        /// Range of cached chunk which can be greater than the view range. 
        /// Chunks in cache range will be only generated and lightened.
        /// </summary>
        public static byte CacheRange = Core.Engine.Instance.Configuration.Cache.CacheRange;

        /// <summary>
        /// Range of viewable chunks by the player.
        /// Chunks in view range will be always generated, lightend and built.
        /// </summary>
        public static byte ViewRange = Core.Engine.Instance.Configuration.Cache.ViewRange;

        /// <summary>
        /// Bounding box for view range.
        /// </summary>
        public BoundingBox ViewRangeBoundingBox { get; set; }

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
        public TerrainGenerator Generator { get; set; }

        /// <summary>
        /// The chunk vertex builder.
        /// </summary>
        protected IVertexBuilder VertexBuilder { get; set; }        

        // required services.
        private IChunkStorage _chunkStorage;
        private ICamera _camera;
        private IPlayer _player;
        private IFogger _fogger;
        private IAssetManager _assetManager;
        private TimeRuler _timeRuler;

        public bool CacheThreadStarted { get; private set; }

        public Dictionary<ChunkState, int> StateStatistics { get; private set; }

        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility.

        public ChunkCache(Game game)
            : base(game)
        {
            this.Game.Services.AddService(typeof (IChunkCache), this); // export service.

            if (ViewRange > CacheRange) // check if cache range is big enough to include view-range.
                throw new ChunkCacheException(); 

            this.CacheThreadStarted = false;

            this.StateStatistics = new Dictionary<ChunkState, int> // init. the debug stastics.
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

            this._assetManager = (IAssetManager)this.Game.Services.GetService(typeof(IAssetManager));
            if (this._assetManager == null)
                throw new NullReferenceException("Can not find asset manager component.");

            this.Generator = new BiomedTerrain(new RainForest());
            base.Initialize();
        }

        protected override void LoadContent()
        {
            this._blockEffect = this._assetManager.BlockEffect;
            this._blockTextureAtlas = this._assetManager.BlockTextureAtlas;
            this._crackTextureAtlas = this._assetManager.CrackTextureAtlas;
        }

        /// <summary>
        /// Returns a boolean stating if chunk is current in view range.
        /// </summary>
        /// <param name="chunk">Chunk to check.</param>
        /// <returns><see cref="bool"/></returns>
        public bool IsChunkInViewRange(Chunk chunk)
        {
            return ViewRangeBoundingBox.Contains(chunk.BoundingBox) == ContainmentType.Contains;
        }

        /// <summary>
        /// Returns a boolean stating if chunk is current in cache range.
        /// </summary>
        /// <param name="chunk">Chunk to check.</param>
        /// <returns><see cref="bool"/></returns>
        public bool IsChunkInCacheRange(Chunk chunk)
        {
            return CacheRangeBoundingBox.Contains(chunk.BoundingBox) == ContainmentType.Contains;
        }

        public override void Update(GameTime gameTime)
        {
            this.UpdateBoundingBoxes();

            if (this.CacheThreadStarted) 
                return;

            var cacheThread = new Thread(CacheThread) {IsBackground = true};
            cacheThread.Start();

            this.CacheThreadStarted = true;
        }

        protected void UpdateBoundingBoxes()
        {
            this.ViewRangeBoundingBox = new BoundingBox(
                        new Vector3(this._player.CurrentChunk.WorldPosition.X - (ViewRange*Chunk.WidthInBlocks), 0,
                            this._player.CurrentChunk.WorldPosition.Z - (ViewRange * Chunk.LengthInBlocks)),
                        new Vector3(this._player.CurrentChunk.WorldPosition.X + ((ViewRange + 1)*Chunk.WidthInBlocks),
                            Chunk.HeightInBlocks, this._player.CurrentChunk.WorldPosition.Z + ((ViewRange + 1) * Chunk.LengthInBlocks))
                );

            this.CacheRangeBoundingBox = new BoundingBox(
                        new Vector3(this._player.CurrentChunk.WorldPosition.X - (CacheRange*Chunk.WidthInBlocks), 0,
                            this._player.CurrentChunk.WorldPosition.Z - (CacheRange * Chunk.LengthInBlocks)),
                        new Vector3(this._player.CurrentChunk.WorldPosition.X + ((CacheRange + 1)*Chunk.WidthInBlocks),
                            Chunk.HeightInBlocks,
                            this._player.CurrentChunk.WorldPosition.Z + ((CacheRange + 1) * Chunk.LengthInBlocks))
                );
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
            foreach (var chunk in this._chunkStorage.Values)
            {
                if (this.IsChunkInViewRange(chunk))
                    this.ProcessChunkInViewRange(chunk);
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
            }

            if (Core.Engine.Instance.Configuration.World.IsInfinitive)
                this.RecacheChunks();
        }


        private void RecacheChunks()
        {
            this._player.CurrentChunk = this.GetChunkByWorldPosition((int)_player.Position.X, (int)_player.Position.Z);
            
            if (this._player.CurrentChunk == null)
                return;

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
                    new Vector3(southWestEdge.X * Chunk.WidthInBlocks, 0, southWestEdge.Z * Chunk.LengthInBlocks),
                    new Vector3((northEastEdge.X + 1)*Chunk.WidthInBlocks, Chunk.HeightInBlocks,
                                (northEastEdge.Z + 1) * Chunk.LengthInBlocks));
        }

        /// <summary>
        /// Processes chunks in cache range and generates or lightens them.
        /// </summary>
        /// <param name="chunk"><see cref="Chunk"/></param>
        /// <remarks>Note that chunks in cache range only gets generated or lightened. They are built once they get in view-range.</remarks>
        private void ProcessChunkInCacheRange(Chunk chunk)
        {
            if (chunk.ChunkState != ChunkState.AwaitingGenerate && chunk.ChunkState != ChunkState.AwaitingLighting)
                return; // only generate or lighten the chunks.

            // note: we don't care about chunks that await re-lighting because re-lightig only occurs a chunk gets modified.       

            switch (chunk.ChunkState)
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
            _blockEffect.Parameters["BlockTextureAtlas"].SetValue(_blockTextureAtlas);

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

            this.StateStatistics[ChunkState.AwaitingGenerate] = this._chunkStorage.Values.Count(chunk => chunk.ChunkState == ChunkState.AwaitingGenerate);
            this.StateStatistics[ChunkState.Generating] = this._chunkStorage.Values.Count(chunk => chunk.ChunkState == ChunkState.Generating);
            this.StateStatistics[ChunkState.AwaitingLighting] = this._chunkStorage.Values.Count(chunk => chunk.ChunkState == ChunkState.AwaitingLighting);
            this.StateStatistics[ChunkState.Lighting] = this._chunkStorage.Values.Count(chunk => chunk.ChunkState == ChunkState.Lighting);
            this.StateStatistics[ChunkState.AwaitingRelighting] = this._chunkStorage.Values.Count(chunk => chunk.ChunkState == ChunkState.AwaitingRelighting);
            this.StateStatistics[ChunkState.AwaitingBuild] = this._chunkStorage.Values.Count(chunk => chunk.ChunkState == ChunkState.AwaitingBuild);
            this.StateStatistics[ChunkState.Building] = this._chunkStorage.Values.Count(chunk => chunk.ChunkState == ChunkState.Building);
            this.StateStatistics[ChunkState.AwaitingRebuild] = this._chunkStorage.Values.Count(chunk => chunk.ChunkState == ChunkState.AwaitingRebuild);
            this.StateStatistics[ChunkState.Ready] = this._chunkStorage.Values.Count(chunk => chunk.ChunkState == ChunkState.Ready);
            this.StateStatistics[ChunkState.AwaitingRemoval] = this._chunkStorage.Values.Count(chunk => chunk.ChunkState == ChunkState.AwaitingRemoval);
        }

        /// <summary>
        /// Returns chunk that exists in given world position.
        /// </summary>
        /// <param name="x">X in world coordinate.</param>
        /// <param name="z">Z in world coordinate</param>
        /// <returns>Returns the <see cref="Chunk"/> that exists in given position or null if otherwise.</returns>
        public Chunk GetChunkByWorldPosition(int x, int z)
        {
            // fix the negative x coordinates.
            if (x < 0)
                x -= Chunk.WidthInBlocks;

            // fix the negative z coordinates.
            if (z < 0)
                z -= Chunk.LengthInBlocks;

            return !this._chunkStorage.ContainsKey(x / Chunk.WidthInBlocks, z / Chunk.LengthInBlocks) ? null : this._chunkStorage[x / Chunk.WidthInBlocks, z / Chunk.LengthInBlocks];
        }

        /// <summary>
        /// Returns chunk that exists in given relative position.
        /// </summary>
        /// <param name="x">X in relative coordinate.</param>
        /// <param name="z">Z in relative coordinate</param>
        /// <returns>Returns the <see cref="Chunk"/> that exists in given position or null if otherwise.</returns>
        public Chunk GetChunkByRelativePosition(int x, int z)
        {
            return !this._chunkStorage.ContainsKey(x, z) ? null : this._chunkStorage[x, z];
        }

        /// <summary>
        /// Returns the chunk in given neighborhood.
        /// </summary>
        /// <param name="origin">The origin chunk.</param>
        /// <param name="edge">The neighbor edge.</param>
        /// <returns></returns>
        public Chunk GetNeighborChunk(Chunk origin, Chunk.Edges edge)
        {
            switch (edge)
            {
                case Chunk.Edges.XDecreasing:
                    return this.GetChunkByRelativePosition(origin.RelativePosition.X - 1, origin.RelativePosition.Z);
                case Chunk.Edges.XIncreasing:
                    return this.GetChunkByRelativePosition(origin.RelativePosition.X + 1, origin.RelativePosition.Z);
                case Chunk.Edges.ZDecreasing:
                    return this.GetChunkByRelativePosition(origin.RelativePosition.X, origin.RelativePosition.Z - 1);
                case Chunk.Edges.ZIncreasing:
                    return this.GetChunkByRelativePosition(origin.RelativePosition.X, origin.RelativePosition.Z + 1);
            }
            return null;
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

    public class ChunkCacheException : Exception
    {
        public ChunkCacheException() : base("View range can not be larger than cache range!")
        { }
    }
}