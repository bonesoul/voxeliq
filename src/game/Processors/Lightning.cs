/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using VolumetricStudios.VoxeliqGame.Blocks;
using VolumetricStudios.VoxeliqGame.Chunks;

namespace VolumetricStudios.VoxeliqGame.Processors
{
    /// <summary>
    /// TODO: client stuff.
    /// </summary>
    public static class Lightning
    {        
        public static void Process(Chunk chunk)
        {
            ClearLighting(chunk);
            FillLighting(chunk);
        }

        private static void ClearLighting(Chunk chunk)
        {
            byte sunValue = Chunk.MaxSunValue;

            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (byte z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    //int offset = x*Chunk.FlattenOffset + z*Chunk.HeightInBlocks;
                    bool inShade = false;
                    for (byte y = Chunk.MaxHeightInBlocks; y > 0; y--)
                    {
                        var blockIndex = BlockCache.GetBlockIndex(chunk, (byte)x, (byte)y, (byte)z);

                        //if (chunk.Blocks[offset + y].Type != BlockType.None) inShade = true;
                        if (BlockCache.Instance.Blocks[blockIndex].Type != BlockType.None)
                            inShade = true;

                        if (!inShade)
                        {
                            //chunk.Blocks[offset + y].Sun = sunValue;
                            BlockCache.Instance.Blocks[blockIndex].Sun = sunValue;
                        }
                        else
                        {
                            //chunk.Blocks[offset + y].Sun = 0;
                            BlockCache.Instance.Blocks[blockIndex].Sun = 0;
                        }

                        //chunk.Blocks[offset + y].R = 0;
                        //chunk.Blocks[offset + y].G = 0;
                        //chunk.Blocks[offset + y].B = 0;

                        BlockCache.Instance.Blocks[blockIndex].R = 0;
                        BlockCache.Instance.Blocks[blockIndex].G = 0;
                        BlockCache.Instance.Blocks[blockIndex].B = 0;
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
                    //int offset = x * Chunk.FlattenOffset + z * Chunk.HeightInBlocks;
                    for (byte y = 0; y < Chunk.HeightInBlocks; y++)
                    {
                        var blockIndex = BlockCache.GetBlockIndex(chunk, (byte)x, (byte)y, (byte)z);

                        //if (chunk.Blocks[offset + y].Type == BlockType.None)
                        if (BlockCache.Instance.Blocks[blockIndex].Type == BlockType.None)
                        {
                            //if (chunk.Blocks[offset + y].Sun > 1)
                            if (BlockCache.Instance.Blocks[blockIndex].Sun > 1)
                            {
                                //byte light = Attenuate(chunk.Blocks[offset + y].Sun);
                                byte light = Attenuate(BlockCache.Instance.Blocks[blockIndex].Sun);

                                if (x > 0) PropagateSunLight(chunk, (byte)(x - 1), y, z, light);
                                if (x < Chunk.MaxWidthInBlocks) PropagateSunLight(chunk, (byte)(x + 1), y, z, light);
                                if (y > 0) PropagateSunLight(chunk, x, (byte)(y - 1), z, light);
                                if (y < Chunk.MaxHeightInBlocks) PropagateSunLight(chunk, x, (byte)(y + 1), z, light);
                                if (z > 0) PropagateSunLight(chunk, x, y, (byte)(z - 1), light);
                                if (z < Chunk.MaxLenghtInBlocks) PropagateSunLight(chunk, x, y, (byte)(z + 1), light);
                            }
                        }
                    }
                }
            }
        }

        private static byte Attenuate(byte light)
        {
            return (byte)((light * 9) / 10);
        }

