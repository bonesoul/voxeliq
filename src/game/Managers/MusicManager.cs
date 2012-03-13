/*
 * Copyright (C) 2011-2012 Volumetric Studios
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace VolumetricStudios.VoxeliqGame.Managers
{
    enum AmbientMusic
    {
        forestAmbient,
        //frogsAmbient
    }

    public class MusicManager : GameComponent
    {
        private Song _backgroundSong;
        private IEnumerable<AmbientMusic> _ambientMusicsNames;
        private SoundEffectInstance _currentAmbientMusic;
        private readonly Dictionary<AmbientMusic, SoundEffect> _ambientMusicSoundEffects = new Dictionary<AmbientMusic, SoundEffect>();

        private readonly Random _random = new Random();

        public MusicManager(Game game)
            : base(game)
        { }

        public override void Initialize()
        {
            this._backgroundSong = Game.Content.Load<Song>(@"audio\music\funandrun");

            // load ambient musics.
            this._ambientMusicsNames = Enum.GetValues(typeof(AmbientMusic)).Cast<AmbientMusic>();

            foreach (var entry in this._ambientMusicsNames.ToArray())
            {
                this._ambientMusicSoundEffects.Add(entry, Game.Content.Load<SoundEffect>(@"audio\music\ambient\" + entry.ToString()));
            }

            this.PlayBackroundSong();

            var ambientMusicThread = new Thread(AmbientMusicLoop);
            ambientMusicThread.Name = "Ambient Music Thread";
            ambientMusicThread.Start();
        }

        private void AmbientMusicLoop()
        {
            while(true)
            {
                if (this._currentAmbientMusic == null || this._currentAmbientMusic.IsDisposed || this._currentAmbientMusic.State == SoundState.Playing)
                {
                    Thread.Sleep(1000);
                }

                this.PlayRandomAmbientMusic();
                Thread.Sleep(2000);
            }
        }

        private void PlayBackroundSong()
        {
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(this._backgroundSong);
            MediaPlayer.Volume = 0.3f;
        }

        private void PlayRandomAmbientMusic()
        {
            var randomAmbientMusic = this._ambientMusicsNames.ToArray()[_random.Next(this._ambientMusicsNames.ToArray().Length)];
            this._currentAmbientMusic = this._ambientMusicSoundEffects[randomAmbientMusic].CreateInstance();

            fadeOut();
            this._currentAmbientMusic.Play();
            fadeIn();
        }

        private void fadeOut()
        {
            if (this._currentAmbientMusic.State == SoundState.Stopped || this._currentAmbientMusic.State == SoundState.Paused) return;

            for (float f = this._currentAmbientMusic.Volume; f > 0f; f -= 0.05f)
            {
                Thread.Sleep(10);
                this._currentAmbientMusic.Volume -= f;
            }
        }

        private void fadeIn()
        {
            for (float f = this._currentAmbientMusic.Volume; f < 1f; f += 0.05f)
            {
                Thread.Sleep(10);
                this._currentAmbientMusic.Volume += f;
            }
        }

    }
}