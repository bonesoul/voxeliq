/*
 * Copyright (C) 2011-2012 voxeliq project 
 *
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxeliqGame.Common.Logging;

namespace VolumetricStudios.VoxeliqGame.Environment
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

        // mdoel, textures, effects.
        private Model _dome; // sky dome model.
        private Texture2D _cloudMap; // the cloud map.
        private Texture2D _staticCloudMap; // gpu generated cloud maps.
        private Effect _perlinNoiseEffect; // noise used for generating clouds.
        private RenderTarget2D _cloudsRenderTarget; // render target for clouds.
        private VertexPositionTexture[] _fullScreenVertices; // vertices.

        // misc.
        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility

        // required services.
        private ICameraService _camera;

        public Sky(Game game, bool enableDynamicClouds = true)
            : base(game)
        {
            this._dynamicCloudsEnabled = enableDynamicClouds;
            this.Game.Services.AddService(typeof(ISkyService), this); // export the service.
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            // import require services.
            this._camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));

            base.Initialize();
        }

        protected override void LoadContent()
        {    
            // load required assets.
            this._dome = Game.Content.Load<Model>("Models\\SkyDome");
            this._dome.Meshes[0].MeshParts[0].Effect = Game.Content.Load<Effect>("Effects\\SkyDome");
            this._cloudMap = Game.Content.Load<Texture2D>("Textures\\cloudmap");
            this._perlinNoiseEffect = Game.Content.Load<Effect>("Effects\\PerlinNoise");

            var presentation = GraphicsDevice.PresentationParameters;
            this._cloudsRenderTarget = new RenderTarget2D(GraphicsDevice, presentation.BackBufferWidth, presentation.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None); // the mipmap does not work on all configurations            
            this._staticCloudMap = this.CreateStaticCloudMap(32);
            this._fullScreenVertices = SetUpFullscreenVertices();
        }               

        /// <summary>
        /// Draws the sky.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (this._dynamicCloudsEnabled) // if dynamic-cloud generation is on, generate them.
                this.GenerateClouds(gameTime);

            this.GraphicsDevice.Clear(Color.WhiteSmoke);
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.None; // disable the depth-buffer for drawing the sky because it's the farthest object we'll be drawing.
            var modelTransforms = new Matrix[this._dome.Bones.Count]; // transform dome's bones.
            this._dome.CopyAbsoluteBoneTransformsTo(modelTransforms);

            var matrix = Matrix.CreateTranslation(Vector3.Zero)*Matrix.CreateScale(100) * Matrix.CreateTranslation(new Vector3(this._camera.Position.X, this._camera.Position.Y - 40, this._camera.Position.Z)); // move sky to camera position and should be scaled -- bigger than the world.
            
                       
            foreach (var mesh in _dome.Meshes)
            {
                foreach (var currentEffect in mesh.Effects)
                {
                    var worldMatrix = modelTransforms[mesh.ParentBone.Index] * matrix;
                    currentEffect.CurrentTechnique = currentEffect.Techniques["SkyDome"];
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(_camera.View);
                    currentEffect.Parameters["xProjection"].SetValue(_camera.Projection);
                    currentEffect.Parameters["xTexture"].SetValue(this._cloudMap);
                    currentEffect.Parameters["SunColor"].SetValue(Color.Blue.ToVector4());
                    currentEffect.Parameters["HorizonColor"].SetValue(Color.White.ToVector4());
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
            var noisyColors = new Color[resolution * resolution];
            for (int x = 0; x < resolution; x++)
                for (int y = 0; y < resolution; y++)
                    noisyColors[x + y * resolution] = new Color(new Vector3(rand.Next(1000) / 1000.0f, 0, 0));

            var noiseImage = new Texture2D(GraphicsDevice, resolution, resolution, true, SurfaceFormat.Color);
            noiseImage.SetData(noisyColors);
            return noiseImage;
        }

        /// <summary>
        /// Genereates dynamic clouds.
        /// </summary>
        private void GenerateClouds(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(this._cloudsRenderTarget);
            GraphicsDevice.Clear(Color.White);

            _perlinNoiseEffect.CurrentTechnique = _perlinNoiseEffect.Techniques["PerlinNoise"];
            _perlinNoiseEffect.Parameters["xTexture"].SetValue(this._staticCloudMap);
            _perlinNoiseEffect.Parameters["xOvercast"].SetValue(0.8f);
            _perlinNoiseEffect.Parameters["xTime"].SetValue((float)gameTime.TotalGameTime.TotalMilliseconds / 100000.0f);

            foreach(EffectPass pass in _perlinNoiseEffect.CurrentTechnique.Passes)
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
