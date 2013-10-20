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
using Engine.Input;
using Microsoft.Xna.Framework.Input;

namespace Engine.Debugging.Console
{
    class InputProcessor
    {
        public event EventHandler Open = delegate { };
        public event EventHandler Close = delegate { };
        public event EventHandler PlayerCommand = delegate { };
        public event EventHandler ConsoleCommand = delegate { };

        public CommandHistory CommandHistory { get; set; }
        public OutputLine Buffer { get; set; }
        public List<OutputLine> Out { get; set; }

        private const int BACKSPACE = 8;
        private const int ENTER = 13;
        private const int TAB = 9;
        private bool isActive, isHandled;
        private CommandProcesser commandProcesser;

        public InputProcessor(CommandProcesser commandProcesser)
        {
            this.commandProcesser = commandProcesser;
            isActive = false;
            CommandHistory = new CommandHistory();
            Out = new List<OutputLine>();
            Buffer = new OutputLine("", OutputLineType.Command);

            var inputManager = (IInputManager)Core.Engine.Instance.Game.Services.GetService(typeof(IInputManager));
            inputManager.KeyDown += new InputManager.KeyEventHandler(OnKeyDown);
        }

        public void AddToBuffer(string text)
        {
            var lines = text.Split('\n').Where(line => line != "").ToArray();
            int i;
            for (i = 0; i < lines.Length - 1; i++)
            {
                var line = lines[i];
                Buffer.Output += line;
                ExecuteBuffer();
            }
            Buffer.Output += lines[i];
        }

        public void AddToOutput(string text)
        {
            if (GameConsoleOptions.Options.OpenOnWrite)
            {
                isActive = true;
                Open(this, EventArgs.Empty);
            }
            foreach (var line in text.Split('\n'))
            {
                Out.Add(new OutputLine(line, OutputLineType.Output));
            }
        }

        void ToggleConsole()
        {
            isActive = !isActive;
            if (isActive)
            {
                Open(this, EventArgs.Empty);
            }
            else
            {
                Close(this, EventArgs.Empty);
            }
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == GameConsoleOptions.Options.ToggleKey)
            {
                ToggleConsole();
                isHandled = true;
            }

            switch (e.KeyCode)
            {
                case Keys.Enter:
                    ExecuteBuffer();
                    break;
                case Keys.Back:
                    if (Buffer.Output.Length > 0)
                        Buffer.Output = Buffer.Output.Substring(0, Buffer.Output.Length - 1);
                    break;
                case Keys.Tab:
                    AutoComplete();
                    break;
                case Keys.Up: 
                    Buffer.Output = CommandHistory.Previous(); 
                    break;
                case Keys.Down: Buffer.Output = CommandHistory.Next(); 
                    break;
                default:
                    var @char = TranslateChar(e.KeyCode);
                    if (IsPrintable(@char))
                    {
                        Buffer.Output += @char;
                    }
                    break;
            }
        }

        /// <summary>
        /// Translates alphanumeric XNA key code to character value.
        /// </summary>
        /// <param name="sfKey">XNA key code.</param>
        /// <returns>Translated character.</returns>
        private static char TranslateChar(global::Microsoft.Xna.Framework.Input.Keys xnaKey)
        {
            if (xnaKey >= global::Microsoft.Xna.Framework.Input.Keys.A && xnaKey <= global::Microsoft.Xna.Framework.Input.Keys.Z)
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                {
                    return (char)('A' + ((int)xnaKey - (int)global::Microsoft.Xna.Framework.Input.Keys.A));
                }
                else
                {
                    return (char)('a' + ((int)xnaKey - (int)global::Microsoft.Xna.Framework.Input.Keys.A));
                }

            if (xnaKey >= global::Microsoft.Xna.Framework.Input.Keys.NumPad0 && xnaKey <= global::Microsoft.Xna.Framework.Input.Keys.NumPad9)
                return (char)('0' + ((int)xnaKey - (int)global::Microsoft.Xna.Framework.Input.Keys.NumPad0));

            if (xnaKey >= global::Microsoft.Xna.Framework.Input.Keys.D0 && xnaKey <= global::Microsoft.Xna.Framework.Input.Keys.D9)
                return (char)('0' + ((int)xnaKey - (int)global::Microsoft.Xna.Framework.Input.Keys.D0));

            if (xnaKey == Keys.OemPeriod)
                return '.';

            return ' ';
        }

        void ExecuteBuffer()
        {
            if (Buffer.Output.Length == 0)
            {
                return;
            }
            var output = commandProcesser.Process(Buffer.Output).Split('\n').Where(l => l != "");
            Out.Add(new OutputLine(Buffer.Output, OutputLineType.Command));
            foreach (var line in output)
            {
                Out.Add(new OutputLine(line, OutputLineType.Output));
            }
            CommandHistory.Add(Buffer.Output);
            Buffer.Output = "";
        }

        void AutoComplete()
        {
            var lastSpacePosition = Buffer.Output.LastIndexOf(' ');
            var textToMatch = lastSpacePosition < 0 ? Buffer.Output : Buffer.Output.Substring(lastSpacePosition + 1, Buffer.Output.Length - lastSpacePosition - 1);
            var match = CommandManager.GetMatchingCommand(textToMatch);
            if (match == null)
            {
                return;
            }
            var restOfTheCommand = match.Attributes.Name.Substring(textToMatch.Length);
            Buffer.Output += restOfTheCommand + " ";
        }

        static bool IsPrintable(char letter)
        {
            return GameConsoleOptions.Options.Font.Characters.Contains(letter);
        }
    }
}
