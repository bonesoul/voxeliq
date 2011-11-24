/*    
 * Copyright (C) 2011, Hüseyin Uslu
 *  
 */

using Microsoft.Xna.Framework;

namespace VolumetricStudios.VoxlrClient.States
{
    /// <summary>
    /// An abstract game-state class.
    /// </summary>
    public abstract class GameState
    {
        public Game Game { get; private set; }

        protected GameState(Game game)
        {
            this.Game = game;
        }

        public virtual void Initialize() { }
        public virtual void LoadContent() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }
    }
}
