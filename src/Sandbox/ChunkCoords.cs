        private void PrintChunk(Chunk chunk)
        {
            for(int x=0;x<Chunk.WidthInBlocks;x++)
            {
                for(int z=0;z<Chunk.LenghtInBlocks;z++)
                {
                    for(int y=0;y<Chunk.HeightInBlocks;y++)
                    {
                        this.PrintBlockPosition(chunk.Blocks[x, y, z].Type, new Vector3Int(chunk.Position.X + x, chunk.Position.Y + y, chunk.Position.Z + z));
                    }
                }
            }
        }

        private void PrintBlockPosition(BlockType type, Vector3Int position)
        {
            _spriteBatch.Begin();
            Vector3 projected = GraphicsDevice.Viewport.Project(Vector3.Zero, _camera.Projection, _camera.View, Matrix.CreateTranslation(new Vector3(position.X, position.Y, position.Z)));
            _spriteBatch.DrawString(_spriteFont, position.ToString(), new Vector2(projected.X, projected.Y), Color.White);
            _spriteBatch.End();
        }