using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqEngine.Logging;

namespace VolumetricStudios.Voxeliq
{
    public interface ICamera
    {
        /// <summary>
        /// Camera's projection matrix (lens).
        /// </summary>
        Matrix Projection { get; }

        /// <summary>
        /// Cameras view matrix (position).
        /// </summary>
        Matrix View { get; }

        /// <summary>
        /// The world matrix.
        /// </summary>
        Matrix World { get; }

        /// <summary>
        /// Camera's position.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// Current camera elevation.
        /// </summary>
        float CurrentElevation { get; }

        /// <summary>
        /// Current camera rotation.
        /// </summary>
        float CurrentRotation { get; }
    }

    public interface ICameraController
    {
        /// <summary>
        /// Rotates camera in x-axis.
        /// </summary>
        /// <param name="step">The amount to rotate.</param>
        void RotateCamera(float step);

        /// <summary>
        /// Elevates camera in y-axis.
        /// </summary>
        /// <param name="step">The amount to elavate</param>
        void ElevateCamera(float step);

        /// <summary>
        /// Turns camera to given target vector.
        /// </summary>
        /// <param name="target"></param>
        void LookAt(Vector3 target);
    }

    public class Camera : GameComponent, ICamera, ICameraController
    {
        // camera properties.
        public Matrix Projection { get; private set; } // the camera lens.
        public Matrix View { get; private set; } // the camera position.
        public Matrix World { get; private set; } // the world.
        public Vector3 Position { get; private set; } // the camera position.
        public float CurrentElevation { get; private set; } // camera's current elevation.
        public float CurrentRotation { get; private set; } // camera's current rotation.

        // constants & values.
        private const float ViewAngle = MathHelper.PiOver4; // the field of view of the lens -- states how wide or narrow it is.
        private const float NearPlaneDistance = 0.01f; // the near plane distance. objects between near and far plane distance will be get rendered.
        private const float FarPlaneDistance = 1000f; // the far plance distance, objects behinds this will not get rendered.
        private const float RotationSpeed = 0.025f; // the rotation speed.

        // imported services.
        private IPlayer _player;

        // misc.
        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility

        public Camera(Game game) : base(game)
        {
            game.Services.AddService(typeof(ICamera),this); // export services.
            game.Services.AddService(typeof (ICameraController), this);
        }

        public override void Initialize()
        {
            Logger.Trace("init");

            // import required services.
            this._player = (IPlayer) this.Game.Services.GetService(typeof (IPlayer));

            // initially setup camera.
            this.World = Matrix.Identity;
            this.Projection = Matrix.CreatePerspectiveFieldOfView(ViewAngle, this.Game.GraphicsDevice.Viewport.AspectRatio, NearPlaneDistance, FarPlaneDistance); // set field of view of the camera.
            this.Position = this._player.Position;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            this.Position = this._player.Position; // attach camera to player.
            var rotation = Matrix.CreateRotationX(CurrentElevation) * Matrix.CreateRotationY(CurrentRotation); // transform camera position based on rotation and elevation.
            var target = Vector3.Transform(Vector3.Forward, rotation) + Position;
            var upVector = Vector3.Transform(Vector3.Up, rotation);
            this.View = Matrix.CreateLookAt(Position, target, upVector);
        }

        /// <summary>
        /// Makes camera to look at given Vector3 target.
        /// </summary>
        /// <param name="target"></param>
        public void LookAt(Vector3 target)
        {
            this.View = Matrix.CreateLookAt(Position, target, Vector3.Up);
        }

        /// <summary>
        /// Rotates camera.
        /// </summary>
        /// <param name="step"></param>
        public void RotateCamera(float step)
        {
            this.CurrentRotation -= RotationSpeed * (step / 25);
        }

        /// <summary>
        /// Elevates camera.
        /// </summary>
        /// <param name="step"></param>
        public void ElevateCamera(float step)
        {
            this.CurrentElevation -= RotationSpeed * (step / 25);
        }
    }
}
