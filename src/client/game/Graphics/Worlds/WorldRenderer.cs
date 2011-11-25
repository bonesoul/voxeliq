/*
 * Copyright (C) 2011 voxeliq project 
 *
 */

using VolumetricStudios.VoxeliqClient.Games;

namespace VolumetricStudios.VoxeliqClient.Graphics.Worlds
{
    public class WorldRenderer
    {
        public GameWorld World {get; private set;}

        public WorldRenderer(GameWorld world)
        {
            this.World = world;
        }
    }
}
