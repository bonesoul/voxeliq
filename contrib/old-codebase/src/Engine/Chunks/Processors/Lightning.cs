/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Engine.Blocks;

namespace Engine.Chunks.Processors
{
    /// <summary>
    /// Ligten processor.
    /// </summary>
    public static class Lightning
    {
        public static void Process(Chunk chunk)
        {            
            if (chunk.ChunkState != ChunkState.AwaitingLighting && chunk.ChunkState != ChunkState.AwaitingRelighting) // if chunk is not awaiting lighting
                return; // just pass it.

            chunk.ChunkState = ChunkState.Lighting; // set chunk state to generating.

            SetInitialLighting(chunk);
            FluidFillSunLight(chunk);
            FluidFillLightR(chunk);
            FluidFillLightG(chunk);
            FluidFillLightB(chunk);

            chunk.ChunkState = ChunkState.AwaitingBuild; // chunk should be built now.
        }

        /// <summary>
        /// Clears all lighting and then applies sun-lighting.
        /// </summary>
        /// <param name="chunk"></param>
        private static void SetInitialLighting(Chunk chunk)
        {
            byte sunValue = Chunk.MaxSunValue;

            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (byte z = 0; z < Chunk.LengthInBlocks; z++)
                {
                    int offset = BlockStorage.BlockIndexByRelativePosition(chunk, x, z);
                    bool inShade = false; // if we get direct sunlight, inShade will be set to false.

                    for (byte y = Chunk.MaxHeightIndexInBlocks; y > 0; y--)
                    {
                        if(!inShade && BlockStorage.Blocks[offset + y].Type != BlockType.None) // if we're under direct sunlight and just hit a solid block.
                                inShade = true; // set inShade to true;

                        BlockStorage.Blocks[offset + y].Sun = inShade ? (byte)0 : sunValue;
                        BlockStorage.Blocks[offset + y].R = 0;
                        BlockStorage.Blocks[offset + y].G = 0;
                        BlockStorage.Blocks[offset + y].B = 0;
                    }
                }
            }
        }

        #region sunlighting

