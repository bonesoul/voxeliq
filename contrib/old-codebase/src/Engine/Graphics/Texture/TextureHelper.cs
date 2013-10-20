/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System.Collections.Generic;
using Engine.Blocks;
using Microsoft.Xna.Framework.Graphics.PackedVector;

// TODO: Document this file!

namespace Engine.Graphics.Texture
{
    /// <summary>
    /// Provides texture-helper methods like uv-mappings for blocks.
    /// This process projects a texture map onto a 3D object. The letters "U" and "V" are used to describe the 2D mesh because "X", "Y" and "Z" are already used to describe the 3D object in model space.
    /// UV texturing permits polygons that make up a 3D object to be painted with color from an image. The image is called a UV texture map, but it's just an ordinary image.
    /// More information available over Microsoft XNA Game Studio Creator's Guide - Chapter 9.
    /// </summary>
    public static class TextureHelper
    {
        /// <summary>
        /// The block texture atlas size.
        /// Marks the texture atlas as made of n * n textures.
        /// </summary>
        public const int BlockTextureAtlasSize = 8;

        /// <summary>
        /// Unit offset for block textures.
        /// </summary>
        private const float UnitBlockTextureOffset = 1f/BlockTextureAtlasSize;

        /// <summary>
        /// The texture atlas size.
        /// </summary>
        public const int CrackTextureAtlasSize = 3;

        /// <summary>
        /// Unit offset for crack textures.
        /// </summary>
        private const float UnitCrackTextureOffset = 1f/CrackTextureAtlasSize;

        /// <summary>
        /// Provides uv-mappings for blocks.
        /// </summary>
        public static readonly Dictionary<int, HalfVector2[]> BlockTextureMappings = new Dictionary<int, HalfVector2[]>();

        /// <summary>
        /// Provides uv-mappings for crack effects.
        /// </summary>
        public static readonly Dictionary<int, HalfVector2[]> CrackTextureMappings = new Dictionary<int, HalfVector2[]>();

        static TextureHelper()
        {
            BuildBlockTextureMappings(); // build the uv-mappings for blocks textures.
            //BuildCrackTextureMappings(); // build the uv-mappings for crack textures.
        }

        /// <summary>
        /// Builds uv-mappings for all available blocks-textures.
        /// </summary>
        private static void BuildBlockTextureMappings()
        {
            for (int i = 0; i < (int) BlockTexture.Maximum; i++)
            {
                BlockTextureMappings.Add((i*6), GetBlockTextureMapping(i, BlockFaceDirection.XIncreasing)); // build x-increasing mapping for the texture.
                BlockTextureMappings.Add((i*6) + 1, GetBlockTextureMapping(i, BlockFaceDirection.XDecreasing)); // build x-decreasing mapping for the texture.
                BlockTextureMappings.Add((i*6) + 2, GetBlockTextureMapping(i, BlockFaceDirection.YIncreasing)); // build y-increasing mapping for the texture.
                BlockTextureMappings.Add((i*6) + 3, GetBlockTextureMapping(i, BlockFaceDirection.YDecreasing)); // build y-decreasing mapping for the texture.
                BlockTextureMappings.Add((i*6) + 4, GetBlockTextureMapping(i, BlockFaceDirection.ZIncreasing)); // build z-increasing mapping for the texture.
                BlockTextureMappings.Add((i*6) + 5, GetBlockTextureMapping(i, BlockFaceDirection.ZDecreasing)); // build z-increasing mapping for the texture.   
            }
        }

        /// <summary>
        /// Calculates uv-mampings for given texture and direction.
        /// </summary>
        /// <param name="textureIndex">The asked texture's texture-index.</param>
        /// <param name="direction">The asked direction.</param>
        /// <returns>Returns list of uv-mappings for given textureIndex and face-direction.</returns>
        private static HalfVector2[] GetBlockTextureMapping(int textureIndex, BlockFaceDirection direction)
        {
            int y = textureIndex/BlockTextureAtlasSize; // y-position for the texture.
            int x = textureIndex%BlockTextureAtlasSize; // x-position for the texture.

            float yOffset = y*UnitBlockTextureOffset; // the unit y-offset.
            float xOffset = x*UnitBlockTextureOffset; // the unit x-offset;

            var mapping = new HalfVector2[6]; // contains texture mapping for the two triangles contained.
            switch (direction)
            {
                case BlockFaceDirection.XIncreasing:
                    mapping[0] = new HalfVector2(xOffset, yOffset); // 0,0 // first triangle.
                    mapping[1] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
                    mapping[2] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1
                    mapping[3] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1 // second triangle.
                    mapping[4] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
                    mapping[5] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset + UnitBlockTextureOffset); // 1,1
                    break;

                case BlockFaceDirection.XDecreasing:
                    mapping[0] = new HalfVector2(xOffset, yOffset); // 0,0
                    mapping[1] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
                    mapping[2] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset + UnitBlockTextureOffset); // 1,1
                    mapping[3] = new HalfVector2(xOffset, yOffset); // 0,0
                    mapping[4] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset + UnitBlockTextureOffset); // 1,1
                    mapping[5] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1
                    break;

