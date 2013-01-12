/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 * 
 * Code based on: http://code.google.com/p/xnagameconsole/
 */

using System;
using Microsoft.Xna.Framework.Input;

namespace VoxeliqEngine.Debugging.Console
{
    public class KeyEventArgs : EventArgs
    {
        public KeyEventArgs( Keys keyCode )
        {
            KeyCode = keyCode;
        }

        public Keys KeyCode { get; private set; }
    }
    
}
