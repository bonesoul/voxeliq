/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using VoxeliqEngine.Common.Vector;

namespace VoxeliqEngine.Blocks
{
    public struct PositionedBlock
    {
        public readonly Vector3Int Position;
        public readonly Block Block;

        public PositionedBlock(Vector3Int position, Block block)
        {
            Position = position;
            Block = block;
        }
    }
}