using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using VolumetricStudios.VoxeliqEngine.Logging;

namespace VolumetricStudios.Voxeliq
{
    public interface IPlayer
    {
        /// <summary>
        /// The real player position.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// Sets camera position.
        /// </summary>
        void Move(GameTime gameTime, MoveDirection direction);
    }

    public class Player : DrawableGameComponent, IPlayer
    {
        public Vector3 Position { get; private set; } // The real player position.
        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility

        public Player(Game game) : base(game)
        {
            this.Game.Services.AddService(typeof (IPlayer), this); // export service.
        }        

        public override void Initialize()
        {
            Logger.Trace("init");
            base.Initialize();
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

        public void Move(GameTime gameTime, MoveDirection direction)
        {

        }
    }

    public enum MoveDirection
    {
        Forward,
        Backward,
        Left,
        Right,
    }
}
