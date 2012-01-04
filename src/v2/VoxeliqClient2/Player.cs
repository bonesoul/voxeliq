using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using VolumetricStudios.VoxeliqEngine;
using VolumetricStudios.VoxeliqEngine.Core;
using VolumetricStudios.VoxeliqEngine.Movement;
using VolumetricStudios.VoxeliqEngine.Screen;
using VolumetricStudios.VoxeliqEngine.Utility.Logging;

namespace VolumetricStudios.VoxeliqClient
{
    public class Player : GameComponent, IPlayer
    {
        public Vector3 Position { get; private set; }

        private ICameraService _camera;

        private const float MoveSpeed = 5f; // the move speed.
        private const float FlySpeed = 25f; // the fly speed.
        private const float Gravity = -15f; // the gravity
        private const float JumpVelocity = 6f; // the jump velocity.

        private static readonly Logger Logger = LogManager.CreateLogger(); // Logging facility.

        public Player(Game game) 
            : base(game, true)
        {
            this.Game.AddService(typeof(IPlayer), this);
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            // import required services.
            this._camera = (ICameraService)this.Game.GetService(typeof(ICameraService));
        }
        
        public void Move(GameTime gameTime, MoveDirection direction)
        {
            var moveVector = Vector3.Zero;

            switch(direction)
            {
                case MoveDirection.Forward:
                    moveVector.Z--;
                    break;
                case MoveDirection.Backward:
                    moveVector.Z++;
                    break;
                case MoveDirection.Left:
                    moveVector.X--;
                    break;
                case MoveDirection.Right:
                    moveVector.X++;
                    break;
            }

            if (moveVector == Vector3.Zero) 
                return;

            moveVector *= MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
