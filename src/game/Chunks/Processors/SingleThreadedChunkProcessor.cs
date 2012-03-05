/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqGame.Generators.Biomes;
using VolumetricStudios.VoxeliqGame.Generators.Terrain;
using VolumetricStudios.VoxeliqGame.Universe;
using VolumetricStudios.VoxeliqGame.Utils.Vector;

namespace VolumetricStudios.VoxeliqGame.Chunks.Processors
{
    /// <summary>
    /// Single threaded chunk processor which processes (generation, lightning, building) in a single thread.
    /// </summary>
    public class SingleThreadedChunkProcessor:ChunkProcessor
    {
        /// <summary>
        /// Creates a new SingleThreadProcessor.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="world"></param>
        public SingleThreadedChunkProcessor(Game game, World world) 
            : base(game, world)
        {
            this.Generator = new ValleyTerrain(new RainForest()); 
        }

        public override void Initialize()
        {
            Logger.Trace("init()");
            base.Initialize();
        }

        protected override void Run()
        {
            var processorThread = new Thread(ProcessorThread) { IsBackground = true };
            processorThread.Start();
        }

        private void ProcessorThread()
        {
            while (this.Active)
            {
                this.Process();
            }            
        }

        protected void Process()
        {
            foreach (var chunk in this.World.Chunks.Values)
            {
                if (chunk.ChunkState == ChunkState.Ready || chunk.ChunkState == ChunkState.AwaitingRemoval)
                    continue;

                if(this.ChunkCache.IsChunkInViewRange(chunk))
                {
                    this.ProcessChunkInViewRange(chunk);
                }
                else
                {
                    if (this.ChunkCache.IsChunkInCacheRange(chunk))
                        this.ProcessChunkInCacheRange(chunk);
                    else
                    {
                        chunk.ChunkState = ChunkState.AwaitingRemoval;
                        this.ChunkStorage.Remove(chunk.RelativePosition.X, chunk.RelativePosition.Z);
                        chunk.Dispose();
                    }
                }                
            }

            if(this.ChunkCache.IsInfinitive)
                this.RecacheChunks();
        }

        private void RecacheChunks()
        {
            this.Player.CurrentChunk = this.ChunkCache.GetChunk((int)Player.Position.X, (int)Player.Position.Z);  

            for (int z = -Chunks.ChunkCache.CacheRange; z <= Chunks.ChunkCache.CacheRange; z++)
            {
                for (int x = -Chunks.ChunkCache.CacheRange; x <= Chunks.ChunkCache.CacheRange; x++)
                {
                    if (this.ChunkStorage.ContainsKey(this.Player.CurrentChunk.RelativePosition.X + x, this.Player.CurrentChunk.RelativePosition.Z + z))
                        continue;

                    var chunk = new Chunk(this.World, new Vector2Int(this.Player.CurrentChunk.RelativePosition.X + x, this.Player.CurrentChunk.RelativePosition.Z + z));
                    this.ChunkStorage[chunk.RelativePosition.X, chunk.RelativePosition.Z] = chunk;
                }
            }

            var southWestEdge = new Vector2Int(this.Player.CurrentChunk.RelativePosition.X - Chunks.ChunkCache.ViewRange, this.Player.CurrentChunk.RelativePosition.Z - Chunks.ChunkCache.ViewRange);
            var northEastEdge = new Vector2Int(this.Player.CurrentChunk.RelativePosition.X + Chunks.ChunkCache.ViewRange, this.Player.CurrentChunk.RelativePosition.Z + Chunks.ChunkCache.ViewRange);

            this.ChunkCache.BoundingBox = new BoundingBox(new Vector3(southWestEdge.X * Chunk.WidthInBlocks, 0, southWestEdge.Z * Chunk.LenghtInBlocks), new Vector3((northEastEdge.X + 1) * Chunk.WidthInBlocks, Chunk.HeightInBlocks, (northEastEdge.Z + 1) * Chunk.LenghtInBlocks));
        }

        private void ProcessChunkInCacheRange(Chunk chunk)
        {
            switch (chunk.ChunkState) // switch on the chunk state.
            {
                case ChunkState.AwaitingGenerate:
                    this.Generate(chunk);
                    break;
                default:
                    break;
            }
        }

        private void ProcessChunkInViewRange(Chunk chunk)
        {
            switch (chunk.ChunkState) // switch on the chunk state.
            {
                case ChunkState.AwaitingGenerate:
                    this.Generate(chunk);
                    break;
                case ChunkState.AwaitingLighting:
                    this.Lighten(chunk);
                    break;
                case ChunkState.AwaitingBuild:
                    this.Build(chunk);
                    break;
                case ChunkState.AwaitingRelighting:
                    this.Lighten(chunk);
                    break;
                case ChunkState.AwaitingRebuild:
                    this.Build(chunk);
                    break;
                default:
                    break;
            }
        }
    }
}
