﻿/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxeliqEngine.Common.Logging;

namespace VoxeliqEngine.Assets
{
    /// <summary>
    /// Asset manager that loads assets at the very first loading.
    /// </summary>
    public interface IAssetManager
    {
        Model AimedBlockModel { get; }
        Model SampleModel { get; }
        Model SkyDomeModel { get; }

        Effect BlockEffect { get; }
        BasicEffect AimedBlockEffect { get; }
        Effect BloomExtractEffect { get;  }
        Effect BloomCombineEffect { get;  }
        Effect GaussianBlurEffect { get;  }
        Effect SkyDomeEffect { get; }
        Effect PerlinNoiseEffect { get; }

        Texture2D BlockTextureAtlas { get; }
        Texture2D CrackTextureAtlas { get; }
        Texture2D AimedBlockTexture { get; }
        Texture2D CrossHairNormalTexture { get; }
        Texture2D CrossHairShovelTexture { get; }
        Texture2D CloudMapTexture { get; }
        Texture2D StarMapTexture { get; }

        SpriteFont Verdana { get; }
    }

    /// <summary>
    /// Asset manager that loads assets at the very first loading.
    /// </summary>
    public class AssetManager : GameComponent, IAssetManager
    {
        public Model AimedBlockModel { get; private set; }
        public Model SampleModel { get; private set; }
        public Model SkyDomeModel { get; private set; }

        public Effect BlockEffect { get; private set; }
        public BasicEffect AimedBlockEffect { get; private set; }
        public Effect BloomExtractEffect { get; private set; }
        public Effect BloomCombineEffect { get; private set; }
        public Effect GaussianBlurEffect { get; private set; }
        public Effect SkyDomeEffect { get; private set; }
        public Effect PerlinNoiseEffect { get; private set; }

        public Texture2D BlockTextureAtlas { get; private set; }
        public Texture2D CrackTextureAtlas { get; private set; }
        public Texture2D AimedBlockTexture { get; private set; }
        public Texture2D CrossHairNormalTexture { get; private set; }
        public Texture2D CrossHairShovelTexture { get; private set; }
        public Texture2D CloudMapTexture { get; private set; }
        public Texture2D StarMapTexture { get; private set; }

        public SpriteFont Verdana { get; private set; }

#if MONOGAME
        private const string EffectShaderExtension = ".mgfxo";
#else 
        private const string EffectShaderExtension = ""; 
#endif

        private static readonly Logger Logger = LogManager.CreateLogger(); // the logger.

        public AssetManager(Game game)
            : base(game)
        {
            this.Game.Services.AddService(typeof(IAssetManager), this); // export service.   
            _instance = this;
        }

        public override void Initialize()
        {
            this.LoadContent();
            base.Initialize();
        }

        public void LoadContent()
        {
            try
            {                                
                this.AimedBlockModel = Game.Content.Load<Model>(@"Models/AimedBlock");
                this.SampleModel = Game.Content.Load<Model>(@"Models/Mii");
                this.SkyDomeModel = Game.Content.Load<Model>(@"Models/SkyDome");

                this.BlockEffect = this.LoadEffectShader(@"Effects/BlockEffect");
                this.AimedBlockEffect = new BasicEffect(Game.GraphicsDevice);
                this.BloomExtractEffect = this.LoadEffectShader(@"Effects/PostProcessing/Bloom/BloomExtract");
                this.BloomCombineEffect = this.LoadEffectShader(@"Effects/PostProcessing/Bloom/BloomCombine");
                this.GaussianBlurEffect = this.LoadEffectShader(@"Effects/PostProcessing/Bloom/GaussianBlur");
                this.SkyDomeEffect = this.LoadEffectShader(@"Effects/SkyDome");
                this.PerlinNoiseEffect = this.LoadEffectShader(@"Effects/PerlinNoise");

                this.BlockTextureAtlas = Game.Content.Load<Texture2D>(@"Textures/terrain");
                this.CrackTextureAtlas = Game.Content.Load<Texture2D>(@"Textures/cracks");
                this.AimedBlockTexture = Game.Content.Load<Texture2D>(@"Textures/AimedBlock");
                this.CrossHairNormalTexture = Game.Content.Load<Texture2D>(@"Textures/Crosshairs/Normal");
                this.CrossHairShovelTexture = Game.Content.Load<Texture2D>(@"Textures/Crosshairs/Shovel");
                this.CloudMapTexture = Game.Content.Load<Texture2D>(@"Textures/cloudmap");
                this.StarMapTexture = Game.Content.Load<Texture2D>(@"Textures/starmap");

                this.Verdana = Game.Content.Load<SpriteFont>(@"Fonts/Verdana");
            }
            catch(Exception e)
            {
                Logger.FatalException(e, "Error while loading assets!");
                Console.ReadLine();
                Environment.Exit(-1);
            }
        }

        private Effect LoadEffectShader(string path)
        {
            return this.Game.Content.Load<Effect>(path + EffectShaderExtension);
        }

        private static AssetManager _instance; // the instance.

        public static AssetManager Instance
        {
            get { return _instance; }
        }
    }
}