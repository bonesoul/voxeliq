/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Engine.Blocks;
using Engine.Common.Vector;
using Engine.Graphics.Texture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

// TODO: Document this file!

namespace Engine.Chunks.Processors
{
    public interface IVertexBuilder
    {
        /// <summary>
        /// Builds vertex for given chunk.
        /// </summary>
        /// <param name="chunk"></param>
        void Build(Chunk chunk);
    }

    public class VertexBuilder : GameComponent, IVertexBuilder
    {
        // import required services.
        private IChunkCache _chunkCache;
        private IBlockStorage _blockStorage;

        public VertexBuilder(Game game)
            : base(game)
        {
            this.Game.Services.AddService(typeof (IVertexBuilder), this); // export the service.
        }

        public override void Initialize()
        {
            // import required services.
            this._chunkCache = (IChunkCache) this.Game.Services.GetService(typeof (IChunkCache));
            this._blockStorage = (IBlockStorage)this.Game.Services.GetService(typeof(IBlockStorage));
        }

        public void Build(Chunk chunk)
        {
            if (chunk.ChunkState != ChunkState.AwaitingBuild) // if chunk is not awaiting building.
                return; // then just skip it

            chunk.ChunkState = ChunkState.Building; // set chunk state to building.

            chunk.CalculateHeightIndexes();

            chunk.BoundingBox = new BoundingBox(
                new Vector3(chunk.WorldPosition.X, chunk.LowestEmptyBlockOffset, chunk.WorldPosition.Z),
                new Vector3(chunk.WorldPosition.X + Chunk.WidthInBlocks, chunk.HighestSolidBlockOffset, chunk.WorldPosition.Z + Chunk.LengthInBlocks));

            this.BuildVertexList(chunk);

            chunk.ChunkState = ChunkState.Ready; // chunk is al ready now.
        }

        public static void Clear(Chunk chunk)
        {
            chunk.VertexList.Clear();
            chunk.IndexList.Clear();
            chunk.Index = 0;
        }

        /// <summary>
        /// Builds vertex list for chunk.
        /// </summary>
        /// <param name="chunk"></param>
        private void BuildVertexList(Chunk chunk)
        {
            if (chunk.Disposed) 
                return;

            Clear(chunk);

            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (byte z = 0; z < Chunk.LengthInBlocks; z++)
                {
                    int offset = BlockStorage.BlockIndexByRelativePosition(chunk, x, z);

                    for (byte y = chunk.LowestEmptyBlockOffset; y <= chunk.HighestSolidBlockOffset; y++)
                    {
                        var blockIndex = offset + y;

                        var block = BlockStorage.Blocks[blockIndex];

                        if (block.Type == BlockType.None)
                            continue;

                        var position = new Vector3Int(chunk.WorldPosition.X + x, y, chunk.WorldPosition.Z + z);
                        this.BuildBlockVertices(chunk, blockIndex, position);
                    }
                }
            }

            var vertices = chunk.VertexList.ToArray();
            var indices = chunk.IndexList.ToArray();

            if (vertices.Length == 0 || indices.Length == 0) return;

            chunk.VertexBuffer = new VertexBuffer(this.Game.GraphicsDevice, typeof(BlockVertex), vertices.Length, BufferUsage.WriteOnly);
            chunk.VertexBuffer.SetData(vertices);

            chunk.IndexBuffer = new IndexBuffer(this.Game.GraphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
            chunk.IndexBuffer.SetData(indices);
        }


