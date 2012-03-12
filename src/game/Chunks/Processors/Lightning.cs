/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using VolumetricStudios.VoxeliqGame.Blocks;

namespace VolumetricStudios.VoxeliqGame.Chunks.Processors
{
    /// <summary>
    /// TODO: client stuff.
    /// </summary>
    public static class Lightning
    {
        public static void Process(Chunk chunk)
        {
            if (chunk.ChunkState != ChunkState.AwaitingLighting) // if chunk is not awaiting lighting
                return; // just pass it.

            chunk.ChunkState = ChunkState.Lighting; // set chunk state to generating.

            ClearLighting(chunk);
            FillLighting(chunk);

            chunk.ChunkState = ChunkState.AwaitingBuild; // chunk should be built now.
        }

        private static void ClearLighting(Chunk chunk)
        {
            byte sunValue = Chunk.MaxSunValue;

            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (byte z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    int offset = BlockStorage.BlockIndexByRelativePosition(chunk, x, z);
                    bool inShade = false;

                    for (byte y = Chunk.MaxHeightIndexInBlocks; y > 0; y--)
                    {
                        if (BlockStorage.Blocks[offset + y].Type != BlockType.None)
                            inShade = true;

                        if (!inShade)
                        {
                            BlockStorage.Blocks[offset + y].Sun = sunValue;
                        }
                        else
                        {
                            BlockStorage.Blocks[offset + y].Sun = 0;
                        }

                        BlockStorage.Blocks[offset + y].R = 0;
                        BlockStorage.Blocks[offset + y].G = 0;
                        BlockStorage.Blocks[offset + y].B = 0;
                    }
                }
            }
        }

        private static void FillLighting(Chunk chunk)
        {
            FillSunLightning(chunk);
            FillLightningR(chunk);
            FillLightningG(chunk);
            FillLightningB(chunk);
        }

