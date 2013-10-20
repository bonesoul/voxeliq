/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using Engine.Assets;
using Engine.Blocks;
using Engine.Chunks;
using Engine.Common.Logging;
using Engine.Common.Vector;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Universe
{
    /// <summary>
    /// Interface for controlling player.
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// The real player position.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// Last chunk the player was on.
        /// </summary>
        Chunk LastChunk { get; set; }

        /// <summary>
        /// The current chunk the player is in.
        /// </summary>
        Chunk CurrentChunk { get; set; }

        /// <summary>
        /// The weapon player is wielding.
        /// </summary>
        Weapon Weapon { get; set; }

        PositionedBlock? AimedSolidBlock { get; }

        PositionedBlock? AimedEmptyBlock { get; }

        /// <summary>
        /// Moves camera in given direction.
        /// </summary>
        /// <param name="gameTime"> </param>
        /// <param name="direction"><see cref="MoveDirection"/></param>
        void Move(GameTime gameTime, MoveDirection direction);

        /// <summary>
        /// Let's the player jump.
        /// </summary>
        void Jump();

        /// <summary>
        /// Sets camera position.
        /// </summary>
        /// <param name="position">The position in Vector3.</param>
        void SpawnPlayer(Vector2Int position);

        /// <summary>
        /// Is flying enabled?
        /// </summary>
        /// <returns></returns>
        bool FlyingEnabled { get; set; }

        /// <summary>
        /// Toggles fly form.
        /// </summary>
        void ToggleFlyForm();
    }

    public class Player : DrawableGameComponent, IPlayer
    {
        public bool FlyingEnabled { get; set; }
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
        private BasicEffect _aimedBlockEffect;
        private Model _aimedBlockModel;
        private Texture2D _aimedBlockTexture;

        private Model _sampleModel;

        private const float MoveSpeed = 5f; // the move speed.
        private const float FlySpeed = 25f; // the fly speed.
        private const float Gravity = -15f;
        private const float JumpVelocity = 6f;

        // required services.
        private ICamera _camera;
        private IChunkCache _chunkCache;
        private IAssetManager _assetManager;

        // misc
        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility

        public Player(Game game, World world)
            : base(game)
        {
            this._world = world;
            game.Services.AddService(typeof (IPlayer), this); // export service.
        }

        public override void Initialize()
        {
            Logger.Trace("init()");

            this.FlyingEnabled = true;
            this.Weapon = new Shovel(Game);

            // import required services.
            this._camera = (ICamera) this.Game.Services.GetService(typeof (ICamera));
            this._chunkCache = (IChunkCache) this.Game.Services.GetService(typeof (IChunkCache));

            this._assetManager = (IAssetManager)this.Game.Services.GetService(typeof(IAssetManager));
            if (this._assetManager == null)
                throw new NullReferenceException("Can not find asset manager component.");

            this.LoadContent();

            this.Weapon.Initialize();
        }

        protected override void LoadContent()
        {
            this._aimedBlockModel = this._assetManager.AimedBlockModel;
            this._aimedBlockEffect = this._assetManager.AimedBlockEffect;
            this._aimedBlockTexture = this._assetManager.AimedBlockTexture;
            this._sampleModel = this._assetManager.SampleModel;
        }

        public override void Update(GameTime gameTime)
        {
            this.ProcessPosition(gameTime);
            this.ProcessView();
        }

        private void ProcessPosition(GameTime gameTime)
        {
            if (FlyingEnabled) return;

            this.Velocity.Y += Gravity*(float) gameTime.ElapsedGameTime.TotalSeconds;
            var footPosition = Position + new Vector3(0f, -1.5f, 0f);
            Block standingBlock = BlockStorage.BlockAt(footPosition);

            if (standingBlock.Exists) this.Velocity.Y = 0;
            this.Position += Velocity*(float) gameTime.ElapsedGameTime.TotalSeconds;
        }

        private void ProcessView()
        {
            if (FlyingEnabled) return;
            var rotationMatrix = Matrix.CreateRotationX(this._camera.CurrentElevation)*
                                 Matrix.CreateRotationY(this._camera.CurrentRotation);
            this.LookVector = Vector3.Transform(Vector3.Forward, rotationMatrix);
            this.LookVector.Normalize();
            this.FindAimedBlock();
        }

        public void Jump()
        {
            var footPosition = Position + new Vector3(0f, -1.5f, 0f);
            Block standingBlock = BlockStorage.BlockAt(footPosition);

            if (!standingBlock.Exists && this.Velocity.Y != 0) return;
            float amountBelowSurface = ((ushort) footPosition.Y) + 1 - footPosition.Y;
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
                moveVector *= FlySpeed*(float) gameTime.ElapsedGameTime.TotalSeconds;
                var rotation = Matrix.CreateRotationX(this._camera.CurrentElevation)*
                               Matrix.CreateRotationY(this._camera.CurrentRotation);
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
            if (BlockStorage.BlockAt(testPosition).Exists) return;

            // There should be some bounding box so his head does not enter a block above ;) /fasbat
            testPosition -= 2*new Vector3(0f, -0.5f, 0f);
            if (BlockStorage.BlockAt(testPosition).Exists) return;


            this.Position += moveVector;
        }

        public void SpawnPlayer(Vector2Int relativePosition)
        {
            this.RelativePosition = relativePosition;
            this.Position = new Vector3(relativePosition.X*Chunk.WidthInBlocks, 150,
                                        relativePosition.Z*Chunk.LenghtInBlocks);
            this._world.SpawnPlayer(relativePosition);
        }

        private void FindAimedBlock()
        {
            for (float x = 0.5f; x < 8f; x += 0.1f)
            {
                Vector3 target = this._camera.Position + (LookVector*x);
                var block = BlockStorage.BlockAt(target);
                if (!block.Exists) this.AimedEmptyBlock = new PositionedBlock(new Vector3Int(target), block);
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

            //// draw sample model
            //var transforms = new Matrix[_sampleModel.Bones.Count];
            //_sampleModel.CopyAbsoluteBoneTransformsTo(transforms);

            //Vector3 modelPosition = new Vector3(this.Position.X, this.Position.Y, this.Position.Z);
            //float modelRotation = 0.0f;

            //foreach(var mesh in _sampleModel.Meshes)
            //{
            //    foreach(BasicEffect effect in mesh.Effects)
            //    {
            //        effect.EnableDefaultLighting();

            //        effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(modelRotation) * Matrix.CreateTranslation(modelPosition);
            //        effect.View = Matrix.CreateLookAt(this._camera.Position, Vector3.Zero, Vector3.Up);
            //        effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), Game.GraphicsDevice.Viewport.AspectRatio, 1.0f, 10000.0f);
            //    }

            //    mesh.Draw();
            //}
        }

        private void RenderAimedBlock()
        {
            Game.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
                // allows any transparent pixels in original PNG to draw transparent
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            var position = this.AimedSolidBlock.Value.Position.AsVector3() + new Vector3(0.5f, 0.5f, 0.5f);
            Matrix matrix_a, matrix_b;
            Matrix identity = Matrix.Identity; // setup the matrix prior to translation and scaling  
            Matrix.CreateTranslation(ref position, out matrix_a);
                // translate the position a half block in each direction
            Matrix.CreateScale(0.505f, out matrix_b);
                // scales the selection box slightly larger than the targetted block
            identity = Matrix.Multiply(matrix_b, matrix_a); // the final position of the block

            _aimedBlockEffect.World = identity;
            _aimedBlockEffect.View = _camera.View;
            _aimedBlockEffect.Projection = _camera.Projection;
            _aimedBlockEffect.Texture = _aimedBlockTexture;
            _aimedBlockEffect.TextureEnabled = true;

            foreach (EffectPass pass in _aimedBlockEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                for (int i = 0; i < _aimedBlockModel.Meshes[0].MeshParts.Count; i++)
                {
                    ModelMeshPart parts = _aimedBlockModel.Meshes[0].MeshParts[i];
                    if (parts.NumVertices == 0) continue;

                    Game.GraphicsDevice.Indices = parts.IndexBuffer;
                    Game.GraphicsDevice.SetVertexBuffer(parts.VertexBuffer);
                    Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, parts.NumVertices,
                                                              parts.StartIndex, parts.PrimitiveCount);
                }
            }
        }

        public void ToggleFlyForm()
        {
            this.FlyingEnabled = !this.FlyingEnabled;
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