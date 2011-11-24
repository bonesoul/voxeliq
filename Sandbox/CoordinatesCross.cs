using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace blocksGame.Objects
{
    class CoordinatesCross
    {
        private VertexPositionColor[] _vertices;
        private readonly GraphicsDevice _device;
        private readonly BasicEffect _basicEffect;        

        public CoordinatesCross(GraphicsDevice device)
        {
            this._device = device;
            _basicEffect = new BasicEffect(device);

            InitVertices();
        }

        private void InitVertices()
        {
            this._vertices = new VertexPositionColor[30];

            this._vertices[0] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Red);
            this._vertices[1] = new VertexPositionColor(Vector3.Right * 5, Color.Red);
            this._vertices[2] = new VertexPositionColor(new Vector3(5, 0, 0), Color.Red);
            this._vertices[3] = new VertexPositionColor(new Vector3(4.5f, 0.5f, 0), Color.Red);
            this._vertices[4] = new VertexPositionColor(new Vector3(5, 0, 0), Color.Red);
            this._vertices[5] = new VertexPositionColor(new Vector3(4.5f, -0.5f, 0), Color.Red);

            this._vertices[6] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Blue);
            this._vertices[7] = new VertexPositionColor(Vector3.Up * 5, Color.Blue);
            this._vertices[8] = new VertexPositionColor(new Vector3(0, 5, 0), Color.Blue);
            this._vertices[9] = new VertexPositionColor(new Vector3(0.5f, 4.5f, 0), Color.Blue);
            this._vertices[10] = new VertexPositionColor(new Vector3(0, 5, 0), Color.Blue);
            this._vertices[11] = new VertexPositionColor(new Vector3(-0.5f, 4.5f, 0), Color.Blue);

            this._vertices[12] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Green);
            this._vertices[13] = new VertexPositionColor(Vector3.Forward * 5, Color.Green);
            this._vertices[14] = new VertexPositionColor(new Vector3(0, 0, -5), Color.Green);
            this._vertices[15] = new VertexPositionColor(new Vector3(0, 0.5f, -4.5f), Color.Green);
            this._vertices[16] = new VertexPositionColor(new Vector3(0, 0, -5), Color.Green);
            this._vertices[17] = new VertexPositionColor(new Vector3(0, -0.5f, -4.5f), Color.Green);
        }

        public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            this._basicEffect.World = Matrix.Identity;
            this._basicEffect.View = viewMatrix;
            this._basicEffect.Projection = projectionMatrix;
            this._basicEffect.VertexColorEnabled = true;

            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this._device.DrawUserPrimitives(PrimitiveType.LineList, _vertices, 0, 9);
            }
        }
    }
}
