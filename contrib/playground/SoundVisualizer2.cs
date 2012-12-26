﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using VolumetricStudios.VoxlrEngine.Universe;
using VolumetricStudios.VoxlrEngine.Vector;

namespace VolumetricStudios.VoxlrClient.Visualizer
{
    public sealed class SoundVisualizer : DrawableGameComponent
    {
        private IWorldService _world;
        private readonly IPlayer _player;
        private VisualizationData _visualizationData;
        private Song _song;
        private Vector2Int _referencePosition;
        private TimeSpan _elapsedTime = TimeSpan.Zero;

        public SoundVisualizer(Game game)
            : base(game)
        {
            this._world = (IWorldService)this.Game.Services.GetService(typeof(IWorldService));
            this._player = (IPlayer)this.Game.Services.GetService(typeof(IPlayer));
        }

        protected override void LoadContent()
        {
            this._visualizationData = new VisualizationData();
            this._referencePosition = new Vector2Int((int)this._player.Position.X - this._visualizationData.Frequencies.Count / 2, (int)this._player.Position.Z);
            MediaPlayer.Volume = 1f;
            MediaPlayer.IsVisualizationEnabled = true;
            new Thread(VisualizationReader) { IsBackground = true }.Start();
            this._song = Game.Content.Load<Song>("yael");
            MediaPlayer.Play(this._song);
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < this._visualizationData.Frequencies.Count / Chunk.WidthInBlocks; i++)
            {
                var chunk = this._world.GetChunk(this._referencePosition.X + i * (Chunk.WidthInBlocks), this._referencePosition.Z);
                for (int j = 0; j < Chunk.WidthInBlocks; j++)
                {
                    var frequency = (byte)(this._visualizationData.Frequencies[i * Chunk.WidthInBlocks + j] * 100);

                    for (int y = Chunk.HeightInBlocks - 1; y >= 1; y--)
                    {
                        for (int z = 0; z < Chunk.LenghtInBlocks; z++)
                        {
                            chunk.Blocks[j, y, z] = new Block(BlockType.None);
                        }
                    }

                    for (int z = 0; z < Chunk.LenghtInBlocks; z++)
                    {
                        chunk.Blocks[j, frequency, z] = new Block(BlockType.Gold);
                    }
                }
                if (!chunk.Dirty) chunk.Dirty = true;
            }
        }

        public override void Draw(GameTime gameTime)
        {

        }

        private void VisualizationReader()
        {
            while (true)
            {
                lock (_visualizationData)
                {
                    MediaPlayer.GetVisualizationData(this._visualizationData);
                    Thread.Sleep(50);
                }
            }
        }
    }
}
