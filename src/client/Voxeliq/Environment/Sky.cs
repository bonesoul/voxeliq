using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxeliqEngine.Logging;

namespace VolumetricStudios.Voxeliq.Environment
{
    public interface ISky
    {
        /// <summary>
        /// Toggled dynamic clouds.
        /// </summary>
        void ToggleDynamicClouds();
    }

    public class Sky : DrawableGameComponent, ISky
    {
        private Model _dome; // sky dome model.
        private Texture2D _cloudMap; // cloud map.
        private Texture2D _staticCloudMap; // gpu generated cloud maps. // TODO: naming conflict?
        private Effect _perlinNoiseEffect; // noise used for generating clouds.
        private RenderTarget2D _cloudsRenderTarget; // render target for clouds.
        private VertexPositionTexture[] _fullScreenVertices; // vertices.
        private bool _dynamicCloudsEnabled; // Is dynamic clouds enabled?
        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility

        public Sky(Game game, bool enableDynamicClouds = true)
            : base(game)
        {
            this._dynamicCloudsEnabled = enableDynamicClouds;
            this.Game.Services.AddService(typeof (ISky), this); // export service.
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public void ToggleDynamicClouds()
        {
            this._dynamicCloudsEnabled = !this._dynamicCloudsEnabled;
        }
    }
}
