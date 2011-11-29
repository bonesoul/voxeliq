/*
 * Copyright (C) 2011 voxeliq project 
 *
 */

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqEngine.Chunks;
using VolumetricStudios.VoxeliqEngine.Processors;
using VolumetricStudios.VoxeliqEngine.Terrain;
using VolumetricStudios.VoxeliqEngine.Terrain.Generators.Biomes;
using VolumetricStudios.VoxeliqEngine.Universe;
using VolumetricStudios.VoxeliqEngine.Utils.Vector;

namespace VolumetricStudios.VoxeliqEngine.Builders
{
    public class ChunkBuilder
    {
        protected readonly BlockingCollection<Chunk> GenerationQueue = new BlockingCollection<Chunk>(); // uses concurrent queues by default.
        protected readonly BlockingCollection<Chunk> BuildingQueue = new BlockingCollection<Chunk>();
        protected IPlayer Player;
        protected World World;
        protected bool Active = false;
        protected readonly TerrainGenerator Generator;
        
        public int GenerationQueueCount { get { return this.GenerationQueue.Count; } }
        public int BuildingQueueCount { get { return this.BuildingQueue.Count; } }

        protected virtual void QueueChunks() { }

        protected ChunkBuilder(IPlayer player, World world)
        {
            this.Player = player;
            this.World = world;
            this.Generator = new RainForest();
        }

        public void Start()
        {
            this.Active = true;
            Task.Factory.StartNew(Worker, TaskCreationOptions.LongRunning);                        
        }
        
        private void Worker()
        {
            while (this.Active)
            {
                if (this.World.IsInfinitive && this.CheckIfPlayerChunkChanged()) this.RecacheChunks();                                    
                this.QueueChunks();
            }
        }

        private bool CheckIfPlayerChunkChanged()
        {
            if (!this.World.IsInfinitive || Player.CurrentChunk.IsInBounds(Player.Position.X, Player.Position.Z)) return false; // he's already in same chunk.
            var chunk = this.World.GetChunk((int)Player.Position.X, (int)Player.Position.Z);

            Player.LastChunk = Player.CurrentChunk;
            Player.CurrentChunk = chunk;

            return Player.CurrentChunk != Player.LastChunk;
        }  

        private void RecacheChunks()
        {
            if (!this.World.IsInfinitive) return;

            var displacement = new Vector2Int(this.Player.CurrentChunk.RelativePosition.X - this.Player.LastChunk.RelativePosition.X, this.Player.CurrentChunk.RelativePosition.Z - this.Player.LastChunk.RelativePosition.Z);

            if (displacement.X != 0) RecacheHorizantalChunks(displacement.X < 0 ? Direction.West : Direction.East, Math.Abs(displacement.X));
            if (displacement.Z != 0) RecacheVerticalChunks(displacement.Z < 0 ? Direction.South : Direction.North, Math.Abs(displacement.Z));

            World.BoundingBox = new BoundingBox(new Vector3(this.World.Chunks.SouthWestEdge.X * Chunk.WidthInBlocks, 0, this.World.Chunks.SouthWestEdge.Z * Chunk.LenghtInBlocks), new Vector3((this.World.Chunks.NorthEastEdge.X + 1) * Chunk.WidthInBlocks, Chunk.HeightInBlocks, (this.World.Chunks.NorthEastEdge.Z + 1) * Chunk.LenghtInBlocks));
        }

