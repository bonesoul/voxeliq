/*
 * Copyright (C) 2011 voxeliq project 
 *
 */

using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqEngine.Common.Logging;
using VolumetricStudios.VoxeliqEngine.Screen;
using VolumetricStudios.VoxeliqEngine.Universe;

namespace VolumetricStudios.VoxeliqClient.Screen
{    
    public class Camera : GameComponent, ICameraService, ICameraControlService
    {
        public Matrix Projection { get; private set; } // the camera lens.
        public Matrix View { get; private set; } // the camera position.
        public Matrix World { get; private set; } // the world.
        public Vector3 Position { get; private set; }
        public float CurrentElevation { get; private set; }
        public float CurrentRotation { get; private set; }

        private IPlayer _player;
        private const float ViewAngle = MathHelper.PiOver4; // the field of view of the lens -- states how wide or narrow it is.
        private const float NearPlaneDistance = 0.01f; // the near plane distance. objects between near and far plane distance will be get rendered.
        private const float FarPlaneDistance = 1000f; // the far plance distance, objects behinds this will not get rendered.
        private const float RotationSpeed = 0.025f; // the rotation speed.
        private float _aspectRatio; //aspect ratio of the field of view (width/height). 

        /// <summary>
        /// Logging facility.
        /// </summary>
        private static readonly Logger Logger = LogManager.CreateLogger();

        public Camera(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(ICameraService), this);
            game.Services.AddService(typeof(ICameraControlService), this);
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            this._player = (IPlayer)this.Game.Services.GetService(typeof(IPlayer));
            this._aspectRatio = Game.GraphicsDevice.Viewport.AspectRatio;
            this.World = Matrix.Identity;
            this.Projection = Matrix.CreatePerspectiveFieldOfView(ViewAngle, _aspectRatio, NearPlaneDistance, FarPlaneDistance); // set field of view of the camera.
            this.Position = this._player.Position;
        }

        public override void Update(GameTime gameTime)
        {
            this.Position = this._player.Position;
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

        public void RotateCamera(float step)
        {
            this.CurrentRotation -= RotationSpeed*(step/25);
        }

        public void ElevateCamera(float step)
        {
            this.CurrentElevation -= RotationSpeed*(step/25);
        }
    }
}
