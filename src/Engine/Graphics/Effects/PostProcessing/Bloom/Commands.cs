/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using VoxeliqEngine.Core;
using VoxeliqEngine.Debugging.Console;

namespace VoxeliqEngine.Graphics.Effects.PostProcessing.Bloom
{
    [Command("bloom", "Changes bloom settings.\nusage: bloom [on|off|mode]")]
    public class BloomCommand : Command
    {
        [DefaultCommand]
        public string Default(string[] @params)
        {
            if (!Engine.Instance.Configuration.Bloom.Enabled)
                return "Bloom is currently off.\nusage: bloom [on|off|mode]";
            else
            {
                return string.Format("Bloom is on and set to {0} mode.\nusage: bloom [on|off|mode]",
                                     Engine.Instance.Configuration.Bloom.State.ToString().ToLower());
            }
        }

        [Subcommand("on", "Sets bloom on.")]
        public string On(string[] @params)
        {
            Engine.Instance.Configuration.Bloom.Enabled = true;
            return string.Format("Bloom is now on and set to {0} mode.",Engine.Instance.Configuration.Bloom.State.ToString().ToLower());
        }

        [Subcommand("off", "Sets bloom off.")]
        public string Off(string[] @params)
        {
            Engine.Instance.Configuration.Bloom.Enabled = false;
            return "Bloom is now off.";
        }

        [Subcommand("mode", "Sets bloom mode.\nusage: bloomm mode [default|soft|desaturated|saturated|blurry|subtle]")]
        public string Mode(string[] @params)
        {
            if (@params.Length != 1)
                return "Invalid arguments. \nusage: bloomm mode [default|soft|desaturated|saturated|blurry|subtle]";

            switch (@params[0].ToLower())
            {
                case "default":
                    Engine.Instance.Configuration.Bloom.State=BloomState.Default;
                    break;
                case "soft":
                    Engine.Instance.Configuration.Bloom.State = BloomState.Soft;
                    break;
                case "desaturated":
                    Engine.Instance.Configuration.Bloom.State = BloomState.Desaturated;
                    break;
                case "saturated":
                    Engine.Instance.Configuration.Bloom.State = BloomState.Saturated;
                    break;
                case "blurry":
                    Engine.Instance.Configuration.Bloom.State = BloomState.Blurry;
                    break;
                case "subtle":
                    Engine.Instance.Configuration.Bloom.State = BloomState.Subtle;
                    break;
            }

            return string.Format("Bloom mode set to {0}.", Engine.Instance.Configuration.Bloom.State.ToString().ToLower());
        }
    }
}