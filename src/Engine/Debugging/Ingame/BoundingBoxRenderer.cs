/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Debugging.Ingame
{
    public static class BoundingBoxRenderer
    {
        #region Fields

        private static VertexPositionColor[] verts = new VertexPositionColor[8];

        private static int[] indices = new int[]
                                           {
                                               0, 1,
                                               1, 2,
                                               2, 3,
                                               3, 0,
                                               0, 4,
                                               1, 5,
                                               2, 6,
                                               3, 7,
                                               4, 5,
                                               5, 6,
                                               6, 7,
                                               7, 4,
                                           };

        private static BasicEffect effect;

        #endregion

        /// <summary>
        /// Renders the bounding box for debugging purposes.
        /// </summary>
        /// <param name="box">The box to render.</param>
        /// <param name="graphicsDevice">The graphics device to use when rendering.</param>
        /// <param name="view">The current view matrix.</param>
        /// <param name="projection">The current projection matrix.</param>
        /// <param name="color">The color to use drawing the lines of the box.</param>
        public static void Render(
            BoundingBox box,
            GraphicsDevice graphicsDevice,
            Matrix view,
            Matrix projection,
            Color color)
        {
            if (effect == null)
            {
                effect = new BasicEffect(graphicsDevice)
                             {TextureEnabled = false, VertexColorEnabled = true, LightingEnabled = false};
            }

            Vector3[] corners = box.GetCorners();
            for (int i = 0; i < 8; i++)
            {
                verts[i].Position = corners[i];
                verts[i].Color = color;
            }


            effect.View = view;
            effect.Projection = projection;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, verts, 0, 8, indices, 0,
                                                         indices.Length/2);
            }
        }
    }
}