                case BlockFaceDirection.YIncreasing:
                    mapping[0] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1
                    mapping[1] = new HalfVector2(xOffset, yOffset); // 0,0
                    mapping[2] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
                    mapping[3] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1
                    mapping[4] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
                    mapping[5] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset + UnitBlockTextureOffset); // 1,1
                    break;

                case BlockFaceDirection.YDecreasing:
                    mapping[0] = new HalfVector2(xOffset, yOffset); // 0,0
                    mapping[1] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
                    mapping[2] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1
                    mapping[3] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1
                    mapping[4] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
                    mapping[5] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset + UnitBlockTextureOffset); // 1,1
                    break;

                case BlockFaceDirection.ZIncreasing:
                    mapping[0] = new HalfVector2(xOffset, yOffset); // 0,0
                    mapping[1] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
                    mapping[2] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset + UnitBlockTextureOffset); // 1,1
                    mapping[3] = new HalfVector2(xOffset, yOffset); // 0,0
                    mapping[4] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset + UnitBlockTextureOffset); // 1,1
                    mapping[5] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1
                    break;

                case BlockFaceDirection.ZDecreasing:
                    mapping[0] = new HalfVector2(xOffset, yOffset); // 0,0
                    mapping[1] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
                    mapping[2] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1
                    mapping[3] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1
                    mapping[4] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
                    mapping[5] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset + UnitBlockTextureOffset); // 1,1
                    break;
            }
            return mapping;
        }

        ///// <summary>
        ///// Builds uv-mappings for all crack textures.
        ///// </summary>
        //private static void BuildCrackTextureMappings()
        //{
        //    for (int i = 0; i < 256; i++)
        //    {
        //        CrackTextureMappings.Add((i*6), GetCrackTextureMapping(i, BlockFaceDirection.XIncreasing));
        //            // build x-increasing mapping for the texture.
        //        CrackTextureMappings.Add((i*6) + 1, GetCrackTextureMapping(i, BlockFaceDirection.XDecreasing));
        //            // build x-decreasing mapping for the texture.
        //        CrackTextureMappings.Add((i*6) + 2, GetCrackTextureMapping(i, BlockFaceDirection.YIncreasing));
        //            // build y-increasing mapping for the texture.
        //        CrackTextureMappings.Add((i*6) + 3, GetCrackTextureMapping(i, BlockFaceDirection.YDecreasing));
        //            // build y-decreasing mapping for the texture.
        //        CrackTextureMappings.Add((i*6) + 4, GetCrackTextureMapping(i, BlockFaceDirection.ZIncreasing));
        //            // build z-increasing mapping for the texture.
        //        CrackTextureMappings.Add((i*6) + 5, GetCrackTextureMapping(i, BlockFaceDirection.ZDecreasing));
        //            // build z-increasing mapping for the texture.   
        //    }
        //}

        ///// <summary>
        ///// Calculates uv-mappings for given texture and direction.
        ///// </summary>
        ///// <param name="textureIndex">The asked texture's texture-index.</param>
        ///// <param name="direction">The asked direction.</param>
        ///// <returns>Returns list of uv-mappings for given textureIndex and face-direction.</returns>
        //private static HalfVector2[] GetCrackTextureMapping(int textureIndex, BlockFaceDirection direction)
        //{
        //    int y = textureIndex/CrackTextureAtlasSize; // y-position for the texture.
        //    int x = textureIndex%CrackTextureAtlasSize; // x-position for the texture.

        //    float yOffset = y*UnitCrackTextureOffset; // the unit y-offset.
        //    float xOffset = x*UnitCrackTextureOffset; // the unit x-offset;

        //    var mapping = new HalfVector2[6]; // contains texture mapping for the two triangles contained.
        //    switch (direction)
        //    {
        //        case BlockFaceDirection.XIncreasing:
        //            mapping[0] = new HalfVector2(xOffset, yOffset); // 0,0 // first triangle.
        //            mapping[1] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
        //            mapping[2] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1
        //            mapping[3] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1 // second triangle.
        //            mapping[4] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
        //            mapping[5] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset + UnitBlockTextureOffset);
        //                // 1,1
        //            break;

        //        case BlockFaceDirection.XDecreasing:
        //            mapping[0] = new HalfVector2(xOffset, yOffset); // 0,0
        //            mapping[1] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
        //            mapping[2] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset + UnitBlockTextureOffset);
        //                // 1,1
        //            mapping[3] = new HalfVector2(xOffset, yOffset); // 0,0
        //            mapping[4] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset + UnitBlockTextureOffset);
        //                // 1,1
        //            mapping[5] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1
        //            break;

        //        case BlockFaceDirection.YIncreasing:
        //            mapping[0] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1
        //            mapping[1] = new HalfVector2(xOffset, yOffset); // 0,0
        //            mapping[2] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
        //            mapping[3] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1
        //            mapping[4] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
        //            mapping[5] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset + UnitBlockTextureOffset);
        //                // 1,1
        //            break;

        //        case BlockFaceDirection.YDecreasing:
        //            mapping[0] = new HalfVector2(xOffset, yOffset); // 0,0
        //            mapping[1] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
        //            mapping[2] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1
        //            mapping[3] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1
        //            mapping[4] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
        //            mapping[5] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset + UnitBlockTextureOffset);
        //                // 1,1
        //            break;

        //        case BlockFaceDirection.ZIncreasing:
        //            mapping[0] = new HalfVector2(xOffset, yOffset); // 0,0
        //            mapping[1] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
        //            mapping[2] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset + UnitBlockTextureOffset);
        //                // 1,1
        //            mapping[3] = new HalfVector2(xOffset, yOffset); // 0,0
        //            mapping[4] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset + UnitBlockTextureOffset);
        //                // 1,1
        //            mapping[5] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1
        //            break;

        //        case BlockFaceDirection.ZDecreasing:
        //            mapping[0] = new HalfVector2(xOffset, yOffset); // 0,0
        //            mapping[1] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
        //            mapping[2] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1
        //            mapping[3] = new HalfVector2(xOffset, yOffset + UnitBlockTextureOffset); // 0,1
        //            mapping[4] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset); // 1,0
        //            mapping[5] = new HalfVector2(xOffset + UnitBlockTextureOffset, yOffset + UnitBlockTextureOffset);
        //                // 1,1
        //            break;
        //    }
        //    return mapping;
        //}
    }
}