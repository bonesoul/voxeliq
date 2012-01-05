using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;
using VolumetricStudios.VoxeliqEngine.Logging;

namespace VolumetricStudios.Voxeliq.Environment
{
    public interface ISky { }

    public class Sky : DrawableGameComponent , ISky
    {
        private Model _skybox;
        private Matrix _skyboxWorld = Matrix.Identity;

        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility.

        public Sky(Game game) : base(game)
        {
            this.Game.Services.AddService(typeof(ISky), this); // export the service
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            this.LoadContent(); // TODO: load content should be called by the framework itself!
        }

        protected override void LoadContent()
        {
            // Load the sky box, which is rendered outside of the lighting system.
            this._skybox = Game.Content.Load<Model>("Models/sky/skybox");
            this._skyboxWorld = Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, -5.0f)) * Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(-MathHelper.Pi * 0.85f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            return;
            //this.Game.GraphicsDevice.Clear(Color.WhiteSmoke);

            if (this._skybox == null)
            {
                // If the skybox is missing clear the entire back and depth buffer.
                this.Game.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.Black, 1.0f, 0);
                return;
            }

            // The skybox automatically clears the back buffer so only clear the depth buffer (saves memory bandwidth/fillrate).
            this.Game.GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.Black, 1.0f, 0);

            this.Game.GraphicsDevice.BlendState = BlendState.Opaque;
            this.Game.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            this.Game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            this.Game.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;

            for (int i = 1; i < 8; i++)
                this.Game.GraphicsDevice.SamplerStates[i] = SamplerState.PointClamp;

            Matrix lockedview = ((VoxeliqGame)this.Game).SceneState.View;
            lockedview.Translation = Vector3.Zero;

            foreach (ModelMesh mesh in this._skybox.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    if (!(effect is BasicEffect))
                        continue;

                    var basic = effect as BasicEffect;
                    basic.LightingEnabled = false;
                    basic.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);

                    basic.View = lockedview;
                    basic.World = this._skyboxWorld;
                    basic.Projection = ((VoxeliqGame)this.Game).SceneState.Projection;
                }

                mesh.Draw();
            }

            this.Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }
    }
}
