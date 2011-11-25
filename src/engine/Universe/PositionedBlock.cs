/*
 * Copyright (C) 2011 voxeliq project 
 *
 */

using VolumetricStudios.VoxeliqEngine.Blocks;
using VolumetricStudios.VoxeliqEngine.Utils.Vector;

namespace VolumetricStudios.VoxeliqEngine.Universe
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
