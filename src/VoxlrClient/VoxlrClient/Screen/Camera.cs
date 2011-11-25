/*
 * Copyright (C) 2011 voxlr project 
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using Microsoft.Xna.Framework;
using VolumetricStudios.VoxlrEngine.Screen;
using VolumetricStudios.VoxlrEngine.Universe;

namespace VolumetricStudios.VoxlrClient.Screen
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

        public Camera(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(ICameraService), this);
            game.Services.AddService(typeof(ICameraControlService), this);
        }

        public override void Initialize()
        {
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
