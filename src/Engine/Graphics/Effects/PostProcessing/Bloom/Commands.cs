/*
 * Copyright (C) 2011 - 2013 Int6 Studios - http://www.int6.org,
 * Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Engine.Debugging.Console;

namespace Engine.Graphics.Effects.PostProcessing.Bloom
{
    [Command("bloom", "Changes bloom settings.\nusage: bloom [on|off|mode]")]
    public class BloomCommand : Command
    {
        [DefaultCommand]
        public string Default(string[] @params)
        {
            if (!Core.Engine.Instance.Configuration.Bloom.Enabled)
                return "Bloom is currently off.\nusage: bloom [on|off|mode]";
            else
            {
                return string.Format("Bloom is on and set to {0} mode.\nusage: bloom [on|off|mode]",
                                     Core.Engine.Instance.Configuration.Bloom.State.ToString().ToLower());
            }
        }

        [Subcommand("on", "Sets bloom on.")]
        public string On(string[] @params)
        {
            Core.Engine.Instance.Configuration.Bloom.Enabled = true;
            return string.Format("Bloom is now on and set to {0} mode.",Core.Engine.Instance.Configuration.Bloom.State.ToString().ToLower());
        }

        [Subcommand("off", "Sets bloom off.")]
        public string Off(string[] @params)
        {
            Core.Engine.Instance.Configuration.Bloom.Enabled = false;
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
                    Core.Engine.Instance.Configuration.Bloom.State=BloomState.Default;
                    break;
                case "soft":
                    Core.Engine.Instance.Configuration.Bloom.State = BloomState.Soft;
                    break;
                case "desaturated":
                    Core.Engine.Instance.Configuration.Bloom.State = BloomState.Desaturated;
                    break;
                case "saturated":
                    Core.Engine.Instance.Configuration.Bloom.State = BloomState.Saturated;
                    break;
                case "blurry":
                    Core.Engine.Instance.Configuration.Bloom.State = BloomState.Blurry;
                    break;
                case "subtle":
                    Core.Engine.Instance.Configuration.Bloom.State = BloomState.Subtle;
                    break;
            }

            return string.Format("Bloom mode set to {0}.", Core.Engine.Instance.Configuration.Bloom.State.ToString().ToLower());
        }
    }
}