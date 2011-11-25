/*
 * Copyright (C) 2011 voxeliq project 
 *
 */

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqClient.Graphics.Processors.ChunkProcessor;
using VolumetricStudios.VoxeliqClient.Worlds;
using VolumetricStudios.VoxeliqEngine.Universe;
using VolumetricStudios.VoxeliqEngine.Universe.Terrain;
using VolumetricStudios.VoxeliqEngine.Universe.Terrain.Generators.Biomes;
using VolumetricStudios.VoxeliqEngine.Utils.Vector;

namespace VolumetricStudios.VoxeliqClient.Graphics.Builders
{
    public class ChunkBuilder
    {
        protected readonly BlockingCollection<Chunk> _generationQueue = new BlockingCollection<Chunk>(); // uses concurrent queues by default.
        protected readonly BlockingCollection<Chunk> _buildingQueue = new BlockingCollection<Chunk>();
        protected IPlayer _player;
        protected GameWorld _world;
        protected bool _active = false;
        protected readonly TerrainGenerator Generator;
        
        public int GenerationQueueCount { get { return this._generationQueue.Count; } }
        public int BuildingQueueCount { get { return this._buildingQueue.Count; } }

        protected virtual void QueueChunks() { }

        protected ChunkBuilder(IPlayer player, GameWorld world)
        {
            this._player = player;
            this._world = world;
            this.Generator = new RainForest();
        }

        public void Start()
        {
            this._active = true;
            Task.Factory.StartNew(Worker, TaskCreationOptions.LongRunning);                        
        }
        
        private void Worker()
        {
            while (this._active)
            {
                if (this._world.IsInfinitive && this.CheckIfPlayerChunkChanged()) this.RecacheChunks();                                    
                this.QueueChunks();
            }
        }

        private bool CheckIfPlayerChunkChanged()
        {
            if (!this._world.IsInfinitive || _player.CurrentChunk.IsInBounds(_player.Position.X, _player.Position.Z)) return false; // he's already in same chunk.
            var chunk = this._world.GetChunk((int)_player.Position.X, (int)_player.Position.Z);

            _player.LastChunk = _player.CurrentChunk;
            _player.CurrentChunk = chunk;

            return _player.CurrentChunk != _player.LastChunk;
        }  

        private void RecacheChunks()
        {
            if (!this._world.IsInfinitive) return;

            var displacement = new Vector2Int(this._player.CurrentChunk.RelativePosition.X - this._player.LastChunk.RelativePosition.X, this._player.CurrentChunk.RelativePosition.Z - this._player.LastChunk.RelativePosition.Z);

            if (displacement.X != 0) RecacheHorizantalChunks(displacement.X < 0 ? Direction.West : Direction.East, Math.Abs(displacement.X));
            if (displacement.Z != 0) RecacheVerticalChunks(displacement.Z < 0 ? Direction.South : Direction.North, Math.Abs(displacement.Z));

            _world.BoundingBox = new BoundingBox(new Vector3(this._world.Chunks.SouthWestEdge.X * Chunk.WidthInBlocks, 0, this._world.Chunks.SouthWestEdge.Z * Chunk.LenghtInBlocks), new Vector3((this._world.Chunks.NorthEastEdge.X + 1) * Chunk.WidthInBlocks, Chunk.HeightInBlocks, (this._world.Chunks.NorthEastEdge.Z + 1) * Chunk.LenghtInBlocks));
        }