        private static void FillLightningR(Chunk chunk)
        {
            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                for (byte z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    //int offset = x*Chunk.FlattenOffset + z*Chunk.HeightInBlocks;
                    for (byte y = 0; y < Chunk.HeightInBlocks; y++)
                    {
                        var blockIndex = BlockCache.GetBlockIndex(chunk, (byte)x, (byte)y, (byte)z);

                        //int blockOffset = offset + y;
                        //if (chunk.Blocks[blockOffset].Exists || chunk.Blocks[blockOffset].R <= 1) continue;
                        if (BlockCache.Instance.Blocks[blockIndex].Exists || BlockCache.Instance.Blocks[blockIndex].R <= 1) 
                            continue;

                        //var light = (byte) ((chunk.Blocks[blockOffset].R/10)*9);
                        var light = (byte)((BlockCache.Instance.Blocks[blockIndex].R / 10) * 9);

                        if (x > 0) PropagateLightR(chunk, (byte) (x - 1), y, z, light);
                        if (x < Chunk.MaxWidthInBlocks) PropagateLightR(chunk, (byte) (x + 1), y, z, light);
                        if (y > 0) PropagateLightR(chunk, x, (byte) (y - 1), z, light);
                        if (y < Chunk.MaxHeightInBlocks) PropagateLightR(chunk, x, (byte) (y + 1), z, light);
                        if (z > 0) PropagateLightR(chunk, x, y, (byte) (z - 1), light);
                        if (z < Chunk.MaxLenghtInBlocks) PropagateLightR(chunk, x, y, (byte) (z + 1), light);
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
                    //int offset = x*Chunk.FlattenOffset + z*Chunk.HeightInBlocks;
                    for (byte y = 0; y < Chunk.HeightInBlocks; y++)
                    {
                        //int blockOffset = offset + y;
                        var blockIndex = BlockCache.GetBlockIndex(chunk, (byte)x, (byte)y, (byte)z);
                        

                        //if (chunk.Blocks[blockOffset].Exists || chunk.Blocks[blockOffset].G <= 1) continue;
                        if (BlockCache.Instance.Blocks[blockIndex].Exists || BlockCache.Instance.Blocks[blockIndex].G <= 1) 
                            continue;

                        //var light = (byte) ((chunk.Blocks[blockOffset].G/10)*9);
                        var light = (byte)((BlockCache.Instance.Blocks[blockIndex].G / 10) * 9);

                        if (x > 0) PropagateLightG(chunk, (byte) (x - 1), y, z, light);
                        if (x < Chunk.MaxWidthInBlocks) PropagateLightG(chunk, (byte) (x + 1), y, z, light);
                        if (y > 0) PropagateLightG(chunk, x, (byte) (y - 1), z, light);
                        if (y < Chunk.MaxHeightInBlocks) PropagateLightG(chunk, x, (byte) (y + 1), z, light);
                        if (z > 0) PropagateLightG(chunk, x, y, (byte) (z - 1), light);
                        if (z < Chunk.MaxLenghtInBlocks) PropagateLightG(chunk, x, y, (byte) (z + 1), light);
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
                    //int offset = x*Chunk.FlattenOffset + z*Chunk.HeightInBlocks;
                    for (byte y = 0; y < Chunk.HeightInBlocks; y++)
                    {
                        //int blockOffset = offset + y;

                        var blockIndex = BlockCache.GetBlockIndex(chunk, (byte)x, (byte)y, (byte)z);

                        //if (chunk.Blocks[blockOffset].Exists || chunk.Blocks[blockOffset].B <= 1) continue;
                        if (BlockCache.Instance.Blocks[blockIndex].Exists || BlockCache.Instance.Blocks[blockIndex].B <= 1) continue;

                        //var light = (byte) ((chunk.Blocks[blockOffset].B/10)*9);
                        var light = (byte)((BlockCache.Instance.Blocks[blockIndex].B / 10) * 9);

                        if (x > 0) PropagateLightB(chunk, (byte) (x - 1), y, z, light);
                        if (x < Chunk.MaxWidthInBlocks) PropagateLightB(chunk, (byte) (x + 1), y, z, light);
                        if (y > 0) PropagateLightB(chunk, x, (byte) (y - 1), z, light);
                        if (y < Chunk.MaxHeightInBlocks) PropagateLightB(chunk, x, (byte) (y + 1), z, light);
                        if (z > 0) PropagateLightB(chunk, x, y, (byte) (z - 1), light);
                        if (z < Chunk.MaxLenghtInBlocks) PropagateLightB(chunk, x, y, (byte) (z + 1), light);
                    }
                }
            }
        }

        private static void PropagateSunLight(Chunk chunk, byte x, byte y, byte z, byte light)
        {
            //int offset = x * Chunk.FlattenOffset + z * Chunk.HeightInBlocks + y;
            var blockIndex = BlockCache.GetBlockIndex(chunk, (byte)x, (byte)y, (byte)z);

            //if (chunk.Blocks[offset].Type != BlockType.None && chunk.Blocks[offset].Type != BlockType.Water) return;
            if (BlockCache.Instance.Blocks[blockIndex].Type != BlockType.None && BlockCache.Instance.Blocks[blockIndex].Type != BlockType.Water) 
                return;

            //if (chunk.Blocks[offset].Sun >= light) return;
            if (BlockCache.Instance.Blocks[blockIndex].Sun >= light) 
                return;

            //chunk.Blocks[offset].Sun = light;
            BlockCache.Instance.Blocks[blockIndex].Sun = light;

            if (light > 1)
            {
                light = Attenuate(light);

                // Propogate light within this chunk
                if (x > 0) PropagateSunLight(chunk, (byte)(x - 1), y, z, light);
                if (x < Chunk.MaxWidthInBlocks) PropagateSunLight(chunk, (byte)(x + 1), y, z, light);
                if (y > 0) PropagateSunLight(chunk, x, (byte)(y - 1), z, light);
                if (y < Chunk.MaxHeightInBlocks) PropagateSunLight(chunk, x, (byte)(y + 1), z, light);
                if (z > 0) PropagateSunLight(chunk, x, y, (byte)(z - 1), light);
                if (z < Chunk.MaxLenghtInBlocks) PropagateSunLight(chunk, x, y, (byte)(z + 1), light);

                if (chunk.East != null && x == 0) PropagateSunLight(chunk.East, (byte)(Chunk.MaxWidthInBlocks), y, z, light);
                if (chunk.West != null && (x == Chunk.MaxWidthInBlocks)) PropagateSunLight(chunk.West, 0, y, z, light);
                if (chunk.South != null && z == 0) PropagateSunLight(chunk.South, x, y, (byte)(Chunk.MaxLenghtInBlocks), light);
                if (chunk.North != null && (z == Chunk.MaxLenghtInBlocks)) PropagateSunLight(chunk.North, x, y, 0, light);

            }
        }

        private static void PropagateLightR(Chunk chunk, byte x, byte y, byte z, byte lightR)
        {
            var blockIndex = BlockCache.GetBlockIndex(chunk, (byte)x, (byte)y, (byte)z);
            //int offset = x*Chunk.FlattenOffset + z*Chunk.HeightInBlocks + y;
            
            //if (chunk.Blocks[offset].Exists || chunk.Blocks[offset].R >= lightR) return;
            if (BlockCache.Instance.Blocks[blockIndex].Exists || BlockCache.Instance.Blocks[blockIndex].R >= lightR) 
                return;

            //chunk.Blocks[offset].R = lightR;
            BlockCache.Instance.Blocks[blockIndex].R = lightR;

            if (lightR <= 1) return;
            lightR = (byte) (lightR - 1);

            if (x > 0) PropagateLightR(chunk, (byte) (x - 1), y, z, lightR);
            if (x < Chunk.MaxWidthInBlocks) PropagateLightR(chunk, (byte) (x + 1), y, z, lightR);
            if (y > 0) PropagateLightR(chunk, x, (byte) (y - 1), z, lightR);
            if (y < Chunk.MaxHeightInBlocks) PropagateLightR(chunk, x, (byte) (y + 1), z, lightR);
            if (z > 0) PropagateLightR(chunk, x, y, (byte) (z - 1), lightR);
            if (z < Chunk.MaxLenghtInBlocks) PropagateLightR(chunk, x, y, (byte) (z + 1), lightR);
        }

        private static void PropagateLightG(Chunk chunk, byte x, byte y, byte z, byte lightG)
        {
            var blockIndex = BlockCache.GetBlockIndex(chunk, (byte)x, (byte)y, (byte)z);

            //int offset = x*Chunk.FlattenOffset + z*Chunk.HeightInBlocks + y;
            //if (chunk.Blocks[offset].Exists || chunk.Blocks[offset].G >= lightG) return;
            if (BlockCache.Instance.Blocks[blockIndex].Exists || BlockCache.Instance.Blocks[blockIndex].G >= lightG) 
                return;

            //chunk.Blocks[offset].G = lightG;
            BlockCache.Instance.Blocks[blockIndex].G = lightG;

            if (lightG <= 1) return;
            lightG = (byte) (lightG - 1);

            if (x > 0) PropagateLightG(chunk, (byte) (x - 1), y, z, lightG);
            if (x < Chunk.MaxWidthInBlocks) PropagateLightG(chunk, (byte) (x + 1), y, z, lightG);
            if (y > 0) PropagateLightG(chunk, x, (byte) (y - 1), z, lightG);
            if (y < Chunk.MaxHeightInBlocks) PropagateLightG(chunk, x, (byte) (y + 1), z, lightG);
            if (z > 0) PropagateLightG(chunk, x, y, (byte) (z - 1), lightG);
            if (z < Chunk.MaxLenghtInBlocks) PropagateLightG(chunk, x, y, (byte) (z + 1), lightG);
        }

        private static void PropagateLightB(Chunk chunk, byte x, byte y, byte z, byte lightB)
        {
            var blockIndex = BlockCache.GetBlockIndex(chunk, (byte)x, (byte)y, (byte)z);

            //int offset = x*Chunk.FlattenOffset + z*Chunk.HeightInBlocks + y;
            //if (chunk.Blocks[offset].Exists || chunk.Blocks[offset].B >= lightB) return;
            if (BlockCache.Instance.Blocks[blockIndex].Exists || BlockCache.Instance.Blocks[blockIndex].B >= lightB) 
                return;

            //chunk.Blocks[offset].B = lightB;
            BlockCache.Instance.Blocks[blockIndex].B = lightB;


            if (lightB <= 1) return;
            lightB = (byte) (lightB - 1);

            if (x > 0) PropagateLightB(chunk, (byte) (x - 1), y, z, lightB);
            if (x < Chunk.MaxWidthInBlocks) PropagateLightB(chunk, (byte) (x + 1), y, z, lightB);
            if (y > 0) PropagateLightB(chunk, x, (byte) (y - 1), z, lightB);
            if (y < Chunk.MaxHeightInBlocks) PropagateLightB(chunk, x, (byte) (y + 1), z, lightB);
            if (z > 0) PropagateLightB(chunk, x, y, (byte) (z - 1), lightB);
            if (z < Chunk.MaxLenghtInBlocks) PropagateLightB(chunk, x, y, (byte) (z + 1), lightB);
        }
    }
}
