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
using Microsoft.Xna.Framework.Graphics;
using VolumetricStudios.VoxlrEngine.Universe;

namespace VolumetricStudios.VoxlrClient.GameEngine
{
    /// <summary>
    /// The game world.
    /// </summary>
    public class GameWorld : World, IGameComponent, IDrawable
    {
        public GameWorld(Game game) : base(game)
        {
            game.Services.AddService(typeof(IWorldStatisticsService), this);
            game.Services.AddService(typeof(IWorldService), this);
        }

        #region world-drawer

        public override void Draw(GameTime gameTime)
        {
            var viewFrustrum = new BoundingFrustum(this.Camera.View * this.Camera.Projection);

            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game.GraphicsDevice.BlendState = BlendState.Opaque;

            _blockEffect.Parameters["World"].SetValue(Matrix.Identity);
            _blockEffect.Parameters["View"].SetValue(this.Camera.View);
            _blockEffect.Parameters["Projection"].SetValue(this.Camera.Projection);
            _blockEffect.Parameters["CameraPosition"].SetValue(this.Camera.Position);
            _blockEffect.Parameters["FogColor"].SetValue(Color.White.ToVector4());
            _blockEffect.Parameters["FogNear"].SetValue(this._fogVectors[(byte)this.FogState].X);
            _blockEffect.Parameters["FogFar"].SetValue(this._fogVectors[(byte)this.FogState].Y);
            _blockEffect.Parameters["SunColor"].SetValue(Color.White.ToVector3());
            _blockEffect.Parameters["BlockTextureAtlas"].SetValue(_blockTextureAtlas);

            this.ChunksDrawn = 0;
            foreach (EffectPass pass in this._blockEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                foreach (Chunk chunk in this.Chunks.Values)
                {
                    if (!chunk.Generated || !chunk.BoundingBox.Intersects(viewFrustrum) || chunk.IndexBuffer == null) continue;

                    lock (chunk)
                    {
                        if (chunk.IndexBuffer.IndexCount == 0) continue;
                        Game.GraphicsDevice.SetVertexBuffer(chunk.VertexBuffer);
                        Game.GraphicsDevice.Indices = chunk.IndexBuffer;
                        Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, chunk.VertexBuffer.VertexCount, 0, chunk.IndexBuffer.IndexCount / 3);
                    }

                    this.ChunksDrawn++;
                }
            }
        }


        #endregion
    }
}