        private static void FluidFillSunLight(Chunk chunk)
        {
            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (byte z = 0; z < Chunk.LengthInBlocks; z++)
                {
                    int offset = BlockStorage.BlockIndexByRelativePosition(chunk, x, z);

                    for (byte y = Chunk.MaxHeightIndexInBlocks; y > 0; y--)
                    {
                        var blockIndex = offset + y;

                        if (BlockStorage.Blocks[blockIndex].Type != BlockType.None) // solid blocks can't propagate light.
                            continue;

                        var blockLight = BlockStorage.Blocks[blockIndex].Sun;
                        if (blockLight <= 1) // if block's light value is too low (dark),
                            continue; // just skip it.

                        var propagatedLight = (byte)((blockLight*9)/10);

                        PropagateSunLight(blockIndex + BlockStorage.XStep, propagatedLight); // propagate light to block in east.
                        PropagateSunLight(blockIndex - BlockStorage.XStep, propagatedLight); // propagate light to block in west.
                        PropagateSunLight(blockIndex + BlockStorage.ZStep, propagatedLight); // propagate light to block in north.
                        PropagateSunLight(blockIndex - BlockStorage.ZStep, propagatedLight); // propagate light to block in south.
                        // DO NOT repropagete to upper block which we don't need to do so and may cause loops!
                        PropagateSunLight(blockIndex - 1, propagatedLight);   // propagate light to block down.
                    }
                }
            }
        }

        private static void PropagateSunLight(int blockIndex, byte incomingLight)
        {
            // make sure block index is within view range.
            blockIndex = blockIndex%BlockStorage.Blocks.Length;
            if (blockIndex < 0)
                blockIndex += BlockStorage.Blocks.Length;

            if (incomingLight <= 1) // if incoming light is too dim, stop propagating.
                return;

            if (BlockStorage.Blocks[blockIndex].Type != BlockType.None) // if we got a solid block we can't propegate light to it.
                return;

            if (incomingLight <= BlockStorage.Blocks[blockIndex].Sun) // if incoming light is already lower than blocks current light, stop propagating.
                return;

            BlockStorage.Blocks[blockIndex].Sun = incomingLight; // set the incoming light.            

            // continue propagation.
            var propagatedLight = (byte)((incomingLight * 9) / 10);

            PropagateSunLight(blockIndex + BlockStorage.XStep, propagatedLight); // propagate light to block in east.
            PropagateSunLight(blockIndex - BlockStorage.XStep, propagatedLight); // propagate light to block in west.
            PropagateSunLight(blockIndex + BlockStorage.ZStep, propagatedLight); // propagate light to block in north.
            PropagateSunLight(blockIndex - BlockStorage.ZStep, propagatedLight); // propagate light to block in south.
            // DO NOT repropagete to upper block which we don't need to do so and may cause loops!
            PropagateSunLight(blockIndex - 1, propagatedLight);   // propagate light to block down.
        }

        #endregion

        #region red lighting

        private static void FluidFillLightR(Chunk chunk)
        {
            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (byte z = 0; z < Chunk.LengthInBlocks; z++)
                {
                    int offset = BlockStorage.BlockIndexByRelativePosition(chunk, x, z);

                    for (byte y = Chunk.MaxHeightIndexInBlocks; y > 0; y--)
                    {
                        var blockIndex = offset + y;

                        if (BlockStorage.Blocks[blockIndex].Type != BlockType.None) // solid blocks can't propagate light.
                            continue;

                        var blockLight = BlockStorage.Blocks[blockIndex].R;
                        if (blockLight < 1) // if block's light value is too low (dark),
                            continue; // just skip it.

                        var propagatedLight = (byte)((blockLight * 9) / 10);

                        PropagateLightR(blockIndex + BlockStorage.XStep, propagatedLight); // propagate light to block in east.
                        PropagateLightR(blockIndex - BlockStorage.XStep, propagatedLight); // propagate light to block in west.
                        PropagateLightR(blockIndex + BlockStorage.ZStep, propagatedLight); // propagate light to block in north.
                        PropagateLightR(blockIndex - BlockStorage.ZStep, propagatedLight); // propagate light to block in south.
                        // DO NOT repropagete to upper block which we don't need to do so and may cause loops!
                        PropagateLightR(blockIndex - 1, propagatedLight);   // propagate light to block down.
                    }
                }
            }
        }

        private static void PropagateLightR(int blockIndex, byte incomingLight)
        {
            // make sure block index is within view range.
            blockIndex = blockIndex % BlockStorage.Blocks.Length;
            if (blockIndex < 0)
                blockIndex += BlockStorage.Blocks.Length;

            if (incomingLight <= 1) // if incoming light is too dim, stop propagating.
                return;

            if (BlockStorage.Blocks[blockIndex].Type != BlockType.None) // if we got a solid block we can't propegate light to it.
                return;

            if (incomingLight <= BlockStorage.Blocks[blockIndex].R) // if incoming light is already lower than blocks current light, stop propagating.
                return;

            BlockStorage.Blocks[blockIndex].R = incomingLight; // set the incoming light.            

            // continue propagation.
            var propagatedLight = (byte)((incomingLight * 9) / 10);

            PropagateLightR(blockIndex + BlockStorage.XStep, propagatedLight); // propagate light to block in east.
            PropagateLightR(blockIndex - BlockStorage.XStep, propagatedLight); // propagate light to block in west.
            PropagateLightR(blockIndex + BlockStorage.ZStep, propagatedLight); // propagate light to block in north.
            PropagateLightR(blockIndex - BlockStorage.ZStep, propagatedLight); // propagate light to block in south.
            // DO NOT repropagete to upper block which we don't need to do so and may cause loops!
            PropagateLightR(blockIndex - 1, propagatedLight);   // propagate light to block down.
        }

        #endregion

        #region green lighting

        private static void FluidFillLightG(Chunk chunk)
        {
            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (byte z = 0; z < Chunk.LengthInBlocks; z++)
                {
                    int offset = BlockStorage.BlockIndexByRelativePosition(chunk, x, z);

                    for (byte y = Chunk.MaxHeightIndexInBlocks; y > 0; y--)
                    {
                        var blockIndex = offset + y;

                        if (BlockStorage.Blocks[blockIndex].Type != BlockType.None) // solid blocks can't propagate light.
                            continue;

                        var blockLight = BlockStorage.Blocks[blockIndex].G;
                        if (blockLight < 1) // if block's light value is too low (dark),
                            continue; // just skip it.

                        var propagatedLight = (byte)((blockLight * 9) / 10);

                        PropagateLightG(blockIndex + BlockStorage.XStep, propagatedLight); // propagate light to block in east.
                        PropagateLightG(blockIndex - BlockStorage.XStep, propagatedLight); // propagate light to block in west.
                        PropagateLightG(blockIndex + BlockStorage.ZStep, propagatedLight); // propagate light to block in north.
                        PropagateLightG(blockIndex - BlockStorage.ZStep, propagatedLight); // propagate light to block in south.
                        // DO NOT repropagete to upper block which we don't need to do so and may cause loops!
                        PropagateLightG(blockIndex - 1, propagatedLight);   // propagate light to block down.
                    }
                }
            }
        }

        private static void PropagateLightG(int blockIndex, byte incomingLight)
        {
            // make sure block index is within view range.
            blockIndex = blockIndex % BlockStorage.Blocks.Length;
            if (blockIndex < 0)
                blockIndex += BlockStorage.Blocks.Length;

            if (incomingLight <= 1) // if incoming light is too dim, stop propagating.
                return;

            if (BlockStorage.Blocks[blockIndex].Type != BlockType.None) // if we got a solid block we can't propegate light to it.
                return;

            if (incomingLight <= BlockStorage.Blocks[blockIndex].G) // if incoming light is already lower than blocks current light, stop propagating.
                return;

            BlockStorage.Blocks[blockIndex].G = incomingLight; // set the incoming light.            

            // continue propagation.
            var propagatedLight = (byte)((incomingLight * 9) / 10);

            PropagateLightG(blockIndex + BlockStorage.XStep, propagatedLight); // propagate light to block in east.
            PropagateLightG(blockIndex - BlockStorage.XStep, propagatedLight); // propagate light to block in west.
            PropagateLightG(blockIndex + BlockStorage.ZStep, propagatedLight); // propagate light to block in north.
            PropagateLightG(blockIndex - BlockStorage.ZStep, propagatedLight); // propagate light to block in south.
            // DO NOT repropagete to upper block which we don't need to do so and may cause loops!
            PropagateLightG(blockIndex - 1, propagatedLight);   // propagate light to block down.
        }

        #endregion

        #region blue lighting

        private static void FluidFillLightB(Chunk chunk)
        {
            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (byte z = 0; z < Chunk.LengthInBlocks; z++)
                {
                    int offset = BlockStorage.BlockIndexByRelativePosition(chunk, x, z);

                    for (byte y = Chunk.MaxHeightIndexInBlocks; y > 0; y--)
                    {
                        var blockIndex = offset + y;

                        if (BlockStorage.Blocks[blockIndex].Type != BlockType.None) // solid blocks can't propagate light.
                            continue;

                        var blockLight = BlockStorage.Blocks[blockIndex].B;
                        if (blockLight < 1) // if block's light value is too low (dark),
                            continue; // just skip it.

                        var propagatedLight = (byte)((blockLight * 9) / 10);

                        PropagateLightB(blockIndex + BlockStorage.XStep, propagatedLight); // propagate light to block in east.
                        PropagateLightB(blockIndex - BlockStorage.XStep, propagatedLight); // propagate light to block in west.
                        PropagateLightB(blockIndex + BlockStorage.ZStep, propagatedLight); // propagate light to block in north.
                        PropagateLightB(blockIndex - BlockStorage.ZStep, propagatedLight); // propagate light to block in south.
                        // DO NOT repropagete to upper block which we don't need to do so and may cause loops!
                        PropagateLightB(blockIndex - 1, propagatedLight);   // propagate light to block down.
                    }
                }
            }
        }

        private static void PropagateLightB(int blockIndex, byte incomingLight)
        {
            // make sure block index is within view range.
            blockIndex = blockIndex % BlockStorage.Blocks.Length;
            if (blockIndex < 0)
                blockIndex += BlockStorage.Blocks.Length;

            if (incomingLight <= 1) // if incoming light is too dim, stop propagating.
                return;

            if (BlockStorage.Blocks[blockIndex].Type != BlockType.None) // if we got a solid block we can't propegate light to it.
                return;

            if (incomingLight <= BlockStorage.Blocks[blockIndex].B) // if incoming light is already lower than blocks current light, stop propagating.
                return;

            BlockStorage.Blocks[blockIndex].B = incomingLight; // set the incoming light.            

            // continue propagation.
            var propagatedLight = (byte)((incomingLight * 9) / 10);

            PropagateLightB(blockIndex + BlockStorage.XStep, propagatedLight); // propagate light to block in east.
            PropagateLightB(blockIndex - BlockStorage.XStep, propagatedLight); // propagate light to block in west.
            PropagateLightB(blockIndex + BlockStorage.ZStep, propagatedLight); // propagate light to block in north.
            PropagateLightB(blockIndex - BlockStorage.ZStep, propagatedLight); // propagate light to block in south.
            // DO NOT repropagete to upper block which we don't need to do so and may cause loops!
            PropagateLightB(blockIndex - 1, propagatedLight);   // propagate light to block down.
        }

        #endregion

        /* todo: put in a configurable attenuate function. 
        private static byte Attenuate(byte light)
        {
            return (byte) ((light*9)/10);
        }
        */       
    }
}
