﻿/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using VoxeliqEngine.Core;
using VoxeliqEngine.Debugging.Console;

namespace VoxeliqEngine.Universe
{
    [Command("fly", "Sets the flying mode.\nusage: fly [on|off]")]
    public class FlyCommand : Command
    {
        private readonly IPlayer _player;

        public FlyCommand()
        {
            this._player = (IPlayer)Engine.Instance.Game.Services.GetService(typeof(IPlayer));
        }

        [DefaultCommand]
        public string Default(string[] @params)
        {
            return string.Format("Fly mode is currently {0}.\nusage: fly [on|off].",
                                 this._player.FlyingEnabled
                                     ? "on"
                                     : "off");
        }

        [Subcommand("on", "Sets flying mode on.")]
        public string On(string[] @params)
        {
            this._player.FlyingEnabled = true;
            return "Fly mode on.";
        }

        [Subcommand("off", "Sets flying off.")]
        public string Off(string[] @params)
        {
            this._player.FlyingEnabled = false;
            return "Fly mode off.";
        }
    }

    [Command("infinitive", "Sets the infinitive mode.\nusage: infinitive [on|off]")]
    public class InfinitiveCommand : Command
    {
        [DefaultCommand]
        public string Default(string[] @params)
        {
            return string.Format("Infinitive mode is currently {0}.\nusage: infinitive [on|off].",
                                 Engine.Instance.Configuration.World.IsInfinitive
                                     ? "on"
                                     : "off");
        }

        [Subcommand("on", "Sets infinitive mode on.")]
        public string On(string[] @params)
        {
            Engine.Instance.Configuration.World.IsInfinitive = true;
            return "Infinitive on.";
        }

        [Subcommand("off", "Sets infinitive off.")]
        public string Off(string[] @params)
        {
            Engine.Instance.Configuration.World.IsInfinitive = false;
            return "Infinitive off.";
        }
    }
}