        /// <summary>
        /// Builds vertices for block.
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="blockIndex"> </param>
        /// <param name="worldPosition"></param>
        private void BuildBlockVertices(Chunk chunk, int blockIndex, Vector3Int worldPosition)
        {
            Block block = BlockStorage.Blocks[blockIndex]; // get the block to process.

            Block blockTopNW, blockTopN, blockTopNE, blockTopW, blockTopM, blockTopE, blockTopSW, blockTopS, blockTopSE;
            Block blockMidNW, blockMidN, blockMidNE, blockMidW, blockMidM, blockMidE, blockMidSW, blockMidS, blockMidSE;
            Block blockBotNW, blockBotN, blockBotNE, blockBotW, blockBotM, blockBotE, blockBotSW, blockBotS, blockBotSE;

            blockTopNW = _blockStorage.BlockAt(worldPosition.X - 1, worldPosition.Y + 1, worldPosition.Z + 1);
            blockTopN = _blockStorage.BlockAt(worldPosition.X, worldPosition.Y + 1, worldPosition.Z + 1);
            blockTopNE = _blockStorage.BlockAt(worldPosition.X + 1, worldPosition.Y + 1, worldPosition.Z + 1);
            blockTopW = _blockStorage.BlockAt(worldPosition.X - 1, worldPosition.Y + 1, worldPosition.Z);
            blockTopM = _blockStorage.BlockAt(worldPosition.X, worldPosition.Y + 1, worldPosition.Z);
            blockTopE = _blockStorage.BlockAt(worldPosition.X + 1, worldPosition.Y + 1, worldPosition.Z);
            blockTopSW = _blockStorage.BlockAt(worldPosition.X - 1, worldPosition.Y + 1, worldPosition.Z - 1);
            blockTopS = _blockStorage.BlockAt(worldPosition.X, worldPosition.Y + 1, worldPosition.Z - 1);
            blockTopSE = _blockStorage.BlockAt(worldPosition.X + 1, worldPosition.Y + 1, worldPosition.Z - 1);

            blockMidNW = _blockStorage.BlockAt(worldPosition.X - 1, worldPosition.Y, worldPosition.Z + 1);
            blockMidN = _blockStorage.BlockAt(worldPosition.X, worldPosition.Y, worldPosition.Z + 1);
            blockMidNE = _blockStorage.BlockAt(worldPosition.X + 1, worldPosition.Y, worldPosition.Z + 1);
            blockMidW = _blockStorage.BlockAt(worldPosition.X - 1, worldPosition.Y, worldPosition.Z);
            
            // here comes the self block in order but we don't need to calculate it ;)

            blockMidE = _blockStorage.BlockAt(worldPosition.X + 1, worldPosition.Y, worldPosition.Z);
            blockMidSW = _blockStorage.BlockAt(worldPosition.X - 1, worldPosition.Y, worldPosition.Z - 1);
            blockMidS = _blockStorage.BlockAt(worldPosition.X, worldPosition.Y, worldPosition.Z - 1);
            blockMidSE = _blockStorage.BlockAt(worldPosition.X + 1, worldPosition.Y, worldPosition.Z - 1);

            blockBotNW = _blockStorage.BlockAt(worldPosition.X - 1, worldPosition.Y - 1, worldPosition.Z + 1);
            blockBotN = _blockStorage.BlockAt(worldPosition.X, worldPosition.Y - 1, worldPosition.Z + 1);
            blockBotNE = _blockStorage.BlockAt(worldPosition.X + 1, worldPosition.Y - 1, worldPosition.Z + 1);
            blockBotW = _blockStorage.BlockAt(worldPosition.X - 1, worldPosition.Y - 1, worldPosition.Z);
            blockBotM = _blockStorage.BlockAt(worldPosition.X, worldPosition.Y - 1, worldPosition.Z);
            blockBotE = _blockStorage.BlockAt(worldPosition.X + 1, worldPosition.Y - 1, worldPosition.Z);
            blockBotSW = _blockStorage.BlockAt(worldPosition.X - 1, worldPosition.Y - 1, worldPosition.Z - 1);
            blockBotS = _blockStorage.BlockAt(worldPosition.X, worldPosition.Y - 1, worldPosition.Z - 1);
            blockBotSE = _blockStorage.BlockAt(worldPosition.X + 1, worldPosition.Y - 1, worldPosition.Z - 1);

            float sunTR, sunTL, sunBR, sunBL;
            float redTR, redTL, redBR, redBL;
            float grnTR, grnTL, grnBR, grnBL;
            float bluTR, bluTL, bluBR, bluBL;
            Color localTR, localTL, localBR, localBL;

            localTR = Color.Black;
            localTL = Color.Yellow;
            localBR = Color.Green;
            localBL = Color.Blue;

            // -X face.
            if (!blockMidW.Exists && !(block.Type == BlockType.Water && blockMidW.Type == BlockType.Water))
            {
                sunTL = (1f/Chunk.MaxSunValue)*((blockTopNW.Sun + blockTopW.Sun + blockMidNW.Sun + blockMidW.Sun)/4);
                sunTR = (1f/Chunk.MaxSunValue)*((blockTopSW.Sun + blockTopW.Sun + blockMidSW.Sun + blockMidW.Sun)/4);
                sunBL = (1f/Chunk.MaxSunValue)*((blockBotNW.Sun + blockBotW.Sun + blockMidNW.Sun + blockMidW.Sun)/4);
                sunBR = (1f/Chunk.MaxSunValue)*((blockBotSW.Sun + blockBotW.Sun + blockMidSW.Sun + blockMidW.Sun)/4);

                redTL = (1f/Chunk.MaxSunValue)*((blockTopNW.R + blockTopW.R + blockMidNW.R + blockMidW.R)/4);
                redTR = (1f/Chunk.MaxSunValue)*((blockTopSW.R + blockTopW.R + blockMidSW.R + blockMidW.R)/4);
                redBL = (1f/Chunk.MaxSunValue)*((blockBotNW.R + blockBotW.R + blockMidNW.R + blockMidW.R)/4);
                redBR = (1f/Chunk.MaxSunValue)*((blockBotSW.R + blockBotW.R + blockMidSW.R + blockMidW.R)/4);

                grnTL = (1f/Chunk.MaxSunValue)*((blockTopNW.G + blockTopW.G + blockMidNW.G + blockMidW.G)/4);
                grnTR = (1f/Chunk.MaxSunValue)*((blockTopSW.G + blockTopW.G + blockMidSW.G + blockMidW.G)/4);
                grnBL = (1f/Chunk.MaxSunValue)*((blockBotNW.G + blockBotW.G + blockMidNW.G + blockMidW.G)/4);
                grnBR = (1f/Chunk.MaxSunValue)*((blockBotSW.G + blockBotW.G + blockMidSW.G + blockMidW.G)/4);

                bluTL = (1f/Chunk.MaxSunValue)*((blockTopNW.B + blockTopW.B + blockMidNW.B + blockMidW.B)/4);
                bluTR = (1f/Chunk.MaxSunValue)*((blockTopSW.B + blockTopW.B + blockMidSW.B + blockMidW.B)/4);
                bluBL = (1f/Chunk.MaxSunValue)*((blockBotNW.B + blockBotW.B + blockMidNW.B + blockMidW.B)/4);
                bluBR = (1f/Chunk.MaxSunValue)*((blockBotSW.B + blockBotW.B + blockMidSW.B + blockMidW.B)/4);

                localTL = new Color(redTL, grnTL, bluTL);
                localTR = new Color(redTR, grnTR, bluTR);
                localBL = new Color(redBL, grnBL, bluBL);
                localBR = new Color(redBR, grnBR, bluBR);

                BuildFaceVertices(chunk, worldPosition, block.Type, BlockFaceDirection.XDecreasing, sunTL, sunTR, sunBL, sunBR, localTL, localTR, localBL, localBR);
            }
            // +X face.
            if (!blockMidE.Exists && !(block.Type == BlockType.Water && blockMidE.Type == BlockType.Water))
            {
                sunTL = (1f/Chunk.MaxSunValue)*((blockTopSE.Sun + blockTopE.Sun + blockMidSE.Sun + blockMidE.Sun)/4);
                sunTR = (1f/Chunk.MaxSunValue)*((blockTopNE.Sun + blockTopE.Sun + blockMidNE.Sun + blockMidE.Sun)/4);
                sunBL = (1f/Chunk.MaxSunValue)*((blockBotSE.Sun + blockBotE.Sun + blockMidSE.Sun + blockMidE.Sun)/4);
                sunBR = (1f/Chunk.MaxSunValue)*((blockBotNE.Sun + blockBotE.Sun + blockMidNE.Sun + blockMidE.Sun)/4);

                redTL = (1f/Chunk.MaxSunValue)*((blockTopSE.R + blockTopE.R + blockMidSE.R + blockMidE.R)/4);
                redTR = (1f/Chunk.MaxSunValue)*((blockTopNE.R + blockTopE.R + blockMidNE.R + blockMidE.R)/4);
                redBL = (1f/Chunk.MaxSunValue)*((blockBotSE.R + blockBotE.R + blockMidSE.R + blockMidE.R)/4);
                redBR = (1f/Chunk.MaxSunValue)*((blockBotNE.R + blockBotE.R + blockMidNE.R + blockMidE.R)/4);

                grnTL = (1f/Chunk.MaxSunValue)*((blockTopSE.G + blockTopE.G + blockMidSE.G + blockMidE.G)/4);
                grnTR = (1f/Chunk.MaxSunValue)*((blockTopNE.G + blockTopE.G + blockMidNE.G + blockMidE.G)/4);
                grnBL = (1f/Chunk.MaxSunValue)*((blockBotSE.G + blockBotE.G + blockMidSE.G + blockMidE.G)/4);
                grnBR = (1f/Chunk.MaxSunValue)*((blockBotNE.G + blockBotE.G + blockMidNE.G + blockMidE.G)/4);

                bluTL = (1f/Chunk.MaxSunValue)*((blockTopSE.B + blockTopE.B + blockMidSE.B + blockMidE.B)/4);
                bluTR = (1f/Chunk.MaxSunValue)*((blockTopNE.B + blockTopE.B + blockMidNE.B + blockMidE.B)/4);
                bluBL = (1f/Chunk.MaxSunValue)*((blockBotSE.B + blockBotE.B + blockMidSE.B + blockMidE.B)/4);
                bluBR = (1f/Chunk.MaxSunValue)*((blockBotNE.B + blockBotE.B + blockMidNE.B + blockMidE.B)/4);

                localTL = new Color(redTL, grnTL, bluTL);
                localTR = new Color(redTR, grnTR, bluTR);
                localBL = new Color(redBL, grnBL, bluBL);
                localBR = new Color(redBR, grnBR, bluBR);

                BuildFaceVertices(chunk, worldPosition, block.Type, BlockFaceDirection.XIncreasing, sunTL, sunTR, sunBL, sunBR, localTL, localTR, localBL, localBR);
            }
            // -Y face.
            if (!blockBotM.Exists && !(block.Type == BlockType.Water && blockBotM.Type == BlockType.Water))
            {
                sunBL = (1f/Chunk.MaxSunValue)*((blockBotSW.Sun + blockBotS.Sun + blockBotM.Sun + blockTopW.Sun)/4);
                sunBR = (1f/Chunk.MaxSunValue)*((blockBotSE.Sun + blockBotS.Sun + blockBotM.Sun + blockTopE.Sun)/4);
                sunTL = (1f/Chunk.MaxSunValue)*((blockBotNW.Sun + blockBotN.Sun + blockBotM.Sun + blockTopW.Sun)/4);
                sunTR = (1f/Chunk.MaxSunValue)*((blockBotNE.Sun + blockBotN.Sun + blockBotM.Sun + blockTopE.Sun)/4);

                redBL = (1f/Chunk.MaxSunValue)*((blockBotSW.R + blockBotS.R + blockBotM.R + blockTopW.R)/4);
                redBR = (1f/Chunk.MaxSunValue)*((blockBotSE.R + blockBotS.R + blockBotM.R + blockTopE.R)/4);
                redTL = (1f/Chunk.MaxSunValue)*((blockBotNW.R + blockBotN.R + blockBotM.R + blockTopW.R)/4);
                redTR = (1f/Chunk.MaxSunValue)*((blockBotNE.R + blockBotN.R + blockBotM.R + blockTopE.R)/4);

                grnBL = (1f/Chunk.MaxSunValue)*((blockBotSW.G + blockBotS.G + blockBotM.G + blockTopW.G)/4);
                grnBR = (1f/Chunk.MaxSunValue)*((blockBotSE.G + blockBotS.G + blockBotM.G + blockTopE.G)/4);
                grnTL = (1f/Chunk.MaxSunValue)*((blockBotNW.G + blockBotN.G + blockBotM.G + blockTopW.G)/4);
                grnTR = (1f/Chunk.MaxSunValue)*((blockBotNE.G + blockBotN.G + blockBotM.G + blockTopE.G)/4);

                bluBL = (1f/Chunk.MaxSunValue)*((blockBotSW.B + blockBotS.B + blockBotM.B + blockTopW.B)/4);
                bluBR = (1f/Chunk.MaxSunValue)*((blockBotSE.B + blockBotS.B + blockBotM.B + blockTopE.B)/4);
                bluTL = (1f/Chunk.MaxSunValue)*((blockBotNW.B + blockBotN.B + blockBotM.B + blockTopW.B)/4);
                bluTR = (1f/Chunk.MaxSunValue)*((blockBotNE.B + blockBotN.B + blockBotM.B + blockTopE.B)/4);

                localTL = new Color(redTL, grnTL, bluTL);
                localTR = new Color(redTR, grnTR, bluTR);
                localBL = new Color(redBL, grnBL, bluBL);
                localBR = new Color(redBR, grnBR, bluBR);

                BuildFaceVertices(chunk, worldPosition, block.Type, BlockFaceDirection.YDecreasing, sunTL, sunTR, sunBL, sunBR, localTL, localTR, localBL, localBR);
            }
            // +Y face.
            if (!blockTopM.Exists && !(block.Type == BlockType.Water && blockTopM.Type == BlockType.Water))
            {
                sunTL = (1f/Chunk.MaxSunValue)*((blockTopNW.Sun + blockTopN.Sun + blockTopW.Sun + blockTopM.Sun)/4);
                sunTR = (1f/Chunk.MaxSunValue)*((blockTopNE.Sun + blockTopN.Sun + blockTopE.Sun + blockTopM.Sun)/4);
                sunBL = (1f/Chunk.MaxSunValue)*((blockTopSW.Sun + blockTopS.Sun + blockTopW.Sun + blockTopM.Sun)/4);
                sunBR = (1f/Chunk.MaxSunValue)*((blockTopSE.Sun + blockTopS.Sun + blockTopE.Sun + blockTopM.Sun)/4);

                redTL = (1f/Chunk.MaxSunValue)*((blockTopNW.R + blockTopN.R + blockTopW.R + blockTopM.R)/4);
                redTR = (1f/Chunk.MaxSunValue)*((blockTopNE.R + blockTopN.R + blockTopE.R + blockTopM.R)/4);
                redBL = (1f/Chunk.MaxSunValue)*((blockTopSW.R + blockTopS.R + blockTopW.R + blockTopM.R)/4);
                redBR = (1f/Chunk.MaxSunValue)*((blockTopSE.R + blockTopS.R + blockTopE.R + blockTopM.R)/4);

                grnTL = (1f/Chunk.MaxSunValue)*((blockTopNW.G + blockTopN.G + blockTopW.G + blockTopM.G)/4);
                grnTR = (1f/Chunk.MaxSunValue)*((blockTopNE.G + blockTopN.G + blockTopE.G + blockTopM.G)/4);
                grnBL = (1f/Chunk.MaxSunValue)*((blockTopSW.G + blockTopS.G + blockTopW.G + blockTopM.G)/4);
                grnBR = (1f/Chunk.MaxSunValue)*((blockTopSE.G + blockTopS.G + blockTopE.G + blockTopM.G)/4);

                bluTL = (1f/Chunk.MaxSunValue)*((blockTopNW.B + blockTopN.B + blockTopW.B + blockTopM.B)/4);
                bluTR = (1f/Chunk.MaxSunValue)*((blockTopNE.B + blockTopN.B + blockTopE.B + blockTopM.B)/4);
                bluBL = (1f/Chunk.MaxSunValue)*((blockTopSW.B + blockTopS.B + blockTopW.B + blockTopM.B)/4);
                bluBR = (1f/Chunk.MaxSunValue)*((blockTopSE.B + blockTopS.B + blockTopE.B + blockTopM.B)/4);

                localTL = new Color(redTL, grnTL, bluTL);
                localTR = new Color(redTR, grnTR, bluTR);
                localBL = new Color(redBL, grnBL, bluBL);
                localBR = new Color(redBR, grnBR, bluBR);

                BuildFaceVertices(chunk, worldPosition, block.Type, BlockFaceDirection.YIncreasing, sunTL, sunTR, sunBL, sunBR, localTL, localTR, localBL, localBR);
            }
            // -Z face.
            if (!blockMidS.Exists && !(block.Type == BlockType.Water && blockMidS.Type == BlockType.Water))
            {
                sunTL = (1f/Chunk.MaxSunValue)*((blockTopSW.Sun + blockTopS.Sun + blockMidSW.Sun + blockMidS.Sun)/4);
                sunTR = (1f/Chunk.MaxSunValue)*((blockTopSE.Sun + blockTopS.Sun + blockMidSE.Sun + blockMidS.Sun)/4);
                sunBL = (1f/Chunk.MaxSunValue)*((blockBotSW.Sun + blockBotS.Sun + blockMidSW.Sun + blockMidS.Sun)/4);
                sunBR = (1f/Chunk.MaxSunValue)*((blockBotSE.Sun + blockBotS.Sun + blockMidSE.Sun + blockMidS.Sun)/4);

                redTL = (1f/Chunk.MaxSunValue)*((blockTopSW.R + blockTopS.R + blockMidSW.R + blockMidS.R)/4);
                redTR = (1f/Chunk.MaxSunValue)*((blockTopSE.R + blockTopS.R + blockMidSE.R + blockMidS.R)/4);
                redBL = (1f/Chunk.MaxSunValue)*((blockBotSW.R + blockBotS.R + blockMidSW.R + blockMidS.R)/4);
                redBR = (1f/Chunk.MaxSunValue)*((blockBotSE.R + blockBotS.R + blockMidSE.R + blockMidS.R)/4);

                grnTL = (1f/Chunk.MaxSunValue)*((blockTopSW.G + blockTopS.G + blockMidSW.G + blockMidS.G)/4);
                grnTR = (1f/Chunk.MaxSunValue)*((blockTopSE.G + blockTopS.G + blockMidSE.G + blockMidS.G)/4);
                grnBL = (1f/Chunk.MaxSunValue)*((blockBotSW.G + blockBotS.G + blockMidSW.G + blockMidS.G)/4);
                grnBR = (1f/Chunk.MaxSunValue)*((blockBotSE.G + blockBotS.G + blockMidSE.G + blockMidS.G)/4);

                bluTL = (1f/Chunk.MaxSunValue)*((blockTopSW.B + blockTopS.B + blockMidSW.B + blockMidS.B)/4);
                bluTR = (1f/Chunk.MaxSunValue)*((blockTopSE.B + blockTopS.B + blockMidSE.B + blockMidS.B)/4);
                bluBL = (1f/Chunk.MaxSunValue)*((blockBotSW.B + blockBotS.B + blockMidSW.B + blockMidS.B)/4);
                bluBR = (1f/Chunk.MaxSunValue)*((blockBotSE.B + blockBotS.B + blockMidSE.B + blockMidS.B)/4);

                localTL = new Color(redTL, grnTL, bluTL);
                localTR = new Color(redTR, grnTR, bluTR);
                localBL = new Color(redBL, grnBL, bluBL);
                localBR = new Color(redBR, grnBR, bluBR);

                BuildFaceVertices(chunk, worldPosition, block.Type, BlockFaceDirection.ZDecreasing, sunTL, sunTR, sunBL, sunBR, localTL, localTR, localBL, localBR);
            }
            // +Z face.
            if (!blockMidN.Exists && !(block.Type == BlockType.Water && blockMidN.Type == BlockType.Water))
            {
                sunTL = (1f/Chunk.MaxSunValue)*((blockTopNE.Sun + blockTopN.Sun + blockMidNE.Sun + blockMidN.Sun)/4);
                sunTR = (1f/Chunk.MaxSunValue)*((blockTopNW.Sun + blockTopN.Sun + blockMidNW.Sun + blockMidN.Sun)/4);
                sunBL = (1f/Chunk.MaxSunValue)*((blockBotNE.Sun + blockBotN.Sun + blockMidNE.Sun + blockMidN.Sun)/4);
                sunBR = (1f/Chunk.MaxSunValue)*((blockBotNW.Sun + blockBotN.Sun + blockMidNW.Sun + blockMidN.Sun)/4);

                redTL = (1f/Chunk.MaxSunValue)*((blockTopNE.R + blockTopN.R + blockMidNE.R + blockMidN.R)/4);
                redTR = (1f/Chunk.MaxSunValue)*((blockTopNW.R + blockTopN.R + blockMidNW.R + blockMidN.R)/4);
                redBL = (1f/Chunk.MaxSunValue)*((blockBotNE.R + blockBotN.R + blockMidNE.R + blockMidN.R)/4);
                redBR = (1f/Chunk.MaxSunValue)*((blockBotNW.R + blockBotN.R + blockMidNW.R + blockMidN.R)/4);

                grnTL = (1f/Chunk.MaxSunValue)*((blockTopNE.G + blockTopN.G + blockMidNE.G + blockMidN.G)/4);
                grnTR = (1f/Chunk.MaxSunValue)*((blockTopNW.G + blockTopN.G + blockMidNW.G + blockMidN.G)/4);
                grnBL = (1f/Chunk.MaxSunValue)*((blockBotNE.G + blockBotN.G + blockMidNE.G + blockMidN.G)/4);
                grnBR = (1f/Chunk.MaxSunValue)*((blockBotNW.G + blockBotN.G + blockMidNW.G + blockMidN.G)/4);

                bluTL = (1f/Chunk.MaxSunValue)*((blockTopNE.B + blockTopN.B + blockMidNE.B + blockMidN.B)/4);
                bluTR = (1f/Chunk.MaxSunValue)*((blockTopNW.B + blockTopN.B + blockMidNW.B + blockMidN.B)/4);
                bluBL = (1f/Chunk.MaxSunValue)*((blockBotNE.B + blockBotN.B + blockMidNE.B + blockMidN.B)/4);
                bluBR = (1f/Chunk.MaxSunValue)*((blockBotNW.B + blockBotN.B + blockMidNW.B + blockMidN.B)/4);

                localTL = new Color(redTL, grnTL, bluTL);
                localTR = new Color(redTR, grnTR, bluTR);
                localBL = new Color(redBL, grnBL, bluBL);
                localBR = new Color(redBR, grnBR, bluBR);

                BuildFaceVertices(chunk, worldPosition, block.Type, BlockFaceDirection.ZIncreasing, sunTL, sunTR, sunBL, sunBR, localTL, localTR, localBL, localBR);
            }
        }

