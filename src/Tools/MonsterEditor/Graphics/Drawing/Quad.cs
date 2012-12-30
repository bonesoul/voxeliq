using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public struct Quad
{
    public Vector3 Origin;
    public Vector3 UpperLeft;
    public Vector3 LowerLeft;
    public Vector3 UpperRight;
    public Vector3 LowerRight;
    public Vector3 Normal;
    public Vector3 Up;
    public Vector3 Left;

    public VertexPositionNormalTexture[] Vertices;
    public int[] Indices;

    public Quad(Vector3 origin, Vector3 normal, Vector3 up, float width, float height)
    {
        Vertices = new VertexPositionNormalTexture[4];
        Indices = new int[6];
        Origin = origin;
        Normal = normal;
        Up = up;

        // Calculate the quad corners
        Left = Vector3.Cross(normal, Up);
        Vector3 uppercenter = (Up * height / 2) + origin;
        UpperLeft = uppercenter + (Left * width / 2);
        UpperRight = uppercenter - (Left * width / 2);
        LowerLeft = UpperLeft - (Up * height);
        LowerRight = UpperRight - (Up * height);

        FillVertices();
    }

    private void FillVertices()
    {
        // Fill in texture coordinates to display full texture
        // on quad
        Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
        Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
        Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
        Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);

        // Provide a normal for each vertex
        for (int i = 0; i < Vertices.Length; i++)
        {
            Vertices[i].Normal = Normal;
        }

        // Set the position and texture coordinate for each
        // vertex
        Vertices[0].Position = LowerLeft;
        Vertices[0].TextureCoordinate = textureLowerLeft;
        Vertices[1].Position = UpperLeft;
        Vertices[1].TextureCoordinate = textureUpperLeft;
        Vertices[2].Position = LowerRight;
        Vertices[2].TextureCoordinate = textureLowerRight;
        Vertices[3].Position = UpperRight;
        Vertices[3].TextureCoordinate = textureUpperRight;

        // Set the index buffer for each vertex, using
        // clockwise winding
        Indices[0] = 0;
        Indices[1] = 1;
        Indices[2] = 2;
        Indices[3] = 2;
        Indices[4] = 1;
        Indices[5] = 3;
    }
}