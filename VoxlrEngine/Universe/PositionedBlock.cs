/*    
 * Copyright (C) 2011, Hüseyin Uslu
 *  
 */

using VolumetricStudios.VoxlrEngine.Utils.Vector;

namespace VolumetricStudios.VoxlrEngine.Universe
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
