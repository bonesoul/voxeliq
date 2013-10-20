/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

namespace Engine.Chunks.Generators.Biomes
{
    public class BiomeGenerator
    {
        public virtual void ApplyBiome(Chunk chunk, int groundLevel, int groundOffset, int worldPositionX, int worldPositionZ)
        { }
    }
}