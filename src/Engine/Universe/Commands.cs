using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return "Fullscreen on.";
        }

        [Subcommand("off", "Sets flying off.")]
        public string Off(string[] @params)
        {
            this._player.FlyingEnabled = false;
            return "Fullscreen off.";
        }
    }

    [Command("infinitive", "Sets the infinitive mode.\nusage: infinitive [on|off]")]
    public class InfinitiveCommand : Command
    {
        [DefaultCommand]
        public string Default(string[] @params)
        {
            return string.Format("Infinitive mode is currently {0}.\nusage: infinitive [on|off].",
                                 Settings.World.IsInfinitive
                                     ? "on"
                                     : "off");
        }

        [Subcommand("on", "Sets infinitive mode on.")]
        public string On(string[] @params)
        {
            Settings.World.IsInfinitive = true;
            return "Infinitive on.";
        }

        [Subcommand("off", "Sets infinitive off.")]
        public string Off(string[] @params)
        {
            Settings.World.IsInfinitive = false;
            return "Infinitive off.";
        }
    }
}
