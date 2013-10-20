/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using Engine.Assets;
using Engine.Common.Logging;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Universe
{
    /// <summary>
    /// Allows interaction with sky service.
    /// </summary>
    public interface ISkyService
    {
        /// <summary>
        /// Toggles dynamic clouds.
        /// </summary>
        void ToggleDynamicClouds();
    }

    /// <summary>
    /// Sky.
    /// </summary>
    public class Sky : DrawableGameComponent, ISkyService
    {
        // settings
        private bool _dynamicCloudsEnabled;

        private Model _skyDome; // Sky dome model
        private Texture2D _cloudMap; // Cloud map.
        private Texture2D _starMap; // Star map.

        private Texture2D _staticCloudMap; // gpu generated cloud maps.
        private Effect _perlinNoiseEffect; // noise used for generating clouds.
        private RenderTarget2D _cloudsRenderTarget; // render target for clouds.
        private VertexPositionTexture[] _fullScreenVertices; // vertices.

        protected Vector3 SunColor = Color.White.ToVector3();

        protected Vector4 OverheadSunColor = Color.DarkBlue.ToVector4();
        protected Vector4 NightColor = Color.Black.ToVector4();
        protected Vector4 HorizonColor = Color.White.ToVector4();
        protected Vector4 EveningTint = Color.Red.ToVector4();
        protected Vector4 MorningTint = Color.Gold.ToVector4();
        protected float CloudOvercast = 1.0f;

        protected float RotationClouds;
        protected float RotationStars;

        // misc.
        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility

        // required services.
        private ICamera _camera;
        private IAssetManager _assetManager;

        public Sky(Game game, bool enableDynamicClouds = true)
            : base(game)
        {
            this._dynamicCloudsEnabled = enableDynamicClouds;
            this.Game.Services.AddService(typeof (ISkyService), this); // export the service.
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            // import require services.
            this._camera = (ICamera) this.Game.Services.GetService(typeof (ICamera));

            this._assetManager = (IAssetManager)this.Game.Services.GetService(typeof(IAssetManager));
            if (this._assetManager == null)
                throw new NullReferenceException("Can not find asset manager component.");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // load required assets.

            // load the dome.
            this._skyDome = this._assetManager.SkyDomeModel;
            this._skyDome.Meshes[0].MeshParts[0].Effect = this._assetManager.SkyDomeEffect;

            // load maps.
            this._cloudMap = this._assetManager.CloudMapTexture;
            this._starMap = this._assetManager.StarMapTexture;

            // for gpu generated clouds.
            this._perlinNoiseEffect = this._assetManager.PerlinNoiseEffect;
            var presentationParameters = GraphicsDevice.PresentationParameters;

            this._cloudsRenderTarget = new RenderTarget2D(GraphicsDevice, presentationParameters.BackBufferWidth,presentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None); // the mipmap does not work on all configurations            

            this._staticCloudMap = this.CreateStaticCloudMap(32);
            this._fullScreenVertices = SetUpFullscreenVertices();
        }

        public override void Update(GameTime gameTime)
        {
            if (!this._dynamicCloudsEnabled)
                return;

            this.GeneratePerlinNoise(gameTime); // if dynamic-cloud generation is on, generate them.
        }

        /// <summary>
        /// Draws the sky.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.None; // disable the depth-buffer for drawing the sky because it's the farthest object we'll be drawing.

            var modelTransforms = new Matrix[this._skyDome.Bones.Count]; // transform dome's bones.
            this._skyDome.CopyAbsoluteBoneTransformsTo(modelTransforms);

            RotationStars += 0.0001f;
            RotationClouds = 0;

            // draw stars
            Matrix wStarMatrix = Matrix.CreateTranslation(Vector3.Zero)*Matrix.CreateScale(100)* Matrix.CreateTranslation(new Vector3(this._camera.Position.X, this._camera.Position.Y - 40, this._camera.Position.Z)); // move sky to camera position and should be scaled -- bigger than the world.
            foreach (ModelMesh mesh in _skyDome.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index]*wStarMatrix;

                    currentEffect.CurrentTechnique = currentEffect.Techniques["SkyStarDome"];

                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(_camera.View);
                    currentEffect.Parameters["xProjection"].SetValue(_camera.Projection);
                    currentEffect.Parameters["xTexture"].SetValue(this._starMap);
                    currentEffect.Parameters["NightColor"].SetValue(NightColor);
                    currentEffect.Parameters["SunColor"].SetValue(OverheadSunColor);
                    currentEffect.Parameters["HorizonColor"].SetValue(HorizonColor);

                    currentEffect.Parameters["MorningTint"].SetValue(MorningTint);
                    currentEffect.Parameters["EveningTint"].SetValue(EveningTint);
                    currentEffect.Parameters["timeOfDay"].SetValue(Time.GetGameTimeOfDay());
                }
                mesh.Draw();
            }

            // draw clouds
            var matrix = Matrix.CreateTranslation(Vector3.Zero)*Matrix.CreateScale(100)* Matrix.CreateTranslation(new Vector3(this._camera.Position.X, this._camera.Position.Y - 40, this._camera.Position.Z)); // move sky to camera position and should be scaled -- bigger than the world.
            foreach (var mesh in _skyDome.Meshes)
            {
                foreach (var currentEffect in mesh.Effects)
                {
                    var worldMatrix = modelTransforms[mesh.ParentBone.Index]*matrix;
                    currentEffect.CurrentTechnique = currentEffect.Techniques["SkyStarDome"];
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(_camera.View);
                    currentEffect.Parameters["xProjection"].SetValue(_camera.Projection);
                    currentEffect.Parameters["xTexture"].SetValue(this._cloudMap);
                    currentEffect.Parameters["NightColor"].SetValue(NightColor);
                    currentEffect.Parameters["SunColor"].SetValue(OverheadSunColor);
                    currentEffect.Parameters["HorizonColor"].SetValue(HorizonColor);
                    currentEffect.Parameters["MorningTint"].SetValue(MorningTint);
                    currentEffect.Parameters["EveningTint"].SetValue(EveningTint);
                    currentEffect.Parameters["timeOfDay"].SetValue(Time.GetGameTimeOfDay());
                }
                mesh.Draw();
            }

            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default; // reset back the depth-buffer.
        }

        /// <summary>
        /// Sets screen vertices for skydome.
        /// </summary>
        /// <returns></returns>
        private static VertexPositionTexture[] SetUpFullscreenVertices()
        {
            var vertices = new VertexPositionTexture[4];

            vertices[0] = new VertexPositionTexture(new Vector3(-1, 1, 0f), new Vector2(0, 1));
            vertices[1] = new VertexPositionTexture(new Vector3(1, 1, 0f), new Vector2(1, 1));
            vertices[2] = new VertexPositionTexture(new Vector3(-1, -1, 0f), new Vector2(0, 0));
            vertices[3] = new VertexPositionTexture(new Vector3(1, -1, 0f), new Vector2(1, 0));

            return vertices;
        }

        private Texture2D CreateStaticCloudMap(int resolution)
        {
            var rand = new Random();
            var noisyColors = new Color[resolution*resolution];
            for (int x = 0; x < resolution; x++)
                for (int y = 0; y < resolution; y++)
                    noisyColors[x + y*resolution] = new Color(new Vector3(rand.Next(1000)/1000.0f, 0, 0));

            var noiseImage = new Texture2D(GraphicsDevice, resolution, resolution, true, SurfaceFormat.Color);
            noiseImage.SetData(noisyColors);
            return noiseImage;
        }

        /// <summary>
        /// Generates dynamic clouds within GPU.
        /// </summary>
        private void GeneratePerlinNoise(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(this._cloudsRenderTarget);
            //GraphicsDevice.Clear(Color.White);

            _perlinNoiseEffect.CurrentTechnique = _perlinNoiseEffect.Techniques["PerlinNoise"];
            _perlinNoiseEffect.Parameters["xTexture"].SetValue(this._staticCloudMap);
            _perlinNoiseEffect.Parameters["xOvercast"].SetValue(CloudOvercast);
            _perlinNoiseEffect.Parameters["xTime"].SetValue((float) gameTime.TotalGameTime.TotalMilliseconds/100000.0f);

            foreach (EffectPass pass in _perlinNoiseEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, _fullScreenVertices, 0, 2);
            }

            GraphicsDevice.SetRenderTarget(null);
            this._cloudMap = _cloudsRenderTarget;
        }

        public void ToggleDynamicClouds()
        {
            this._dynamicCloudsEnabled = !this._dynamicCloudsEnabled;
        }
    }
}