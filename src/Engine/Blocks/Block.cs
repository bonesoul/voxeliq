/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

namespace Engine.Blocks
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
        /// Propated light's red component.
        /// </summary>
        public byte R;

        /// <summary>
        /// Propated light's green component.
        /// </summary>
        public byte G;

        /// <summary>
        /// Propated light's blue component.
        /// </summary>
        public byte B;

        /// <summary>
        /// Creates a new block from given type.
        /// </summary>
        /// <param name="type">The block type.</param>
        public Block(BlockType type)
        {
            Type = type;
            Sun = 16;
            R = 16;
            G = 16;
            B = 16;
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
        /// Return the appropriate texture to render a given face of a block.
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

        /// <summary>
        /// Returns a string that represents block's type.
        /// </summary>      
        /// <returns><see cref="string"/></returns>
        /// <remarks>Used by the Visual Studio debugger.</remarks>
        public override string ToString()
        {
            return this.Type.ToString();
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