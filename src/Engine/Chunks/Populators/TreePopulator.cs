﻿/*
 * Copyright (C) 2011 - 2013 Int6 Studios - http://www.int6.org,
 * Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxeliqEngine.Blocks;

namespace VoxeliqEngine.Chunks.Populators
{
    public static class TreePopulator
    {
        public static void PopulateTree(int worldPositionX, int worldPositionZ, int groundLevel)
        {
            var trunkHeight = 5;
            var trunkOffset = BlockStorage.BlockIndexByWorldPosition(worldPositionX, worldPositionZ);

            for (int y = trunkHeight + groundLevel; y > groundLevel; y--)
            {
                BlockStorage.Blocks[trunkOffset + y] = new Block(BlockType.Tree);
            }

            var radius = 3;
            for (int i = -radius; i < radius; i++)
            {
                for (int j = -radius; j < radius; j++)
                {
                    var offset = BlockStorage.BlockIndexByWorldPosition(worldPositionX + i, worldPositionZ + j);
                    for (int k = radius * 2; k > 0; k--)
                    {
                        BlockStorage.Blocks[offset + k + trunkHeight + 1] = new Block(BlockType.Leaves);
                    }
                }
            }
        }
    }
}