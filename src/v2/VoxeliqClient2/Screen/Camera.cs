using System;
using SlimDX;
using VolumetricStudios.VoxeliqEngine.Core;
using VolumetricStudios.VoxeliqEngine.Screen;
using VolumetricStudios.VoxeliqEngine.Utility.Logging;
using VolumetricStudios.VoxeliqEngine.Utility.Math;

namespace VolumetricStudios.VoxeliqClient.Screen
{
    /// <summary>
    /// The screen camera.
    /// </summary>
    public class Camera : GameComponent, ICameraService, ICameraMen
    {
        public Matrix Projection { get; private set; } // the camera lens.
        public Matrix View { get; private set; } // the camera position.
        public Matrix World { get; private set; } // the world matrix.
        public Vector3 Position { get; private set; } // the position of the camera.
        public float CurrentElevation { get; private set; } // camera elevetaion.
        public float CurrentRotation { get; private set; } // camera rotation.

        private const float ViewAngle = MathHelper.PiOver4; // the field of view of the lens -- states how wide or narrow it is.
        private const float NearPlaneDistance = 0.01f; // the near plane distance. objects between near and far plane distance will be get rendered.
        private const float FarPlaneDistance = 1000f; // the far plance distance, objects behinds this will not get rendered.
        private const float RotationSpeed = 0.025f; // the rotation speed.
        
        private readonly IGameWindow _gameWindow;

        private static readonly Logger Logger = LogManager.CreateLogger(); // Logging facility.

        public Camera(Game game)
            :base(game)
        {
            // export services.
            this.Game.AddService(typeof(ICameraService), this);
            this.Game.AddService(typeof(ICameraMen), this);

            // import required services.
            this._gameWindow = (IGameWindow) this.Game.GetService(typeof (IGameWindow));
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            this.World = Matrix.Identity; // set world.
            this.Projection = Matrix.PerspectiveFovRH(ViewAngle, this._gameWindow.AspectRatio, NearPlaneDistance, FarPlaneDistance); // set field of view of the camera.

            this.Position = new Vector3(0f, 0f, 0f);
        }

        public override void Update()
        {            
            var rotation = Matrix.RotationX(CurrentElevation) * Matrix.RotationY(CurrentRotation); // transform camera position based on rotation and elevation.
            var target = Vector3.Transform(Vectors.ForwardVector, rotation) + new Vector4(this.Position, 0);
            var upVector = Vector3.Transform(Vectors.UpVector, rotation);
            this.View = Matrix.LookAtRH(this.Position, new Vector3(target.X, target.Y, target.Z), new Vector3(upVector.X, upVector.Y, upVector.Z));
        }

        public void LookAt(Vector3 target)
        {
            this.View = Matrix.LookAtRH(this.Position, target, Vectors.UpVector);
        }

        public void RotateCamera(float step)
        {
            this.CurrentRotation -= RotationSpeed * (step / 25);
        }

        public void ElevateCamera(float step)
        {
            this.CurrentElevation -= RotationSpeed * (step / 25);
        }
    }
}
