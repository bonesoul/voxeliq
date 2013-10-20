/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

/* Code based on: http://code.google.com/p/xnagameconsole/ */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Debugging.Console
{
    class Renderer
    {
        enum State
        {
            Opened,
            Opening,
            Closed,
            Closing
        }

        public bool IsOpen
        {
            get
            {
                return currentState == State.Opened;
            }
        }

        private readonly SpriteBatch spriteBatch;
        private readonly InputProcessor inputProcessor;
        private readonly Texture2D pixel;
        private readonly int width;
        private State currentState;
        private Vector2 openedPosition, closedPosition, position;
        private DateTime stateChangeTime;
        private Vector2 firstCommandPositionOffset;
        private Vector2 FirstCommandPosition
        {
            get
            {
                return new Vector2(InnerBounds.X, InnerBounds.Y) + firstCommandPositionOffset;
            }
        }

        Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, width - (GameConsoleOptions.Options.Margin * 2), GameConsoleOptions.Options.Height);
            }
        }

        Rectangle InnerBounds
        {
            get
            {
                return new Rectangle(Bounds.X + GameConsoleOptions.Options.Padding, Bounds.Y + GameConsoleOptions.Options.Padding, Bounds.Width - GameConsoleOptions.Options.Padding, Bounds.Height);
            }
        }

        private readonly float oneCharacterWidth;
        private readonly int maxCharactersPerLine;

        public Renderer(Game game, SpriteBatch spriteBatch, InputProcessor inputProcessor)
        {
            currentState = State.Closed;
            width = game.GraphicsDevice.Viewport.Width;
            position = closedPosition = new Vector2(GameConsoleOptions.Options.Margin, -GameConsoleOptions.Options.Height - GameConsoleOptions.Options.RoundedCorner.Height);
            openedPosition = new Vector2(GameConsoleOptions.Options.Margin, 0);
            this.spriteBatch = spriteBatch;
            this.inputProcessor = inputProcessor;
            pixel = new Texture2D(game.GraphicsDevice, 1, 1,false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
            firstCommandPositionOffset = Vector2.Zero;
            oneCharacterWidth = GameConsoleOptions.Options.Font.MeasureString("x").X;
            maxCharactersPerLine = (int)((Bounds.Width - GameConsoleOptions.Options.Padding * 2) / oneCharacterWidth);
        }

        public void Update(GameTime gameTime)
        {
            if (currentState == State.Opening)
            {
                position.Y = MathHelper.SmoothStep(position.Y, openedPosition.Y, ((float)((DateTime.Now - stateChangeTime).TotalSeconds / GameConsoleOptions.Options.AnimationSpeed)));
                if (position.Y == openedPosition.Y)
                {
                    currentState = State.Opened;
                }
            }
            if (currentState == State.Closing)
            {
                position.Y = MathHelper.SmoothStep(position.Y, closedPosition.Y, ((float)((DateTime.Now - stateChangeTime).TotalSeconds / GameConsoleOptions.Options.AnimationSpeed)));
                if (position.Y == closedPosition.Y)
                {
                    currentState = State.Closed;
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (currentState == State.Closed) //Do not draw if the console is closed
            {
                return;
            }
            spriteBatch.Draw(pixel, Bounds, GameConsoleOptions.Options.BackgroundColor);
            DrawRoundedEdges();
            var nextCommandPosition = DrawCommands(inputProcessor.Out, FirstCommandPosition);
            nextCommandPosition = DrawPrompt(nextCommandPosition);
            var bufferPosition = DrawCommand(inputProcessor.Buffer.ToString(), nextCommandPosition, GameConsoleOptions.Options.BufferColor); //Draw the buffer
            DrawCursor(bufferPosition, gameTime);
        }

        void DrawRoundedEdges()
        {
            //Bottom-left edge
            spriteBatch.Draw(GameConsoleOptions.Options.RoundedCorner, new Vector2(position.X, position.Y + GameConsoleOptions.Options.Height), null, GameConsoleOptions.Options.BackgroundColor, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            //Bottom-right edge 
            spriteBatch.Draw(GameConsoleOptions.Options.RoundedCorner, new Vector2(position.X + Bounds.Width - GameConsoleOptions.Options.RoundedCorner.Width, position.Y + GameConsoleOptions.Options.Height), null, GameConsoleOptions.Options.BackgroundColor, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 1);
            //connecting bottom-rectangle
            spriteBatch.Draw(pixel, new Rectangle(Bounds.X + GameConsoleOptions.Options.RoundedCorner.Width, Bounds.Y + GameConsoleOptions.Options.Height, Bounds.Width - GameConsoleOptions.Options.RoundedCorner.Width * 2, GameConsoleOptions.Options.RoundedCorner.Height), GameConsoleOptions.Options.BackgroundColor);
        }

        void DrawCursor(Vector2 pos, GameTime gameTime)
        {
            if (!IsInBounds(pos.Y))
            {
                return;
            }
            var split = SplitCommand(inputProcessor.Buffer.ToString(), maxCharactersPerLine).Last();
            pos.X += GameConsoleOptions.Options.Font.MeasureString(split).X;
            pos.Y -= GameConsoleOptions.Options.Font.LineSpacing;
            spriteBatch.DrawString(GameConsoleOptions.Options.Font, (int)(gameTime.TotalGameTime.TotalSeconds / GameConsoleOptions.Options.CursorBlinkSpeed) % 2 == 0 ? GameConsoleOptions.Options.Cursor.ToString() : "", pos, GameConsoleOptions.Options.CursorColor);
        }

        /// <summary>
        /// Draws the specified command and returns the position of the next command to be drawn
        /// </summary>
        /// <param name="command"></param>
        /// <param name="pos"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        Vector2 DrawCommand(string command, Vector2 pos, Color color)
        {
            var splitLines = command.Length > maxCharactersPerLine ? SplitCommand(command, maxCharactersPerLine) : new[] { command };
            foreach (var line in splitLines)
            {
                if (IsInBounds(pos.Y))
                {
                    spriteBatch.DrawString(GameConsoleOptions.Options.Font, line, pos, color);
                }
                ValidateFirstCommandPosition(pos.Y + GameConsoleOptions.Options.Font.LineSpacing);
                pos.Y += GameConsoleOptions.Options.Font.LineSpacing;
            }
            return pos;
        }

        static IEnumerable<string> SplitCommand(string command, int max)
        {
            var lines = new List<string>();
            while (command.Length > max)
            {
                var splitCommand = command.Substring(0, max);
                lines.Add(splitCommand);
                command = command.Substring(max, command.Length - max);
            }
            lines.Add(command);
            return lines;
        }

        /// <summary>
        /// Draws the specified collection of commands and returns the position of the next command to be drawn
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        Vector2 DrawCommands(IEnumerable<OutputLine> lines, Vector2 pos)
        {
            var originalX = pos.X;
            foreach (var command in lines)
            {
                if (command.Type == OutputLineType.Command)
                {
                    pos = DrawPrompt(pos);
                }
                //position.Y = DrawCommand(command.ToString(), position, GameConsoleOptions.Options.FontColor).Y;
                pos.Y = DrawCommand(command.ToString(), pos, command.Type == OutputLineType.Command ? GameConsoleOptions.Options.PastCommandColor : GameConsoleOptions.Options.PastCommandOutputColor).Y;
                pos.X = originalX;
            }
            return pos;
        }

        /// <summary>
        /// Draws the prompt at the specified position and returns the position of the text that will be drawn next to it
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        Vector2 DrawPrompt(Vector2 pos)
        {
            spriteBatch.DrawString(GameConsoleOptions.Options.Font, GameConsoleOptions.Options.Prompt, pos, GameConsoleOptions.Options.PromptColor);
            pos.X += oneCharacterWidth * GameConsoleOptions.Options.Prompt.Length + oneCharacterWidth;
            return pos;
        }

        public void Open()
        {
            if (currentState == State.Opening || currentState == State.Opened)
            {
                return;
            }
            stateChangeTime = DateTime.Now;
            currentState = State.Opening;
        }

        public void Close()
        {
            if (currentState == State.Closing || currentState == State.Closed)
            {
                return;
            }
            stateChangeTime = DateTime.Now;
            currentState = State.Closing;
        }

        void ValidateFirstCommandPosition(float nextCommandY)
        {
            if (!IsInBounds(nextCommandY))
            {
                firstCommandPositionOffset.Y -= GameConsoleOptions.Options.Font.LineSpacing;
            }
        }

        bool IsInBounds(float yPosition)
        {
            return yPosition < openedPosition.Y + GameConsoleOptions.Options.Height;
        }
    }
}