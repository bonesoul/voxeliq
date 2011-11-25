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
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxlrEngine.Profiling;
using VolumetricStudios.VoxlrEngine.Screen;
using VolumetricStudios.VoxlrEngine.Utils.Vector;
using VolumetricStudios.VoxlrEngine.Universe.Builders;

namespace VolumetricStudios.VoxlrEngine.Universe
{
    /// <summary>
    /// World service interface.
    /// </summary>
    public interface IWorldService
    {
        ChunkManager Chunks { get; }
        void ToggleFog();
        void ToggleInfinitiveWorld();
        void SetBlock(int x, int y, int z, Block block);
        void SetBlock(Vector3Int position, Block block);
        Chunk GetChunk(int x, int z);
    }

    /// <summary>
    /// Statistics interface.
    /// </summary>
    public interface IWorldStatisticsService
    {
        int TotalChunks { get; }
        int ChunksDrawn { get; }
        int GenerationQueueCount { get; }
        int BuildingQueueCount { get; }
        bool IsInfinitive { get; }
        FogState FogState { get; }
    }

    /// <summary>
    /// World.
    /// </summary>
    public class World : DrawableGameComponent, IWorldStatisticsService, IWorldService
    {
        /// <summary>
        /// block effect.
        /// </summary>
        protected Effect _blockEffect;

        /// <summary>
        /// block texture atlas
        /// </summary>
        protected Texture2D _blockTextureAtlas;

        /// <summary>
        /// crack texture atlas
        /// </summary>
        private Texture2D _crackTextureAtlas;        

        /// <summary>
        /// camera controller
        /// </summary>
        private ICameraControlService _cameraController;

        /// <summary>
        /// player
        /// </summary>
        private IPlayer _player;

        /// <summary>
        /// fog vectors.
        /// </summary>
        protected readonly Vector2[] _fogVectors = new[] {new Vector2(0, 0), new Vector2(175, 250), new Vector2(250, 400)};

        /// <summary>
        /// Chunk manager
        /// </summary>
        public ChunkManager Chunks { get; private set; }

        /// <summary>
        /// Chunk builder.
        /// </summary>
        public ChunkBuilder ChunkBuilder { get; private set; }

        /// <summary>
        /// Bounding box for the world.
        /// </summary>
        public BoundingBox BoundingBox { get; set; }

        /// <summary>
        /// View range for the world.
        /// </summary>
        public const byte ViewRange = 6;

        /// <summary>
        /// Fog state.
        /// </summary>
        public FogState FogState { get; private set; }

        /// <summary>
        /// The camera service.
        /// </summary>
        public ICameraService Camera;

        /// <summary>
        /// Chunks drawn statistics.
        /// </summary>
        public int ChunksDrawn { get; protected set; } // chunks drawn statistics.

        /// <summary>
        /// Total chunks
        /// </summary>
        public int TotalChunks { get { return this.Chunks.Count; } }

        /// <summary>
        /// Is the world infinitive?
        /// </summary>
        public bool IsInfinitive { get; private set; }

        /// <summary>
        /// Generation queue count.
        /// </summary>
        public int GenerationQueueCount { get { return this.ChunkBuilder.GenerationQueueCount; } }

        /// <summary>
        /// Building queue count.
        /// </summary>
        public int BuildingQueueCount { get { return this.ChunkBuilder.BuildingQueueCount; } }
        
        /// <summary>
        /// TODO: shouldn't be getting game object really and abstract of game.
        /// </summary>
        /// <param name="game"></param>
        public World(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            this.IsInfinitive = true; // set infinitive.
            this.FogState = FogState.None; // fog-state.

            // TODO: these are client stuff.
            this.Camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService)); //
            this._cameraController = (ICameraControlService)this.Game.Services.GetService(typeof(ICameraControlService));
            this._player = (IPlayer)this.Game.Services.GetService(typeof(IPlayer));    

            this.Chunks = new ChunkManager(); // startup the chunk manager.
            this.ChunkBuilder = new TaskedBuilder(this._player, this); // the chunk builder.        