        private static void FillSunLightning(Chunk chunk)
        {
            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (byte z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    int offset = BlockStorage.BlockIndexByRelativePosition(chunk, x, z);

                    for (byte y = 0; y < Chunk.HeightInBlocks; y++)
                    {
                        if (BlockStorage.Blocks[offset + y].Type == BlockType.None)
                        {
                            if (BlockStorage.Blocks[offset + y].Sun > 1)
                            {
                                byte light = Attenuate(BlockStorage.Blocks[offset + y].Sun);

                                if (x > 0) PropagateSunLight(chunk, (byte) (x - 1), y, z, light);
                                if (x < Chunk.MaxWidthIndexInBlocks) PropagateSunLight(chunk, (byte) (x + 1), y, z, light);
                                if (y > 0) PropagateSunLight(chunk, x, (byte) (y - 1), z, light);
                                if (y < Chunk.MaxHeightIndexInBlocks) PropagateSunLight(chunk, x, (byte) (y + 1), z, light);
                                if (z > 0) PropagateSunLight(chunk, x, y, (byte) (z - 1), light);
                                if (z < Chunk.MaxLenghtIndexInBlocks) PropagateSunLight(chunk, x, y, (byte) (z + 1), light);
                            }
                        }
                    }
                }
            }
        }

        private static byte Attenuate(byte light)
        {
            return (byte) ((light*9)/10);
        }

        private static void FillLightningR(Chunk chunk)
        {
            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (byte z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    int offset = BlockStorage.BlockIndexByRelativePosition(chunk, x, z);

                    for (byte y = 0; y < Chunk.HeightInBlocks; y++)
                    {
                        if (BlockStorage.Blocks[offset + y].Exists || BlockStorage.Blocks[offset + y].R <= 1)
                            continue;

                        var light = (byte) ((BlockStorage.Blocks[offset + y].R/10)*9);

                        if (x > 0) PropagateLightR(chunk, (byte) (x - 1), y, z, light);
                        if (x < Chunk.MaxWidthIndexInBlocks) PropagateLightR(chunk, (byte) (x + 1), y, z, light);
                        if (y > 0) PropagateLightR(chunk, x, (byte) (y - 1), z, light);
                        if (y < Chunk.MaxHeightIndexInBlocks) PropagateLightR(chunk, x, (byte) (y + 1), z, light);
                        if (z > 0) PropagateLightR(chunk, x, y, (byte) (z - 1), light);
                        if (z < Chunk.MaxLenghtIndexInBlocks) PropagateLightR(chunk, x, y, (byte) (z + 1), light);
                    }
                }
            }
        }

        private static void FillLightningG(Chunk chunk)
        {
            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (byte z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    int offset = BlockStorage.BlockIndexByRelativePosition(chunk, x, z);

                    for (byte y = 0; y < Chunk.HeightInBlocks; y++)
                    {
                        if (BlockStorage.Blocks[offset + y].Exists || BlockStorage.Blocks[offset + y].G <= 1)
                            continue;

                        var light = (byte) ((BlockStorage.Blocks[offset + y].G/10)*9);

                        if (x > 0) PropagateLightG(chunk, (byte) (x - 1), y, z, light);
                        if (x < Chunk.MaxWidthIndexInBlocks) PropagateLightG(chunk, (byte) (x + 1), y, z, light);
                        if (y > 0) PropagateLightG(chunk, x, (byte) (y - 1), z, light);
                        if (y < Chunk.MaxHeightIndexInBlocks) PropagateLightG(chunk, x, (byte) (y + 1), z, light);
                        if (z > 0) PropagateLightG(chunk, x, y, (byte) (z - 1), light);
                        if (z < Chunk.MaxLenghtIndexInBlocks) PropagateLightG(chunk, x, y, (byte) (z + 1), light);
                    }
                }
            }
        }

        private static void FillLightningB(Chunk chunk)
        {
            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (byte z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    int offset = BlockStorage.BlockIndexByRelativePosition(chunk, x, z);

                    for (byte y = 0; y < Chunk.HeightInBlocks; y++)
                    {
                        if (BlockStorage.Blocks[offset + y].Exists || BlockStorage.Blocks[offset + y].B <= 1)
                            continue;

                        var light = (byte) ((BlockStorage.Blocks[offset + y].B/10)*9);

                        if (x > 0) PropagateLightB(chunk, (byte) (x - 1), y, z, light);
                        if (x < Chunk.MaxWidthIndexInBlocks) PropagateLightB(chunk, (byte) (x + 1), y, z, light);
                        if (y > 0) PropagateLightB(chunk, x, (byte) (y - 1), z, light);
                        if (y < Chunk.MaxHeightIndexInBlocks) PropagateLightB(chunk, x, (byte) (y + 1), z, light);
                        if (z > 0) PropagateLightB(chunk, x, y, (byte) (z - 1), light);
                        if (z < Chunk.MaxLenghtIndexInBlocks) PropagateLightB(chunk, x, y, (byte) (z + 1), light);
                    }
                }
            }
        }

        private static void PropagateSunLight(Chunk chunk, byte x, byte y, byte z, byte light)
        {
            var blockIndex = BlockStorage.BlockIndexByRelativePosition(chunk, x, y, z);

            if (BlockStorage.Blocks[blockIndex].Type != BlockType.None &&
                BlockStorage.Blocks[blockIndex].Type != BlockType.Water)
                return;

            if (BlockStorage.Blocks[blockIndex].Sun >= light)
                return;

            BlockStorage.Blocks[blockIndex].Sun = light;

            if (light > 1)
            {
                light = Attenuate(light);

                if (x > 0) PropagateSunLight(chunk, (byte) (x - 1), y, z, light);
                if (x < Chunk.MaxWidthIndexInBlocks) PropagateSunLight(chunk, (byte) (x + 1), y, z, light);
                if (y > 0) PropagateSunLight(chunk, x, (byte) (y - 1), z, light);
                if (y < Chunk.MaxHeightIndexInBlocks) PropagateSunLight(chunk, x, (byte) (y + 1), z, light);
                if (z > 0) PropagateSunLight(chunk, x, y, (byte) (z - 1), light);
                if (z < Chunk.MaxLenghtIndexInBlocks) PropagateSunLight(chunk, x, y, (byte) (z + 1), light);

                if (chunk.East != null && x == 0) PropagateSunLight(chunk.East, (byte) (Chunk.MaxWidthIndexInBlocks), y, z, light);
                if (chunk.West != null && (x == Chunk.MaxWidthIndexInBlocks)) PropagateSunLight(chunk.West, 0, y, z, light);
                if (chunk.South != null && z == 0) PropagateSunLight(chunk.South, x, y, (byte) (Chunk.MaxLenghtIndexInBlocks), light);
                if (chunk.North != null && (z == Chunk.MaxLenghtIndexInBlocks)) PropagateSunLight(chunk.North, x, y, 0, light);
            }
        }

        private static void PropagateLightR(Chunk chunk, byte x, byte y, byte z, byte lightR)
        {
            var blockIndex = BlockStorage.BlockIndexByRelativePosition(chunk, x, y, z);

            if (BlockStorage.Blocks[blockIndex].Exists || BlockStorage.Blocks[blockIndex].R >= lightR)
                return;

            BlockStorage.Blocks[blockIndex].R = lightR;

            if (lightR <= 1) return;
            lightR = (byte) (lightR - 1);

            if (x > 0) PropagateLightR(chunk, (byte) (x - 1), y, z, lightR);
            if (x < Chunk.MaxWidthIndexInBlocks) PropagateLightR(chunk, (byte) (x + 1), y, z, lightR);
            if (y > 0) PropagateLightR(chunk, x, (byte) (y - 1), z, lightR);
            if (y < Chunk.MaxHeightIndexInBlocks) PropagateLightR(chunk, x, (byte) (y + 1), z, lightR);
            if (z > 0) PropagateLightR(chunk, x, y, (byte) (z - 1), lightR);
            if (z < Chunk.MaxLenghtIndexInBlocks) PropagateLightR(chunk, x, y, (byte) (z + 1), lightR);
        }

        private static void PropagateLightG(Chunk chunk, byte x, byte y, byte z, byte lightG)
        {
            var blockIndex = BlockStorage.BlockIndexByRelativePosition(chunk, x, y, z);

            if (BlockStorage.Blocks[blockIndex].Exists || BlockStorage.Blocks[blockIndex].G >= lightG)
                return;

            BlockStorage.Blocks[blockIndex].G = lightG;

            if (lightG <= 1) return;
            lightG = (byte) (lightG - 1);

            if (x > 0) PropagateLightG(chunk, (byte) (x - 1), y, z, lightG);
            if (x < Chunk.MaxWidthIndexInBlocks) PropagateLightG(chunk, (byte) (x + 1), y, z, lightG);
            if (y > 0) PropagateLightG(chunk, x, (byte) (y - 1), z, lightG);
            if (y < Chunk.MaxHeightIndexInBlocks) PropagateLightG(chunk, x, (byte) (y + 1), z, lightG);
            if (z > 0) PropagateLightG(chunk, x, y, (byte) (z - 1), lightG);
            if (z < Chunk.MaxLenghtIndexInBlocks) PropagateLightG(chunk, x, y, (byte) (z + 1), lightG);
        }

        private static void PropagateLightB(Chunk chunk, byte x, byte y, byte z, byte lightB)
        {
            var blockIndex = BlockStorage.BlockIndexByRelativePosition(chunk, x, y, z);

            if (BlockStorage.Blocks[blockIndex].Exists || BlockStorage.Blocks[blockIndex].B >= lightB)
                return;

            BlockStorage.Blocks[blockIndex].B = lightB;

            if (lightB <= 1) return;
            lightB = (byte) (lightB - 1);

            if (x > 0) PropagateLightB(chunk, (byte) (x - 1), y, z, lightB);
            if (x < Chunk.MaxWidthIndexInBlocks) PropagateLightB(chunk, (byte) (x + 1), y, z, lightB);
            if (y > 0) PropagateLightB(chunk, x, (byte) (y - 1), z, lightB);
            if (y < Chunk.MaxHeightIndexInBlocks) PropagateLightB(chunk, x, (byte) (y + 1), z, lightB);
            if (z > 0) PropagateLightB(chunk, x, y, (byte) (z - 1), lightB);
            if (z < Chunk.MaxLenghtIndexInBlocks) PropagateLightB(chunk, x, y, (byte) (z + 1), lightB);
        }
    }
}