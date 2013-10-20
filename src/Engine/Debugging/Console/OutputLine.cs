/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

/* Code based on: http://code.google.com/p/xnagameconsole/ */

namespace Engine.Debugging.Console
{
    enum OutputLineType
    {
        Command,
        Output
    }

    class OutputLine
    {
        public string Output { get; set; }
        public OutputLineType Type { get; set; }

        public OutputLine(string output, OutputLineType type)
        {
            Output = output;
            Type = type;
        }

        public override string ToString()
        {
            return Output;
        }
    }
}