        private static void BuildFaceVertices(Chunk chunk, Vector3Int position, BlockType blockType, BlockFaceDirection faceDir, 
                                                float sunLightTL, float sunLightTR, float sunLightBL, float sunLightBR, 
                                                Color localLightTL, Color localLightTR, Color localLightBL, Color localLightBR)
        {
            if (chunk.Disposed) return;

            BlockTexture texture = Block.GetTexture(blockType, faceDir);

            int faceIndex = 0; // techcraft actually uses (int)faceDir here. Further investigate it. /raist.
            HalfVector2[] textureUVMappings = TextureHelper.BlockTextureMappings[(int) texture*6 + faceIndex];

            //int crackStage = 0;
            //HalfVector2[] crackUVMappings = TextureHelper.BlockTextureMappings[crackStage * 6 + faceIndex];

            switch (faceDir)
            {
                case BlockFaceDirection.XIncreasing:
                {
                    //TR,TL,BR,BR,TL,BL
                    AddVertex(chunk, position, new Vector3(1, 1, 1), textureUVMappings[0], sunLightTR, localLightTR);
                    AddVertex(chunk, position, new Vector3(1, 1, 0), textureUVMappings[1], sunLightTL, localLightTL);
                    AddVertex(chunk, position, new Vector3(1, 0, 1), textureUVMappings[2], sunLightBR, localLightBR);
                    AddVertex(chunk, position, new Vector3(1, 0, 0), textureUVMappings[5], sunLightBL, localLightBL);
                    AddIndex(chunk, 0, 1, 2, 2, 1, 3);
                }
                break;

                case BlockFaceDirection.XDecreasing:
                {
                    //TR,TL,BL,TR,BL,BR
                    AddVertex(chunk, position, new Vector3(0, 1, 0), textureUVMappings[0], sunLightTR, localLightTR);
                    AddVertex(chunk, position, new Vector3(0, 1, 1), textureUVMappings[1], sunLightTL, localLightTL);
                    AddVertex(chunk, position, new Vector3(0, 0, 0), textureUVMappings[5], sunLightBR, localLightBR);
                    AddVertex(chunk, position, new Vector3(0, 0, 1), textureUVMappings[2], sunLightBL, localLightBL);
                    AddIndex(chunk, 0, 1, 3, 0, 3, 2);
                }
                break;

                case BlockFaceDirection.YIncreasing:
                {
                    //BL,BR,TR,BL,TR,TL
                    AddVertex(chunk, position, new Vector3(1, 1, 1), textureUVMappings[0], sunLightTR, localLightTR);
                    AddVertex(chunk, position, new Vector3(0, 1, 1), textureUVMappings[2], sunLightTL, localLightTL);
                    AddVertex(chunk, position, new Vector3(1, 1, 0), textureUVMappings[4], sunLightBR, localLightBR);
                    AddVertex(chunk, position, new Vector3(0, 1, 0), textureUVMappings[5], sunLightBL, localLightBL);
                    AddIndex(chunk, 3, 2, 0, 3, 0, 1);
                }
                break;

                case BlockFaceDirection.YDecreasing:
                {
                    //TR,BR,TL,TL,BR,BL
                    AddVertex(chunk, position, new Vector3(1, 0, 1), textureUVMappings[0], sunLightTR, localLightTR);
                    AddVertex(chunk, position, new Vector3(0, 0, 1), textureUVMappings[2], sunLightTL, localLightTL);
                    AddVertex(chunk, position, new Vector3(1, 0, 0), textureUVMappings[4], sunLightBR, localLightBR);
                    AddVertex(chunk, position, new Vector3(0, 0, 0), textureUVMappings[5], sunLightBL, localLightBL);
                    AddIndex(chunk, 0, 2, 1, 1, 2, 3);
                }
                break;

                case BlockFaceDirection.ZIncreasing:
                {
                    //TR,TL,BL,TR,BL,BR
                    AddVertex(chunk, position, new Vector3(0, 1, 1), textureUVMappings[0], sunLightTR, localLightTR);
                    AddVertex(chunk, position, new Vector3(1, 1, 1), textureUVMappings[1], sunLightTL, localLightTL);
                    AddVertex(chunk, position, new Vector3(0, 0, 1), textureUVMappings[5], sunLightBR, localLightBR);
                    AddVertex(chunk, position, new Vector3(1, 0, 1), textureUVMappings[2], sunLightBL, localLightBL);
                    AddIndex(chunk, 0, 1, 3, 0, 3, 2);
                }
                break;

                case BlockFaceDirection.ZDecreasing:
                {
                    //TR,TL,BR,BR,TL,BL
                    AddVertex(chunk, position, new Vector3(1, 1, 0), textureUVMappings[0], sunLightTR, localLightTR);
                    AddVertex(chunk, position, new Vector3(0, 1, 0), textureUVMappings[1], sunLightTL, localLightTL);
                    AddVertex(chunk, position, new Vector3(1, 0, 0), textureUVMappings[2], sunLightBR, localLightBR);
                    AddVertex(chunk, position, new Vector3(0, 0, 0), textureUVMappings[5], sunLightBL, localLightBL);
                    AddIndex(chunk, 0, 1, 2, 2, 1, 3);
                }
                break;
            }
        }

        private static void AddVertex(Chunk chunk, Vector3Int position, Vector3 addition, HalfVector2 textureCoordinate, float sunlight, Color localLight)
        {
            chunk.VertexList.Add(new BlockVertex(position.AsVector3() + addition, textureCoordinate, sunlight));

            //chunk.VertexList.Add(new BlockVertex(position.AsVector3() + addition, textureCoordinate, sunlight, localLight.ToVector3()));            
        }

        private static void AddIndex(Chunk chunk, short i1, short i2, short i3, short i4, short i5, short i6)
        {
            chunk.IndexList.Add((short) (chunk.Index + i1));
            chunk.IndexList.Add((short) (chunk.Index + i2));
            chunk.IndexList.Add((short) (chunk.Index + i3));
            chunk.IndexList.Add((short) (chunk.Index + i4));
            chunk.IndexList.Add((short) (chunk.Index + i5));
            chunk.IndexList.Add((short) (chunk.Index + i6));
            chunk.Index += 4;
        }
    }
}