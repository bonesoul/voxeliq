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

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxlrClient.GraphicsEngine.Builders;
using VolumetricStudios.VoxlrEngine.Profiling;
using VolumetricStudios.VoxlrEngine.Screen;
using VolumetricStudios.VoxlrEngine.Universe;
using VolumetricStudios.VoxlrEngine.Utils.Vector;

namespace VolumetricStudios.VoxlrClient.GameEngine
{
    /// <summary>
    /// The game world.
    /// </summary>
    public class GameWorld : World, IGameComponent, IDrawable, IWorldStatisticsService
    {
        /// <summary>
        /// fog vectors.
        /// </summary>
        private readonly Vector2[] _fogVectors = new[] { new Vector2(0, 0), new Vector2(175, 250), new Vector2(250, 400) };

        /// <summary>
        /// Fog state.
        /// </summary>
        public FogState FogState { get; private set; }

        /// <summary>
        /// camera controller
        /// </summary>
        private ICameraControlService _cameraController;

        public bool Visible
        {
            get { return true; }
        }

        public int DrawOrder { get; set; }

        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<EventArgs> DrawOrderChanged;

        /// <summary>
        /// The camera service.
        /// </summary>
        public ICameraService Camera;

        /// <summary>
        /// Chunk builder.
        /// </summary>
        public ChunkBuilder ChunkBuilder { get; protected set; }

        /// <summary>
        /// Generation queue count.
        /// </summary>
        public int GenerationQueueCount { get { return this.ChunkBuilder.GenerationQueueCount; } }

        /// <summary>
        /// Building queue count.
        /// </summary>
        public int BuildingQueueCount { get { return this.ChunkBuilder.BuildingQueueCount; } }

        public Game Game { get; private set; }

        /// <summary>
        /// player
        /// </summary>
        private IPlayer _player;

        /// <summary>
        /// block effect.
        /// </summary>
        private Effect _blockEffect;

        /// <summary>
        /// block texture atlas
        /// </summary>
        private Texture2D _blockTextureAtlas;

        /// <summary>
        /// crack texture atlas
        /// </summary>
        private Texture2D _crackTextureAtlas;

        public int UpdateOrder { get; set; }

        public GameWorld(Game game):
            base(true) // set world to infinitive.
        {
            this.Game = game;
            this.Game.Services.AddService(typeof(IWorldStatisticsService), this);
            this.Game.Services.AddService(typeof(IWorldService), this);
        }

        public void Initialize()
        {
            this.FogState = FogState.None; // fog-state.

            // TODO: these are client stuff.
            this.Camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService)); //
            this._cameraController = (ICameraControlService)this.Game.Services.GetService(typeof(ICameraControlService));
            this._player = (IPlayer)this.Game.Services.GetService(typeof(IPlayer));

            this.Chunks = new ChunkManager(); // startup the chunk manager.
            this.ChunkBuilder = new QueuedBuilder(this._player, this); // the chunk builder.        

            this._blockEffect = Game.Content.Load<Effect>("Effects\\BlockEffect");
            this._blockTextureAtlas = Game.Content.Load<Texture2D>("Textures\\blocks");
            this._crackTextureAtlas = Game.Content.Load<Texture2D>("Textures\\cracks");

            this._cameraController.LookAt(Vector3.Down);
            this._player.SpawnPlayer(new Vector2Int(1000, 1000)); // TODO: client stuff.
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

        #region world-drawer

        public void Draw(GameTime gameTime)
        {
            var viewFrustrum = new BoundingFrustum(this.Camera.View * this.Camera.Projection);

            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game.GraphicsDevice.BlendState = BlendState.Opaque;

            _blockEffect.Parameters["World"].SetValue(Matrix.Identity);
            _blockEffect.Parameters["View"].SetValue(this.Camera.View);
            _blockEffect.Parameters["Projection"].SetValue(this.Camera.Projection);
            _blockEffect.Parameters["CameraPosition"].SetValue(this.Camera.Position);
            _blockEffect.Parameters["FogColor"].SetValue(Color.White.ToVector4());
            _blockEffect.Parameters["FogNear"].SetValue(this._fogVectors[(byte)this.FogState].X);
            _blockEffect.Parameters["FogFar"].SetValue(this._fogVectors[(byte)this.FogState].Y);
            _blockEffect.Parameters["SunColor"].SetValue(Color.White.ToVector3());
            _blockEffect.Parameters["BlockTextureAtlas"].SetValue(_blockTextureAtlas);

            this.ChunksDrawn = 0;
            foreach (EffectPass pass in this._blockEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                foreach (Chunk chunk in this.Chunks.Values)
                {
                    if (!chunk.Generated || !chunk.BoundingBox.Intersects(viewFrustrum) || chunk.IndexBuffer == null) continue;

                    lock (chunk)
                    {
                        if (chunk.IndexBuffer.IndexCount == 0) continue;
                        Game.GraphicsDevice.SetVertexBuffer(chunk.VertexBuffer);
                        Game.GraphicsDevice.Indices = chunk.IndexBuffer;
                        Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, chunk.VertexBuffer.VertexCount, 0, chunk.IndexBuffer.IndexCount / 3);
                    }

                    this.ChunksDrawn++;
                }
            }
        }

        #endregion
    }

    // TODO: client stuff.
    public enum FogState : byte
    {
        None,
        Near,
        Far
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
}
