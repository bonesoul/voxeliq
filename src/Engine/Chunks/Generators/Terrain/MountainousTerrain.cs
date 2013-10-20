/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Engine.Chunks.Generators.Biomes;
using Engine.Common.Noise;

namespace Engine.Chunks.Generators.Terrain
{
    public class MountainousTerrain : BiomedTerrain
    {
        public MountainousTerrain(BiomeGenerator biomeGenerator)
            : base(biomeGenerator)
        { }

        protected override float GetRockHeight(int blockX, int blockZ)
        {
            blockX += this.Seed;

            int minimumGroundheight = Chunk.HeightInBlocks/4;
            int minimumGroundDepth = (int) (Chunk.HeightInBlocks*0.7f);

            float octave1 = SimplexNoise.noise(blockX*0.0001f, blockZ*0.0001f)*0.5f;
            float octave2 = SimplexNoise.noise(blockX*0.0005f, blockZ*0.0005f)*0.25f;
            float octave3 = SimplexNoise.noise(blockX*0.005f, blockZ*0.005f)*0.12f;
            float octave4 = SimplexNoise.noise(blockX*0.01f, blockZ*0.01f)*0.12f;
            float octave5 = SimplexNoise.noise(blockX*0.03f, blockZ*0.03f)*octave4;

            float lowerGroundHeight = octave1 + octave2 + octave3 + octave4 + octave5;

            lowerGroundHeight = lowerGroundHeight*minimumGroundDepth + minimumGroundheight;

            return lowerGroundHeight;
        }
    }
}