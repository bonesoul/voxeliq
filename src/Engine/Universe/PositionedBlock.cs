/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using VoxeliqEngine.Blocks;
using VoxeliqEngine.Utils.Vector;

namespace VoxeliqEngine.Universe
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