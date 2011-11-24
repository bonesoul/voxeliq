/*
 * Copyright (C) 2011 - Hüseyin Uslu shalafiraistlin@gmail.com
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
using VolumetricStudios.VoxlrEngine.Screen;
using VolumetricStudios.VoxlrEngine.Universe;
using VolumetricStudios.VoxlrEngine.Utils.Vector;

namespace VolumetricStudios.VoxlrClient
{
    public sealed class Player : DrawableGameComponent, IPlayer
    {        
        public bool FlyingEnabled { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector2Int RelativePosition { get; private set; }
        
        public Chunk CurrentChunk { get; set; }
        public Chunk LastChunk { get; set; }
        public Weapon Weapon { get; set; }
        public Vector3 LookVector { get; set; }
        public PositionedBlock? AimedSolidBlock { get; private set; } // nullable object.        
        public PositionedBlock? AimedEmptyBlock { get; private set; } // nullable object.        
        public Vector3 Velocity;

        private readonly World _world;
        private ICameraService _camera;
        private BasicEffect _aimedBlockEffect;
        private Model _aimedBlockModel;
        private Texture2D _aimedBlockTexture;
        private const float MoveSpeed = 5f; // the move speed.
        private const float FlySpeed = 25f; // the fly speed.
        private const float Gravity = -15f;
        private const float JumpVelocity = 6f;
        
        public Player(Game game,World world)
            : base(game)
        {
            game.Services.AddService(typeof(IPlayer), this);
            this._world = world;
        }

        public override void Initialize()
        {
            this._camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));
            this.FlyingEnabled = true;
            this.Weapon = new Shovel(Game);
            this.LoadContent();

            this.Weapon.Initialize();
        }

        protected override void LoadContent()
        {            
            this._aimedBlockEffect = new BasicEffect(Game.GraphicsDevice);
            this._aimedBlockModel = Game.Content.Load<Model>("Models\\AimedBlock");
            this._aimedBlockTexture = Game.Content.Load<Texture2D>("Textures\\AimedBlock");
        }

        public override void Update(GameTime gameTime)
        {
            this.ProcessPosition(gameTime);
            this.ProcessView();
        }

        private void ProcessPosition(GameTime gameTime)
        {
            if (FlyingEnabled) return;

            this.Velocity.Y += Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            var footPosition = Position + new Vector3(0f, -1.5f, 0f);
            Block standingBlock = _world.BlockAt(footPosition);

            if (standingBlock.Exists) this.Velocity.Y = 0;
            this.Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;            
        }

        private void ProcessView()
        {
            if (FlyingEnabled) return;
            var rotationMatrix = Matrix.CreateRotationX(this._camera.CurrentElevation) * Matrix.CreateRotationY(this._camera.CurrentRotation);
            this.LookVector = Vector3.Transform(Vector3.Forward, rotationMatrix);
            this.LookVector.Normalize();
            this.FindAimedBlock();
        }

        public void Jump()
        {
            var footPosition = Position + new Vector3(0f, -1.5f, 0f);
            Block standingBlock = _world.BlockAt(footPosition);

            if (!standingBlock.Exists && this.Velocity.Y != 0) return;
            float amountBelowSurface = ((ushort)footPosition.Y) + 1 - footPosition.Y;
            Position += new Vector3(0, amountBelowSurface + 0.01f, 0);

            this.Velocity.Y = JumpVelocity;
        }

        public void Move(GameTime gameTime, MoveDirection direction)
        {
            var moveVector = Vector3.Zero;

            switch (direction)
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

            if (moveVector == Vector3.Zero) return;

            if (!FlyingEnabled)
            {
                moveVector *= MoveSpeed*(float) gameTime.ElapsedGameTime.TotalSeconds;
                var rotation = Matrix.CreateRotationY(this._camera.CurrentRotation);
                var rotatedVector = Vector3.Transform(moveVector, rotation);
                TryMove(rotatedVector);
            }            
            else
            {
                moveVector *= FlySpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                var rotation = Matrix.CreateRotationX(this._camera.CurrentElevation)*Matrix.CreateRotationY(this._camera.CurrentRotation);
                var rotatedVector = Vector3.Transform(moveVector, rotation);
                this.Position += (rotatedVector);
            }
        }
        
        private void TryMove(Vector3 moveVector)
        {
            // build a test move-vector slightly longer than moveVector.
            Vector3 testVector = moveVector;
            testVector.Normalize();
            testVector *= moveVector.Length() + 0.3f;
            var footPosition = Position + new Vector3(0f, -0.5f, 0f);
            Vector3 testPosition = footPosition + testVector;            
            if(_world.BlockAt(testPosition).Exists) return;

            this.Position += moveVector;
        }

        public void SpawnPlayer(Vector2Int relativePosition)
        {
            this.RelativePosition = relativePosition;
            this.Position = new Vector3(relativePosition.X*Chunk.WidthInBlocks, 150, relativePosition.Z*Chunk.LenghtInBlocks);
            this._world.SpawnPlayer(relativePosition);
        }

        public void ToggleFlyForm()
        {
            this.FlyingEnabled = !this.FlyingEnabled;
        }

        private void FindAimedBlock()
        {
            for (float x = 0.5f; x < 8f; x += 0.1f)
            {
                Vector3 target = this._camera.Position + (LookVector*x);
                var block = _world.BlockAt(target);
                if(!block.Exists) this.AimedEmptyBlock = new PositionedBlock(new Vector3Int(target), block);                                        
                else
                {
                    this.AimedSolidBlock = new PositionedBlock(new Vector3Int(target), block);
                    return;
                }
            }

            this.AimedSolidBlock = null;
        }

        public override void Draw(GameTime gameTime)
        {
            if (this.AimedSolidBlock.HasValue) RenderAimedBlock();
        }

        private void RenderAimedBlock()
        {
            Game.GraphicsDevice.BlendState = BlendState.NonPremultiplied; // allows any transparent pixels in original PNG to draw transparent
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            var position = this.AimedSolidBlock.Value.Position.AsVector3() + new Vector3(0.5f, 0.5f, 0.5f);
            Matrix matrix_a, matrix_b;
            Matrix identity = Matrix.Identity;                       // setup the matrix prior to translation and scaling  
            Matrix.CreateTranslation(ref position, out matrix_a);    // translate the position a half block in each direction
            Matrix.CreateScale(0.505f, out matrix_b);          // scales the selection box slightly larger than the targetted block
            identity = Matrix.Multiply(matrix_b, matrix_a);          // the final position of the block

            _aimedBlockEffect.World = identity;
            _aimedBlockEffect.View = _camera.View;
            _aimedBlockEffect.Projection = _camera.Projection;
            _aimedBlockEffect.Texture = _aimedBlockTexture;
            _aimedBlockEffect.TextureEnabled = true;

            foreach (EffectPass pass in _aimedBlockEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                for(int i=0;i< _aimedBlockModel.Meshes[0].MeshParts.Count; i++)
                {
                    ModelMeshPart parts = _aimedBlockModel.Meshes[0].MeshParts[i];
                    if (parts.NumVertices == 0) continue;

                    Game.GraphicsDevice.Indices = parts.IndexBuffer;
                    Game.GraphicsDevice.SetVertexBuffer(parts.VertexBuffer);
                    Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, parts.NumVertices, parts.StartIndex, parts.PrimitiveCount);
                }
            }
        }
    }
}
