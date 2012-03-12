/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace VolumetricStudios.VoxeliqGame.Managers
{
    public class MusicManager : GameComponent
    {
        public MusicManager(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            var music = Game.Content.Load<Song>(@"audio\music\funandrun");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(music);
        }
    }
}