using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace VolumetricStudios.VoxeliqGame.Managers
{
    public class MusicManager:GameComponent
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
