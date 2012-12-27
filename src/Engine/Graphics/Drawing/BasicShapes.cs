using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxeliqEngine.Graphics.Drawing
{
    public static class BasicShapes
    {
        public static void DrawSolidPolygon(PrimitiveBatch primitiveBatch, Vector2[] vertices, int count, float red, float green, float blue)
        {
            DrawSolidPolygon(primitiveBatch, vertices, count, new Color(red, green, blue), true);
        }

        public static void DrawSolidPolygon(PrimitiveBatch primitiveBatch, Vector2[] vertices, int count, Color color)
        {
            DrawSolidPolygon(primitiveBatch, vertices, count, color, true);
        }

        public static void DrawSolidPolygon(PrimitiveBatch primitiveBatch, Vector2[] vertices, int count, Color color, bool outline)
        {
            if (!primitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            if (count == 2)
            {
                DrawPolygon(primitiveBatch, vertices, count, color);
                return;
            }

            Color colorFill = color * (outline ? 0.5f : 1.0f);

            for (int i = 1; i < count - 1; i++)
            {
                primitiveBatch.AddVertex(vertices[0], colorFill, PrimitiveType.TriangleList);
                primitiveBatch.AddVertex(vertices[i], colorFill, PrimitiveType.TriangleList);
                primitiveBatch.AddVertex(vertices[i + 1], colorFill, PrimitiveType.TriangleList);
            }

            if (outline)
            {
                DrawPolygon(primitiveBatch, vertices, count, color);
            }
        }

        public static void DrawPolygon(PrimitiveBatch primitiveBatch, Vector2[] vertices, int count, float red, float green, float blue)
        {
            DrawPolygon(primitiveBatch, vertices, count, new Color(red, green, blue));
        }

        public static void DrawPolygon(PrimitiveBatch primitiveBatch, Vector2[] vertices, int count, Color color)
        {
            if (!primitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            for (int i = 0; i < count - 1; i++)
            {
                primitiveBatch.AddVertex(vertices[i], color, PrimitiveType.LineList);
                primitiveBatch.AddVertex(vertices[i + 1], color, PrimitiveType.LineList);
            }

            primitiveBatch.AddVertex(vertices[count - 1], color, PrimitiveType.LineList);
            primitiveBatch.AddVertex(vertices[0], color, PrimitiveType.LineList);
        }

        public static void DrawSegment(PrimitiveBatch primitiveBatch,Vector2 start, Vector2 end, float red, float green, float blue)
        {
            DrawSegment(primitiveBatch, start, end, new Color(red, green, blue));
        }

        public static void DrawSegment(PrimitiveBatch primitiveBatch, Vector2 start, Vector2 end, Color color)
        {
            if (!primitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            primitiveBatch.AddVertex(start, color, PrimitiveType.LineList);
            primitiveBatch.AddVertex(end, color, PrimitiveType.LineList);
        }
    }
}