        private void RecacheHorizantalChunks(Direction direction, int delta)
        {
            Debug.Assert(delta == 1 || _player.FlyingEnabled);

            int xCord = _world.Chunks.SouthWestEdge.X;
            if (direction == Direction.West) xCord--;

            for (int z = this._world.Chunks.SouthWestEdge.Z; z <= this._world.Chunks.SouthWestEdge.Z + World.ViewRange * 2; z++)
            {
                var pos = new Vector2Int(xCord, z);
                Chunk chunk;
                switch (direction)
                {
                    case Direction.West: 
                        chunk = new Chunk(_world, pos);
                        this._world.Chunks[xCord, z] = chunk;
                        break;
                    case Direction.East:
                        chunk = this._world.Chunks[xCord, z];
                        this._world.Chunks.Remove(xCord, z);
                        chunk.Dispose();
                        break;
                }
            }

            xCord = this._world.Chunks.NorthEastEdge.X;
            if (direction == Direction.East) xCord++;

            for (int z = this._world.Chunks.NorthEastEdge.Z; z >= this._world.Chunks.NorthEastEdge.Z - World.ViewRange * 2; z--)
            {
                var pos = new Vector2Int(xCord, z);
                Chunk chunk;
                switch (direction)
                {
                    case Direction.West:
                        chunk = this._world.Chunks[xCord, z];
                        this._world.Chunks.Remove(xCord, z);
                        chunk.Dispose();
                        break;
                    case Direction.East:
                        chunk = new Chunk(_world, pos);
                        this._world.Chunks[xCord, z] = chunk;
                        break;
                }
            }

            switch (direction)
            {
                case Direction.West:
                    this._world.Chunks.SouthWestEdge = new Vector2Int(this._world.Chunks.SouthWestEdge.X - 1, this._world.Chunks.SouthWestEdge.Z);
                    this._world.Chunks.NorthEastEdge = new Vector2Int(this._world.Chunks.NorthEastEdge.X - 1, this._world.Chunks.NorthEastEdge.Z);
                    break;
                case Direction.East:
                    this._world.Chunks.SouthWestEdge = new Vector2Int(this._world.Chunks.SouthWestEdge.X + 1, this._world.Chunks.SouthWestEdge.Z);
                    this._world.Chunks.NorthEastEdge = new Vector2Int(this._world.Chunks.NorthEastEdge.X + 1, this._world.Chunks.NorthEastEdge.Z);
                    break;
            }
        }

        private void RecacheVerticalChunks(Direction direction, int delta)
        {
            Debug.Assert(delta == 1 || _player.FlyingEnabled);

            int zCord = this._world.Chunks.SouthWestEdge.Z;
            if (direction == Direction.South) zCord--;

            for (int x = this._world.Chunks.SouthWestEdge.X; x <= this._world.Chunks.SouthWestEdge.X + World.ViewRange * 2; x++)
            {
                var pos = new Vector2Int(x, zCord);
                Chunk chunk;
                switch (direction)
                {
                    case Direction.South:
                        chunk = new Chunk(_world, pos);
                        this._world.Chunks[x, zCord] = chunk;
                        break;
                    case Direction.North:
                        chunk = this._world.Chunks[x, zCord];
                        this._world.Chunks.Remove(x, zCord);
                        chunk.Dispose();
                        break;
                }
            }

            zCord = this._world.Chunks.NorthEastEdge.Z;
            if (direction == Direction.North) zCord++;

            for (int x = this._world.Chunks.NorthEastEdge.X; x >= this._world.Chunks.NorthEastEdge.X - World.ViewRange * 2; x--)
            {
                var pos = new Vector2Int(x, zCord);
                Chunk chunk;
                switch (direction)
                {
                    case Direction.South:
                        chunk = this._world.Chunks[x, zCord];
                        this._world.Chunks.Remove(x, zCord);
                        chunk.Dispose();
                        break;
                    case Direction.North:
                        chunk = new Chunk(_world, pos);
                        this._world.Chunks[x, zCord] = chunk;
                        break;
                }
            }

            switch (direction)
            {
                case Direction.South:
                    this._world.Chunks.SouthWestEdge = new Vector2Int(this._world.Chunks.SouthWestEdge.X, this._world.Chunks.SouthWestEdge.Z - 1);
                    this._world.Chunks.NorthEastEdge = new Vector2Int(this._world.Chunks.NorthEastEdge.X, this._world.Chunks.NorthEastEdge.Z - 1);
                    break;
                case Direction.North:
                    this._world.Chunks.SouthWestEdge = new Vector2Int(this._world.Chunks.SouthWestEdge.X, this._world.Chunks.SouthWestEdge.Z + 1);
                    this._world.Chunks.NorthEastEdge = new Vector2Int(this._world.Chunks.NorthEastEdge.X, this._world.Chunks.NorthEastEdge.Z + 1);
                    break;
            }

        }        

        protected void Process()
        {
            Chunk chunk;
            BlockingCollection<Chunk>.TakeFromAny(new[] { _generationQueue, _buildingQueue }, out chunk); // take from any will always prefer _generationQueue over _building code
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
                VertexBuilder.Build(this._world.Game.GraphicsDevice, chunk);
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