        private void RecacheHorizantalChunks(Direction direction, int delta)
        {
            Debug.Assert(delta == 1 || Player.FlyingEnabled);

            int xCord = World.Chunks.SouthWestEdge.X;
            if (direction == Direction.West) xCord--;

            for (int z = this.World.Chunks.SouthWestEdge.Z; z <= this.World.Chunks.SouthWestEdge.Z + World.ViewRange * 2; z++)
            {
                var pos = new Vector2Int(xCord, z);
                Chunk chunk;
                switch (direction)
                {
                    case Direction.West: 
                        chunk = new Chunk(World, pos);
                        this.World.Chunks[xCord, z] = chunk;
                        break;
                    case Direction.East:
                        chunk = this.World.Chunks[xCord, z];
                        this.World.Chunks.Remove(xCord, z);
                        chunk.Dispose();
                        break;
                }
            }

            xCord = this.World.Chunks.NorthEastEdge.X;
            if (direction == Direction.East) xCord++;

            for (int z = this.World.Chunks.NorthEastEdge.Z; z >= this.World.Chunks.NorthEastEdge.Z - World.ViewRange * 2; z--)
            {
                var pos = new Vector2Int(xCord, z);
                Chunk chunk;
                switch (direction)
                {
                    case Direction.West:
                        chunk = this.World.Chunks[xCord, z];
                        this.World.Chunks.Remove(xCord, z);
                        chunk.Dispose();
                        break;
                    case Direction.East:
                        chunk = new Chunk(World, pos);
                        this.World.Chunks[xCord, z] = chunk;
                        break;
                }
            }

            switch (direction)
            {
                case Direction.West:
                    this.World.Chunks.SouthWestEdge = new Vector2Int(this.World.Chunks.SouthWestEdge.X - 1, this.World.Chunks.SouthWestEdge.Z);
                    this.World.Chunks.NorthEastEdge = new Vector2Int(this.World.Chunks.NorthEastEdge.X - 1, this.World.Chunks.NorthEastEdge.Z);
                    break;
                case Direction.East:
                    this.World.Chunks.SouthWestEdge = new Vector2Int(this.World.Chunks.SouthWestEdge.X + 1, this.World.Chunks.SouthWestEdge.Z);
                    this.World.Chunks.NorthEastEdge = new Vector2Int(this.World.Chunks.NorthEastEdge.X + 1, this.World.Chunks.NorthEastEdge.Z);
                    break;
            }
        }

        private void RecacheVerticalChunks(Direction direction, int delta)
        {
            Debug.Assert(delta == 1 || Player.FlyingEnabled);

            int zCord = this.World.Chunks.SouthWestEdge.Z;
            if (direction == Direction.South) zCord--;

            for (int x = this.World.Chunks.SouthWestEdge.X; x <= this.World.Chunks.SouthWestEdge.X + World.ViewRange * 2; x++)
            {
                var pos = new Vector2Int(x, zCord);
                Chunk chunk;
                switch (direction)
                {
                    case Direction.South:
                        chunk = new Chunk(World, pos);
                        this.World.Chunks[x, zCord] = chunk;
                        break;
                    case Direction.North:
                        chunk = this.World.Chunks[x, zCord];
                        this.World.Chunks.Remove(x, zCord);
                        chunk.Dispose();
                        break;
                }
            }

            zCord = this.World.Chunks.NorthEastEdge.Z;
            if (direction == Direction.North) zCord++;

            for (int x = this.World.Chunks.NorthEastEdge.X; x >= this.World.Chunks.NorthEastEdge.X - World.ViewRange * 2; x--)
            {
                var pos = new Vector2Int(x, zCord);
                Chunk chunk;
                switch (direction)
                {
                    case Direction.South:
                        chunk = this.World.Chunks[x, zCord];
                        this.World.Chunks.Remove(x, zCord);
                        chunk.Dispose();
                        break;
                    case Direction.North:
                        chunk = new Chunk(World, pos);
                        this.World.Chunks[x, zCord] = chunk;
                        break;
                }
            }

            switch (direction)
            {
                case Direction.South:
                    this.World.Chunks.SouthWestEdge = new Vector2Int(this.World.Chunks.SouthWestEdge.X, this.World.Chunks.SouthWestEdge.Z - 1);
                    this.World.Chunks.NorthEastEdge = new Vector2Int(this.World.Chunks.NorthEastEdge.X, this.World.Chunks.NorthEastEdge.Z - 1);
                    break;
                case Direction.North:
                    this.World.Chunks.SouthWestEdge = new Vector2Int(this.World.Chunks.SouthWestEdge.X, this.World.Chunks.SouthWestEdge.Z + 1);
                    this.World.Chunks.NorthEastEdge = new Vector2Int(this.World.Chunks.NorthEastEdge.X, this.World.Chunks.NorthEastEdge.Z + 1);
                    break;
            }

        }        

        protected void Process()
        {
            Chunk chunk;
            BlockingCollection<Chunk>.TakeFromAny(new[] { GenerationQueue, BuildingQueue }, out chunk); // take from any will always prefer _generationQueue over _building code
            // it other means, if there are chunks to generate, it'll take items from building queue.
            if (!chunk.Generated)
            {
                Generator.Generate(chunk);
                chunk.Generated = true;
                chunk.QueuedForGeneration = false;
            }            
            else if (chunk.Dirty)
            {
                Lightning.Process(chunk);
                VertexBuilder.Build(this.World.GraphicsDevice, chunk);
            }
        }     
    }

    public enum Direction : byte
    {
        North,
        South,
        East,
        West,
    }
}
