/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 * 
 * Code based on: http://code.google.com/p/xnagameconsole/
 */


using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using VoxeliqEngine.Debugging.Console.KeyboardCapture;

namespace VoxeliqEngine.Debugging.Console
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
            EventInput.CharEntered += EventInput_CharEntered; //Handles the typable characters
            EventInput.KeyDown += EventInput_KeyDown; //Handles the non-typable characters
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

        void EventInput_KeyDown(object sender, KeyEventArgs e)
        {
            //if (Keyboard.GetState().IsKeyDown(Keys.V) && Keyboard.GetState().IsKeyDown(Keys.LeftControl)) // CTRL + V
            //{
            //    if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA) //Thread Apartment must be in Single-Threaded for the Clipboard to work
            //    {
            //        AddToBuffer(Clipboard.GetText());
            //    }
            //}

            if (e.KeyCode == GameConsoleOptions.Options.ToggleKey)
            {
                ToggleConsole();
                isHandled = true;
            }

            switch (e.KeyCode)
            {
                case Keys.Up: Buffer.Output = CommandHistory.Previous(); break;
                case Keys.Down: Buffer.Output = CommandHistory.Next(); break;
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

        void EventInput_CharEntered(object sender, CharacterEventArgs e)
        {
            if (isHandled)
            {
                isHandled = false;
                return;
            }
            CommandHistory.Reset();
            switch ((int)e.Character)
            {
                case ENTER: ExecuteBuffer(); break;
                case BACKSPACE:
                    if (Buffer.Output.Length > 0)
                    {
                        Buffer.Output = Buffer.Output.Substring(0, Buffer.Output.Length - 1);
                    }
                    break;
                case TAB: AutoComplete(); break;
                default:
                    if (IsPrintable(e.Character))
                    {
                        Buffer.Output += e.Character;
                    }
                    break;
            }
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
