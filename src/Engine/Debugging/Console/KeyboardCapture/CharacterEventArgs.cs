/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 * 
 * Code based on: http://code.google.com/p/xnagameconsole/
 */

using System;

namespace VoxeliqEngine.Debugging.Console.KeyboardCapture
{
    class CharacterEventArgs : EventArgs
    {
        private readonly char character;
        private readonly int lParam;

        public CharacterEventArgs( char character, int lParam )
        {
            this.character = character;
            this.lParam = lParam;
        }

        public char Character
        {
            get { return character; }
        }

        public int Param
        {
            get { return lParam; }
        }

        public int RepeatCount
        {
            get { return lParam & 0xffff; }
        }

        public bool ExtendedKey
        {
            get { return ( lParam & ( 1 << 24 ) ) > 0; }
        }

        public bool AltPressed
        {
            get { return ( lParam & ( 1 << 29 ) ) > 0; }
        }

        public bool PreviousState
        {
            get { return ( lParam & ( 1 << 30 ) ) > 0; }
        }

        public bool TransitionState
        {
            get { return ( lParam & ( 1 << 31 ) ) > 0; }
        }
    }
}
