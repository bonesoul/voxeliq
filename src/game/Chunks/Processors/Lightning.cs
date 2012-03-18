/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using VolumetricStudios.VoxeliqGame.Blocks;

namespace VolumetricStudios.VoxeliqGame.Chunks.Processors
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
                for (byte z = 0; z < Chunk.LenghtInBlocks; z++)
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
                for (byte z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    int offset = BlockStorage.BlockIndexByRelativePosition(chunk, x, z);

                    for (byte y = Chunk.MaxHeightIndexInBlocks; y > 0; y--)
                    {
                        var blockIndex = offset + y;

                        if (BlockStorage.Blocks[blockIndex].Type != BlockType.None) // solid blocks can't propagate light.
                            continue;

                        var light = BlockStorage.Blocks[blockIndex].Sun;
                        if (light <= 1) // if block's light value is too low (dark),
                            continue; // just skip it.

                        light--; // dim the light a bit.

                        PropagateSunLight(blockIndex + BlockStorage.XStep, light); // propagate light to block in east.
                        PropagateSunLight(blockIndex - BlockStorage.XStep, light); // propagate light to block in west.
                        PropagateSunLight(blockIndex + BlockStorage.ZStep, light); // propagate light to block in north.
                        PropagateSunLight(blockIndex - BlockStorage.ZStep, light); // propagate light to block in south.
                        // DO NOT repropagete to upper block which we don't need to do so and may cause loops!
                        PropagateSunLight(blockIndex - 1, light);   // propagate light to block down.
                    }
                }
            }
        }

        private static void PropagateSunLight(int blockIndex, byte light)
        {
            // make sure block index is within view range.
            blockIndex = blockIndex%BlockStorage.Blocks.Length;
            if (blockIndex < 0)
                blockIndex += BlockStorage.Blocks.Length;

            if (light <= 1) // if incoming light is too dim, stop propagating.
                return;

            if (BlockStorage.Blocks[blockIndex].Type != BlockType.None) // if we got a solid block we can't propegate light to it.
                return;

            if (light <= BlockStorage.Blocks[blockIndex].Sun) // if incoming light is already lower than blocks current light, stop propagating.
                return;

            BlockStorage.Blocks[blockIndex].Sun = light; // set the incoming light.            

            light--; // dim the light a bit.

            PropagateSunLight(blockIndex + BlockStorage.XStep, light); // propagate light to block in east.
            PropagateSunLight(blockIndex - BlockStorage.XStep, light); // propagate light to block in west.
            PropagateSunLight(blockIndex + BlockStorage.ZStep, light); // propagate light to block in north.
            PropagateSunLight(blockIndex - BlockStorage.ZStep, light); // propagate light to block in south.
            // DO NOT repropagete to upper block which we don't need to do so and may cause loops!
            PropagateSunLight(blockIndex - 1, light);   // propagate light to block down.
        }

        #endregion

        #region red lighting

        private static void FluidFillLightR(Chunk chunk)
        {
            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (byte z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    int offset = BlockStorage.BlockIndexByRelativePosition(chunk, x, z);

                    for (byte y = Chunk.MaxHeightIndexInBlocks; y > 0; y--)
                    {
                        var blockIndex = offset + y;

                        if (BlockStorage.Blocks[blockIndex].Type != BlockType.None) // solid blocks can't propagate light.
                            continue;

                        var light = BlockStorage.Blocks[blockIndex].R;
                        if (light < 1) // if block's light value is too low (dark),
                            continue; // just skip it.

                        light--; // dim the light a bit.

                        PropagateLightR(blockIndex + BlockStorage.XStep, light); // propagate light to block in east.
                        PropagateLightR(blockIndex - BlockStorage.XStep, light); // propagate light to block in west.
                        PropagateLightR(blockIndex + BlockStorage.ZStep, light); // propagate light to block in north.
                        PropagateLightR(blockIndex - BlockStorage.ZStep, light); // propagate light to block in south.
                        // DO NOT repropagete to upper block which we don't need to do so and may cause loops!
                        PropagateLightR(blockIndex - 1, light);   // propagate light to block down.
                    }
                }
            }
        }

        private static void PropagateLightR(int blockIndex, byte light)
        {
            // make sure block index is within view range.
            blockIndex = blockIndex % BlockStorage.Blocks.Length;
            if (blockIndex < 0)
                blockIndex += BlockStorage.Blocks.Length;

            if (light <= 1) // if incoming light is too dim, stop propagating.
                return;

            if (BlockStorage.Blocks[blockIndex].Type != BlockType.None) // if we got a solid block we can't propegate light to it.
                return;

            if (light <= BlockStorage.Blocks[blockIndex].R) // if incoming light is already lower than blocks current light, stop propagating.
                return;

            BlockStorage.Blocks[blockIndex].R = light; // set the incoming light.            

            light--; // dim the light a bit.

            PropagateLightR(blockIndex + BlockStorage.XStep, light); // propagate light to block in east.
            PropagateLightR(blockIndex - BlockStorage.XStep, light); // propagate light to block in west.
            PropagateLightR(blockIndex + BlockStorage.ZStep, light); // propagate light to block in north.
            PropagateLightR(blockIndex - BlockStorage.ZStep, light); // propagate light to block in south.
            // DO NOT repropagete to upper block which we don't need to do so and may cause loops!
            PropagateLightR(blockIndex - 1, light);   // propagate light to block down.
        }

        #endregion

        #region green lighting

        private static void FluidFillLightG(Chunk chunk)
        {
            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (byte z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    int offset = BlockStorage.BlockIndexByRelativePosition(chunk, x, z);

                    for (byte y = Chunk.MaxHeightIndexInBlocks; y > 0; y--)
                    {
                        var blockIndex = offset + y;

                        if (BlockStorage.Blocks[blockIndex].Type != BlockType.None) // solid blocks can't propagate light.
                            continue;

                        var light = BlockStorage.Blocks[blockIndex].G;
                        if (light < 1) // if block's light value is too low (dark),
                            continue; // just skip it.

                        light--; // dim the light a bit.

                        PropagateLightG(blockIndex + BlockStorage.XStep, light); // propagate light to block in east.
                        PropagateLightG(blockIndex - BlockStorage.XStep, light); // propagate light to block in west.
                        PropagateLightG(blockIndex + BlockStorage.ZStep, light); // propagate light to block in north.
                        PropagateLightG(blockIndex - BlockStorage.ZStep, light); // propagate light to block in south.
                        // DO NOT repropagete to upper block which we don't need to do so and may cause loops!
                        PropagateLightG(blockIndex - 1, light);   // propagate light to block down.
                    }
                }
            }
        }

        private static void PropagateLightG(int blockIndex, byte light)
        {
            // make sure block index is within view range.
            blockIndex = blockIndex % BlockStorage.Blocks.Length;
            if (blockIndex < 0)
                blockIndex += BlockStorage.Blocks.Length;

            if (light <= 1) // if incoming light is too dim, stop propagating.
                return;

            if (BlockStorage.Blocks[blockIndex].Type != BlockType.None) // if we got a solid block we can't propegate light to it.
                return;

            if (light <= BlockStorage.Blocks[blockIndex].G) // if incoming light is already lower than blocks current light, stop propagating.
                return;

            BlockStorage.Blocks[blockIndex].G = light; // set the incoming light.            

            light--; // dim the light a bit.

            PropagateLightG(blockIndex + BlockStorage.XStep, light); // propagate light to block in east.
            PropagateLightG(blockIndex - BlockStorage.XStep, light); // propagate light to block in west.
            PropagateLightG(blockIndex + BlockStorage.ZStep, light); // propagate light to block in north.
            PropagateLightG(blockIndex - BlockStorage.ZStep, light); // propagate light to block in south.
            // DO NOT repropagete to upper block which we don't need to do so and may cause loops!
            PropagateLightG(blockIndex - 1, light);   // propagate light to block down.
        }

        #endregion

        #region blue lighting

        private static void FluidFillLightB(Chunk chunk)
        {
            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (byte z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    int offset = BlockStorage.BlockIndexByRelativePosition(chunk, x, z);

                    for (byte y = Chunk.MaxHeightIndexInBlocks; y > 0; y--)
                    {
                        var blockIndex = offset + y;

                        if (BlockStorage.Blocks[blockIndex].Type != BlockType.None) // solid blocks can't propagate light.
                            continue;

                        var light = BlockStorage.Blocks[blockIndex].B;
                        if (light < 1) // if block's light value is too low (dark),
                            continue; // just skip it.

                        light--; // dim the light a bit.

                        PropagateLightB(blockIndex + BlockStorage.XStep, light); // propagate light to block in east.
                        PropagateLightB(blockIndex - BlockStorage.XStep, light); // propagate light to block in west.
                        PropagateLightB(blockIndex + BlockStorage.ZStep, light); // propagate light to block in north.
                        PropagateLightB(blockIndex - BlockStorage.ZStep, light); // propagate light to block in south.
                        // DO NOT repropagete to upper block which we don't need to do so and may cause loops!
                        PropagateLightB(blockIndex - 1, light);   // propagate light to block down.
                    }
                }
            }
        }

        private static void PropagateLightB(int blockIndex, byte light)
        {
            // make sure block index is within view range.
            blockIndex = blockIndex % BlockStorage.Blocks.Length;
            if (blockIndex < 0)
                blockIndex += BlockStorage.Blocks.Length;

            if (light <= 1) // if incoming light is too dim, stop propagating.
                return;

            if (BlockStorage.Blocks[blockIndex].Type != BlockType.None) // if we got a solid block we can't propegate light to it.
                return;

            if (light <= BlockStorage.Blocks[blockIndex].B) // if incoming light is already lower than blocks current light, stop propagating.
                return;

            BlockStorage.Blocks[blockIndex].B = light; // set the incoming light.            

            // continue propagation.
            light--; // dim the light a bit.

            PropagateLightB(blockIndex + BlockStorage.XStep, light); // propagate light to block in east.
            PropagateLightB(blockIndex - BlockStorage.XStep, light); // propagate light to block in west.
            PropagateLightB(blockIndex + BlockStorage.ZStep, light); // propagate light to block in north.
            PropagateLightB(blockIndex - BlockStorage.ZStep, light); // propagate light to block in south.
            // DO NOT repropagete to upper block which we don't need to do so and may cause loops!
            PropagateLightB(blockIndex - 1, light);   // propagate light to block down.
        }

        #endregion      
    }
}