            this._blockEffect = Game.Content.Load<Effect>("Effects\\BlockEffect");
            this._blockTextureAtlas = Game.Content.Load<Texture2D>("Textures\\blocks");
            this._crackTextureAtlas = Game.Content.Load<Texture2D>("Textures\\cracks");

            this._cameraController.LookAt(Vector3.Down);
            this._player.SpawnPlayer(new Vector2Int(1000, 1000)); // TODO: client stuff.

            base.Initialize();
        }

        // TODO: client stuff.
        public void ToggleFog()
        {
            switch (FogState)
            {
                case FogState.None:
                    FogState = FogState.Near;
                    break;
                case FogState.Near:
                    FogState = FogState.Far;
                    break;
                case FogState.Far:
                    FogState = FogState.None;
                    break;
            }
        }

        public void ToggleInfinitiveWorld()
        {
            this.IsInfinitive = !this.IsInfinitive;
        }

        public bool IsInBounds(int x, int y, int z)
        {
            if (x < this.BoundingBox.Min.X || z < this.BoundingBox.Min.Z || x >= this.BoundingBox.Max.X || z >= this.BoundingBox.Max.Z || y < this.BoundingBox.Min.Y || y >= this.BoundingBox.Max.Y) return false;
            return true;
        }

        public Block BlockAt(Vector3 position)
        {
            return BlockAt((int)position.X, (int)position.Y, (int)position.Z);
        }

        public Block BlockAt(int x, int y, int z)
        {
            if (!IsInBounds(x, y, z)) return Block.Empty;

            if (!this.Chunks.ContainsKey(x / Chunk.WidthInBlocks, z / Chunk.LenghtInBlocks)) 
                return Block.Empty;

            return this.Chunks[x/Chunk.WidthInBlocks, z/Chunk.LenghtInBlocks].BlockAt(x%Chunk.WidthInBlocks, y, z%Chunk.LenghtInBlocks);
        }

        public void SetBlock(Vector3Int position, Block block)
        {
            this.SetBlock(position.X,position.Y,position.Z,block);
        }

        public void SetBlock(int x, int y, int z, Block block)
        {
            var chunk = GetChunk(x, z);
            chunk.SetBlock((byte) (x%Chunk.WidthInBlocks), (byte) y, (byte) (z%Chunk.LenghtInBlocks), block);
        }

        public Chunk GetChunk(int x, int z)
        {
            return !this.Chunks.ContainsKey(x / Chunk.WidthInBlocks, z / Chunk.LenghtInBlocks) ? null : this.Chunks[x / Chunk.WidthInBlocks, z / Chunk.LenghtInBlocks];
        }

        // TODO: client stuff.
        public void SpawnPlayer(Vector2Int relativePosition)
        {
            Profiler.Start("terrain-generation");
            for (int z = -ViewRange; z <= ViewRange; z++)
            {
                for (int x = -ViewRange; x <= ViewRange; x++)
                {
                    var chunk = new Chunk(this, new Vector2Int(relativePosition.X + x, relativePosition.Z + z));
                    this.Chunks[chunk.RelativePosition.X, chunk.RelativePosition.Z] = chunk;

                    if (chunk.RelativePosition == relativePosition) this._player.CurrentChunk = chunk;
                }
            }

            this.Chunks.SouthWestEdge = new Vector2Int(relativePosition.X - ViewRange, relativePosition.Z - ViewRange);
            this.Chunks.NorthEastEdge = new Vector2Int(relativePosition.X + ViewRange, relativePosition.Z + ViewRange);

            BoundingBox = new BoundingBox(new Vector3(this.Chunks.SouthWestEdge.X * Chunk.WidthInBlocks, 0, this.Chunks.SouthWestEdge.Z * Chunk.LenghtInBlocks), new Vector3((this.Chunks.NorthEastEdge.X + 1) * Chunk.WidthInBlocks, Chunk.HeightInBlocks, (this.Chunks.NorthEastEdge.Z + 1) * Chunk.LenghtInBlocks));
            
            this.ChunkBuilder.Start();
        }
    }

    #region enums

    // TODO: client stuff.
    public enum FogState:byte
    {
        None,
        Near,
        Far
    }

    #endregion
}
