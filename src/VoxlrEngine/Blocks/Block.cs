/*
 * Copyright (C) 2011 voxlr project 
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace VolumetricStudios.VoxlrEngine.Blocks
{
    /// <summary>
    /// Basic block structure.
    /// </summary>
    public struct Block
    {
        /// <summary>
        /// Block type.
        /// </summary>
        public BlockType Type;
        
        /// <summary>
        /// Propogated sun-light value.
        /// </summary>
        public byte Sun;

        /// <summary>
        /// Propated light's RGB.
        /// </summary>
        public byte R, G, B;
        
        /// <summary>
        /// Creates a new block from given type.
        /// </summary>
        /// <param name="type"></param>
        public Block(BlockType type)
        {
            Type = type;
            Sun = 16;
            R = 16; G = 16; B = 16;
        }

        /// <summary>
        /// Returns an empty block (air).
        /// </summary>
        public static Block Empty
        {
            get { return new Block(BlockType.None); }
        }

        /// <summary>
        /// Returns true if a block exists, false it it's air.
        /// </summary>
        public bool Exists
        {
            get { return Type != BlockType.None; }
        }

        /// <summary>
        /// Return the appropriate texture to render a given face of a block
        /// </summary>
        /// <param name="blockType"></param>
        /// <param name="faceDir"></param>
        /// <returns></returns>
        public static BlockTexture GetTexture(BlockType blockType, BlockFaceDirection faceDir)
        {
            switch (blockType)
            {
                case BlockType.Brick:
                    return BlockTexture.Brick;
                case BlockType.Dirt:
                    return BlockTexture.Dirt;
                case BlockType.Gold:
                    return BlockTexture.Gold;
                case BlockType.Grass:
                    switch (faceDir)
                    {
                        case BlockFaceDirection.XIncreasing:
                        case BlockFaceDirection.XDecreasing:
                        case BlockFaceDirection.ZIncreasing:
                        case BlockFaceDirection.ZDecreasing:
                            return BlockTexture.GrassSide;
                        case BlockFaceDirection.YIncreasing:
                            return BlockTexture.GrassTop;
                        case BlockFaceDirection.YDecreasing:
                            return BlockTexture.Dirt;
                        default:
                            return BlockTexture.Rock;
                    }
                case BlockType.Iron:
                    return BlockTexture.Iron;
                case BlockType.Lava:
                    return BlockTexture.Lava;
                case BlockType.Leaves:
                    return BlockTexture.Leaves;
                case BlockType.Gravel:
                    return BlockTexture.Gravel;
                case BlockType.Rock:
                    return BlockTexture.Rock;
                case BlockType.Sand:
                    return BlockTexture.Sand;
                case BlockType.Snow:
                    return BlockTexture.Snow;
                case BlockType.Tree:
                    switch (faceDir)
                    {
                        case BlockFaceDirection.XIncreasing:
                        case BlockFaceDirection.XDecreasing:
                        case BlockFaceDirection.ZIncreasing:
                        case BlockFaceDirection.ZDecreasing:
                            return BlockTexture.TreeSide;
                        case BlockFaceDirection.YIncreasing:
                        case BlockFaceDirection.YDecreasing:
                            return BlockTexture.TreeTop;
                        default:
                            return BlockTexture.Rock;
                    }
                case BlockType.Water:
                    return BlockTexture.Water;
                default:
                    return BlockTexture.Rock;
            }
        }
    }

    /// <summary>
    /// Facing direction.
    /// </summary>
    public enum BlockFaceDirection
    {
        XIncreasing = 1,
        XDecreasing = 2,
        YIncreasing = 4,
        YDecreasing = 8,
        ZIncreasing = 16,
        ZDecreasing = 32,
    }

    /// <summary>
    /// Block types.
    /// </summary>
    public enum BlockType : byte
    {
        None,
        Brick,
        Dirt,
        Gold,
        Grass,
        Iron,
        Lava,
        Leaves,
        Gravel,
        Rock,
        Sand,
        Snow,
        Tree,
        Water,
        Maximum
    }

    /// <summary>
    /// Available block textures.
    /// </summary>
    public enum BlockTexture
    {
        Brick,
        Dirt,
        Gold,
        GrassSide,
        GrassTop,
        Iron,
        Lava,
        Leaves,
        Gravel,
        Rock,
        Sand,
        Snow,
        TreeTop,
        TreeSide,
        Water,
        Maximum
    }
}
